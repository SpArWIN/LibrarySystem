using Common.Contracts.Settings;
using Common.Db.Factory;
using Core.Infrastructure;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;


namespace Common.Migrator.Services;

/// <inheritdoc />
public sealed class DatabaseMigrator : IDatabaseMigrator
{
    
    private static readonly ILogger Logger = Log.ForContext<DatabaseMigrator>();
    private readonly IServiceProvider _serviceProvider;
    private readonly IAppDbContextFactory<LibraryDbContext> _dbContextFactory;
    private readonly IOptions<TenantDatabaseOptions> _options;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/>.</param>
    /// <param name="dbContextFactory"><see cref="IAppDbContextFactory{TDbContext}"/>.</param>
    /// <param name="options"><see cref="TenantDatabaseOptions"/>.</param>
    public DatabaseMigrator(IServiceProvider serviceProvider,
        IAppDbContextFactory<LibraryDbContext> dbContextFactory, IOptions<TenantDatabaseOptions> options)
    {
        _serviceProvider = serviceProvider;
        _dbContextFactory = dbContextFactory;
        _options = options;
    }
    
    /// <inheritdoc />
    public async Task MigrateAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.Debug("-> Migrating all database");
            using var scope = _serviceProvider.CreateScope();
            var central = scope.ServiceProvider.GetRequiredService<CentralDbContext>();
            Logger.Information(" -> Migrating CentralDb");
            await central.Database.MigrateAsync(cancellationToken);

            var instances = await central.LibraryInstances
                .AsNoTracking()
                .Select(x => new { x.Id, x.ConnectionString, x.MigrationsAssembly })
                .ToListAsync(cancellationToken);

            Logger.Information(" -> Found {Count} library instances", instances.Count);


            foreach (var i in instances)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (string.IsNullOrWhiteSpace(i.ConnectionString))
                {
                    Logger.Warning("Skipping library {Id}: empty ConnectionString", i.Id);
                    continue;
                }

                var settings = new DataBaseSettings()
                {
                    ConnectionString = i.ConnectionString,
                    Provider = _options.Value.Provider,
                    MigrationsAssembly = string.IsNullOrWhiteSpace(i.MigrationsAssembly)
                        ? _options.Value.MigrationsAssembly
                        : i.MigrationsAssembly

                };
                await using var libraryDb = _dbContextFactory.Create(settings);

                Logger.Information(" -> Migrating library db {Id}", i.Id);
                var pending = await libraryDb.Database.GetPendingMigrationsAsync(cancellationToken);
                Logger.Information("Pending migrations for {Id}: {Count}", i.Id, pending.Count());

                await libraryDb.Database.MigrateAsync(cancellationToken);
                Logger.Information(" <- Migrating Done");
            }
        }
        catch (OperationCanceledException)
        {
            Logger.Warning("Migration process canceled");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Migration process failed");
        }
        finally
        {
            Logger.Debug("<- Migrated all database");
        }
    }
}