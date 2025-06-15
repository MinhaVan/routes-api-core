using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Routes.Domain.Interfaces.Repositories;

namespace Routes.Data.Implementations;

public class RabbitMqRepository(
    IConnectionFactory _factory,
    ILogger<RabbitMqRepository> _logger
) : IRabbitMqRepository
{
    public void Publish<T>(string queue, T data, bool shouldThrowException = true)
    {
        try
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
            var message = JsonSerializer.Serialize(data);
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