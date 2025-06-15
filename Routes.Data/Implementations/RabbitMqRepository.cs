using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Routes.Domain.Interfaces.Repositories;

namespace Routes.Data.Implementations;

public class RabbitMqRepository : IRabbitMqRepository
{
    private readonly IConnectionFactory _factory;

    public RabbitMqRepository(IConnectionFactory factory)
    {
        _factory = factory;
    }

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
        catch (System.Exception ex)
        {
            if (shouldThrowException)
                throw;
        }
    }
}