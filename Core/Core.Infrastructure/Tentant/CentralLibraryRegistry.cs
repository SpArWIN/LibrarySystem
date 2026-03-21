using Common.Contracts.Settings;
using Common.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Tentant;

/// <inheritdoc />
public sealed class CentralLibraryRegistry(CentralDbContext context, IOptions<TenantDatabaseOptions> options) : ICentralLibraryRegistry
{
    /// <inheritdoc />
    public async Task<DataBaseSettings> GetDbSettingsAsync(Guid libraryId, CancellationToken ct = default)
    {
        var instance = await context.LibraryInstances
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == libraryId, ct);
        if (instance is null)
        {
            throw new InvalidOperationException($"LibraryInstance '{libraryId}' not found in CentralDb.");
        }

        if (string.IsNullOrWhiteSpace(instance.ConnectionString))
        {
            throw new InvalidOperationException($"LibraryInstance '{libraryId}' has empty ConnectionString.");
        }

        var tentantOptions = options.Value;

        if (string.IsNullOrWhiteSpace(tentantOptions.Provider))
        {
            throw new InvalidOperationException($"LibraryInstance '{libraryId}' has no Provider.");
        }
        var migrations = string.IsNullOrWhiteSpace(instance.MigrationsAssembly)
            ? tentantOptions.MigrationsAssembly
            : instance.MigrationsAssembly;

        return new DataBaseSettings()
        {
            Provider = tentantOptions.Provider,
            ConnectionString = instance.ConnectionString,
            MigrationsAssembly = migrations
        };
    }
}
