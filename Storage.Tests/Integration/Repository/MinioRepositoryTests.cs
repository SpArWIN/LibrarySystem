using System.Text;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Minio.DataModel.Args;
using Storage.Application.Extensions;
using Storage.Domain.Repository;
using Storage.Tests.Integration.Fixture;
namespace Storage.Tests.Integration.Repository;

[Collection("Minio")]
public class MinioRepositoryTests
{
    private readonly IMinioRepository _minioRepository;
    private const string BucketName = "testbucket";
    private readonly MinioFixture _minioFixture;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="fixture"></param>
    public MinioRepositoryTests(MinioFixture fixture, MinioFixture minioFixture)
    {
        _minioFixture = minioFixture;
        _minioRepository = fixture.ServiceProvider.GetRequiredService<IMinioRepository>();
    }

    /// <summary>
    /// Успешно загрузить файл.
    /// </summary>
    [Fact]
    public async Task UploadFileAsync_WithValidStream_ShouldUploadSuccessfully()
    {
        //arrange
        var objectName = $"upload-test-{Guid.NewGuid()}.txt";
        var contentType = "text/plain";
        var fileContent = "Test file content for upload";
        var fileBytes = Encoding.UTF8.GetBytes(fileContent);
        using var fileStream = new MemoryStream(fileBytes);
        
        //act 
        await _minioRepository.UploadFileAsync(
            bucketName: BucketName,
            objectName: objectName,
            fileStream: fileStream,
            contentType: contentType);
        
        // assert
        var stat = await _minioRepository.ExistsAsync(BucketName, objectName);
        stat.Should().BeTrue();
        var downLoadContent = await _minioRepository.DownloadFileAsync(BucketName, objectName);
        downLoadContent.Should().NotBeNull();
        using var reader = new StreamReader(downLoadContent);
        var downloadedContent = await reader.ReadToEndAsync();
        downloadedContent.Should().Be(fileContent);
    }
    
    /// <summary>
    /// Проверить загрузку файлов с разным контентом.
    /// </summary>
    [Theory]
    [InlineData("text/plain", "Simple text content")]
    [InlineData("application/json", "{\"key\": \"value\", \"number\": 42}")]
    [InlineData("text/html", "<html><body><h1>Hello</h1></body></html>")]
    [InlineData("application/xml", "<root><item>test</item></root>")]
    public async Task DownLoadFileAsync_WithDifferentContents_ReturnCorrectStreams(
        string contentType,
        string content)
    {
        // arrange
        var objectName = $"{contentType.Replace("/", "-")}-{Guid.NewGuid()}.dat";
        var bytes = Encoding.UTF8.GetBytes(content);
       
        using var fileStream = new MemoryStream(bytes);
        
        await _minioRepository.UploadFileAsync(BucketName, objectName, fileStream, contentType);
        
        // act
        await using var downloadStream  = await _minioRepository.DownloadFileAsync(BucketName, objectName);
        using var reader = new StreamReader(downloadStream);
        var downloadedContent = await reader.ReadToEndAsync();
        
        //assert
        downloadedContent.Should().Be(content);
    }

    /// <summary>
    /// Сгенерировать Url Для существующего объекта.
    /// </summary>
    [Fact]
    public async Task GetImageUrlAsync_WithExistingObject_ShouldReturnCorrectImageUrl()
    {
        // arrange
        var objectName = $"test-image-{Guid.NewGuid()}.jpg";
        var contentType = BucketTypeExtensions.GetContentType(objectName);
        var content = "Fake image content";
        var bytes = Encoding.UTF8.GetBytes(content);

        using var stream = new MemoryStream(bytes);
        await _minioRepository.UploadFileAsync(BucketName, objectName, stream, contentType);
        
        // act
        var url = await _minioRepository.GetImageUrlAsync(BucketName, objectName);
        
        // assert
        url.Should().NotBeNullOrWhiteSpace();
        url.Should().StartWith($"http://{_minioFixture.Hostname}:{_minioFixture.Port}/{BucketName}/{objectName}");
        var uri = new Uri(url);
        uri.Scheme.Should().Be("http");
        uri.Host.Should().Be(_minioFixture.Hostname);
        uri.Port.Should().Be(_minioFixture.Port);
        uri.AbsolutePath.Should().Be($"/{BucketName}/{objectName}");
    }

    /// <summary>
    /// Получить список всех бакетов.
    /// </summary>
    [Fact]
    public async Task ListBucketsAsync_Bucket_ReturnBuckets()
    {
        // arrange
        var (expectedBucketNames, _) =  await CreateBucketAsync();
        
        // act
        var buckets = await _minioRepository.ListBucketsAsync();
        
        buckets.Should().NotBeNull();

        var bucketsNames = buckets.Select(b => b.Name).ToList();

        foreach (var expectedBucket  in expectedBucketNames)
        {
            bucketsNames.Contains(expectedBucket).Should().BeTrue();
        }
        
        buckets.Should().HaveCount(expectedBucketNames.Count);
    }

    /// <summary>
    /// Получить все файлы бакетов.
    /// </summary>
    [Fact]
    public async Task ListFilesAsync_Bucket_ReturnFiles()
    {
        //arrange
        var (bucketNames, fileLists) = await CreateBucketAsync();
        
        // act & assert
        foreach (var bucketName in bucketNames)
        {
            var expectedFiles = fileLists[bucketName];
            var files = await _minioRepository.ListFilesAsync(bucketName);
        
            files.Should().NotBeNull();
        
            var actualFileNames = files.Select(f => f.ObjectName).ToList();
        
            foreach (var expectedFile in expectedFiles)
            {
                actualFileNames.Should().Contain(expectedFile);
            }
        
            files.Should().HaveCount(expectedFiles.Count);
        }
    }

    /// <summary>
    /// Получить список файлов во всех бакетах.
    /// </summary>
    [Fact]
    public async Task ListAllFilesAsync_ShouldReturnAllFilesFromAllBuckets()
    {
        // arrange
        var (_, expectedBucketFiles) = await CreateBucketAsync();

        var expectedAllFiles = expectedBucketFiles
            .SelectMany(kvp => kvp.Value.Select(fileName => (kvp.Key, fileName)))
            .ToList();
        
        // act
        var allFiles = await _minioRepository.ListAllFilesAsync();
        allFiles.Should().NotBeNull();
        var expectedCount = expectedBucketFiles.Sum(kvp => kvp.Value.Count);
        allFiles.Should().HaveCount(expectedCount);

        foreach (var (bucketName, fileName) in expectedAllFiles)
        {
            allFiles.Should().Contain(f => f.Bucket == bucketName && f.ObjectName == fileName);
        }
    }
    [Theory]
    [InlineData("image.jpg", "image/jpeg")]
    [InlineData("image.JPG", "image/jpeg")]
    [InlineData("photo.jpeg", "image/jpeg")]
    [InlineData("picture.JPEG", "image/jpeg")]
    [InlineData("screenshot.png", "image/png")]
    [InlineData("icon.PNG", "image/png")]
    [InlineData("animation.gif", "image/gif")]
    [InlineData("avatar.GIF", "image/gif")]
    [InlineData("photo.bmp", "image/bmp")]
    [InlineData("image.BMP", "image/bmp")]
    [InlineData("picture.webp", "image/webp")]
    [InlineData("photo.WEBP", "image/webp")]
    public async Task GetContentType_FWithDifferentContentTypes_ShouldReturnCorrectContentType(string fileName, string expectedContentType)
    {
        // arrange
        var content = "Test content";
        var bytes = Encoding.UTF8.GetBytes(content);
       
        using var fileStream = new MemoryStream(bytes);
        
        await _minioRepository.UploadFileAsync(BucketName, fileName, fileStream, expectedContentType);
        
        //act
        var contentType = await _minioRepository.GetContentAsync(BucketName, fileName);
        
        // assert
        contentType.Should().NotBeNull();
        contentType.Should().Be(expectedContentType);
    }

    /// <summary>
    /// Проверить существование бакета, если не существует, создать.
    /// </summary>
    [Fact]
    public async Task EnsureBucketAsync_ForNonExistingBucket_ShouldCreateBucket()
    {
        // arrange
        var bucketName = $"new-bucket-{Guid.NewGuid():N}";
        var bucketsBefore = await _minioRepository.ListBucketsAsync();
        bucketsBefore.Should().NotContain(b=> b.Name == bucketName);
        
        // act
        await _minioRepository.EnsureBucketAsync(bucketName);
        
        // assert
        var bucketsAfter = await _minioRepository.ListBucketsAsync();
        bucketsAfter.Should().Contain(b => b.Name == bucketName);
        
    }

    /// <summary>
    /// Проверить существование файла, когда он есть.
    /// </summary>
    [Fact]
    public async Task ExistsAsync_ForExistingFile_ShouldReturnTrue()
    {
        // arrange
        var bucketName = $"exists-test-{Guid.NewGuid():N}";
        var objectName = "test-file.txt";
        var content = "Test content";
        var contentType = BucketTypeExtensions.GetContentType(objectName);
        var bytes = Encoding.UTF8.GetBytes(content);
        
        using var stream = new MemoryStream(bytes);
        await _minioRepository.UploadFileAsync(bucketName, objectName, stream, contentType);
        
        // act
        var exists = await _minioRepository.ExistsAsync(bucketName, objectName);
        
        // assert
        exists.Should().BeTrue();
    }

    /// <summary>
    /// Проверить существование файла, когда его нет.
    /// </summary>
    [Fact]
    public async Task ExistsAsync_ForNonExistingFile_ShouldReturnFalse()
    {
        var bucketName = $"non-exist-file-{Guid.NewGuid():N}";
        var nonExistentObject = $"non-existent-{Guid.NewGuid()}.txt";
        
        var exists  = await _minioRepository.ExistsAsync(bucketName, nonExistentObject);
        
        exists.Should().BeFalse();
    }

    /// <summary>
    /// Попытаться удалить бакет, когда там есть файл.
    /// </summary>
    [Fact]
    public async Task DeleteBucketIfEmptyAsync_ForBucketWithFiles_ShouldNotDeleteBucket()
    {
        // arrange
        var bucketName = $"bucket-{Guid.NewGuid():N}";
        var objectName = "test-file.txt";
        var content = "Test content";
        var contentType = BucketTypeExtensions.GetContentType(objectName);
        var bytes = Encoding.UTF8.GetBytes(content);
        using var stream = new MemoryStream(bytes);
        
        await _minioRepository.UploadFileAsync(bucketName, objectName, stream, contentType);

        var result = await _minioRepository.DeleteBucketIfEmptyAsync(bucketName);
        
        result.Should().BeFalse();
    }

    /// <summary>
    /// Удалить бакет, в котором ничего нет.
    /// </summary>
    [Fact]
    public async Task DeleteBucketIfEmptyAsync_ForBucketWithOutFiles_ShouldNotDeleteBucket()
    {
        var bucketName = $"bucket-{Guid.NewGuid():N}";
        await _minioFixture.MinioClient.MakeBucketAsync(new MakeBucketArgs()
            .WithBucket(bucketName));
        
        var result = await _minioRepository.DeleteBucketIfEmptyAsync(bucketName);
        result.Should().BeTrue();
    }
    

    private async Task<(List<string> bucketsNames, Dictionary<string, List<string>> bucketsFiles)> CreateBucketAsync()
    {
        var bucketsName = new List<string>();
        var bucketFiles = new Dictionary<string, List<string>>();
        var buckets = new[]
        {
            ("users", new[] { "user1.jpg", "user2.png", "profile/user3.gif" }),
            ("documents", new[] { "report.pdf", "invoice.docx", "archive/data.zip" }),
            ("logs", new[] { "app.log", "error.log", "debug/verbose.log" })
        };

        foreach (var (bucketName, files) in buckets)
        {
          await CreateBucketWithFilesAsync(bucketName, files);
          bucketsName.Add(bucketName);
          bucketFiles[bucketName] = files.ToList();
        }
        return (bucketsName, bucketFiles);
    }

    private async Task CreateBucketWithFilesAsync(string bucketName, string[] filesNames)
    {
        foreach (var fileName in filesNames)
        {
            var content = $"Content for {fileName} in {bucketName}";
            var contentType = BucketTypeExtensions.GetContentType(fileName);
            var bytes = Encoding.UTF8.GetBytes(content);
            using var stream = new MemoryStream(bytes);
            await _minioRepository.UploadFileAsync(bucketName, fileName, stream, contentType);
        }
    }
}