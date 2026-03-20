namespace Common.Messaging.Nats.Settings;

/// <summary>
/// Настройки JetStream.
/// </summary>
public sealed class NatsJetStreamOptions
{
    /// <summary>
    /// Префикс для JetStream API.
    /// </summary>
    public string ApiPrefix { get; set; } = "$JS.API";
    
    /// <summary>
    /// Домен JetStream
    /// </summary>
    public string? Domain { get; set; }
    
    /// <summary>
    /// Таймаут запросов к JetStream API (секунды).
    /// </summary>
    public int RequestTimeout { get; set; } = 15;
    
    /// <summary>
    /// Стратегия восстановления после потери сообщений.
    /// </summary>
    public NatsJsRetryPolicy RetryPolicy { get; set; } = new();
    
    /// <summary>
    /// Максимальное количество сообщений в буфере.
    /// </summary>
    public int MaxMessagesBufferSize { get; set; } = 1024;

    /// <summary>
    /// Максимальный размер буфера в байтах.
    /// </summary>
    public int MaxBytesBufferSize { get; set; } = 65536;

    /// <summary>
    /// Включить проверку домена.
    /// </summary>
    public bool EnableDomainCheck { get; set; } = true;

    /// <summary>
    /// Использовать оптимизированные подписки.
    /// </summary>
    public bool UseOptimizedSubscriptions { get; set; } = true;
    
    /// <summary>
    /// Дефолтные настройки публицации сообщения.
    /// </summary>
    public NatsJsPubOptions? DefaultPubOptions { get; set; }
    
    /// <summary>
    /// Дефолтные настройки подписки..
    /// </summary>
    public NatsJsSubOptions? DefaultSubOptions { get; set; }

}