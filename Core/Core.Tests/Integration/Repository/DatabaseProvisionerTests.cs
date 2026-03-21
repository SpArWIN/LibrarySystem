using Common.Contracts.Constaints.Providers;
using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Common.Db.Factory;
using Core.Infrastructure.Context;
using Core.Infrastructure.DbProvisioning;
using Core.Tests.Integration.Fixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Core.Tests.Integration.Repository;

/// <summary>
/// Интеграционные тесты на <see cref="PostgresDatabaseProvisioner"/>
/// </summary>
[Collection(Provider.Postgres)]
public sealed class DatabaseProvisionerTests
{
    private readonly PostgresFixture _fixture;
    private readonly IDatabaseProvisioner? _databaseProvisioner;
    private readonly ITenantDatabaseMigrator? _tenantDatabaseMigrator;
    private readonly IAppDbContextFactory<LibraryDbContext> _appDbContextFactory;
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="fixture"></param>
    public DatabaseProvisionerTests(PostgresFixture fixture)
    {
        _fixture = fixture;
        _databaseProvisioner = fixture.ServiceProvider.GetRequiredService<IDatabaseProvisioner>();
        _tenantDatabaseMigrator = fixture.ServiceProvider.GetRequiredService<ITenantDatabaseMigrator>();
        _appDbContextFactory = fixture.ServiceProvider.GetRequiredService<IAppDbContextFactory<LibraryDbContext>>();
    }

    /// <summary>
    /// База данных не существует.
    /// </summary>
    [Fact]
    public async Task DatabaseExistsAsync_DataBaseNotExists_ReturnsFalse()
    {
        // arrange
      
        var dbName = $"database_{Guid.NewGuid()}";
        
        // act
        var exists = await _databaseProvisioner.DatabaseExistsAsync(dbName);
        
        // assert
        exists.Should().BeFalse();
    }

    /// <summary>
    /// База данных, которую пытаются создать уже существует.
    /// </summary>
    [Fact]
    public async Task CreateDatabaseAsync_NewDatabaseExists_ReturnsTrue()
    {
        // arrange
       
        var dbName = $"database_{Guid.NewGuid()}";
        
        // act
        try
        {
            var beforeExists = await _databaseProvisioner.DatabaseExistsAsync(dbName);
            beforeExists.Should().BeFalse();
            await _databaseProvisioner.CreateDatabaseAsync(dbName);
            var afterExists = await _databaseProvisioner.DatabaseExistsAsync(dbName);
            afterExists.Should().BeTrue();
        }
        finally
        {
            await _databaseProvisioner.DropDatabaseAsync(dbName);
        }
    }

    /// <summary>
    /// Создается база данных и к ней применяются миграции.
    /// </summary>
    [Fact]
    public async Task CreateDatabaseAsync_CreatedDatabase_ApplyMigrations()
    {
        // arrange 
        var dbName = $"database_{Guid.NewGuid()}";
        var options = _fixture.ServiceProvider.GetRequiredService<IOptions<TenantProvisioningOptions>>();
        
        // act && assert
        try
        {
            await _databaseProvisioner.CreateDatabaseAsync(dbName);
            var tenTantConnectionString = options.Value.TenantConnectionStringTemplate
                .Replace("{db}", dbName, StringComparison.Ordinal);
            var settings = new DataBaseSettings()
            {
                Provider = options.Value.Provider,
                ConnectionString = tenTantConnectionString,
                MigrationsAssembly = options.Value.MigrationsAssembly,
            };
            await _tenantDatabaseMigrator.MigrateAsync(settings);
            var database = _appDbContextFactory.Create(settings);
            var pendingMigrations = await database.Database.GetPendingMigrationsAsync();
            pendingMigrations.Should().BeEmpty();

        }
        finally
        {
            Console.WriteLine(dbName);
        }
    }
}