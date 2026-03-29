using Common.Messaging.Nats.Contracts.Based;
using Common.Messaging.Nats.Messages;
using Common.Messaging.Nats.Status;
using Serilog;

namespace Common.Messaging.Nats.Handlers;

/// <inheritdoc />
public abstract class NatsMessageHandler<TMessage> : INatsMessageHandler<TMessage>
where TMessage : LibraryMessageBase
{
    protected static readonly ILogger Logger = Log.ForContext<NatsMessageHandler<TMessage>>();
    
    /// <inheritdoc />
    public abstract Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Подтвердить обработку.
    /// </summary>
    /// <param name="message"></param>
    protected void Ack(NatsMessage<TMessage> message)
    {
        message.Status = MessageStatus.Completed;
        message.Error = string.Empty;
    }

    /// <summary>
    /// Отклонить сообщение с возможностью повтора.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="error">Ошибка.</param>
    protected void Nack(NatsMessage<TMessage> message, string? error = null)
    {
        message.Status = MessageStatus.Failed;
        message.Error = error;
        Logger.Error("Failed to send Nack message : @{mes} ", error);
    }
    
}