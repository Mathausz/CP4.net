namespace Novvi.Domain.Commons;

/// <summary>
/// Classe base para as entidades que são agregados (raiz) do domínio.
/// Fornece identificador único, soft delete (Ativo) e data de criação,
/// usados pelo repositório genérico (IRepository&lt;T&gt;).
/// </summary>
public abstract class EntidadeBase
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public bool Ativo { get; private set; } = true;
    public DateTime CriadoEm { get; private set; } = DateTime.Now;

    /// <summary>Marca a entidade como inativa (soft delete).</summary>
    public void Desativar() => Ativo = false;

    /// <summary>Reativa uma entidade previamente desativada.</summary>
    public void Ativar() => Ativo = true;
}
