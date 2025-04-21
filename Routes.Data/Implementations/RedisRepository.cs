using System;
using System.Threading.Tasks;
using Routes.Domain.Interfaces.Repositories;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Routes.Data.Implementations;

public class RedisRepository : IRedisRepository
{
    private readonly IDatabase _db;

    public RedisRepository(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public Task SetAsync<T>(string key, T value, int? expirationInMinutes = null)
    {
        TimeSpan? expiry = expirationInMinutes.HasValue ? TimeSpan.FromMinutes(expirationInMinutes.Value) : null;
        return _db.StringSetAsync(key, JsonConvert.SerializeObject(value), expiry);
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? JsonConvert.DeserializeObject<T>(value.ToString()) : default;
    }

    public Task RemoveAsync(string key)
        => _db.KeyDeleteAsync(key);
}