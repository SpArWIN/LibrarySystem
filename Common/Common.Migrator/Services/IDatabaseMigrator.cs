namespace Common.Migrator.Services;

/// <summary>
/// Мигратор баз данных.
/// </summary>
public interface IDatabaseMigrator
{
    /// <summary>
    /// Запускает миграции центральной и всех tenant баз данных.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>.</returns>
    Task MigrateAllAsync(CancellationToken cancellationToken = default);
}