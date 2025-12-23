using Core.Domain.Models;
using Core.Domain.Models.Pagination;

namespace Core.Domain.Repository
{
    /// <summary>
    /// Репозиторий книг и жанров.
    /// </summary>
    public interface IBookGenreRepository
    {
        /// <summary>
        /// Получить список всех жанров всех книг.
        /// </summary>
        /// <param name="pagination"><see cref="Pagination"/>.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns><see cref="BookGenre"/>.</returns>
        Task<(IEnumerable<BookGenre?> Items, int TotalCount)> GetAllBookGenresAsync(Pagination pagination,
            CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Получить все жанры книг.
        /// </summary>
        /// <param name="booksIds">Список книг.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns><see cref="BookGenre"/>.</returns>
        Task<IEnumerable<BookGenre?>> GetGenresByBooksIdsAsync(IEnumerable<Guid> booksIds,
            CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Получить книги по идентификатору жанров..
        /// </summary>
        /// <param name="genreIds">Идентификаторы жанров.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns><see cref="BookGenre"/>.</returns>
        Task<IEnumerable<BookGenre?>> GetBooksByGenreIdAsync(IEnumerable<Guid> genreIds,
            CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Создать несколько связей между книгами и жанрами.
        /// </summary>
        /// <param name="bookGenres">Список связей книг и жанров.</param>
        /// <returns><see cref="BookGenre"/>.</returns>
        Task<IEnumerable<BookGenre>> CreateBookGenresAsync(IEnumerable<BookGenre> bookGenres);
    
        /// <summary>
        /// Проверка существования связи между книгой и жанром.
        /// </summary>
        /// <param name="bookId">Идентификатор книги.</param>
        /// <param name="genreId">Идентификатор жанра.</param>
        /// <returns>Ture/False.</returns>
        Task<bool> IsBookGenreExistsAsync(Guid bookId, Guid genreId);
    
        /// <summary>
        /// Удалить несколько связей между книгами и жанрами.
        /// </summary>
        /// <param name="bookGenres">Список связей для удаления.</param>
        /// <returns>Идентификаторы удалённых связей.</returns>
        Task<IEnumerable<Guid>> DeleteBookGenresAsync(IEnumerable<BookGenre> bookGenres);
    }
}