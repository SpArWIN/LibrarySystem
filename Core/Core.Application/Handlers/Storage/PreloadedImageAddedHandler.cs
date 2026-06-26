
using Common.Cached.Repositoryies;
using Common.Messaging.Nats.Contracts.Files;
using Common.Messaging.Nats.Handlers;
using Core.Application.Caching;
using Core.Application.Options;
using Microsoft.Extensions.Options;

namespace Core.Application.Handlers.Storage;

/// <summary>
/// Сохраняет в кеш метаданные и публичный URL предзагруженного изображения из Storage.
/// </summary>
public sealed class PreloadedImageAddedHandler(
    ICachedRepository<PreloadedImageCacheEntry> cache,
    IOptions<PreloadedImageCacheOptions> options)
    : NatsMessageHandler<PreloadedImageAdded>
{
    /// <inheritdoc />
    public override async Task HandleAsync(PreloadedImageAdded message, CancellationToken cancellationToken = default)
    {
        var key = PreloadedImageCacheKeys.For(message.Bucket, message.ObjectKey);
        Logger.Information("-> LoadPreloadedImage with key {@Key} on time {@time}", key, options.Value.UrlCacheTtl);
        var entry = new PreloadedImageCacheEntry(
            message.Bucket,
            message.ObjectKey,
            message.PublicUrl,
            message.ContentType,
            message.SizeBytes,
            message.SourceFileName,
            message.UploadedAtUtc);
        await cache.SetAsync(key, entry, options.Value.UrlCacheTtl, cancellationToken);
        Logger.Information("<- LoadPreloadedImage");
    }
}
