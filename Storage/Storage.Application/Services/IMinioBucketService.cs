namespace Storage.Application.Services;

/// <summary>
/// Интерфейс сервиса работы с бакетами.
/// </summary>
public interface IMinioBucketService
{
    /// <summary>
    /// Запустить.
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>.</returns>
    Task StartAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверить, существует ли бакет.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>.</returns>
    Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удалить бакет, если там ничего нет.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<bool> DeleteBucketIfEmptyAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить существует ли объект в бакете.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="objectName">Имя объекта (файла).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если объект существует, иначе false.</returns>
    Task<bool> ExistsAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
}