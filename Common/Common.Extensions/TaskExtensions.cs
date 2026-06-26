namespace Common.Extensions;

/// <summary>
/// Расширене для Task.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Синхронно дождаться завершения задачи.
    /// </summary>
    /// <param name="task">Таска.</param>
    /// <typeparam name="T">Тип таски.</typeparam>
    /// <returns>Таску.</returns>
    public static T GetResultSync<T>(this Task<T> task)
    {
        if (task is null)
        {
            throw new ArgumentNullException(nameof(task));
        }
        return task.GetAwaiter().GetResult();
    }

    /// <summary>
    /// Синхронно выполнить таску.
    /// </summary>
    /// <param name="task">.</param>
    public static void GetResultSync(this Task task)
    {
        if (task is null)
        {
            throw new ArgumentNullException(nameof(task));
        }
        task.GetAwaiter().GetResult();
    }
}