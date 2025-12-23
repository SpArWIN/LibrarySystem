using DotNet.Testcontainers.Builders;
using Testcontainers.Minio;

namespace Storage.Tests.Integration.Builders;

public static class MinioBuilders
{
    public static MinioContainer BuildTestContainer()
    {
        return new MinioBuilder()
            .WithImage("minio/minio:latest")
            .WithExposedPort(9000)
            .WithExposedPort(9001)
            .WithEnvironment("MINIO_ROOT_USER", "minioadmin")
            .WithEnvironment("MINIO_ROOT_PASSWORD", "minioadmin")
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilExternalTcpPortIsAvailable(9000))   
            .Build();
    }
    
    
    public static MinioContainer BuildTestContainer(Action<MinioBuilder> configure)
    {
        var builder = new MinioBuilder()
            .WithImage("minio/minio:latest")
            .WithCommand("server", "/data", "--console-address", ":9001")
            .WithExposedPort(9000)
            .WithExposedPort(9001);
        
        configure?.Invoke(builder);
        
        return builder.Build();
    }
}
