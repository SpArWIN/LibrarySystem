namespace Core.Domain.Models.Instanse;

/// <summary>
/// Конфигурационная таблица инстансов Баз данных.
/// </summary>
public sealed class LibraryInstance
{
    /// <summary>
    /// Идентификатор базы.
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Название базы.
    /// </summary>
    public string Name { get; init; } = string.Empty;
    
    /// <summary>
    /// Строка подключения.
    /// </summary>
    public string ConnectionString { get; init; } = string.Empty;
    
    /// <summary>Сборка миграций.</summary>
    public string? MigrationsAssembly { get; init; }

    /// <summary>Дата создания (UTC).</summary>
    public DateTimeOffset CreatedAtUtc { get; init; }

    /// <summary>Дата обновления (UTC).</summary>
    public DateTimeOffset UpdatedAtUtc { get; init; }
}