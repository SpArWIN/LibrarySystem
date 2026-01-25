using Core.Domain.Models.Inventory;

namespace Core.Domain.Repository;

/// <summary>
/// Репозиторий подсчётов инвентаризации книг.
/// </summary>
public interface IInventoryReadRepository
{
    /// <summary>
    /// Получить счётчики для набора книг
    /// </summary>
    /// <param name="bookIds">Идентификаторы книг.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="CopyCountres"/>.</returns>
    Task<IReadOnlyList<CopyCountres>> GetCopyCountersAsync(IEnumerable<Guid> bookIds, CancellationToken ct = default);
    
   /// <summary>
   /// Получить суммарное кол-во копий по названию и издателю.
   /// </summary>
   /// <param name="title">Название книги.</param>
   /// <param name="publisherId">Идентификатор издателя.</param>
   /// <param name="ct"><see cref="CancellationToken"/>.</param>
   /// <returns>Суммарное количество копиий.</returns>
    Task<int> GetCopiesCountAsync(string title, Guid publisherId, CancellationToken ct = default);
    
    /// <summary>
    /// Проверить наличие и существование книги с автором.
    /// </summary>
    /// <param name="keys">Ключ в виде названия книги и идентификатора жанра.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<IReadOnlyDictionary<(string Title, Guid PublisherId), bool>> BooksExistAsync(
        IEnumerable<(string Title, Guid PublisherId)> keys, 
            CancellationToken ct = default);
    
   /// <summary>
   /// Быстрый подсчёт доступности для одной книги.
   /// </summary>
   /// <param name="bookId">ID книги.</param>
   /// <param name="ct"><see cref="CancellationToken"/>.</param>
   /// <returns>Количество экземпляров доступных для этой книги.</returns>
    Task<int> GetAvailableCopiesAsync(Guid bookId, CancellationToken ct = default);
}