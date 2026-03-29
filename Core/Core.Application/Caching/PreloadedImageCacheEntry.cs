namespace Core.Application.Caching;

/// <summary>
/// Снимок метаданных предзагруженного изображения для кеша (не источник правды — объект остаётся в MinIO).
/// </summary>
public sealed record PreloadedImageCacheEntry(
    string Bucket,
    string ObjectKey,
    string PublicUrl,
    string? ContentType,
    long? SizeBytes,
    string? SourceFileName,
    DateTime UploadedAtUtc);
