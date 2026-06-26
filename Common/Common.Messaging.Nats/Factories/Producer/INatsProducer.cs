using Common.Messaging.Nats.Contracts.Based;

namespace Common.Messaging.Nats.Factories.Producer;

/// <summary>
/// Продюсер Nats.
/// </summary>
public interface INatsProducer<in TMessage>
    where TMessage : ILibraryMessage
{
    /// <summary>
    ///   Опубликовать сообщение.
    /// </summary>
    /// <param name="subject">В какой подтип.</param>
    /// <param name="body">СОобщение.</param>
    /// <param name="tenantId">Какая бд, если это необходимо.</param>
    /// <param name="correlationId">Идентификатор запроса, если это необходимо.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    Task PublishAsync(
        string subject,
        TMessage body,
        string? tenantId = null,
        string? correlationId = null,
        CancellationToken ct = default);
}