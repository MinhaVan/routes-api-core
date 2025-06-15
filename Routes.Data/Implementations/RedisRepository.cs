using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Routes.Domain.Interfaces.Repositories;
using StackExchange.Redis;

namespace Routes.Data.Implementations;

public class RedisRepository : IRedisRepository
{
    private readonly IDatabase _db;

    public RedisRepository(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, int durationInMinutes = 30)
    {
        await DeleteAsync(key);

        var expiry = TimeSpan.FromMinutes(durationInMinutes);

        await _db.StringSetAsync(
            key,
            JsonConvert.SerializeObject(value),
            expiry: expiry
        );
    }

    public async Task DeleteAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(value);
    }
}