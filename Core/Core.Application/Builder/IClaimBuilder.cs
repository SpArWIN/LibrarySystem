using System.Security.Claims;

namespace Core.Application.Builder;

/// <summary>
/// Билдер построения Claim для Access токена.
/// </summary>
public interface IClaimBuilder
{
    /// <summary>
    /// Построить набор claims для access token.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="username">Ник.</param>
    /// <param name="roleNames">Массив ролей.</param>
    /// <param name="scopes">Права.</param>
    /// <param name="libraryId">Идентификатор базы данных.</param>
    IReadOnlyCollection<Claim> BuildAccessClaims(
        Guid userId,
        string username,
        IReadOnlyCollection<string> roleNames,
        IReadOnlyCollection<string> scopes,
        Guid? libraryId);
}