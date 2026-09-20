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

    bool ExistsById(Guid id);

    void Update(T entity);

    bool Delete(Guid id);
}
