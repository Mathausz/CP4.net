using Microsoft.Extensions.Logging;
using Novvi.Application.DTO;
using Novvi.Application.Interfaces.Repositories;
using Novvi.Domain.Entities;
using Novvi.Application.Interfaces.Services;
using Novvi.Domain.Exceptions;

namespace Novvi.Application.Services;

/// <summary>
/// Orquestra a criação de pedidos. Combina o repositório específico de Pedido
/// (consultas com Include de Produtos/Pagamento) com os repositórios genéricos
/// de Usuario e Funcionario (apenas para validar existência).
/// </summary>
public class PedidoService(
    IPedidoRepository pedidoRepository,
    IProdutoRepository produtoRepository,
    IRepository<Usuario> usuarioRepository,
    IRepository<Funcionario> funcionarioRepository,
    ILogger<PedidoService> logger) : IPedidoService
{
    public IReadOnlyList<PedidoResponse> GetAll() =>
        pedidoRepository.GetAllCompleto().Select(PedidoResponse.FromDomain).ToList();

    public PedidoResponse GetById(Guid id) => PedidoResponse.FromDomain(BuscarOuFalhar(id));

    public PedidoResponse Create(PedidoRequest request)
    {
        logger.LogInformation(
            "Criando pedido para Usuario {IdUsuario} atendido por Funcionario {IdFuncionario} com {QuantidadeProdutos} produto(s)",
            request.IdUsuario, request.IdFuncionario, request.ProdutosIds.Count);

        if (!usuarioRepository.ExistsById(request.IdUsuario))
            throw new NotFoundException($"Usuário {request.IdUsuario} não encontrado");

        if (!funcionarioRepository.ExistsById(request.IdFuncionario))
            throw new NotFoundException($"Funcionário {request.IdFuncionario} não encontrado");

        var produtos = produtoRepository.GetByIds(request.ProdutosIds);
        if (produtos.Count != request.ProdutosIds.Distinct().Count())
            throw new NotFoundException("Um ou mais produtos informados não foram encontrados");

        var pedido = new Pedido(request.IdUsuario, request.IdFuncionario, produtos.ToList());
        pedidoRepository.Add(pedido);

        logger.LogInformation(
            "Pedido {IdPedido} criado com sucesso. Total {Total}",
            pedido.Id, pedido.CalcularTotal());

        return PedidoResponse.FromDomain(pedido);
    }

    public PedidoResponse RegistrarPagamento(Guid id, PagamentoRequest request)
    {
        var pedido = BuscarOuFalhar(id);
        var pagamento = new Pagamento(request.TipoPagamento, pedido.Id);
        pedido.RegistrarPagamento(pagamento);
        pedidoRepository.Update(pedido);
        return PedidoResponse.FromDomain(pedido);
    }

    public void Delete(Guid id)
    {
        if (!pedidoRepository.Delete(id))
            throw new NotFoundException($"Pedido {id} não encontrado");
    }

    private Pedido BuscarOuFalhar(Guid id) =>
        pedidoRepository.GetByIdCompleto(id)
        ?? throw new NotFoundException($"Pedido {id} não encontrado");
}
