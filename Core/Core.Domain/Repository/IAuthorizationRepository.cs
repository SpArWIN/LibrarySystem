using Core.Domain.Models;
using Core.Domain.Models.Instanse;

namespace Core.Domain.Repository;

/// <summary>
/// Репозиторий авторизации.
/// </summary>
public interface IAuthorizationRepository
{
    /// <summary>
    /// Получить пользователя по логину с ролями.
    /// </summary>
    /// <param name="username">Имя пользователя.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="User"/>.</returns>
    Task<User?> FindUserByUsernameAsync(string username, CancellationToken ct = default);
    
   /// <summary>
   /// Получить пользователя по Id вместе с ролями.
   /// </summary>
   /// <param name="userId">Идентикатор пользователя.</param>
   /// <param name="ct"><see cref="CancellationToken"/>.</param>
   /// <returns><see cref="User"/>.</returns>
    Task<User?> FindUserByIdAsync(Guid userId, CancellationToken ct = default);
   
    /// <summary>
    /// Найти refresh-сессию по хэшу токена
    /// </summary>
    /// <param name="tokenHash">Хеш токена.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<RefreshSession?> FindRefreshSessionByHashAsync(string tokenHash, CancellationToken ct = default);
    
    /// <summary>
    /// Добавить связи с ролями пользователя.
    /// </summary>
    /// <param name="userRoles"><see cref="UserRole"/>.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>.</returns>
    Task AddUserRolesAsync(IEnumerable<UserRole> userRoles, CancellationToken ct = default);
  
}