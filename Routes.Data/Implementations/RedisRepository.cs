using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Routes.Domain.Interfaces.Repositories;
using StackExchange.Redis;

namespace Routes.Data.Implementations;

public class RedisRepository : IRedisRepository
{
    private readonly IDatabase _db;
    private const string Prefix = "Routes:";

    public RedisRepository(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    private string FormatKey(string key) => $"{Prefix}{key}";

    public async Task SetAsync<T>(string key, T value, string group = null, int durationInMinutes = 30)
    {
        key = FormatKey(key);
        await DeleteAsync(key);

        var expiry = TimeSpan.FromMinutes(durationInMinutes);
        string json = JsonConvert.SerializeObject(value);

        await _db.StringSetAsync(key, json, expiry);

        if (!string.IsNullOrEmpty(group))
        {
            string groupKey = FormatKey($"group:{group}");
            await _db.SetAddAsync(groupKey, key);
            await _db.KeyExpireAsync(groupKey, expiry);
        }
    }

    public async Task<List<T>> GetListAsync<T>(List<string> keys)
    {
        var redisKeys = keys.Select(k => (RedisKey)FormatKey(k)).ToArray();
        var values = await _db.StringGetAsync(redisKeys);

        var list = new List<T>();
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i].HasValue)
            {
                list.Add(JsonConvert.DeserializeObject<T>(values[i]));
            }
        }
        return list;
    }

    public async Task<T> GetAsync<T>(string key)
    {
        key = FormatKey(key);
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
            return default;

        return JsonConvert.DeserializeObject<T>(value);
    }

    public async Task DeleteAsync(string key)
    {
        key = FormatKey(key);
        await _db.KeyDeleteAsync(key);
    }

    public async Task RemoveGroupAsync(string group)
    {
        string groupKey = FormatKey($"group:{group}");
        var keys = await _db.SetMembersAsync(groupKey);

        foreach (var redisKey in keys)
        {
            await _db.KeyDeleteAsync(redisKey.ToString());
        }

        await _db.KeyDeleteAsync(groupKey);
    }
}
