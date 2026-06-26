namespace Common.Cached.PrefixStrategy;

/// <summary>
/// Базовое поведение получении стратегии префикса.
/// </summary>
public abstract class CachePrefixStrategy<T> : ICachePrefixStrategy
{
    /// <inheritdoc />
    public Type TargetType => typeof(T);

    /// <inheritdoc />
    public abstract string GetPrefix();
}