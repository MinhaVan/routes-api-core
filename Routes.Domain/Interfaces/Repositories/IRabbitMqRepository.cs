namespace Routes.Domain.Interfaces.Repositories;

public interface IRabbitMqRepository
{
    void Publish<T>(string queue, T data, bool shouldThrowException = true);
}