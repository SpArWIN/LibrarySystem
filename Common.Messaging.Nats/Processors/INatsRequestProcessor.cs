namespace Common.Messaging.Nats.Processors;

/// <summary>
/// Интерфейс обработчика сообщений Nats.
/// </summary>
/// <typeparam name="TRequest">Запрос.</typeparam>
/// <typeparam name="TResponse">Ответ.</typeparam>
public interface INatsRequestProcessor<in TRequest,  TResponse>
{
    /// <summary>
    /// Обработать запрос.
    /// </summary>
    /// <param name="request">Запрос.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Ответ.</returns>
    Task<TResponse> ProcessAsync(TRequest request, CancellationToken cancellationToken = default);
}