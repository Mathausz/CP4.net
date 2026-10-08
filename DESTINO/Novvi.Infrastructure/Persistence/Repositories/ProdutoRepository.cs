using Novvi.Application.Interfaces.Repositories;
using Novvi.Domain.Entities;

namespace Novvi.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositório específico de Produto: reaproveita o CRUD genérico e adiciona
/// a busca de vários produtos de uma vez (usada na criação de pedidos).
/// </summary>
public class ProdutoRepository(NovviContext context)
    : Repository<Produto>(context), IProdutoRepository
{
    public IReadOnlyList<Produto> GetByIds(IEnumerable<Guid> ids)
    {
        var idsList = ids.Distinct().ToList();
        return context.Produtos
            .Where(p => idsList.Contains(p.Id) && p.Ativo)
            .ToList();
    }
}
