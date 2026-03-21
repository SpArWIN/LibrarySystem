namespace Common.Contracts.Settings;

/// <summary>
/// Настройки контекста LibraryDb.
/// </summary>
public sealed record LibraryDbContextFactoryOptions
{
    /// <summary>Включить подробные ошибки EF.</summary>
    public bool EnableDetailedErrors { get; init; } = true;
    
    /// <summary>Включить чувствительные данные в логах.</summary>
    public bool EnableSensitiveDataLogging { get; init; }
}