using Core.Domain.Models;
using Core.Domain.Models.Pagination;

namespace Core.Domain.Repository;

/// <summary>
/// Репозиторий <see cref="User"/>
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Получить всех пользователей.
    /// </summary>
    /// <param name="pagination"><see cref="Pagination"/>.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="User"/>.</returns>
    Task<(IEnumerable<User?> Items, int TotalCount)> GetAllUsersAsync(Pagination pagination,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить пользователей по их идентификаторам.
    /// </summary>
    /// <param name="usersIds">Идентификаторы юзверей.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="User"/>.</returns>
    Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<Guid> usersIds,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить пользователя по логину.
    /// </summary>
    /// <param name="userName">Логин.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="User"/>.</returns>
    Task<User?> GetUserByNameAsync(string userName,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновить список пользователей.
    /// </summary>
    /// <param name="users">Список пользователей для обновления.</param>
    Task UpdateUsersAsync(IEnumerable<User> users);
    
    /// <summary>
    /// Удалить список пользователей.
    /// </summary>
    /// <param name="users">Списко пользователей на удаление.</param>
    Task DeleteUsersAsync(IEnumerable<User> users);

    /// <summary>
    /// Получить список ролей пользователей.
    /// </summary>
    /// <returns>Слоаврь, где ID- идентификатор пользователя. Значение, его роль.</returns>
    Task<Dictionary<Guid, string>> GetRolesUser();
    
    /// <summary>
    /// Добавить пользователей.
    /// </summary>
    /// <param name="users"><see cref="User"/> Список пользователей.</param>
    Task AddUsersAsync(IEnumerable<User> users);
}