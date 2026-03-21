namespace Common.Cached.Service;

/// <summary>
/// Интерфейс сервиса кеширования.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Получить из кеша по ключу.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <typeparam name="TKey">Тип ключа.</typeparam>
    /// <typeparam name="TValue">Тип значения.</typeparam>
    /// <returns>Значение или null.</returns>
    Task<TValue?> GetAsync<TKey, TValue>(TKey key, CancellationToken cancellationToken = default)
        where TKey : notnull;
    
    /// <summary>
    /// Установить в кеш.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="value">Значение.</param>
    /// <param name="expiration">Время истечения срока.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <typeparam name="TKey">Тип ключа.</typeparam>
    /// <typeparam name="TValue">Тип значения.</typeparam>
    Task SetAsync<TKey, TValue>(TKey key, TValue value, TimeSpan? expiration = null, 
        CancellationToken ct = default)
        where TKey : notnull
        where TValue : notnull;
    
    /// <summary>
    /// Удалить из кеша.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <typeparam name="TKey">Тип ключа.</typeparam>
    /// <returns>Удалено - не удалено.</returns>
    Task<bool> RemoveAsync<TKey>(TKey key, CancellationToken ct = default)
        where TKey : notnull;
    
    /// <summary>
    /// Существует ли в кеше.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <typeparam name="TKey">Тип ключа.</typeparam>
    /// <returns>True - да, False - нет.</returns>
    Task<bool> ExistsAsync<TKey>(TKey key, CancellationToken ct = default)
        where TKey : notnull;
    
    /// <summary>
    /// Получить или создать значение (thread-safe).
    /// </summary>
    Task<TValue> GetOrCreateAsync<TKey, TValue>(
        TKey key,
        Func<Task<TValue>> factory,
        TimeSpan? expiration = null,
        CancellationToken ct = default)
        where TKey : notnull
        where TValue : class;
}