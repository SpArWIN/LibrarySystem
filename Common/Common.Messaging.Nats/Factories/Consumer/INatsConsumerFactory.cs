using Common.Messaging.Nats.Contracts.Based;

namespace Common.Messaging.Nats.Factories.Consumer;

/// <summary>
/// Фабрика консьюмеров.
/// </summary>
public interface INatsConsumerFactory
{
    /// <summary>
    /// Создать консьюмера.
    /// </summary>
    /// <typeparam name="T">Тип сообщения.</typeparam>
    /// <returns>.</returns>
    INatsConsumer<T> Create<T>()
        where T : ILibraryMessage;
}