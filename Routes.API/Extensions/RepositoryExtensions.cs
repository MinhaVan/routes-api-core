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
        services.AddScoped<IBaseRepository<Endereco>, BaseRepository<Endereco>>();
        services.AddScoped<IBaseRepository<RotaHistorico>, BaseRepository<RotaHistorico>>();
        services.AddScoped<IRotaHistoricoRepository, RotaHistoricoRepository>();
        services.AddScoped<IBaseRepository<Veiculo>, BaseRepository<Veiculo>>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IBaseRepository<UsuarioPermissao>, BaseRepository<UsuarioPermissao>>();
        services.AddScoped<IBaseRepository<Permissao>, BaseRepository<Permissao>>();
        services.AddScoped<IBaseRepository<Aluno>, BaseRepository<Aluno>>();
        services.AddScoped<IRedisRepository, RedisRepository>();

        services.AddQueue(secretManager);

        Console.WriteLine("Configuração de repository realizada com sucesso!");

        return services;
    }

    public static IServiceCollection AddQueue(this IServiceCollection services, SecretManager secretManager)
    {
        var connection = secretManager.ConnectionStrings.RabbitConnection.Split(':');

        services.AddSingleton(sp =>
            new ConnectionFactory
            {
                HostName = connection.ElementAt(0), //"localhost",
                Port = int.Parse(connection.ElementAt(1)), // 5672,
                UserName = connection.ElementAt(2), // admin
                Password = connection.ElementAt(3) // admin
            }
        );

        services.AddScoped<IRabbitMqRepository, RabbitMqRepository>();

        return services;
    }
}