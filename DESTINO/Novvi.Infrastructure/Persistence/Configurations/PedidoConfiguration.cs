using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Novvi.Domain.Entities;

namespace Novvi.Infrastructure.Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("PEDIDOS");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Frete).HasColumnType("NUMBER(18,2)");

        // N:N Pedido <-> Produto (tabela de junção implícita, EF Core 8+)
        builder.HasMany(p => p.Produtos)
            .WithMany(pr => pr.Pedidos)
            .UsingEntity(j => j.ToTable("PEDIDO_PRODUTO"));

        // 1:1 Pedido -> Pagamento (FK está em Pagamento.IdPedido)
        builder.HasOne(p => p.Pagamento)
            .WithOne()
            .HasForeignKey<Pagamento>(pg => pg.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
