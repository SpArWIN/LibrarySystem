namespace Common.Contracts.Auth;

/// <summary>
/// Ответ на обновление токена.
/// </summary>
public sealed record RefreshResponseDto
{
    /// <summary>Access token (JWT).</summary>
    public required string AccessToken { get; init; }

    /// <summary>UTC время истечения access token.</summary>
    public required DateTimeOffset AccessExpiresAtUtc { get; init; }

    /// <summary>Новый refresh token.</summary>
    public required string RefreshToken { get; init; }

    /// <summary>UTC время истечения refresh token.</summary>
    public required DateTimeOffset RefreshExpiresAtUtc { get; init; }
}