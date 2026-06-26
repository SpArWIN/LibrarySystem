namespace Common.Extensions;

/// <summary>
/// Расширение для коллекций.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Для каждого элемента выполнить действие.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="ArgumentNullException"></exception>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        foreach (var item in source)
        {
            action(item);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> source, Func<T, T> action)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var forEach = source as T[] ?? source.ToArray();
        foreach (var item in forEach)
        {
             action(item);
        }
    }

    /// <summary>
    /// Асинхронный ForEach.
    /// </summary>
    /// <returns></returns>
    public static async Task ForEachAsync<T>(
        this IEnumerable<T> source, 
        Func<T, CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        foreach (var item in source)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await action(item, cancellationToken);
        }
    }

    /// <summary>
    /// Асинхронный ForEach с параллельным выполнением. 
    /// </summary>
    /// <param name="source">Коллекция.</param>
    /// <param name="action">Действие.</param>
    /// <param name="maxDegreeOfParallelism">Максимальное количество параллельных операций.</param>
    /// <param name="cancellationToken">Токен.</param>
    /// <typeparam name="T">Тип элемента.</typeparam>
    public static async Task ParallelForEachAsync<T>(
        this IEnumerable<T> source,
        Func<T, CancellationToken?, Task> action,
        int maxDegreeOfParallelism = 5,
        CancellationToken cancellationToken = default
    )
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));
        
        if (action is null)
            throw new ArgumentNullException(nameof(action));
        
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism ,
            CancellationToken = cancellationToken
        };
        
        await Parallel.ForEachAsync(source, options, async (item, ct) =>
        {
            await action(item, ct);
        });
    }
}