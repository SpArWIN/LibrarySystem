using System.ComponentModel.DataAnnotations;

namespace Common.Contracts.Settings;

/// <summary>
/// Настройки JWT.
/// </summary>
public sealed record JwtOptions
{
    /// <summary>Issuer.</summary>
    [Required]
    public required string Issuer { get; init; }

    /// <summary>Audience.</summary>
    [Required]
    public required string Audience { get; init; }

    /// <summary>Секрет для подписи (симметричный ключ).</summary>
    [Required]
    [StringLength(int.MaxValue, MinimumLength = 32)]
    public required string SigningKey { get; init; }

    /// <summary>Время жизни access token в минутах.</summary>
    [Range(0, int.MaxValue, ErrorMessage = "AccessTokenLifetimeMinutes должен быть неотрицательным числом.")]
    public int AccessTokenLifetimeMinutes { get; init; } = 60;
}