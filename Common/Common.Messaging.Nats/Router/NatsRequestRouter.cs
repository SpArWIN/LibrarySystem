using System.Collections.Concurrent;
using Common.Messaging.Nats.Contracts.Based;
using Common.Messaging.Nats.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Nats.Router;

/// <summary>
/// Реестр маршрутизации запросов.
/// </summary>
public sealed class NatsRequestRouter<TBaseMessage>
where TBaseMessage : LibraryMessageBase
{
    private readonly
        ConcurrentDictionary<Type, 
            Func<IServiceScope, TBaseMessage, CancellationToken, Task<TBaseMessage>>>
        _processors = new();

    /// <summary>
    /// Регистрирует обработчик запроса/ответа для заданных типов.
    /// </summary>
    /// <typeparam name="TRequest">Тип запроса.</typeparam>
    /// <typeparam name="TResponse">Тип ответа.</typeparam>
    public void RegisterProcessors<TRequest, TResponse>()
        where TRequest : TBaseMessage
        where TResponse : TBaseMessage
    {
        var key = typeof(TRequest);
        if (_processors.ContainsKey(key))
        {
            throw new InvalidOperationException($"Processor already registered for request type '{key.Name}'.");
        }

        _processors.TryAdd(
            key,
            async (scope, request, cancellationToken) =>
            {
                var processor = scope.ServiceProvider.GetRequiredService<INatsRequestProcessor<TRequest, TResponse>>();
                var response = await processor.ProcessAsync((TRequest)request, cancellationToken);
                return response;
            });
    }

    /// <summary>
    /// Выполнить обработку сообщения.
    /// </summary>
    /// <param name="scope"><see cref="IServiceScope"/>.</param>
    /// <param name="message">Сообщение.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    public Task<TBaseMessage> ProcessAsync(IServiceScope scope, TBaseMessage message,
        CancellationToken cancellationToken = default)
    {
        var key = message.GetType();
        if (!_processors.TryGetValue(key, out var handler))
        {
            throw new InvalidOperationException($"No processor registered for request type '{key.Name}'.");
        }
        return handler(scope, message, cancellationToken);
    }
}