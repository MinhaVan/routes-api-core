using System.Threading.Tasks;

namespace Routes.Domain.Interfaces.Repositories;

public interface IRedisRepository
{
    Task SetAsync<T>(string key, T value, int durationInMinutes = 30);
    Task DeleteAsync(string key);
    Task<T> GetAsync<T>(string key);
}