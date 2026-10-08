using Microsoft.EntityFrameworkCore;
using Novvi.Application.Interfaces.Repositories;
using Novvi.Domain.Entities;

namespace Novvi.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositório específico de Pedido: reaproveita o CRUD genérico e adiciona
/// consultas com Include para carregar Produtos e Pagamento.
/// </summary>
public class PedidoRepository(NovviContext context)
    : Repository<Pedido>(context), IPedidoRepository
{
    public Pedido? GetByIdCompleto(Guid id) =>
        context.Pedidos
            .Include(p => p.Produtos)
            .Include(p => p.Pagamento)
            .SingleOrDefault(p => p.Id == id && p.Ativo);

    public IReadOnlyList<Pedido> GetAllCompleto() =>
        context.Pedidos
            .Include(p => p.Produtos)
            .Include(p => p.Pagamento)
            .Where(p => p.Ativo)
            .OrderBy(p => p.CriadoEm)
            .ToList();

    public (IReadOnlyList<Pedido> Items, int TotalItems) GetPagedCompleto(int page, int pageSize)
    {
        var query = context.Pedidos.AsNoTracking().Where(p => p.Ativo);

        var totalItems = query.Count();

        var items = query
            .Include(p => p.Produtos)
            .Include(p => p.Pagamento)
            .OrderBy(p => p.CriadoEm)
            .ThenBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalItems);
    }
}
