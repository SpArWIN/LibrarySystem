using Common.Messaging.Nats.Contracts.Based;

namespace Common.Messaging.Nats.Handlers;

/// <summary>
/// Обработчик сообщений Nats.
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public interface INatsMessageHandler<in TMessage>
where TMessage : LibraryMessageBase
{
    /// <summary>
    /// Обработать сообщение.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}