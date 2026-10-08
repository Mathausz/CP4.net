using Novvi.Domain.Entities;

namespace Novvi.Application.DTO;

/// <summary>Dados de um produto retornados pela API.</summary>
public record ProdutoResponse(Guid Id, string Nome, string Descricao, double Preco, DateTime CriadoEm)
{
    public static ProdutoResponse FromDomain(Produto produto) => new(
        produto.Id,
        produto.Nome,
        produto.Descricao,
        produto.Preco,
        produto.CriadoEm);
}
