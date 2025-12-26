namespace Common.Contracts.Settings;

/// <summary>
/// Настройки создания дополнительных баз.
/// </summary>
public sealed record TenantProvisioningOptions
{
    /// <summary>Поставщик (postgres).</summary>
    public required string Provider { get; init; }

    /// <summary>
    /// Админская строка подключения к серверу (обычно к базе postgres),
    /// с правами CREATE DATABASE / DROP DATABASE.
    /// </summary>
    public required string AdminConnectionString { get; init; }

    /// <summary>
    /// Шаблон строки подключения к tenant-базе. Используй маркер {db}.
    /// Пример: Host=...;Port=5432;Database={db};Username=...;Password=...
    /// </summary>
    public required string TenantConnectionStringTemplate { get; init; }

    /// <summary>Префикс имени создаваемой базы.</summary>
    public string DatabaseNamePrefix { get; init; } = "library_";

    /// <summary>Сборка миграций  снимок в LibraryInstance.</summary>
    public string? MigrationsAssembly { get; init; }
}