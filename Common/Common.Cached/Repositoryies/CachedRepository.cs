using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Common.Cached.Configurations;
using Common.Cached.PrefixStrategy;
using Common.Extensions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Common.Cached.Repositoryies;

public sealed class CachedRepository<T> : ICachedRepository<T>
where T : class
{
    private readonly IDatabase _database;
    private readonly string _prefix;
    private readonly TimeSpan _defaultTtl;
    private readonly bool _enableCompression;
    private readonly JsonSerializerOptions _jsonOptions;


    public CachedRepository(
        IConnectionMultiplexer redis,
        IOptions<RedisCacheOptions> cacheOptions,
        ICachedResolver prefixResolver)
    {
        _database = redis.GetDatabase();
        _defaultTtl = cacheOptions.Value.DefaultTtl;
        _enableCompression = cacheOptions.Value.EnableCompression;
        var basePrefix = prefixResolver.GetPrefix<T>();
        var instancePrefix = cacheOptions.Value.InstanceName;
        _prefix = instancePrefix.IsNullOrEmpty()
            ? basePrefix
            : $"{instancePrefix}:{basePrefix}";

        _jsonOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }


    public async Task<T?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
       var value = await _database.StringGetAsync(BuildKey(key));
       return await DeserializeAsync(value, cancellationToken);
    }

    public async Task<IEnumerable<T?>> GetManyAsync(IEnumerable<string> keys, CancellationToken ct = default)
    {
       var redisKeys = keys.Select(BuildKey).Select(x=>(RedisKey)x).ToArray();
       var values = await _database.StringGetAsync(redisKeys);
       var results = new List<T?>();
       await values.ForEachAsync(async (redisValue, token) =>
       {
           results.Add( await DeserializeAsync(redisValue, token));
       }, cancellationToken: ct);
       return results;
    }

    /// <inheritdoc />
    public async Task SetAsync(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default)
    {
       var serialized = await SerializeAsync(value, ct);
       await _database.StringSetAsync(BuildKey(key), serialized, ttl ?? _defaultTtl);
    }

    /// <inheritdoc />
    public async Task SetManyAsync(IDictionary<string, T> values, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        var entries = new List<KeyValuePair<RedisKey, RedisValue>>();

        await values.ForEachAsync(async (kv, token) =>
        {
            var serialized = await SerializeAsync(kv.Value, token);
            entries.Add(new KeyValuePair<RedisKey, RedisValue>(BuildKey(kv.Key), serialized));
        }, cancellationToken: ct);
        await _database.StringSetAsync(entries.ToArray());

        if (ttl.HasValue)
        {
            await values.ForEachAsync(async (key, _) =>
            {
                await _database.KeyExpireAsync(BuildKey(key.Key), ttl.Value);
            }, cancellationToken: ct);
        }
    }

    /// <inheritdoc />
    public async Task RemoveManyAsync(IEnumerable<string> keys, CancellationToken ct = default)
    {
        var redisKeys = keys.Select(BuildKey).Select(x=>(RedisKey)x).ToArray();
        await _database.KeyDeleteAsync(redisKeys);
    }
    

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        return await _database.KeyExistsAsync(BuildKey(key));
    }

    /// <inheritdoc />
    public Task<IEnumerable<string>> GetKeysAsync(string pattern, CancellationToken ct = default)
    {
       var endPoints = _database.Multiplexer.GetEndPoints();
       var server = _database.Multiplexer.GetServer(endPoints.First());
       var redisPattern = $"{_prefix}{pattern}*";
       var keys = server.Keys(pattern:redisPattern);
       return Task.FromResult(keys.Select(k => k.ToString().Replace(_prefix, string.Empty)));
    }

    /// <inheritdoc />
    public async Task RemoveByPatternAsync(string pattern, CancellationToken ct = default)
    {
        var keys = await GetKeysAsync(pattern, ct);
        await RemoveManyAsync(keys, ct);
    }
    
    private string BuildKey(string key) => $"{_prefix}{key}";

    
    private async Task<RedisValue> SerializeAsync(T value, CancellationToken cancellationToken = default)
    {
        var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
        if (_enableCompression)
        {
            var bytes = Encoding.UTF8.GetBytes(serializedValue);
            var compressed = await CompressAsync(bytes, cancellationToken);
            return compressed;
        }
        return serializedValue;
    }


    private async Task<T?> DeserializeAsync(RedisValue value, CancellationToken cancellationToken = default)
    {
        if (value.IsNullOrEmpty)
        {
            return default;
        }

        string json;
        if (_enableCompression && value.HasValue)
        {
            var bytes = (byte[])value;
            var decompressed = await DecompressAsync(bytes, cancellationToken);
            json = Encoding.UTF8.GetString(decompressed);
        }
        else
        {
            json = value.ToString();
        }
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }
    private static async Task<byte[]> CompressAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        using var compressed = new MemoryStream();
        await using var gzip = new GZipStream(compressed, CompressionMode.Compress);
        await gzip.WriteAsync(data, cancellationToken);
        return compressed.ToArray();
    }

    private static async Task<byte[]> DecompressAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        using var decompressed = new MemoryStream(data);
        await using var gzip = new GZipStream(decompressed, CompressionMode.Decompress);
        using var outPut  = new MemoryStream();
        await gzip.CopyToAsync(outPut, cancellationToken);
        return outPut.ToArray();
    }
}