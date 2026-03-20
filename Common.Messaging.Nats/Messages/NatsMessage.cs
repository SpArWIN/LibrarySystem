using Common.Messaging.Nats.Contracts.Based;
using Common.Messaging.Nats.Status;
using NATS.Client.Core;

namespace Common.Messaging.Nats.Messages;

/// <summary>
/// Обертка для сообщений NATS с метаданными.
/// </summary>
public sealed class NatsMessage<TMessage>
where TMessage : ILibraryMessage
{
    /// <summary>
    /// Тело сообщения.
    /// </summary>
    public required TMessage Body { get; init; }
    
    /// <summary>
    /// Идентификатор клиента/пользователя.
    /// </summary>
    public string? ClientId { get; init; }
    
    /// <summary>
    /// Идентификатор тенаната.
    /// </summary>
    public string? TenantId { get; init; }
    
    /// <summary>
    /// Корреляционный ID для отслеживания цепочек запросов.
    /// </summary>
    public string? CorrelationId { get; init; }
    
    /// <summary>
    /// Нет ответа. - true, есть ответ, false.
    /// </summary>
    public bool IsNoResponders { get; init; }
    
    /// <summary>
    /// Subject, в который следует отправить ответ.
    /// </summary>
    public string? Reply { get; init; }
    
    /// <summary>
    /// Subject, откуда пришло сообщение.
    /// </summary>
    public required string Subject { get; init; }
    
    /// <summary>
    /// Порядковый номер сообщения в JetStream.
    /// </summary>
    public ulong? Sequence { get; init; }
    
    /// <summary>
    /// Время сообщения в JetStream.
    /// </summary>
    public DateTimeOffset? Timestamp { get; init; }
    
    /// <summary>
    /// Количество попыток доставки.
    /// </summary>
    public int? DeliveryCount { get; init; }
    
    /// <summary>
    /// Заголовки сообщения.
    /// </summary>
    public NatsHeaders? Headers { get; init; }
    
    /// <summary>
    /// Статус обработки сообщения.
    /// </summary>
    public MessageStatus Status { get; set; } = MessageStatus.Pending;
    
    /// <summary>
    /// Ошибка обработки сообщения (если есть).
    /// </summary>
    public string? Error { get; set; }
    
    /// <summary>
    /// Признак, что сообщение является дубликатом.
    /// </summary>
    public bool IsDuplicate { get; init; }
}