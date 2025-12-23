namespace Core.Infrastructure.Settings;

/// <summary>
/// Настройки подключения к базе данных.
/// </summary>
public sealed record DataBaseSettings
{
    /// <summary>
    /// Поставщик. Mysql,Postgres 
    /// </summary>
    public required string Provider { get; init; }
    
    /// <summary>
    /// Строка подключения к базе данных.
    /// </summary>
    public required string ConnectionString { get; init; }
    
    /// <summary>
    /// Сборки миграций.
    /// </summary>
    public string? MigrationsAssembly { get; init; }
}