namespace Common.Cached.Repositoryies;

/// <summary>
/// Интерфейс работы с Redis.
/// </summary>
public interface ICachedRepository<T>
where T : class
{
    /// <summary>
    /// Получить элемент с Redis.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Элемент.</returns>
    Task<T?> GetAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить множество элементов по ключам. 
    /// </summary>
    /// <param name="keys">Массив ключей.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>Массив элементов.</returns>
    Task<IEnumerable<T?>> GetManyAsync(IEnumerable<string> keys, CancellationToken ct = default);
    
    /// <summary>
    /// Установить значение.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="value">Значение.</param>
    /// <param name="ttl">Время при котором значение будет удалено из кеша.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <typeparam name="T">Тип коллекции.</typeparam>
    /// <returns>.</returns>
    Task SetAsync(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default);
    
    /// <summary>
    /// Установить несколько значений.
    /// </summary>
    /// <param name="values">Значения.</param>
    /// <param name="ttl">Время удаления.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>.</returns>
    Task SetManyAsync(IDictionary<string, T> values, TimeSpan? ttl = null, CancellationToken ct = default);
    
    /// <summary>
    /// Удалить значения по ключам.
    /// </summary>
    /// <param name="keys">Массив ключей.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>.</returns>
    Task RemoveManyAsync(IEnumerable<string> keys, CancellationToken ct = default);
    
    /// <summary>
    /// Проверить, существует ли элеменнт.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>True/ False.</returns>
    Task<bool> ExistsAsync(string key, CancellationToken ct = default);
    
    /// <summary>
    /// Получить ключи по паттерну.
    /// </summary>
    /// <param name="pattern">Паттерн.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>Ключи.</returns>
    Task<IEnumerable<string>> GetKeysAsync(string pattern, CancellationToken ct = default);
    
    /// <summary>
    /// Удалить по паттерну.
    /// </summary>
    /// <param name="pattern">Патетрн.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>.</returns>
    Task RemoveByPatternAsync(string pattern, CancellationToken ct = default);
}