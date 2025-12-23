using System.Reactive.Linq;
using Common.Contracts.Storage.Files;
using Common.Contracts.Storage.Settings;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Serilog;
using Storage.Domain.Repository;

namespace Storage.Infrastructure.Repository;

/// <inheritdoc />
public sealed class MinioRepository : IMinioRepository
{
    private static readonly ILogger Logger = Log.ForContext<MinioRepository>();

    private readonly IMinioClient _client;
    private readonly MinioOptions _options;

    public MinioRepository(IOptions<MinioOptions> options)
    {
        _options = options.Value;

        var builder = new MinioClient()
            .WithEndpoint(_options.Endpoint, _options.Port)
            .WithCredentials(_options.AccessKey, _options.SecretKey);
        if (_options.WithSsl)
        {
            builder.WithSSL();
        }

        _client = builder.Build();
    }

    /// <inheritdoc />
    public async Task UploadFileAsync(string bucketName, string objectName, Stream fileStream, string contentType,
        CancellationToken cancellationToken = default)
    {
        await EnsureBucketAsync( bucketName, cancellationToken);
        await _client.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadFileAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();
        await _client.GetObjectAsync(new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream)), cancellationToken);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
    
    /// <inheritdoc />
    public async Task<string> GetImageUrlAsync(string bucketName, string objectName,
        CancellationToken cancellationToken = default)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithExpiry(60 * 60);
        if (_options is { PublicEndpoint: not null } options)
        {
            var publicClient = new MinioClient()
                .WithEndpoint(options.PublicEndpoint!, options.PublicPort)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .WithSSL(options.WithSsl)
                .Build();
            return await publicClient.PresignedGetObjectAsync(args);
        }
        var url = await _client.PresignedGetObjectAsync(args);
        return RewritePublicUrl(url);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Bucket>> ListBucketsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var list = await _client.ListBucketsAsync(cancellationToken);
            return  list.Buckets.Select(x=> new Bucket()
            {
                Name = x.Name,
                CreationDate = x.CreationDate,
            });
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Failed to list buckets");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FileDescriptor>> ListFilesAsync(string bucketName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var objects = new List<FileDescriptor>();
            var listArg = new ListObjectsArgs()
                .WithBucket(bucketName)
                .WithRecursive(true);
            var tcs = new TaskCompletionSource<bool>();
            var subscription = _client.ListObjectsAsync(listArg, cancellationToken)
                .Select(async item =>
                {
                    var contentType = await GetContentAsync(bucketName, item.Key, cancellationToken);
                    return new FileDescriptor()
                    {
                        Bucket = bucketName,
                        ObjectName = item.Key,
                        LastModified = item.LastModifiedDateTime,
                        Size = item.Size,
                        ContentType = contentType,
                    };
                }).Concat()
                .Subscribe(
                    onNext: obj => objects.Add(obj),
                    onError: ex => tcs.TrySetException(ex),
                    onCompleted: () => tcs.TrySetResult(true)
                );
            await tcs.Task;
            subscription.Dispose();
            return objects;
        }
        catch (Exception e)
        {
            Logger.Fatal(e, "Failed to list files");
            throw;
        }
    }
    
    /// <inheritdoc />
    public async Task<bool> DeleteFileAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        try
        {
            var rm = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);
            await _client.RemoveObjectAsync(rm, cancellationToken);
            return true;
        }
        catch (MinioException)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FileDescriptor>> ListAllFilesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var allFiles = new List<FileDescriptor>();
            var buckets = await ListBucketsAsync(cancellationToken);
            foreach (var bucket in buckets)
            {
                var files = await ListFilesAsync(bucket.Name, cancellationToken);
                allFiles.AddRange(files);
            }
            return allFiles;
        }
        catch (Exception e)
        {
             Logger.Fatal(e, "Failed to list files");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string?> GetContentAsync(string bucketName, string objectName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var args = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);
            var stat = await _client.StatObjectAsync(args, cancellationToken);
            return stat.ContentType;
        }
        catch (ObjectNotFoundException)
        {
            Logger.Warning("Not Found Type Objects");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task EnsureBucketAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        var exist = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName), cancellationToken);
        if (!exist)
        {
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName), cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
       var status = await _client.StatObjectAsync(new StatObjectArgs()
           .WithBucket(bucketName)
           .WithObject(objectName), cancellationToken);
       return status is not null;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteBucketIfEmptyAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        var isEmpty = true;

        var listArgs = new ListObjectsArgs()
            .WithBucket(bucketName)
            .WithRecursive(true);
        var observable = _client.ListObjectsAsync(listArgs);
        var tcs = new TaskCompletionSource<bool>();
        var subscriber = observable.Subscribe(
            _ =>
            {
                isEmpty = false;
                tcs.TrySetResult(true);
            },
            ex => { tcs.TrySetException(ex); }, () => tcs.TrySetResult(true)

        );
        await tcs.Task;
        subscriber.Dispose();
        if (isEmpty)
        {
            await _client.RemoveBucketAsync(new RemoveBucketArgs().WithBucket(bucketName), cancellationToken);
            return isEmpty;
        }
        return isEmpty;
    }

    private string RewritePublicUrl(string pressingUrl)
    {
        if (string.IsNullOrEmpty(_options.PublicEndpoint))
        {
            return pressingUrl;
        }
        var uri = new Uri(pressingUrl);
        var scheme = _options.WithSsl ? "https" : "http";
        var builder = new UriBuilder(uri)
        {
            Scheme = scheme,
            Host   = _options.PublicEndpoint,
            Port   = _options.PublicPort != 0 ? _options.PublicPort : -1
        };
        return builder.Uri.ToString();
    }

}