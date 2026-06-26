namespace Common.Contracts.Auth;

/// <summary>
/// Роль пользователя.
/// </summary>
public sealed class RoleDto
{
    /// <summary>Идентификатор роли.</summary>
    public required Guid Id { get; init; }
    
    /// <summary>Название роли.</summary>
    public required string Name { get; init; }
    
    /// <summary>Описание роли.</summary>
    public string? Description { get; init; }
}