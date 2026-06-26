namespace Common.Cached.PrefixStrategy;

/// <summary>
/// Резолвер стратегии префикса по типу.
/// </summary>
public interface ICachedResolver
{
    /// <summary>
    /// Получить префикс.
    /// </summary>
    /// <typeparam name="T">Тип префикса.</typeparam>
    /// <returns>Префикс.</returns>
    string GetPrefix<T>() 
        where T : class;
}