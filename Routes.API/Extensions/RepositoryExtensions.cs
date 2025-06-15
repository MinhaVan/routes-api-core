using System;
using System.Linq;
using Routes.Data.Implementations;
using Routes.Data.Repositories;
using Routes.Domain.Interfaces.Repositories;
using Routes.Domain.Interfaces.Repository;
using Routes.Domain.Models;
using Routes.Service.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Routes.API.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddCustomRepository(
        this IServiceCollection services,
        SecretManager secretManager)
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

        services.AddQueue(secretManager);

        Console.WriteLine("Configuração de repository realizada com sucesso!");

        return services;
    }

    public static IServiceCollection AddQueue(this IServiceCollection services, SecretManager secretManager)
    {
        var configuration = secretManager.Infra.RabbitMQ.Split(',');

        services.AddSingleton<IConnectionFactory>(sp =>
        {
            return new ConnectionFactory
            {
                UserName = configuration[0],
                Password = configuration[1],
                HostName = configuration[2],
                Port = int.Parse(configuration[3])
            };
        });

        services.AddScoped<IRabbitMqRepository, RabbitMqRepository>();
        return services;
    }
}