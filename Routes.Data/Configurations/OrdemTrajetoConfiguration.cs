using Routes.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Routes.Data.Configurations;

public class OrdemTrajetoConfiguration : IEntityTypeConfiguration<OrdemTrajeto>
{
    public void Configure(EntityTypeBuilder<OrdemTrajeto> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();
        modelBuilder.ToTable("ordem_trajeto");

        modelBuilder.HasOne(x => x.Rota)
            .WithMany(y => y.OrdemTrajetos)
            .HasForeignKey(x => x.RotaId);

        modelBuilder.HasMany(x => x.Marcadores)
            .WithOne(y => y.OrdemTrajeto)
            .HasForeignKey(x => x.OrdemTrajetoId);
    }
}