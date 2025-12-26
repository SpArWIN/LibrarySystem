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
    /// <returns></returns>
    Task<Publisher> GetPublishersByParams(string parameters);
    
    /// <summary>
    /// Добавить издателей.
    /// </summary>
    /// <param name="publishers">Список издаталей.</param>
    Task<IEnumerable<Guid>> AddPublishersAsync(IEnumerable<Publisher> publishers);

    /// <summary>
    /// Обновить несколько издателей.
    /// </summary>
    /// <param name="publishers">Список издателей, которые нужно обновить.</param>
    Task UpdatePublishersAsync(IEnumerable<Publisher> publishers);
    
    /// <summary>
    /// Удалить издателей.
    /// </summary>
    /// <param name="publishers">Список издаталей, которых нужно удалить.</param>
    Task DeletePublishersAsync(IEnumerable<Publisher> publishers);
    
    /// <summary>
    /// Проверить не существующих издателей.
    /// </summary>
    /// <param name="publisherIds">Идентификаторы издателей.</param>
    /// <returns>Список существующих издателей.</returns>
    Task<List<Guid>> GetMissingPublisherIdsAsync(IEnumerable<Guid> publisherIds);

}