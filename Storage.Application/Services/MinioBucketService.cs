
using Minio.Exceptions;
using Serilog;
using Storage.Application.Extensions;
using Storage.Domain.Buckets;
using Storage.Infrastructure.KeyFolder;

namespace Storage.Application.Services;

/// <summary>
///   Сервис работы с бакетами.
/// </summary>
public sealed class MinioBucketService : IMinioBucketService
{
    private static readonly ILogger Logger = Log.ForContext<MinioBucketService>();
    private const string AddPreload = "PreloadImage";

    private readonly string[] _bucketsToInitialize =
    [
        MinioBuckets.Images,
        MinioBuckets.Avatars,
        MinioBuckets.Documents,
        MinioBuckets.MetaDataImages
    ];

    private readonly Dictionary<string, string> _bucketToFolder;
    private readonly IImageStorageService _imageStorageService;
    private readonly IBucketHelperService _bucketHelperService;

    /// <summary>
    /// Конструктор. 
    /// </summary>
    /// <param name="imageStorageService"></param>
    /// <param name="bucketHelperService"><see cref="IBucketHelperService"/>.</param>
    public MinioBucketService(IImageStorageService imageStorageService,
        IBucketHelperService bucketHelperService)
    {
        _imageStorageService = imageStorageService;
        _bucketHelperService = bucketHelperService;
        var baseDir = AppContext.BaseDirectory;
        _bucketToFolder = new Dictionary<string, string>()
        {
            { MinioBuckets.MetaDataImages, Path.Combine(baseDir, AddPreload, MinioBuckets.MetaDataImages) },
        };
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        Logger.Information("-> Initializing Minio Bucket..");
        foreach (var bucket in _bucketsToInitialize)
        {
            await EnsureBucketExistsAsync(bucket, cancellationToken);
            if (_bucketToFolder.TryGetValue(bucket, out var folderPath))
            {
                await AddPrevertImages(bucket, folderPath, cancellationToken);
            }
            else
            {
                Logger.Information("No preload folder for bucket {bucket}, skipping image upload.", bucket);
            }
        }
        Logger.Information("<- Initialized All Bucket..");
    }

    /// <inheritdoc />
    public async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        await _bucketHelperService.EnsureBucketExistsAsync(bucketName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteBucketIfEmptyAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        return await _bucketHelperService.DeleteBucketIfEmptyAsync(bucketName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string bucketName, string objectName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var status = await _bucketHelperService.ExistFileAsync(bucketName, objectName, cancellationToken);
            return status;
        }
        catch (ObjectNotFoundException)
        {
            Logger.Warning("File {ObjectName} not found in bucket {BucketName}.", objectName, bucketName);
            return false;
        }
    }

    /// <summary>
    /// Добавить все необходимые предзаданные изображения.
    /// </summary>
    /// <param name="bucketName">Имя бакета.</param>
    /// <param name="folderPath">Путь.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    private async Task AddPrevertImages(string bucketName, string folderPath,
        CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(folderPath))
        {
            Logger.Warning("Preload folder not found: {folderPath}", folderPath);
            return;
        }

        var files = Directory.GetFiles(folderPath);
        foreach (var filePath in files)
        {
            try
            {
                var fileName = Path.GetFileName(filePath);
                var objectKey = KeyGenerator.Global(MinioBuckets.MetaDataImages, fileName);
                var exists = await ExistsAsync(bucketName, objectKey, cancellationToken);
                if (exists)
                {
                    Logger.Information(" -> File Exists... {fileName} in bucket {bucketName}", objectKey, bucketName);
                    continue;
                }

                await using var fileStream = File.OpenRead(filePath);
                var contentType = await _imageStorageService.GetContentTypeAsync(bucketName, objectKey, cancellationToken)
                                  ?? 
                                  BucketTypeExtensions.GetContentType(objectKey);
                await _imageStorageService.UploadImageAsync(
                    bucketName,
                    objectKey,
                    fileStream,
                    contentType,
                    cancellationToken
                );
                var messageKey = KeyGenerator.GenerateKey(bucketName, objectKey);
                
                //TODO тут отправить в Nats.
            }
            catch (IOException e)
            {
                Logger.Error(e, "Exception {Path}", filePath);
            }
        }
    }
}