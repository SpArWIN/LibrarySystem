using Common.Policies.Models;
using Polly;
using Polly.Retry;

namespace Common.Policies.Pollicies.Extension;

/// <summary>
/// Расширение на политики.
/// </summary>
public static class ResilienceBuilderExtensions
{
    /// <summary>
    /// Добавить ретраи с джиттером и предикатом «транзиентности».
    /// </summary>
    public static ResiliencePipelineBuilder AddJitterPolicy(this ResiliencePipelineBuilder pipelineBuilder,
        RetryOptions options,
        Func<Exception, bool> isTransient)
    {
        ArgumentNullException.ThrowIfNull(pipelineBuilder);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(isTransient);
        var retry = new RetryStrategyOptions()
        {
            MaxRetryAttempts = options.MaxRetryAttempts,
            BackoffType = options.Backoff,
            Delay = options.BaseDelay,
            UseJitter = options.UseJitter,
            ShouldHandle = new PredicateBuilder().Handle(isTransient)
        };
        return pipelineBuilder.AddRetry(retry);
    }
    
    /// <summary>
    /// Сконструировать pipeline по делегату.
    /// </summary>
    public static ResiliencePipeline Build(this ResiliencePipelineBuilder builder)
        => builder.Build();
}