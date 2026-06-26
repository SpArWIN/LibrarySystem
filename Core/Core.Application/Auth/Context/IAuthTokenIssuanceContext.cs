namespace Core.Application.Auth.Context;

/// <summary>
/// Данные выданных токенов: заполняется behavior, читается handler при сборке ответа.
/// </summary>
public interface IAuthTokenIssuanceContext
{
    /// <summary>JWT access-токен.</summary>
    string AccessToken { get; set; }

    /// <summary>UTC-время истечения access-токена.</summary>
    DateTimeOffset AccessExpiresAtUtc { get; set; }

    /// <summary>Новый refresh-токен (выдаётся клиенту один раз).</summary>
    string RefreshToken { get; set; }

    /// <summary>UTC-время истечения refresh-токена.</summary>
    DateTimeOffset RefreshExpiresAtUtc { get; set; }

    /// <summary>Имена ролей пользователя для <c>UserInfo</c>.</summary>
    string?[] Roles { get; set; }

    /// <summary>Permissions (scopes), попадают в JWT и в ответ login/register.</summary>
    IReadOnlyCollection<string> Permissions { get; set; }
}
