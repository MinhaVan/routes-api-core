using Routes.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Routes.Data.Configurations;

public class AjusteAlunoRotaConfiguration : IEntityTypeConfiguration<AjusteAlunoRota>
{
    public void Configure(EntityTypeBuilder<AjusteAlunoRota> modelBuilder)
    {
        modelBuilder.ConfigureBaseEntity();
        modelBuilder.ToTable("ajusteAlunoRota");

        modelBuilder.HasOne(x => x.EnderecoPartida)
            .WithMany(y => y.AjusteAlunoRotasPartida)
            .HasForeignKey(x => x.NovoEnderecoPartidaId);

        modelBuilder.HasOne(x => x.EnderecoDestino)
            .WithMany(y => y.AjusteAlunoRotasDestino)
            .HasForeignKey(x => x.NovoEnderecoDestinoId);

        modelBuilder.HasOne(x => x.EnderecoRetorno)
            .WithMany(y => y.AjusteAlunoRotasRetorno)
            .HasForeignKey(x => x.NovoEnderecoRetornoId);
    }
}
