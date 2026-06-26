namespace Common.Cached.PrefixStrategy;

/// <summary>
/// Интерфейс стратегии получения префикса для типа.
/// </summary>
public interface ICachePrefixStrategy
{
    /// <summary>
    /// Тип для которого применяется стратегия.
    /// </summary>
    Type TargetType { get; }
    
    /// <summary>
    /// Получить префикс для ключа.
    /// </summary>
    string GetPrefix();
}