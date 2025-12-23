using Storage.Domain.Repository;

namespace Storage.Application.Services;

public sealed class BucketHelperService : IBucketHelperService
{
    private readonly IMinioRepository _minioRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="minioRepository"></param>
    public BucketHelperService(IMinioRepository minioRepository)
    {
        _minioRepository = minioRepository;
    }

    /// <inheritdoc />
    public async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        await _minioRepository.EnsureBucketAsync(bucketName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExsistFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        return await _minioRepository.ExistsAsync(bucketName, objectName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        return await _minioRepository.DeleteFileAsync(bucketName, objectName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteBucketIfEmptyAsync(string bucketName, CancellationToken cancellationToken = default)
    {
      return await _minioRepository.DeleteBucketIfEmptyAsync(bucketName, cancellationToken);
    }
}