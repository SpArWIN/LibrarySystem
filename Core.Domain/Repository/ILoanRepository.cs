using Core.Domain.Enum.BookEnum;
using Core.Domain.Filter;
using Core.Domain.Models;
using Core.Domain.Models.Loans;
using Core.Domain.Models.Pagination;

namespace Core.Domain.Repository;

/// <summary>
/// Репозиторий на <see cref="Loan"/>.
/// </summary>
public interface ILoanRepository
{
    /// <summary>
    /// Получить запись о выдаче по идентификатору.
    /// </summary>
    /// <param name="loanId">Идентификатор записи о выдаче.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task<Loan?> GetByIdAsync(Guid loanId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Постраничная выборка по критериям.
    /// </summary>
    /// <param name="criteria">Критерии фильтрации.</param>
    /// <param name="page">Параметры страницы.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task<(IEnumerable<Loan> Items, int TotalCount)> GetPageAsync(LoanCriteria criteria, 
        Pagination page, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить активные(не возвращённые) выдачи по списку пользователей.
    /// </summary>
    /// <param name="userIds">Идентификаторы пользователей.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Список выдач.</returns>
    Task<IReadOnlyList<Loan>> GetActiveByUsersAsync(IEnumerable<Guid> userIds, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить активную запись по книге (с пользователем), если она существует.
    /// </summary>
    /// <param name="bookId">Идентификатор книги.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task<Loan?> GetActiveWithUserByBookIdAsync(Guid bookId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Добавить пачку записей о выдаче/бронировании.
    /// </summary>
    /// <param name="loans">Коллекция записей.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task AddRangeAsync(IEnumerable<Loan> loans,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Применить частичные обновления к пачке записей.
    /// </summary>
    /// <param name="updates">Коллекция патчей.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task UpdateRangeAsync(IEnumerable<LoanPatch> updates, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удалить пачку записей о выдаче.
    /// </summary>
    /// <param name="loanIds">Идентификаторы записей.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task DeleteRangeAsync(IEnumerable<Guid> loanIds,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Массово обновить статус по списку записей.
    /// </summary>
    /// <param name="loanIds">Идентификаторы записей.</param>
    /// <param name="newStatus">Новый статус.</param>
    /// <param name="nowUtc">Текущее время (UTC) для аудита/полей дат.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task UpdateStatusRangeAsync(IEnumerable<Guid> loanIds,
        BookStatus newStatus, 
        DateTime nowUtc, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Отметить список записей как возвращённые.
    /// </summary>
    /// <param name="loanIds">Идентификаторы записей.</param>
    /// <param name="returnUtc">Момент возврата (UTC).</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task ReturnRangeAsync(IEnumerable<Guid> loanIds, 
        DateTime returnUtc,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Массово проставить статус «просрочено» на текущий момент.
    /// </summary>
    /// <param name="nowUtc">Текущее время (UTC).</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task<int> MarkOverdueAsync(DateTime nowUtc, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Отменить истекшие бронирования (снятие броней).
    /// </summary>
    /// <param name="nowUtc">Текущее время (UTC).</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task<int> CancelExpiredBookingsAsync(DateTime nowUtc, CancellationToken cancellationToken = default);
    
}