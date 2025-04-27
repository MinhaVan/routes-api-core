using Routes.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Routes.Data.Configurations;

public class LocalizacaoTrajetoConfiguration : IEntityTypeConfiguration<LocalizacaoTrajeto>
{

    public void Configure(EntityTypeBuilder<LocalizacaoTrajeto> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();

        modelBuilder.ToTable("localizacaoTrajeto");
        modelBuilder.HasOne(x => x.RotaHistorico)
            .WithMany(y => y.LocalizacaoTrajeto)
            .HasForeignKey(x => x.RotaHistoricoId);
    }
}