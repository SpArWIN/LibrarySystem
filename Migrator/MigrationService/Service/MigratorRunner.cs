using Common.Migrator.Services;
using Common.Policies.Services;
using Microsoft.Extensions.Hosting;

namespace MigrationService.Service;

/// <summary>
/// Сервис запуска миграций.
/// </summary>
public sealed class MigratorRunner : IHostedService
{
    private readonly IDatabaseMigrator _migrator;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IDbResilience _dbResilience;
    
    public MigratorRunner(IDatabaseMigrator migrator, 
        IHostApplicationLifetime lifetime, IDbResilience dbResilience)
    {
        _migrator = migrator;
        _lifetime = lifetime;
        _dbResilience = dbResilience;
    }

   
    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbResilience.DatabaseConnect.ExecuteAsync(async token =>
            {
                var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
                await _migrator.MigrateAllAsync(cancellationTokenSource.Token);
            }, cancellationToken);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _lifetime.StopApplication();
        }
    }

       
    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
     => await Task.CompletedTask;
}