using System;
using Microsoft.Extensions.DependencyInjection;
using Routes.Service.Configuration;
using StackExchange.Redis;
using Routes.Domain.Interfaces.Repositories;
using Routes.Data.Implementations;

namespace Routes.API.Extensions;

public static class CacheExtensions
{
    public static IServiceCollection AddCache(this IServiceCollection services, SecretManager secretManager)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = secretManager.Infra.Redis;
            return ConnectionMultiplexer.Connect(configuration);
        });

        services.AddScoped<IRedisRepository, RedisRepository>();
        Console.WriteLine("Configuração do Redis realizada com sucesso!");

        return services;
    }
}