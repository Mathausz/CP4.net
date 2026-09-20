using Novvi.Application.DTO;
using Novvi.Application.Interfaces.Repositories;
using Novvi.Application.Interfaces.Services;
using Novvi.Domain.Entities;
using Novvi.Domain.Exceptions;

namespace Novvi.Application.Services;

/// <summary>Usa diretamente o repositório genérico IRepository&lt;Produto&gt; (CRUD simples).</summary>
public class ProdutoService(IRepository<Produto> produtoRepository) : IProdutoService
{
    public IReadOnlyList<ProdutoResponse> GetAll() =>
        produtoRepository.GetAll().Select(ProdutoResponse.FromDomain).ToList();

    public ProdutoResponse GetById(Guid id) => ProdutoResponse.FromDomain(BuscarOuFalhar(id));

    public ProdutoResponse Create(ProdutoRequest request)
    {
        var produto = new Produto(request.Nome, request.Descricao, request.Preco);
        produtoRepository.Add(produto);
        return ProdutoResponse.FromDomain(produto);
    }

    public ProdutoResponse Update(Guid id, ProdutoRequest request)
    {
        var produto = BuscarOuFalhar(id);
        produto.Atualizar(request.Nome, request.Descricao, request.Preco);
        produtoRepository.Update(produto);
        return ProdutoResponse.FromDomain(produto);
    }

    public void Delete(Guid id)
    {
        if (!produtoRepository.Delete(id))
            throw new NotFoundException($"Produto {id} não encontrado");
    }

    private Produto BuscarOuFalhar(Guid id) =>
        produtoRepository.GetById(id)
        ?? throw new NotFoundException($"Produto {id} não encontrado");
}
