using System;
using Routes.Data.Implementations;
using Routes.Data.Repositories;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Routes.API.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddCustomRepository(
        this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();

        // Repositories
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IBaseRepository<Rota>, BaseRepository<Rota>>();
        services.AddScoped<IBaseRepository<MotoristaRota>, BaseRepository<MotoristaRota>>();
        services.AddScoped<IBaseRepository<Endereco>, BaseRepository<Endereco>>();
        services.AddScoped<IBaseRepository<RotaHistorico>, BaseRepository<RotaHistorico>>();
        services.AddScoped<IRotaHistoricoRepository, RotaHistoricoRepository>();
        services.AddScoped<IBaseRepository<Veiculo>, BaseRepository<Veiculo>>();

        Console.WriteLine("Configuração de repository realizada com sucesso!");

        return services;
    }
}