using Core.Application.Caching;

namespace Core.Application.Services.PreloadedImages;

/// <summary>
/// Высокоуровневый сервис для получения URL и метаданных предзагруженных изображений.
/// </summary>
public interface IPreloadedImageUrlService
{
    /// <summary>
    /// Получить информацию о предзагруженном изображении.
    /// </summary>
    /// <param name="bucket">Имя бакета в MinIO.</param>
    /// <param name="objectKey">Полный ключ объекта (как в сообщении <c>PreloadedImageAdded.ObjectKey</c>).</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>Информация о файле или null, если объект не существует.</returns>
    Task<PreloadedImageCacheEntry?> GetAsync(string bucket, string objectKey, CancellationToken ct = default);
}
