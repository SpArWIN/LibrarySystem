namespace Common.Contracts.Auth;

/// <summary>
/// Трансвер связи ролей и пользователя.
/// </summary>
public sealed class UserRoleDto
{
    /// <summary>Идентификатор пользователя.</summary>
    public required Guid UserId { get; init; }

    /// <summary>Идентификатор роли.</summary>
    public required Guid RoleId { get; init; }
    
    /// <summary>Информация о роли.</summary>
    public RoleDto? Role { get; init; }
}