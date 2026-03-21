using Common.Messaging.Nats.Contracts.Files;

namespace Storage.Application.Abstractions;

/// <summary>
/// Клиент публикации событий Nats.
/// </summary>
public interface INatsStorage
{
    /// <summary>
    /// Опубликовать предзаданное изображение.
    /// </summary>
    /// <param name="evt"><see cref="PreloadedImageAdded"/>.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>.</returns>
    Task PublishPreloadedImageAddedAsync(PreloadedImageAdded evt, CancellationToken ct = default);
}