using Core.Domain.Models.Inventory;

namespace Core.Domain.Repository;

/// <summary>
/// Репозиторий подсчётов инвентаризации книг.
/// </summary>
public interface IInventoryReadRepository
{
    /// <summary>Счётчики для набора книг.</summary>
    Task<IReadOnlyList<CopyCountres>> GetCopyCountersAsync(IEnumerable<Guid> bookIds, CancellationToken ct = default);
    
    /// <summary>Суммарное кол-во копий по названию и издателю.</summary>
    Task<int> GetCopiesCountAsync(string title, Guid publisherId, CancellationToken ct = default);
    
    /// <summary>
    /// Проверить наличие и существование книги с автором.
    /// </summary>
    /// <param name="keys">Ключ в виде названия книги и идентификатора жанра.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<IReadOnlyDictionary<(string Title, Guid PublisherId), bool>> 
        BooksExistAsync(IEnumerable<(string Title, Guid PublisherId)> keys, CancellationToken ct = default);
    
    
    /// <summary>Быстрый подсчёт доступности для одной книги.</summary>
    Task<int> GetAvailableCopiesAsync(Guid bookId, CancellationToken ct = default);
}