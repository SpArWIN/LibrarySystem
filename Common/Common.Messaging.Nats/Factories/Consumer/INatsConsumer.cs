using Common.Messaging.Nats.Contracts.Based;
using Common.Messaging.Nats.Messages;

namespace Common.Messaging.Nats.Factories.Consumer;

/// <summary>
/// Консьюмер NATS.
/// </summary>
public interface INatsConsumer<TMessage>
where TMessage : ILibraryMessage
{
    /// <summary>
    /// Подписаться на durable consumer (с сохранением позиции).
    /// </summary>
    /// <param name="streamName">Имя стрима.</param>
    /// <param name="durableConsumer">Имя консьюмера.</param>
    /// <param name="subjectFilter">Фильтр подтипа.</param>
    IObservable<NatsMessage<TMessage>> FromDurableConsumer(
        string streamName,
        string durableConsumer,
        string subjectFilter);
    
    /// <summary>
    /// Подписаться на subject с группой.
    /// </summary>
    IObservable<NatsMessage<TMessage>> FromSubject(
        string subject,
        string? queueGroup = null);
}