using Routes.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Routes.Data.Configurations;

public class OrdemTrajetoMarcadorConfiguration : IEntityTypeConfiguration<OrdemTrajetoMarcador>
{
    public void Configure(EntityTypeBuilder<OrdemTrajetoMarcador> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();
        modelBuilder.ToTable("ordemTrajetoMarcador");
        modelBuilder.HasOne(x => x.OrdemTrajeto)
            .WithMany(y => y.Marcadores)
            .HasForeignKey(x => x.OrdemTrajetoId);
    }
}