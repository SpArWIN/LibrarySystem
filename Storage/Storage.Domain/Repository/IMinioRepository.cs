using Common.Contracts.Storage.Files;
using Minio.DataModel;

namespace Storage.Domain.Repository;

/// <summary>
/// Репозиторий взаимодействия с IMinio.
/// </summary>
public interface IMinioRepository
{
    /// <summary>
    /// Загрузить файл.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="objectName">Имя объекта (файла).</param>
    /// <param name="fileStream">Поток данных файла.</param>
    /// <param name="contentType">Тип содержимого (например, image/jpeg).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>.</returns>
    Task UploadFileAsync(string bucketName, string objectName, Stream fileStream, string contentType,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Загрузить файл из хранилища.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="objectName">Имя объекта.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Поток с содержимым файла.</returns>
    Task<Stream> DownloadFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Сгенерировать Url для файла.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="objectName">Имя объекта.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>URL/</returns>
    Task<string> GetImageUrlAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить список всех бакетов
    /// </summary>
    Task<IEnumerable<Bucket>> ListBucketsAsync(
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить список файлов в бакете
    /// </summary>
    Task<IEnumerable<FileDescriptor>> ListFilesAsync(
        string bucketName, 
        CancellationToken cancellationToken = default);
    
    /// <summary>Удалить объект (если существует).</summary>
    Task<bool> DeleteFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить список файлов во всех бакетах
    /// </summary>
    Task<IEnumerable<FileDescriptor>> ListAllFilesAsync(
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить тип файла.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="objectName">Имя файла.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Тип файла.</returns>
    Task<string?> GetContentAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить, существует ли бакет.
    /// </summary>
    /// <param name="bucketName">Имя бакетаю</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task EnsureBucketAsync(string bucketName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Существует ли файл в бакете.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="objectName">Имя объекта.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>True- есть, False- нет.</returns>
    Task<bool> ExistsAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удалить бакет, если там ничего нет.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>True, False.</returns>
    Task<bool> DeleteBucketIfEmptyAsync(string bucketName, CancellationToken cancellationToken = default);
}