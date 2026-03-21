namespace Common.Messaging.Nats.Settings;

/// <summary>
/// Настройки публикации сообщений Nats.
/// </summary>
public sealed class NatsJsPubOptions
{
    
    /// <summary>
    /// Максимальное ожидание подтверждения.
    /// </summary>
    public int MaxWait { get; set; } = 30;

    /// <summary>
    /// Интервал отправления сообщений при неуспешном завершении.
    /// </summary>
    public TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// ID сообщения для дедупликации.
    /// </summary>
    public string? MsgId { get; set; }

    /// <summary>
    /// Ожидаемый последний ID сообщения.
    /// </summary>
    public string? ExpectedLastMsgId { get; set; }
    
    /// <summary>
    /// Ожидаемый последний sequence.
    /// </summary>
    public ulong? ExpectedLastSequence { get; set; }

    /// <summary>
    /// Ожидаемый stream.
    /// </summary>
    public string? ExpectedStream { get; set; }
}