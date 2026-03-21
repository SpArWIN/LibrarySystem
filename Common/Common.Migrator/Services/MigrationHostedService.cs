using Common.Contracts.Settings;
using Common.Db.Factory;
using Core.Infrastructure;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
namespace Common.Migrator.Services;

/// <summary>
/// Фоновый сервис миграций.
/// </summary>
public class MigrationHostedService : BackgroundService
{
    private static readonly ILogger Logger = Log.ForContext<MigrationHostedService>();
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IAppDbContextFactory<LibraryDbContext> _dbContextFactory;
    private readonly IOptions<TenantDatabaseOptions> _options;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/>.</param>
    /// <param name="lifetime"><see cref="IHostApplicationLifetime"/>.</param>
    /// <param name="libraryDbContextFactory"><see cref="IAppDbContextFactory{TDbContext}"/>.</param>
    /// <param name="options"><see cref="TenantDatabaseOptions"/>.</param>
    public MigrationHostedService(
        IServiceProvider serviceProvider, 
        IHostApplicationLifetime lifetime,
        IAppDbContextFactory<LibraryDbContext> libraryDbContextFactory, 
        IOptions<TenantDatabaseOptions> options
        )
    {
        _serviceProvider = serviceProvider;
        _lifetime = lifetime;
        _options = options;
        _dbContextFactory = libraryDbContextFactory;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var central = scope.ServiceProvider.GetRequiredService<CentralDbContext>();
            Logger.Information(" -> Migrating CentralDb");
            await central.Database.MigrateAsync(stoppingToken);

            var instances = await central.LibraryInstances
                .AsNoTracking()
                .Select(x => new { x.Id, x.ConnectionString, x.MigrationsAssembly })
                .ToListAsync(stoppingToken);

            Logger.Information(" -> Found {Count} library instances", instances.Count);


            foreach (var i in instances)
            {
                stoppingToken.ThrowIfCancellationRequested();
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
                var pending = await libraryDb.Database.GetPendingMigrationsAsync(stoppingToken);
                Logger.Information("Pending migrations for {Id}: {Count}", i.Id, pending.Count());
                
                await libraryDb.Database.MigrateAsync(stoppingToken);
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
            _lifetime.StopApplication();
        }
    }
}