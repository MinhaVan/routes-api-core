using Routes.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Routes.Data.Configurations;

public class AlunoRotaHistoricoConfiguration : IEntityTypeConfiguration<AlunoRotaHistorico>
{
    public void Configure(EntityTypeBuilder<AlunoRotaHistorico> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();
        modelBuilder.ToTable("alunoRotaHistorico");
        modelBuilder.HasOne(x => x.RotaHistorico)
            .WithMany(y => y.AlunoRotaHistorico)
            .HasForeignKey(x => x.RotaHistoricoId);
    }
}