using System.Threading.Tasks;

namespace Routes.Domain.Interfaces.Repositories;

public interface IRabbitMqRepository
{
    Task PublishAsync<T>(string queue, T message);
}