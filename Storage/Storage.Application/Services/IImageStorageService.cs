using Common.Contracts.Storage.Files;
using Minio.DataModel;

namespace Storage.Application.Services;

/// <summary>
/// Интерфейс сервиса хранения изображений.
/// </summary>
public interface IImageStorageService
{
    /// <summary>
    /// Загрузить изображение в хранилище.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="name">Имя изображения.</param>
    /// <param name="imageStream">Поток изображения.</param>
    /// <param name="contentType">Тип содержимого.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task UploadImageAsync(string bucketName ,string name, Stream imageStream, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить изображение из хранилища.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="name">Имя изображения.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Поток изображения.</returns>
    Task<Stream> GetImageAsync(string bucketName,string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить URL изображения.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="fileName">Имя файла.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Url расположения.</returns>
    Task<string>GetImageUrlAsync(string bucketName,string fileName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить список бакетов.
    /// </summary>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>Список бакетов.</returns>
    Task<IEnumerable<Bucket>> ListBucketsAsync(CancellationToken ct);
    
    /// <summary>
    /// Получить список файлов в бакете.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>Список файлов в бакете.</returns>
    Task<IEnumerable<FileDescriptor>> ListFilesAsync(string bucketName, CancellationToken ct);
    
    /// <summary>
    /// Получить все файлы со всех бакетов.
    /// </summary>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>Список всех файлов со всех бакетов.</returns>
    Task<IEnumerable<FileDescriptor>> ListAllFilesAsync(CancellationToken ct);
    
    /// <summary>
    /// Получить тип файла.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="fileName">Имя файла.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Тип файла.</returns>
    Task<string?> GetContentTypeAsync(string bucketName, string fileName, CancellationToken cancellationToken = default);
}