using Common.Contracts.Constaints.Providers;
using Core.Tests.Integration.Fixture;

namespace Core.Tests.Integration.Collections;

[CollectionDefinition(Provider.Postgres, DisableParallelization = true)]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    
}