using Common.Contracts.Storage.Files;
using Minio.DataModel;
using Serilog;
using Storage.Domain.Repository;

namespace Storage.Application.Services;

/// <summary>
/// Сервис хранения изображений.
/// </summary>
public sealed class ImageStorageService : IImageStorageService
{
    private readonly IMinioRepository _minioRepository;
    
    private static readonly ILogger Logger = Log.ForContext<ImageStorageService>();
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="minioRepository"><see cref="IMinioRepository"/>.</param>
    public ImageStorageService(IMinioRepository minioRepository)
    {
        _minioRepository = minioRepository;
    }
    
    /// <inheritdoc />
    public async Task UploadImageAsync(string bucketName, string name, Stream imageStream, string contentType,
        CancellationToken cancellationToken = default)
    {
        Logger.Information("-> UploadImageAsync {bucketName}, ImageName: {img}, Type: {type}"
            , bucketName, name, contentType);
        await _minioRepository.UploadFileAsync(bucketName, name, imageStream, contentType, cancellationToken);
        Logger.Information("<- UploadImageAsync");
    }
    
    /// <inheritdoc />
    public async Task<Stream> GetImageAsync(string bucketName, string name, CancellationToken cancellationToken = default)
    {
        Logger.Information("-> GetImageAsync {bucketName}, ImageName: {img}", bucketName, name);
        return await _minioRepository.DownloadFileAsync(bucketName, name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> GetImageUrlAsync(string bucketName, string fileName, CancellationToken cancellationToken = default)
    {
        Logger.Information("-> GetImageAsync {bucketName}, ImageName: {img}", bucketName, fileName);
        return await _minioRepository.GetImageUrlAsync(bucketName, fileName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Bucket>> ListBucketsAsync(CancellationToken ct)
    {
        Logger.Information("-> ListBucketsAsync");
        var buckets = await _minioRepository.ListBucketsAsync(ct);
        var listBucketsAsync = buckets.ToList();
        var infoBuckets = listBucketsAsync
          .Select(x => new
          {
              x.Name,
              x.CreationDate,
          });
      Logger.Information("<- ListBucketsAsync : {infoBuckets}", infoBuckets);
      return listBucketsAsync;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FileDescriptor>> ListFilesAsync(string bucketName, CancellationToken ct)
    {
        return await _minioRepository.ListFilesAsync(bucketName, ct);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FileDescriptor>> ListAllFilesAsync(CancellationToken ct)
    {
        return await _minioRepository.ListAllFilesAsync(ct);
    }

    /// <inheritdoc />
    public async Task<string?> GetContentTypeAsync(string bucketName, string fileName, CancellationToken cancellationToken = default)
    {
        return await _minioRepository.GetContentAsync(bucketName, fileName, cancellationToken);
    }
}