using Microsoft.EntityFrameworkCore;
using Novvi.Domain.Entities;

namespace Novvi.Infrastructure.Persistence;

/// <summary>Contexto EF Core do domínio Novvi (persistência via Oracle).</summary>
public class NovviContext(DbContextOptions<NovviContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();
    public DbSet<Funcionario> Funcionarios => Set<Funcionario>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NovviContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
