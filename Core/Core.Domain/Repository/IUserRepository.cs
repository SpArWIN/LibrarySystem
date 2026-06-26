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
    /// <param name="login">Логин.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="User"/>.</returns>
    Task<User?> GetUserByLoginAsync(string login,
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
    Task<Dictionary<Guid, string>> GetRolesUser( CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить список ролей пользователей.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Список ролей пользователя.</returns>
    Task<List<Role>> GetUsersRoles(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить названия ролей пользователя.
    /// </summary>
    /// <param name="user"><see cref="User"/>.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Массив названий ролей пользователя.</returns>
    Task<List<string?>> GetUserRolesName(User user, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить предзаданные роли.
    /// </summary>
    /// <returns><see cref="Role"/>.</returns>
    Task<List<Role>> GetDefaultRole();

    /// <summary>
    /// Получить идентификаторы ролей.
    /// </summary>
    /// <returns>Список идентификаторов ролей.</returns>
    Task<IReadOnlyCollection<Guid>> GetDefaultRoleIdsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить названия ролей по их идентификаторам.
    /// </summary>
    /// <param name="roleIds">Список идентификаторов ролей.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Список названий ролей.</returns>
    Task<IReadOnlyCollection<string>> GetRoleNamesByIdsAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить пользователей.
    /// </summary>
    /// <param name="users"><see cref="User"/> Список пользователей.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    Task AddUsersAsync(IEnumerable<User> users, CancellationToken ct = default);
}