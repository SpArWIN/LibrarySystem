using Common.Messaging.Nats.Contracts.Based;

namespace Common.Messaging.Nats.Factories.Producer;

/// <summary>
/// Фабрика продюсера.
/// </summary>
public interface INatsProducerFactory
{
    /// <summary>
    /// Создать продюсера.
    /// </summary>
    /// <typeparam name="TMessage">Сообщение.</typeparam>
    /// <returns><see cref="INatsProducer{TMessage}"/>.</returns>
    INatsProducer<TMessage> Create<TMessage>()
        where TMessage : ILibraryMessage;
}