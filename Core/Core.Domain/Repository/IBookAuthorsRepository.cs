using Core.Domain.Models;

namespace Core.Domain.Repository;

/// <summary>
/// Репозиторий на <see cref="BookAuthor"/>.
/// </summary>
public interface IBookAuthorsRepository
{
    /// <summary>
    /// Получить всех авторов книг по их идентификаторам.
    /// </summary>
    /// <param name="booksIds">Идентификаторы книг.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Авторы.</returns>
    Task<IEnumerable<Author?>> GetAuthorsByBooksIds(IEnumerable<Guid> booksIds, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить список связей  по идентификаторам авторов и книг.
    /// </summary>
    /// <param name="authorIds">Идентификаторы авторов.</param>
    /// <param name="bookIds">Идентификаторы книг.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="BookAuthor"/>.</returns>
    Task<List<BookAuthor>> GetAssociationsAsync(IEnumerable<Guid> authorIds, IEnumerable<Guid> bookIds
        ,CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все книги по авторам.
    /// </summary>
    /// <param name="authorIds">Идентификаторы авторов.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="Book"/>.</returns>
    Task<IEnumerable<Book?>> GetBooksByAuthorsIdsAsync(IEnumerable<Guid> authorIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить связи между автором и книгами.
    /// </summary>
    /// <param name="associations">Связи.</param>
    Task AddAuthorsToBooksAsync(IEnumerable<(Guid AuthorId, Guid BookId)> associations);
    
    /// <summary>
    /// Массовое удаление связей между авторами и её книгой.
    /// </summary>
    /// <param name="associations"><see cref="BookAuthor"/>.</param>
    ValueTask RemoveRangeAsync(IEnumerable<BookAuthor> associations);
    
    /// <summary>
    /// Проверить существует ли связь между автором и книгой.
    /// </summary>
    /// <param name="authorId">Идентификатор автора.</param>
    /// <param name="bookId">Идентификатор книги.</param>
    /// <returns>True, если связь существует; иначе - False.</returns>
    Task<bool> IsAuthorAssociatedWithBookAsync(Guid authorId, Guid bookId);
}