using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Novvi.Domain.Entities;

namespace Novvi.Infrastructure.Persistence.Configurations;

public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.ToTable("PAGAMENTOS");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.TipoPagamento).IsRequired();
        builder.HasIndex(p => p.IdPedido).IsUnique();
    }
}
