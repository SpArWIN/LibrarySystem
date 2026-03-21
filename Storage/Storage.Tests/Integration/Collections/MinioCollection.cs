using Storage.Tests.Integration.Fixture;

namespace Storage.Tests.Integration.Collections;
[CollectionDefinition("Minio" , DisableParallelization = true)]
public class MinioCollection : IClassFixture<MinioFixture>
{
    
}