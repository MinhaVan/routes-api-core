using System.Collections.Generic;
using System.Threading.Tasks;

namespace Routes.Domain.Interfaces.Repositories;

public interface IRedisRepository
{
    Task SetAsync<T>(string key, T value, string group = null, int durationInMinutes = 30);
    Task<List<T>> GetListAsync<T>(List<string> keys);
    Task<T> GetAsync<T>(string key);
    Task DeleteAsync(string key);
    Task RemoveGroupAsync(string group);
}