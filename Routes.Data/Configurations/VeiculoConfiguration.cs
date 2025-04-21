using Routes.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Routes.Data.Configurations;

public class VeiculoConfiguration : IEntityTypeConfiguration<Veiculo>
{
    public void Configure(EntityTypeBuilder<Veiculo> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();
        modelBuilder.ToTable("veiculos");
        modelBuilder.HasOne(x => x.Empresa)
            .WithMany(y => y.Veiculos)
            .HasForeignKey(x => x.EmpresaId);
        modelBuilder.HasMany(x => x.Rotas)
            .WithOne(y => y.Veiculo)
            .HasForeignKey(x => x.VeiculoId);
    }
}