using Core.Domain.Models;
using Core.Domain.Models.Pagination;

namespace Core.Domain.Repository
{
    /// <summary>
    /// Репозиторий книги.
    /// </summary>
    public interface IBookRepository
    {
        /// <summary>
        /// Получить все книги с пагинацией.
        /// </summary>
        /// <param name="pagination"><see cref="Pagination"/>.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns>Список книг.</returns>
        Task<(IEnumerable<Book> Items, int TotalCount)> GetAllBooksAsync(Pagination pagination,
            CancellationToken cancellationToken);
    
        /// <summary>
        /// Получить список книг по их идентификаторам.
        /// </summary>
        /// <param name="bookIds">Список идентификаторов.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns>Список книг.</returns>
        Task<IEnumerable<Book>> GetBooksByIdsAsync(IEnumerable<Guid> bookIds, CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Для задач бизнес логики.
        /// </summary>
        /// <param name="batchSize">Количество.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Book"/>.</returns>
        IAsyncEnumerable<Book> StreamAllAsync(int batchSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить книги.
        /// </summary>
        /// <param name="booksIds">Идентификаторы книг.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        Task DeleteBooks(IEnumerable<Guid> booksIds, CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Добавить книги.
        /// </summary>
        /// <param name="books">Идентификаторы книг.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        Task AddRangeAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Обновить книги.
        /// </summary>
        /// <param name="books">Список книг.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns>Список книг.</returns>
        Task<IEnumerable<Book>> UpdateRangeAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Удалить книги из базы.
        /// </summary>
        /// <param name="bookIds">Идентификаторы книг.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        Task RemoveBooksIdsAsync(IEnumerable<Guid> bookIds, CancellationToken cancellationToken = default);
    }
}