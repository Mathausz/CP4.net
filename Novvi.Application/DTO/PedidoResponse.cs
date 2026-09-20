using Novvi.Domain.Entities;

namespace Novvi.Application.DTO;

/// <summary>Dados de um pedido retornados pela API.</summary>
public record PedidoResponse(
    Guid Id,
    DateTime Data,
    double Frete,
    double Total,
    Guid IdUsuario,
    Guid IdFuncionario,
    List<ProdutoResponse> Produtos,
    PagamentoResponse? Pagamento,
    DateTime CriadoEm)
{
    public static PedidoResponse FromDomain(Pedido pedido) => new(
        pedido.Id,
        pedido.Data,
        pedido.Frete,
        pedido.CalcularTotal(),
        pedido.IdUsuario,
        pedido.IdFuncionario,
        pedido.Produtos.Select(ProdutoResponse.FromDomain).ToList(),
        pedido.Pagamento is null ? null : PagamentoResponse.FromDomain(pedido.Pagamento),
        pedido.CriadoEm);
}
