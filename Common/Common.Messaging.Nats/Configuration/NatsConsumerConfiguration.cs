using Common.Messaging.Nats.Settings;
using NATS.Client.JetStream.Models;

namespace Common.Messaging.Nats.Configuration;

/// <summary>
/// Конфигурация консьюмера Nats..
/// </summary>
public sealed class NatsConsumerConfiguration
{
    /// <summary>
    /// Название stream в JetStream.
    /// </summary>
    public string StreamName { get; set; } = string.Empty;
    
    /// <summary>
    /// Имя durable consumer.
    /// </summary>
    public string DurableConsumerName { get; set; } = string.Empty;
    
    /// <summary>
    /// Subject фильтр.
    /// </summary>
    public string SubjectFilter { get; set; } = string.Empty;
    
    /// <summary>
    /// Группа консьюмеров (queue group).
    /// </summary>
    public string? QueueGroup { get; set; }
    
    /// <summary>
    /// Максимальное количество параллельных обработчиков.
    /// </summary>
    public int MaxConcurrentHandlers { get; set; } = 1;
    
    /// <summary>
    /// Настройки подписки JetStream.
    /// </summary>
    public Action<NatsJsSubOptions>? ConfigureSubscription { get; set; }
    
    /// <summary>
    /// Настройки консьюмера.
    /// </summary>
    public Action<ConsumerConfig>? ConfigureConsumer { get; set; }
}