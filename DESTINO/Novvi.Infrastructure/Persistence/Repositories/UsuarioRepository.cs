using Microsoft.EntityFrameworkCore;
using Novvi.Application.Interfaces.Repositories;
using Novvi.Domain.Entities;

namespace Novvi.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositório específico de Usuario: reaproveita o CRUD genérico (Repository&lt;Usuario&gt;)
/// e adiciona consultas com Include para carregar Enderecos.
/// </summary>
public class UsuarioRepository(NovviContext context)
    : Repository<Usuario>(context), IUsuarioRepository
{
    public Usuario? GetByIdCompleto(Guid id) =>
        context.Usuarios
            .Include(u => u.Enderecos)
            .SingleOrDefault(u => u.Id == id && u.Ativo);

    public IReadOnlyList<Usuario> GetAllCompleto() =>
        context.Usuarios
            .Include(u => u.Enderecos)
            .Where(u => u.Ativo)
            .OrderBy(u => u.CriadoEm)
            .ToList();
}
