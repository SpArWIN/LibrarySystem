using Core.Domain.Models;
using Core.Domain.Models.Pagination;

namespace Core.Domain.Repository;

/// <summary>
/// Репозиторий на <see cref="Publisher"/>.
/// </summary>
public interface IPublisherRepository
{
    /// <summary>
    /// Получить всех издателей.
    /// </summary>
    /// <param name="pagination"><see cref="Pagination"/>.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Список издателей.</returns>
    Task<(IEnumerable<Publisher> Items, int TotalCount)> GetAllPublishersAsync(Pagination pagination,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить издателей по их идентификаторам.
    /// </summary>
    /// <param name="publisherIds">Идентификаторы издателей.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="Publisher"/>.</returns>
    Task<IEnumerable<Publisher>> GetPublishersByIdsAsync(IEnumerable<Guid> publisherIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить издателя по параметрам.
    /// </summary>
    /// <param name="parameters">Совпадения в виде названия, адреса. </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns></returns>
    Task<Publisher> GetPublishersByParams(string parameters, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить издателей.
    /// </summary>
    /// <param name="publishers">Список издаталей.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task<IEnumerable<Guid>> AddPublishersAsync(IEnumerable<Publisher> publishers, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить несколько издателей.
    /// </summary>
    /// <param name="publishers">Список издателей, которые нужно обновить.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    ValueTask UpdatePublishersAsync(IEnumerable<Publisher> publishers,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить издателей.
    /// </summary>
    /// <param name="publisherIds">Идентификаторы издетелей, которые нужно удалить..</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task DeletePublishersAsync(IEnumerable<Guid> publisherIds, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить не существующих издателей.
    /// </summary>
    /// <param name="publisherIds">Идентификаторы издателей.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Список существующих издателей.</returns>
    Task<List<Guid>> GetMissingPublisherIdsAsync(IEnumerable<Guid> publisherIds,  
        CancellationToken cancellationToken = default);
}