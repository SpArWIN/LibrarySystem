namespace Core.Application.Caching;

/// <summary>
/// Ключи кеша для предзагруженных изображений.
/// </summary>
public static class PreloadedImageCacheKeys
{
    /// <summary>
    /// Ключ по бакету и ключу объекта в хранилище.
    /// </summary>
    public static string For(string bucket, string objectKey) => $"{bucket}:{objectKey}";
}
