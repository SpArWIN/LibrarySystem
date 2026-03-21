using Polly;

namespace Common.Policies.Models;

/// <summary>
/// Настройки опций ретраев.
/// </summary>
public sealed class RetryOptions
{
    /// <summary>Макс. число попыток.</summary>
    public int MaxRetryAttempts { get; init; } = 3;

    /// <summary>Базовая задержка между попытками.</summary>
    public TimeSpan BaseDelay { get; init; } = TimeSpan.FromMilliseconds(200);

    /// <summary>Тип нарастания задержки.</summary>
    public DelayBackoffType Backoff { get; init; } = DelayBackoffType.Exponential;

    /// <summary>Включить джиттер.</summary>
    public bool UseJitter { get; init; } = true;
}