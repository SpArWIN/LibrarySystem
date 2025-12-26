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
   /// Добавить refresh-сессию.
   /// </summary>
   /// <param name="session"><see cref="RefreshSession"/>.</param>
   /// <param name="ct"><see cref="CancellationToken"/>.</param>
    Task AddRefreshSessionAsync(RefreshSession session, CancellationToken ct = default);
   
  /// <summary>
  /// Отозвать refresh-сессию (опционально указать замену при ротации).
  /// </summary>
  /// <param name="sessionId">Id сессии.</param>
  /// <param name="revokedAtUtc">Дата отзыва.</param>
  /// <param name="replacedBySessionId">Замена идентификатором сессии.</param>
  /// <param name="ct"><see cref="CancellationToken"/>.</param>
    Task RevokeRefreshSessionAsync(Guid sessionId, DateTimeOffset revokedAtUtc,
      Guid? replacedBySessionId, 
      CancellationToken ct = default);
  
}