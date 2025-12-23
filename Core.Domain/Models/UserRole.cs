namespace Core.Domain.Models
{
    /// <summary>
    /// Связь роль с пользователем.
    /// </summary>
    public sealed class UserRole
    {
        /// <summary>Идентификатор пользователя.</summary>
        public Guid UserId { get; init; }

        /// <summary>Идентификатор роли.</summary>
        public Guid RoleId { get; init; }
    
        /// <summary>
        /// Пользователь.
        /// </summary>
        public User? User { get; init; }
    
        /// <summary>
        /// Роль пользователя.
        /// </summary>
        public Role? Role { get; init; }
    
        /// <summary>
        /// Роли пользователя.
        /// </summary>
        public List<UserRole> UserRoles { get; } = [];
    }
}