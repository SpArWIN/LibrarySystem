using Polly;

namespace Common.Policies.Services;

/// <summary>
/// Сервис выдачи конкретных политик.
/// </summary>
public interface IDbResilience
{
    /// <summary>
    /// Политика записи.
    /// </summary>
    ResiliencePipeline Write { get; }
    
    /// <summary>
    /// Политика чтения.
    /// </summary>
    ResiliencePipeline Read { get; }
    
    /// <summary>
    /// Политика публикации в Nats.
    /// </summary>
    ResiliencePipeline PublishNats { get; }
    
    /// <summary>
    /// Политика запросов.
    /// </summary>
    ResiliencePipeline  RequestNats { get; }
    
    /// <summary>
    /// Политика подключения к Nats.
    /// </summary>
    ResiliencePipeline NatsConnect { get; }
    
    /// <summary>
    /// Политика NatsJetStream.
    /// </summary>
    ResiliencePipeline NatsJetStream { get; }
    
    /// <summary>
    /// Политика подключения к базе данных.
    /// </summary>
    ResiliencePipeline DatabaseConnect { get; }
}