using System.Collections.Concurrent;
using System.Text.Json;
using Common.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace Common.Cached.Service;

/// <inheritdoc />
public sealed class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _keyLocks;

    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="cache">.</param>
    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
        _keyLocks = new ConcurrentDictionary<string, SemaphoreSlim>(1, 1 );
    }
    
    /// <inheritdoc />
    public async Task<TValue?> GetAsync<TKey, TValue>(TKey key, CancellationToken cancellationToken = default) where TKey : notnull
    {
        var cacheKey = GetCacheKey(key);
        var value = await _cache.GetStringAsync(cacheKey, cancellationToken);
        
        if (value.IsNullOrEmpty())
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<TValue>(value!);
        }
        catch (JsonException)
        {
           return default;
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<TKey, TValue>(TKey key,
        TValue value, TimeSpan? expiration = null, 
        CancellationToken ct = default) 
        where TValue : notnull
        where TKey : notnull
    {
        var cacheKey = GetCacheKey(key);
        var jsonValue = JsonSerializer.Serialize(value);
        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }
        await _cache.SetStringAsync(cacheKey, jsonValue, options, ct);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveAsync<TKey>(TKey key, CancellationToken ct = default) where TKey : notnull
    {
        var cacheKey = GetCacheKey(key);
        await _cache.RemoveAsync(cacheKey, ct);
        var exists = await _cache.GetStringAsync(cacheKey, ct);
        return exists.IsNullOrEmpty();
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync<TKey>(TKey key, CancellationToken ct = default) where TKey : notnull
    {
        var cacheKey = GetCacheKey(key);
        var value = await _cache.GetStringAsync(cacheKey, ct);
        return value.IsNullOrEmpty();
    }

    /// <inheritdoc />
    public async Task<TValue> GetOrCreateAsync<TKey, TValue>(TKey key, Func<Task<TValue>> factory, TimeSpan? expiration = null,
        CancellationToken ct = default) where TKey : notnull where TValue : class
    {
        var cacheKey = GetCacheKey(key);
        await GetAsync<TKey, TValue>(key, ct);
        var keyLock = _keyLocks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));
        await keyLock.WaitAsync(ct);
        try
        {
            var value = await factory();
            await SetAsync(cacheKey, value, expiration, ct);

            return value;
        }
        finally
        {
            keyLock.Release();
            if (keyLock.CurrentCount == 1)
            {
                _keyLocks.TryRemove(cacheKey, out _);
            }
        }
    }

    private static string GetCacheKey<TKey>(TKey key)
        where TKey : notnull
        => key switch
        {
            string keyString => keyString,
            int keyInt => keyInt.ToString(),
            Guid keyGuid => keyGuid.ToString(),
            _ => JsonSerializer.Serialize(key)
        };
}