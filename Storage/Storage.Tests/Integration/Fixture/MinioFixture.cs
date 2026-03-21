using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Storage.Tests.Extensions;
using Storage.Tests.Integration.Builders;
using Testcontainers.Minio;

namespace Storage.Tests.Integration.Fixture;

/// <summary>
/// Фикстура на Minio.
/// </summary>
public sealed class MinioFixture : IAsyncLifetime
{
    
    private readonly MinioContainer _minioContainer;
    private static string AccessKey => "minioadmin";
    private static string SecretKey => "minioadmin";

    /// <summary>
    /// Конструктор.
    /// </summary>
    public MinioFixture()
    {
        _minioContainer = MinioBuilders.BuildTestContainer();
    }
    
    /// <inheritdoc />
    public async Task InitializeAsync()
    {
       await _minioContainer.StartAsync();
       
       ServiceProvider = ConfigureServices();
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        await CleanupTestBuckets();
        await _minioContainer.DisposeAsync();
    }
    
    /// <summary>
    /// Хост.
    /// </summary>
    public string Hostname => _minioContainer.Hostname;
    
    /// <summary>
    /// Порт.
    /// </summary>
    public int Port => _minioContainer.GetMappedPublicPort(9000);
    
    /// <summary>
    /// Поставщик.
    /// </summary>
    public IServiceProvider ServiceProvider { get; private set; } = null!;
    
    /// <summary>
    /// Клиент.
    /// </summary>
    public IMinioClient MinioClient { get; set; } = null!;
    
    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        MinioClient = new MinioClient()
            .WithEndpoint(Hostname, port: Port)
            .WithCredentials(AccessKey, SecretKey)
            .WithSSL(false)
            .Build();
        services.AddTestsMinio(
            hostname: Hostname,
            port: Port,
            accessKey: AccessKey,
            secretKey: SecretKey);
        
        return services.BuildServiceProvider();
    }
    
    private async Task RemoveBucketWithContentsAsync(string bucket)
    {
        var listArgs = new ListObjectsArgs()
            .WithBucket(bucket)
            .WithRecursive(true);
        
        IObservable<Item> observable = MinioClient.ListObjectsAsync(listArgs);
        var items = await observable.ToList().ToTask();
        
         foreach (var obj in items)
        {
            var removeArgs = new RemoveObjectArgs()
                .WithBucket(bucket)
                .WithObject(obj.Key);
            await MinioClient.RemoveObjectAsync(removeArgs);
        }
        
        var removeBucketArgs = new RemoveBucketArgs()
            .WithBucket(bucket);
        await MinioClient.RemoveBucketAsync(removeBucketArgs);
    }

    private async Task CleanupTestBuckets()
    {
        var bucketsList = await MinioClient.ListBucketsAsync();
        foreach (var bucket in bucketsList.Buckets)
        {
            await RemoveBucketWithContentsAsync(bucket.Name);
        }
    }
}