using Core.Domain.Enum.BookEnum;
using Core.Domain.Models;

namespace Core.Domain.Repository;

/// <summary>
/// Репозиторий копий книг.
/// </summary>
public interface IBookCopyRepository
{
    /// <summary>
    /// Получить записи копий книги по их идентификатору.
    /// </summary>
    /// <param name="bookIds">Идентификаторы книги.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>.</returns>
    Task<IReadOnlyList<BookCopy>> GetCopiesByBookIdAsync(IEnumerable<Guid> bookIds, CancellationToken ct = default);
    
    /// <summary>
    /// Получить идентификаторы копий книг по книгам.
    /// </summary>
    /// <param name="bookIds">Идентификаторы книг.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>.</returns>
    Task<IReadOnlyList<Guid>>GetCopiesIdsByBookIdAsync(IEnumerable<Guid> bookIds, CancellationToken ct = default);
    
    /// <summary>
    /// Получить доступные копии книг.
    /// </summary>
    /// <param name="bookIds">Идентификаторы книг.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>.</returns>
    Task<IReadOnlyList<BookCopy>> GetAvailableCopiesByBookIdAsync(IEnumerable<Guid> bookIds, CancellationToken ct = default);

    /// <summary>
    /// Обновление статуса копии книг.
    /// </summary>
    /// <param name="copyIds">Идентификаторы копий книг.</param>
    /// <param name="newStatus"><see cref="BookStatus"/>.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>.</returns>
    Task UpdateCopiesAsync(IEnumerable<Guid> copyIds, BookStatus newStatus, CancellationToken ct = default);
    
    
}