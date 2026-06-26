namespace Common.Contracts.Auth;

/// <summary>
/// Права текущего пользователя из JWT (для UI: меню, кнопки, guards).
/// </summary>
public sealed record CurrentUserPermissionsDto
{
    /// <summary>Идентификатор пользователя.</summary>
    public required Guid UserId { get; init; }

    /// <summary>Логин.</summary>
    public required string Username { get; init; }

    /// <summary>Роли (claim <c>role</c>).</summary>
    public required IReadOnlyCollection<string> Roles { get; init; }

    /// <summary>
    /// Permissions / scopes (claim <c>scope</c>) — то же, что проверяет <c>HasPermission</c>.
    /// </summary>
    public required IReadOnlyCollection<string> Permissions { get; init; }

    /// <summary>
    /// Имена политик ASP.NET, для которых у пользователя есть permission.
    /// </summary>
    public required IReadOnlyCollection<string> Policies { get; init; }

    /// <summary>Tenant из JWT, если был при логине.</summary>
    public Guid? LibraryId { get; init; }
}
