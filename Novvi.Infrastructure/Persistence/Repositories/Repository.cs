using Microsoft.EntityFrameworkCore;
using Novvi.Application.Interfaces.Repositories;
using Novvi.Domain.Commons;

namespace Novvi.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação genérica de IRepository&lt;T&gt; via EF Core (CP3).
/// Delete é lógico (soft delete usando EntidadeBase.Ativo), preservando histórico.
/// </summary>
public class Repository<T>(NovviContext context) : IRepository<T>
    where T : EntidadeBase
{
    private readonly DbSet<T> _table = context.Set<T>();

    public T Add(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _table.Add(entity);
        context.SaveChanges();
        return entity;
    }

    public T? GetById(Guid id) =>
        _table.AsNoTracking().SingleOrDefault(x => x.Id == id && x.Ativo);

    public IReadOnlyList<T> GetAll() =>
        _table
            .AsNoTracking()
            .Where(x => x.Ativo)
            .OrderBy(x => x.CriadoEm)
            .ToList();

    public bool ExistsById(Guid id) =>
        _table.Any(x => x.Id == id && x.Ativo);

    public void Update(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        // Entidades com chave Guid gerada no cliente (todas aqui) exigem cuidado:
        // se a entidade já está sendo rastreada pelo contexto (ex.: veio de uma consulta
        // "completa" com Include, como GetByIdCompleto, e ganhou um novo filho na coleção -
        // um novo Endereco ou um Pagamento), o SaveChanges já detecta e insere o filho novo
        // corretamente via fixup de navegação. Chamar _table.Update(entity) nesse caso
        // usaria a heurística de grafo desconectado (Id != default => Modified) e tentaria
        // um UPDATE para o filho que ainda nem existe no banco.
        // Só quando a entidade está desanexada (ex.: veio de GetById com AsNoTracking) é que
        // precisamos reanexá-la e marcá-la como Modified explicitamente.
        if (context.Entry(entity).State == EntityState.Detached)
            _table.Update(entity);

        context.SaveChanges();
    }

    public bool Delete(Guid id)
    {
        var entity = _table.SingleOrDefault(x => x.Id == id);
        if (entity is null)
            return false;

        if (!entity.Ativo)
            return true;

        entity.Desativar();
        context.SaveChanges();
        return true;
    }
}
