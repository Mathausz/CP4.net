using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Novvi.Domain.Entities;

namespace Novvi.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("USUARIOS");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome).IsRequired().HasMaxLength(150);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasMany(u => u.Enderecos)
            .WithOne()
            .HasForeignKey(e => e.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.HistoricoPedidos)
            .WithOne()
            .HasForeignKey(p => p.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
