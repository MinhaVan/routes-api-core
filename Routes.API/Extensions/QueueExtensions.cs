using Routes.Data.Implementations;
using Routes.Domain.Interfaces.Repositories;
using Routes.Service.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Routes.API.Extensions;

public static class QueueExtensions
{
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