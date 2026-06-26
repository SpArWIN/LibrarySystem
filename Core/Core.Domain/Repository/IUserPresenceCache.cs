namespace Core.Domain.Repository;

/// <summary>
/// Сервис для проверки существования пользователя с использованием Bloom Filter.
/// </summary>
public interface IUserPresenceCache
{
    /// <summary>
    ///  Проверить, существует ли пользователь с таким username.
    /// </summary>
    /// <param name="userName">Имя пользователя.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>true — возможно существует (нужно проверить в БД), false — точно не существует</returns>
    Task<bool> MightExistAsync(string userName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Добавить пользователя в кэш (при регистрации или первом успешном входе).
    /// </summary>
    Task AddAsync(string username, CancellationToken ct = default);
    
    /// <summary>
    /// Добавить нескольких пользователей (при загрузке данных).
    /// </summary>
    Task AddRangeAsync(IEnumerable<string> usernames, CancellationToken ct = default);
}