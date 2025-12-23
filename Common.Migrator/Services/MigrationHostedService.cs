using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
namespace Common.Migrator.Services;

/// <summary>
/// Фоновый сервис миграций.
/// </summary>
public class MigrationHostedService : IHostedService
{
    private static readonly ILogger Logger = Log.ForContext<MigrationHostedService>();
    private readonly IServiceProvider _serviceProvider;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/>.</param>
    public MigrationHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dataBase = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            Logger.Information(" -> Starting Migration Hosted Service");
            await dataBase.Database.MigrateAsync(cancellationToken);
            Logger.Information(" <- Migration Hosted Service done");
        }
        catch (OperationCanceledException)
        {
            Logger.Warning("Migration Hosted Service canceled");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Migration Hosted Service failed");
        }
        finally
        {
            Environment.Exit(0);
        }

    }
    
    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
        => await Task.CompletedTask;
}