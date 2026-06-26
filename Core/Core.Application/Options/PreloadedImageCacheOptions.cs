namespace Core.Application.Options;

/// <summary>
/// TTL кеша публичных URL предзагруженных изображений.
/// </summary>
public sealed class PreloadedImageCacheOptions
{
    /// <summary>
    /// Время хранения записи в распределённом кеше.
    /// </summary>
    public TimeSpan UrlCacheTtl { get; init; } = TimeSpan.FromHours(23);
}
