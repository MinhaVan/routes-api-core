using Routes.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Routes.Data.Configurations;

public class MotoristaRotaConfiguration : IEntityTypeConfiguration<MotoristaRota>
{
    public void Configure(EntityTypeBuilder<MotoristaRota> modelBuilder)
    {
        modelBuilder.HasKey(x => new { x.MotoristaId, x.RotaId });
        modelBuilder.ToTable("motorista_rota");
        modelBuilder.HasOne(x => x.Rota)
            .WithMany(y => y.MotoristaRotas)
            .HasForeignKey(x => x.RotaId);
    }
}