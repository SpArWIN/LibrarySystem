using Common.Cached.Repositoryies;
using Core.Application.Caching;

namespace Core.Application.Services.PreloadedImages;

/// <inheritdoc />
public sealed class PreloadedImageUrlService : IPreloadedImageUrlService
{
    private readonly ICachedRepository<PreloadedImageCacheEntry> _cache;

    public PreloadedImageUrlService(ICachedRepository<PreloadedImageCacheEntry> cache)
    {
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<PreloadedImageCacheEntry?> GetAsync(string bucket, string objectKey, CancellationToken ct = default)
    {
        var cacheKey = PreloadedImageCacheKeys.For(bucket, objectKey);
        return await _cache.GetAsync(cacheKey, ct);
    }
}

