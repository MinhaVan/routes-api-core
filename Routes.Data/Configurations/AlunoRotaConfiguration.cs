using Routes.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Routes.Data.Configurations;

public class AlunoRotaConfiguration : IEntityTypeConfiguration<AlunoRota>
{
    public void Configure(EntityTypeBuilder<AlunoRota> modelBuilder)
    {
        modelBuilder.HasKey(x => new { x.AlunoId, x.RotaId });
        modelBuilder.ToTable("alunoRota");
        modelBuilder.HasOne(x => x.Rota)
            .WithMany(y => y.AlunoRotas)
            .HasForeignKey(x => x.RotaId);
    }
}