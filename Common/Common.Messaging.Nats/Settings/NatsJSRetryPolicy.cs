namespace Common.Messaging.Nats.Settings;

/// <summary>
/// Политики повторений отправки сообщений для Nats.
/// </summary>
public sealed class NatsJsRetryPolicy
{
    /// <summary>
    /// Максимальное количество попыток.
    /// </summary>
    public int MaxRetries { get; set; } = 5;

    /// <summary>
    /// Задержка между попытками (миллисекунды).
    /// </summary>
    public int RetryDelayMs { get; set; } = 100;

    /// <summary>
    /// Максимальная задержка (миллисекунды).
    /// </summary>
    public int MaxRetryDelayMs { get; set; } = 5000;

    /// <summary>
    /// Использовать экспоненциальную задержку.
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;
}