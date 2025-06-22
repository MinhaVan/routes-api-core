using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Routes.Data.Utils;
using Routes.Domain.Interfaces.Repositories;

namespace Routes.Data.Implementations;

public class RabbitMqRepository(
    IConnectionFactory _factory,
    ILogger<RabbitMqRepository> _logger
) : IRabbitMqRepository
{
    public void Publish<T>(string queue, T data, bool shouldThrowException = false)
    {
        try
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", $"{queue}.retry" }
            });

            var message = data.ToJson();
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish("", queue, null, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem na fila {QueueName}", queue);
            if (shouldThrowException)
                throw new Exception($"Erro ao publicar mensagem na fila {queue}", ex);
        }
    }
}