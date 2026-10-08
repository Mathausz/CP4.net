using Novvi.Domain.Commons;

namespace Novvi.Application.Interfaces.Repositories;

/// <summary>
/// Repositório genérico usado como contrato padrão de acesso a dados (CP3).
/// Implementado por Repository&lt;T&gt; na Infrastructure via EF Core.
/// </summary>
public interface IRepository<T> where T : EntidadeBase
{
    T Add(T entity);

    T? GetById(Guid id);

    IReadOnlyList<T> GetAll();

    /// <summary>
    /// Página de entidades ativas (CP5). Count + OrderBy + Skip + Take executados no banco;
    /// só então a página é materializada. page é 1-based e já chega validado pela Application.
    /// </summary>
    (IReadOnlyList<T> Items, int TotalItems) GetPaged(int page, int pageSize);

    bool ExistsById(Guid id);

    void Update(T entity);

    bool Delete(Guid id);
}
