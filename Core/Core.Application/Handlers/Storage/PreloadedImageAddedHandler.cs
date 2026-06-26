using Common.Cached.Service;
using Common.Messaging.Nats.Contracts.Files;
using Common.Messaging.Nats.Handlers;
using Core.Application.Caching;
using Core.Application.Options;
using Microsoft.Extensions.Options;

namespace Core.Application.Handlers.Storage;

/// <summary>
/// Сохраняет в кеш метаданные и публичный URL предзагруженного изображения из Storage.
/// </summary>
public sealed class PreloadedImageAddedHandler : NatsMessageHandler<PreloadedImageAdded>
{
    private readonly ICacheService _cache;
    private readonly IOptions<PreloadedImageCacheOptions> _options;

    public PreloadedImageAddedHandler(ICacheService cache, IOptions<PreloadedImageCacheOptions> options)
    {
        _cache = cache;
        _options = options;
    }

    /// <inheritdoc />
    public override async Task HandleAsync(PreloadedImageAdded message, CancellationToken cancellationToken = default)
    {
        var key = PreloadedImageCacheKeys.For(message.Bucket, message.ObjectKey);
        Logger.Information("-> LoadPreloadedImage with key {@Key} on time {@time}", key, _options.Value.UrlCacheTtl);
        var entry = new PreloadedImageCacheEntry(
            message.Bucket,
            message.ObjectKey,
            message.PublicUrl,
            message.ContentType,
            message.SizeBytes,
            message.SourceFileName,
            message.UploadedAtUtc);
        await _cache.SetAsync(key, entry, _options.Value.UrlCacheTtl, cancellationToken);
        Logger.Information("<- LoadPreloadedImage");
    }
}
