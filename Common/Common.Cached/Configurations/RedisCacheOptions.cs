using System.ComponentModel.DataAnnotations;

namespace Common.Cached.Configurations;

/// <summary>
/// Конфигурация подключения к Redis.
/// </summary>
public sealed class RedisCacheOptions
{
    /// <summary>
    /// Имя секции настроек.
    /// </summary>
    public const string SectionName = "Cache:Redis";
    
    /// <summary>
    /// Конфигурация подключения.
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = string.Empty;
    
    /// <summary>
    /// Имя для кеша.
    /// </summary>
    public string InstanceName { get; set; } = "LibraryCache";
    
    /// <summary>
    /// Минимальная задерка по времени.
    /// </summary>
    [Range(1, 3600, ErrorMessage = "DefaultTtlSeconds must be between 1 and 3600")]
    private int DefaultTtlSeconds { get; set; } = 600;
    
    /// <summary>
    /// Настройка, которая активирует сжатие данных в системе Redis.
    /// </summary>
    public bool EnableCompression { get; set; }
    
    /// <summary>
    /// Истекаемый срок по умолчанию.
    /// </summary>
    public TimeSpan DefaultTtl => TimeSpan.FromSeconds(DefaultTtlSeconds);
}

/// <summary>
/// Префиксы для различных сущностей.
/// </summary>
public sealed class CachePrefixesOptions
{
    /// <summary>
    /// Имя секции.
    /// </summary>
    public const string SectionName = "Cache:Prefixes";
    
    /// <summary>
    /// Для книг.
    /// </summary>
    public string Book { get; set; } = "book:";
    
    /// <summary>
    /// ДЛя экземпляров.
    /// </summary>
    public string BookCopy { get; set; } = "copy:";
    
    /// <summary>
    /// Для пользователей.
    /// </summary>
    public string User { get; set; } = "user:";
    
    /// <summary>
    /// Для выдачи.
    /// </summary>
    public string Loan { get; set; } = "loan:";
    
    /// <summary>
    /// Для Установки библиотек.
    /// </summary>
    public string LibraryInstance { get; set; } = "lib:";
}