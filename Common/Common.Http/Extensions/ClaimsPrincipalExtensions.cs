using System.Security.Claims;
using Common.Contracts.Claims;
using Microsoft.AspNetCore.Http;

namespace Common.Http.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Попытаться получить id базы данных из Claim.
    /// </summary>
    /// <param name="user"><see cref="ClaimsPrincipal"/>.</param>
    /// <returns>Ид.</returns>
    public static Guid? TryGetLibraryIdFromClaims(this ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue(ClaimNames.LibraryId);
        return Guid.TryParse(raw, out var id) ? id : null;
    }
    
    /// <summary>
    /// Попытаться получить Id базы данных из заголовка.
    /// </summary>
    /// <param name="request"><see cref="HttpRequest"/>.</param>
    /// <returns>Id.</returns>
    public static Guid? TryGetLibraryIdFromHeader(this HttpRequest request)
    {
        if (!request.Headers.TryGetValue("X-Library-Id", out var values))
            return null;

        var raw = values.FirstOrDefault();
        return Guid.TryParse(raw, out var id) ? id : null;
    }

    /// <summary>
    /// Получить "X-Correlation-Id"; 
    /// </summary>
    /// <param name="request"><see cref="HttpRequest"/>.</param>
    /// <returns>Id.</returns>
    public static Guid? GetCorrelationId(this HttpRequest request)
    {
        if (!request.Headers.TryGetValue("X-Correlation-Id", out var values))
        {
            return Guid.Empty;
        }
        var correlationId = values.FirstOrDefault();
        return Guid.TryParse(correlationId, out var id) ? id : null;
    }
    
    /// <summary>Получить идентификатор пользователя из claims.</summary>
    public static Guid GetUserId(this ClaimsPrincipal user)
        => Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? throw new InvalidOperationException("UserId claim is missing."));
    
    /// <summary>Проверить наличие permission (scope).</summary>
    public static bool HasScope(this ClaimsPrincipal user, string scope)
        => user.HasClaim(ClaimNames.Scope, scope);
}