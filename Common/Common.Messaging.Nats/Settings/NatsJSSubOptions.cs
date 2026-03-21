using Common.Messaging.Nats.SubscribeMode;
using Common.Policies.Pollicies.Nats;

namespace Common.Messaging.Nats.Settings;

/// <summary>
/// Настройки подписки сообщений Nats.
/// </summary>
public sealed class NatsJsSubOptions
{
    /// <summary>
    /// Режим доставки.
    /// </summary>
    public NatsJsSubscribeMode Mode { get; set; } = NatsJsSubscribeMode.Push;
    
    /// <summary>
    /// Имя durable потребителя.
    /// </summary>
    public string? Durable { get; set; }

    /// <summary>
    /// Имя группы доставки.
    /// </summary>
    public string? DeliverGroup { get; set; }

    /// <summary>
    /// Максимальное количество доставляемых сообщений.
    /// </summary>
    public int MaxDeliver { get; set; } = -1;

    /// <summary>
    /// Таймаут Ack (секунды).
    /// </summary>
    public int AckWait { get; set; } = 30;
    
    /// <summary>
    /// Политика повторной доставки.
    /// </summary>
    public NatsJsReplayPolicy ReplyPolicy { get; set; } = NatsJsReplayPolicy.Instant;

    /// <summary>
    /// Политика доставки.
    /// </summary>
    public NatsJsDeliverPolicy DeliverPolicy { get; set; } = NatsJsDeliverPolicy.All;
    
    /// <summary>
    /// Начальная позиция (sequence).
    /// </summary>
    public ulong StartSequence { get; set; }
    
    /// <summary>
    /// Время начала доставки.
    /// </summary>
    public DateTimeOffset StartTime { get; set; }
    
    /// <summary>
    /// Только headers (без тела сообщения).
    /// </summary>
    public bool HeadersOnly { get; set; }
    
    /// <summary>
    /// Максимальное количество сообщений в очереди.
    /// </summary>
    public int MaxMessages { get; set; } = 1024;
    
    /// <summary>
    /// Максимальный размер очереди в байтах.
    /// </summary>
    public int MaxBytes { get; set; } = 65536;
    
    /// <summary>
    /// Таймаут подписки (секунды).
    /// </summary>
    public int Timeout { get; set; } = 30;
    
}