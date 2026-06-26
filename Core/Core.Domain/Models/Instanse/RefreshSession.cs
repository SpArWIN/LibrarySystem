namespace Core.Domain.Models.Instanse;

/// <summary>
/// Таблица сессий.
/// </summary>
public sealed class RefreshSession
{
    /// <summary>Идентификатор refresh-сессии.</summary>
    public Guid Id { get; init; }

    /// <summary>Идентификатор пользователя.</summary>
    public Guid UserId { get; init; }

    /// <summary>Хэш refresh токена</summary>
    public required string TokenHash { get; init; }

    /// <summary>UTC дата создания.</summary>
    public DateTimeOffset CreatedAtUtc { get; init; }

    /// <summary>UTC дата истечения.</summary>
    public DateTimeOffset ExpiresAtUtc { get; init; }

    /// <summary>UTC дата отзыва (если null — активен).</summary>
    public DateTimeOffset? RevokedAtUtc { get; set; }

    /// <summary>Последний выданный access token (для повторного login без ротации).</summary>
    public string? AccessToken { get; set; }

    /// <summary>UTC истечения access token.</summary>
    public DateTimeOffset? AccessExpiresAtUtc { get; set; }

    /// <summary>Идентификатор сессии, которая заменила эту.</summary>
    public Guid? ReplacedBySessionId { get; set; }

    /// <summary>Проверить, активна ли сессия.</summary>
    public bool IsActive(DateTimeOffset nowUtc)
        => RevokedAtUtc is null && ExpiresAtUtc > nowUtc;
}