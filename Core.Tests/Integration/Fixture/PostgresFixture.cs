using System.ComponentModel;
using Common.Contracts.Constaints.Providers;
using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Common.Db.Extensions;
using Common.Db.Factory;
using Common.Http;
using Common.Http.Accessors;
using Common.Policies.Di;
using Core.Infrastructure;
using Core.Infrastructure.Context;
using Core.Infrastructure.DbProvisioning;
using Core.Infrastructure.Extensions.Context;
using Core.Infrastructure.Factory;
using Core.Infrastructure.Repository;
using Core.Infrastructure.Tentant;
using Core.Tests.Integration.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Testcontainers.PostgreSql;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace Core.Tests.Integration.Fixture;

/// <summary>
/// Фикстура на Постгрес.
/// </summary>
public sealed class PostgresFixture : DataBaseFixture
{
    private PostgreSqlContainer PostgresContainer => (PostgreSqlContainer)DataBaseContainer;

    public PostgresFixture()
    { }  

    /// <inheritdoc />
    protected override string BuildAdminConnectionString(string baseConnectionString)
        => new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            Database = Provider.Postgres,
            Pooling = false
        }.ToString();

    /// <inheritdoc />
    protected override string BuildDatabaseConnectionString(string baseConnectionString, string databaseName)
        => new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            Database = databaseName,
            Pooling = false
        }.ToString();

    /// <inheritdoc />
    protected override async Task CreateDatabaseAsync(string baseConnectionString, string databaseName)
    {
        await using var conn = new NpgsqlConnection(baseConnectionString);
        await conn.OpenAsync();

        var sql = $"CREATE DATABASE \"{databaseName}\"";
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
        await conn.CloseAsync();
    }

    /// <inheritdoc />
    protected override IContainer CreateDataBaseContainer()
    {
        return PostgresBuilders.BuildTestContainer();
    }

    /// <inheritdoc />
    protected override string GetConnectionStringFromContainer()
    {
        return PostgresContainer.GetConnectionString();
    }

    /// <inheritdoc />
    protected override void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            CentralConnectionString,
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(LibraryDbContext).Assembly.GetName().Name);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    
    /// <inheritdoc />
    protected override IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<CentralDbContext>(opt =>
            opt.UseNpgsql(CentralConnectionString,
                o => o.MigrationsAssembly(typeof(CentralDbContext).Assembly.GetName().Name)));
        
       services.AddCommonDb<LibraryDbContext>([typeof(BookRepository).Assembly]);
       services.AddContextConfiguration(Configuration);
       
       ConfigurateTentantOptionst(services);
        services.AddSingleton<ITenantDatabaseMigrator, TenantDatabaseMigrator>();
        services.AddSingleton<ICentralLibraryRegistry, CentralLibraryRegistry>();
       
       services.AddSingleton<IAppDbContextFactory<LibraryDbContext>, LibraryDbContextFactory>();
       services.AddSingleton<ITenantContextAccessor, TenantContextAccessor>();
       services.AddSingleton<IUnitOfWorkFactory<LibraryDbContext>, UnitOfWorkFactory>();
       services.AddSingleton<IUnitOfWorkFactory<CentralDbContext>, CentralUnitOfWorkFactory>();
       services.AddDefaultPolicies();
       services.AddPoliciesService();
       return services;
    }

    private IServiceCollection ConfigurateTentantOptionst(IServiceCollection services)
    {
        var adminConnectionString = ForceDataBase(PostgresContainer.GetConnectionString(), Provider.Postgres);
        services.AddSingleton<IOptions<TenantProvisioningOptions>>(_ =>
            Options.Create(new TenantProvisioningOptions()
        {
            Provider = Provider.Postgres,
            AdminConnectionString = adminConnectionString,
            TenantConnectionStringTemplate = ForceDataBase(adminConnectionString, "{db}"),
            DatabaseNamePrefix = "library_",
            MigrationsAssembly = typeof(LibraryDbContext).Assembly.GetName().Name
        }));
        
        services.AddSingleton<IDatabaseProvisioner, PostgresDatabaseProvisioner>();
        return services;
    }

    private string ForceDataBase(string connectionString, string databaseName)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = databaseName
        };
        return builder.ConnectionString;
    }

    
    /// <inheritdoc />
    protected override string GetDatabaseProviderName()
        => Provider.Postgres;

    
    /// <inheritdoc />
    protected override async Task CleanupTestDataAsync()
    {
        using var scope = ServiceProvider.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IAppDbContextFactory<LibraryDbContext>>();
        var settings = new DataBaseSettings()
        {
            Provider = Provider.Postgres,
            ConnectionString = CentralConnectionString,
            MigrationsAssembly = typeof(LibraryDbContext).Assembly.GetName().Name
        };
        await using var dataBase = factory.Create(settings);
        await dataBase.Database.EnsureDeletedAsync();
    }

    
    /// <inheritdoc />
    protected override async Task ApplyCentralMigrationsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var centralDb = scope.ServiceProvider.GetRequiredService<CentralDbContext>();
        await centralDb.Database.MigrateAsync();
    }
}