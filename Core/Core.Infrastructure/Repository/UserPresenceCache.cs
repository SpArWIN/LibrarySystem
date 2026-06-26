using BloomFilter;
using Common.Cached.Configurations;
using Common.Extensions;
using Core.Domain.Repository;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Repository;

/// <inheritdoc />
public sealed class UserPresenceCache : IUserPresenceCache
{
    private readonly IBloomFilter _bloomFilter;
    private readonly BloomUserFilterOptions _options;

    public UserPresenceCache(
        IBloomFilter bloomFilter,
        IOptions<BloomUserFilterOptions> options)
    {
        _bloomFilter = bloomFilter;
        _options = options.Value;
        
    }
    
    public async Task<bool> MightExistAsync(string userName, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return true;
        }
        return await _bloomFilter.ContainsAsync(userName);
    }

    /// <inheritdoc />
    public async Task AddAsync(string username, CancellationToken ct = default)
    {
        if (!_options.Enabled)
        {
            return;
        }
        await _bloomFilter.AddAsync(username);
    }
    

    /// <inheritdoc />
    public async Task AddRangeAsync(IEnumerable<string> usernames, CancellationToken ct = default)
    {
        if (!_options.Enabled)
        {
            return;
        }
        await usernames.ForEachAsync(async (userName, _) =>
        {
            await _bloomFilter.AddAsync(userName);
        }, ct);
    }
}