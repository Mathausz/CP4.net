using Novvi.Domain.Entities;

namespace Novvi.Application.Interfaces.Repositories;

/// <summary>
/// Repositório específico de Pedido. Convive com IRepository&lt;Pedido&gt; (CRUD básico)
/// para consultas que exigem carregar Produtos e Pagamento.
/// </summary>
public interface IPedidoRepository : IRepository<Pedido>
{
    /// <summary>Busca um pedido já carregando produtos e pagamento.</summary>
    Pedido? GetByIdCompleto(Guid id);

    /// <summary>Lista pedidos ativos já carregando produtos e pagamento.</summary>
    IReadOnlyList<Pedido> GetAllCompleto();

    /// <summary>Página de pedidos ativos com produtos e pagamento, cortada no banco (CP5).</summary>
    (IReadOnlyList<Pedido> Items, int TotalItems) GetPagedCompleto(int page, int pageSize);
}
