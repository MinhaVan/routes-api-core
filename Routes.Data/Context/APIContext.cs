using Microsoft.EntityFrameworkCore;
using Routes.Domain.Models;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Threading;
using Routes.Data.Configurations;

namespace Routes.Data.Context;

public class APIContext : DbContext
{
    public APIContext(DbContextOptions<APIContext> options) : base(options)
    { }

    public override int SaveChanges()
    {
        AtualizarDatas();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AtualizarDatas();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MotoristaRotaConfiguration());
        modelBuilder.ApplyConfiguration(new MotoristaConfiguration());
        modelBuilder.ApplyConfiguration(new AlunoRotaConfiguration());
        modelBuilder.ApplyConfiguration(new VeiculoConfiguration());
        modelBuilder.ApplyConfiguration(new OrdemTrajetoConfiguration());
        modelBuilder.ApplyConfiguration(new OrdemTrajetoMarcadorConfiguration());
        modelBuilder.ApplyConfiguration(new EmpresaConfiguration());
        modelBuilder.ApplyConfiguration(new RotaConfiguration());
        modelBuilder.ApplyConfiguration(new RotaHistoricoConfiguration());
        modelBuilder.ApplyConfiguration(new AlunoRotaHistoricoConfiguration());
        modelBuilder.ApplyConfiguration(new EnderecoConfiguration());
        modelBuilder.ApplyConfiguration(new AjusteAlunoRotaConfiguration());
        modelBuilder.ApplyConfiguration(new LocalizacaoTrajetoConfiguration());

        base.OnModelCreating(modelBuilder);
    }
    private void AtualizarDatas()
    {
        var now = DateTime.UtcNow;
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Entity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                ((Entity)entry.Entity).DataCriacao = now;
                ((Entity)entry.Entity).Status = Domain.Enums.StatusEntityEnum.Ativo;
            }
            ((Entity)entry.Entity).DataAlteracao = now;
        }
    }

    public DbSet<AjusteAlunoRota> AjusteAlunoRotas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Rota> Rotas { get; set; }
    public DbSet<RotaHistorico> RotaHistoricos { get; set; }
    public DbSet<LocalizacaoTrajeto> LocalizacaoTrajetos { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }
    public DbSet<AlunoRota> AlunoRotas { get; set; }
    public DbSet<MotoristaRota> MotoristaRotas { get; set; }
    public DbSet<Motorista> Motoristas { get; set; }
    public DbSet<Permissao> Permissoes { get; set; }
    public DbSet<UsuarioPermissao> UsuarioPermissoes { get; set; }
    public DbSet<AlunoRotaHistorico> AlunoRotaHistoricos { get; set; }
    public DbSet<OrdemTrajeto> OrdemTrajetos { get; set; }
}