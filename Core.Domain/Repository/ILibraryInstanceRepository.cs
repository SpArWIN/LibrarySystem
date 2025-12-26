using Core.Domain.Models.Instanse;

namespace Core.Domain.Repository;

/// <summary>
/// Репозиторий реестра библиотек (CentralDb).
/// </summary>
public interface ILibraryInstanceRepository
{
    /// <summary>Проверить существует ли база..</summary>
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    
    /// <summary>Добавить запись о библиотеке.</summary>
    Task AddAsync(LibraryInstance instance, CancellationToken ct = default);

    /// <summary>Найти библиотеку по идентификатору.</summary>
    Task<LibraryInstance?> FindAsync(Guid id, CancellationToken ct = default);

    /// <summary>Удалить запись о библиотеке.</summary>
    Task RemoveAsync(LibraryInstance instance, CancellationToken ct = default);
}