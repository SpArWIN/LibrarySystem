using Common.Contracts.User;

namespace Common.Contracts.Auth;

/// <summary>
/// Ответ на авторизацию.
/// </summary>
public record AuthorizeResponse
{
    /// <summary>Access token (JWT).</summary>
    public required string AccessToken { get; init; }

    /// <summary>UTC время истечения access token.</summary>
    public required DateTimeOffset AccessExpiresAtUtc { get; init; }

    /// <summary>Refresh token (возвращается клиенту один раз).</summary>
    public required string RefreshToken { get; init; }

    /// <summary>UTC время истечения refresh token.</summary>
    public required DateTimeOffset RefreshExpiresAtUtc { get; init; }
    
    /// <summary>
    /// Пользователь.
    /// </summary>
    public required UserInfoDto UserInfo { get; init; }
}