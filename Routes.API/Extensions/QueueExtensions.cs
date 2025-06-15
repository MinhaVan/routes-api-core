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
        services.AddSingleton<IConnectionFactory>(sp =>
        {
            return new ConnectionFactory
            {
                UserName = secretManager.Infra.RabbitMQ.UserName,
                Password = secretManager.Infra.RabbitMQ.Password,
                HostName = secretManager.Infra.RabbitMQ.Host,
                Port = int.Parse(secretManager.Infra.RabbitMQ.Port)
            };
        });

        services.AddScoped<IRabbitMqRepository, RabbitMqRepository>();
        return services;
    }
}