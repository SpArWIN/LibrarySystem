using Common.Contracts.Settings;

namespace Common.Db.Abstractions;

/// <summary>
/// Мигратор после основного создания базы данных. Применяется, после первой общей миграции мигратором.
/// </summary>
public interface ITenantDatabaseMigrator
{
    /// <summary>
    /// Применить миграции к tenant-базе.
    /// </summary>
    Task MigrateAsync(DataBaseSettings dbSettings, CancellationToken ct = default);
}