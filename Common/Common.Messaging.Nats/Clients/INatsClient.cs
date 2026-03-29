using Common.Messaging.Nats.Contracts.Based;
using Common.Messaging.Nats.Messages;
using Common.Messaging.Nats.Settings;
using NATS.Client.JetStream.Models;

namespace Common.Messaging.Nats.Clients;

/// <summary>
/// Клиент Nats.
/// </summary>
public interface INatsClient : IAsyncDisposable
{
    /// <summary>
    /// Статус подключения.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Идентификатор клиента NATS (<c>NatsOpts.Name</c>): из настроек подключения или сгенерированный при старте.
    /// </summary>
    string ClientId { get; }
    
    /// <summary>
    /// Устанавливает подключение к NATS серверу.
    /// </summary>
    Task ConnectAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Публикует сообщение в указанный subject.
    /// </summary>
    /// <param name="subject">Куда отправить.</param>
    /// <param name="message">Сообщение.</param>
    /// <param name="opts">Настройки подключения.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <typeparam name="T">.</typeparam>
    /// <returns></returns>
    Task PublishAsync<T>(
        string subject, 
        T message, 
        NatsJsPubOptions? opts = null,
        CancellationToken cancellationToken = default) 
        where T : ILibraryMessage;
    
   /// <summary>
   /// Отправляет запрос и ожидает ответ.
   /// </summary>
   /// <param name="subject">Куда отправить.</param>
   /// <param name="request">Запрос.</param>
   /// <param name="timeout">Время выполнения.</param>
   /// <param name="pubOpts">Дополнительные настройки публикации сооющения.</param>
   /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
   /// <typeparam name="TRequest">Тип запроса.</typeparam>
   /// <typeparam name="TResponse">Тип ответа.</typeparam>
   /// <returns></returns>
    Task<TResponse> RequestAsync<TRequest, TResponse>(
        string subject,
        TRequest request,
        TimeSpan? timeout = null,
        NatsJsPubOptions? pubOpts = null,
        CancellationToken cancellationToken = default)
        where TRequest : ILibraryMessage
        where TResponse : ILibraryMessage;

    /// <summary>
    /// Подписаться на JetStream как durable(consumer) и читать поток сообщений.
    /// </summary>
    /// <param name="subjectFilter">Фильтр по темам.</param>
    /// <param name="streamName">Имя Subject для прослушивания.</param>
    /// <param name="durableConsumer">Имя консьюмера.</param>
    /// <param name="pubOpts">Доп настройки отправки сообщения.</param>
    /// <param name="consumerConfig">Конфигурация конкретного консьюмера.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <typeparam name="T">Тип.</typeparam>
    /// <returns>Observable с потоком сообщений.</returns>
    IObservable<NatsMessage<T>> FromJetStream<T>(
        string subjectFilter,
        string streamName,
        string durableConsumer,
        NatsJsPubOptions? pubOpts = null,
        ConsumerConfig? consumerConfig = null,
        CancellationToken cancellationToken = default)
    where T : ILibraryMessage;
   
   /// <summary>
   /// Подписаться на сообщение.
   /// </summary>
   /// <param name="subject">Тема, откуда читать.</param>
   /// <param name="queueGroup">Группа.</param>
   /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
   /// <typeparam name="T">Тип.</typeparam>
   /// <returns>Поток сообщений Subject.</returns>
   IObservable<NatsMessage<T>> Observe<T>(
       string subject, 
       string? queueGroup = null, 
       CancellationToken cancellationToken = default )
   where T : ILibraryMessage;
}