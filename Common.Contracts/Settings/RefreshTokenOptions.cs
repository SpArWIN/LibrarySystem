namespace Common.Contracts.Settings;

/// <summary>
/// Настройки хеширования.
/// </summary>
public sealed record RefreshTokenOptions
{
    /// <summary>Время жизни refresh token в днях.</summary>
    public int LifetimeDays { get; init; } = 30;

    /// <summary>Секрет для хэширования refresh token.</summary>
    public required string Pepper { get; init; }
}