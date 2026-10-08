using Novvi.Domain.Entities;

namespace Novvi.Application.Interfaces.Repositories;

/// <summary>
/// Repositório específico de Usuario. Convive com IRepository&lt;Usuario&gt; (CRUD básico)
/// para consultas que exigem carregar agregados relacionados (Enderecos, Pedidos).
/// </summary>
public interface IUsuarioRepository : IRepository<Usuario>
{
    /// <summary>Busca um usuário já carregando seus endereços.</summary>
    Usuario? GetByIdCompleto(Guid id);

    /// <summary>Lista usuários ativos já carregando seus endereços.</summary>
    IReadOnlyList<Usuario> GetAllCompleto();
}
