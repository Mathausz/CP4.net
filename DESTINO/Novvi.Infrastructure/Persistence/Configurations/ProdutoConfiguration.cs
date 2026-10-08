using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Novvi.Domain.Entities;

namespace Novvi.Infrastructure.Persistence.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("PRODUTOS");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Descricao).IsRequired().HasMaxLength(500);
        builder.Property(p => p.Preco).HasColumnType("NUMBER(18,2)");
    }
}
