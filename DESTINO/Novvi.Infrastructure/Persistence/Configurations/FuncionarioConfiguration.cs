using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Novvi.Domain.Entities;

namespace Novvi.Infrastructure.Persistence.Configurations;

public class FuncionarioConfiguration : IEntityTypeConfiguration<Funcionario>
{
    public void Configure(EntityTypeBuilder<Funcionario> builder)
    {
        builder.ToTable("FUNCIONARIOS");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Nome).IsRequired().HasMaxLength(150);
        builder.Property(f => f.Salario).HasColumnType("NUMBER(18,2)");

        builder.HasMany(f => f.PedidosRealizados)
            .WithOne()
            .HasForeignKey(p => p.IdFuncionario)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
