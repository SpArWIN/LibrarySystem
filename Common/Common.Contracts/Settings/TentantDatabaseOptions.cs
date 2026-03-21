namespace Common.Contracts.Settings;

/// <summary>
/// Настройки tenant-базы центральной базы.
/// </summary>
public sealed record TenantDatabaseOptions
{
    /// <summary>Поставщик (postgres/mysql и т.п.).</summary>
    public required string Provider { get; init; }

    /// <summary>Сборка миграций.</summary>
    public string? MigrationsAssembly { get; init; }
}
