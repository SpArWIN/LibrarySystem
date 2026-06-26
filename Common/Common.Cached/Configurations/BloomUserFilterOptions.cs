using System.ComponentModel.DataAnnotations;

namespace Common.Cached.Configurations;

/// <summary>
/// Настройки Bloom Filter для проверки существования пользователей.
/// </summary>
public sealed class BloomUserFilterOptions
{
    /// <summary>
    /// Имя секции настроек.
    /// </summary>
    public const string SectionName = "Cache:BloomFilter";
    
    /// <summary>
    /// Включен ли фильтр.
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// Ключ в Redis для хранения фильтра.
    /// </summary>
    [Required(ErrorMessage = "RedisKey is required")]
    public string RedisKey { get; set; } = "bf:users";
    
    /// <summary>
    /// Ожидаемое количество элементов.
    /// </summary>
    [Range(1, 100_000_000, ErrorMessage = "ExpectedElements must be between 1 and 100 million")]
    public int ExpectedElements { get; set; } = 1_000_000;
    
    /// <summary>
    /// Допустимый процент ложных срабатываний (0.01 = 1%).
    /// </summary>
    [Range(0.001, 0.1, ErrorMessage = "ErrorRate must be between 0.001 and 0.1")]
    public double ErrorRate { get; set; } = 0.01;
    
    /// <summary>
    /// Префикс для ключа (обычно InstanceName из RedisCacheOptions).
    /// </summary>
    public string? InstanceName { get; set; }
}