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
}