using Novvi.Domain.Entities;

namespace Novvi.Application.Interfaces.Repositories;

/// <summary>Repositório específico de Produto, usado para resolver múltiplos ids de uma vez.</summary>
public interface IProdutoRepository : IRepository<Produto>
{
    /// <summary>Busca vários produtos ativos a partir de uma lista de ids.</summary>
    IReadOnlyList<Produto> GetByIds(IEnumerable<Guid> ids);
}
