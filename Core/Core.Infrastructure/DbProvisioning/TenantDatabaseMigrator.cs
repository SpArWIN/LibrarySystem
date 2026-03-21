using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Common.Db.Factory;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.DbProvisioning;

/// <inheritdoc />
public sealed class TenantDatabaseMigrator(IAppDbContextFactory<LibraryDbContext> dbContextFactory): ITenantDatabaseMigrator
{
    /// <inheritdoc />
    public async Task MigrateAsync(DataBaseSettings dbSettings, CancellationToken ct = default)
    {
        await using var db = dbContextFactory.Create(dbSettings);
        await db.Database.MigrateAsync(ct);
    }
}