using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Novvi.Domain.Entities;

namespace Novvi.Infrastructure.Persistence.Configurations;

public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.ToTable("ENDERECOS");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Cep).IsRequired().HasMaxLength(8);
        builder.Property(e => e.Complemento).IsRequired().HasMaxLength(200);
    }
}
