namespace Storage.Application.Services;

/// <summary>
/// Вспомогательный сервис работы с бакетами.
/// </summary>
public interface IBucketHelperService
{
    /// <summary>
    /// Проверить, существует ли бакет.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>.</returns>
    Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Существует ли объект в бакете.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="objectName">Имя файла.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>True/False.</returns>
    Task<bool> ExistFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удалить файл.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="objectName">Имя объекта.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>True- удалено, False- нет.</returns>
    Task<bool> DeleteFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удалить бакет, если он пустой.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>True- Удален, False- Нет.</returns>
    Task<bool> DeleteBucketIfEmptyAsync(string bucketName, CancellationToken cancellationToken = default);
}