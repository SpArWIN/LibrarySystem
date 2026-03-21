namespace Common.Contracts.Settings;

/// <summary>
/// Настройки JWT.
/// </summary>
public sealed record JwtOptions
{
    /// <summary>Issuer.</summary>
    public required string Issuer { get; init; }

    /// <summary>Audience.</summary>
    public required string Audience { get; init; }

    /// <summary>Секрет для подписи (симметричный ключ).</summary>
    public required string SigningKey { get; init; }

    /// <summary>Время жизни access token в минутах.</summary>
    public int AccessTokenLifetimeMinutes { get; init; } = 60;
}