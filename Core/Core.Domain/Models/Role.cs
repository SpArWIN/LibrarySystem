namespace Core.Domain.Models;

/// <summary>
/// Роли полььзователя.
/// </summary>
public sealed class Role
{
    /// <summary>
    /// Идентификатор Роли.
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Название роли.
    /// </summary>
    public string Name { get; init; }
    
    /// <summary>
    /// Описание роли.
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// Списки ролей для пользователя..
    /// </summary>
    public IReadOnlyCollection<UserRole>? UserRoles { get; init; }

}