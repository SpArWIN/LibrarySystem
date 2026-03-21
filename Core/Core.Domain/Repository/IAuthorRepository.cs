using Core.Domain.Models;
using Core.Domain.Models.Pagination;

namespace Core.Domain.Repository;

/// <summary>
/// Интерфейс репозитория автора.
/// </summary>
public interface IAuthorRepository
{
    /// <summary>
    /// Получить список авторов c пагинацией.
    /// </summary>
    /// <param name="pagination"><see cref="Pagination"/>.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Список авторов с общим количеством.</returns>
    Task<(IEnumerable<Author> items, int totalCount)> GetAuthorsAsync(Pagination pagination,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить список всех авторов по идентификаторам.
    /// </summary>
    /// <param name="authorIds">Список авторов.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Список авторов.</returns>
    Task<IEnumerable<Author>> GetAuthorsByIdsAsync(IEnumerable<Guid> authorIds, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить автора по параметрам (имя, фамилия или отчество.)
    /// </summary>
    /// <param name="parameters">Строка в виде параметра.</param>
    /// <returns><see cref="Author"/>.</returns>
    Task<Author> GetByParamsAsync(string parameters);
    
    /// <summary>
    /// Для задач бизнес логики.
    /// </summary>
    /// <param name="batchSize">Количество.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="Author"/>.</returns>
    IAsyncEnumerable<Author> StreamAllAuthorsAsync( int batchSize,CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удалить авторов.
    /// </summary>
    /// <param name="authorIds">Идентификаторы книг.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task DeleteAuthorsAsync(IEnumerable<Guid> authorIds, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Добавить книги.
    /// </summary>
    /// <param name="authors">Авторы.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task<List<Guid>> AddRangeAsync(IEnumerable<Author> authors, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновить авторов.
    /// </summary>
    /// <param name="authors">Список авторов.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Список авторов.</returns>
    ValueTask<IEnumerable<Author>> UpdateAuthorsAsync(IEnumerable<Author> authors, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удалить авторов из базы.
    /// </summary>
    /// <param name="authorIds">Идентификаторы авторов.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task RemoveAuthorIdsAsync(IEnumerable<Guid> authorIds, CancellationToken cancellationToken = default);
}