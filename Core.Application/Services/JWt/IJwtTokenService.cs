using System.Security.Claims;

namespace Core.Application.Services.JWt;

/// <summary>
/// Сервис выдачи JWT.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>Создать access token (JWT).</summary>
    (string Token, DateTimeOffset ExpiresAtUtc) CreateAccessToken(IReadOnlyCollection<Claim> claims);
}