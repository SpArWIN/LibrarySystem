using Core.Domain.Enum.BookEnum;

namespace Core.Domain.Models;

/// <summary>
/// Сущность выдачи книги пользователю.
/// </summary>
public class Loan
{
    /// <summary>
    /// Идентификатор выдачи.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Идентификатор экземпляра книги.
    /// </summary>
    public Guid? BookCopyId { get; init; }

    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Дата начала аренды книги.
    /// </summary>
    public DateTime LoanDate { get; init; }

    /// <summary>
    /// Дата окончания аренды.
    /// </summary>
    public DateTime ExpiryDate { get; init; }
    
    /// <summary>
    /// Флаг, вернули ли книгу.
    /// </summary>
    public bool IsReturned { get; init; }
    
    /// <summary>
    /// Задолженность (в днях)
    /// </summary>
    public int? OverdueDays { get; init; }
    
    /// <summary>
    /// Статус книги.
    /// </summary>
    public BookStatus BookStatus { get; init; }
    
    /// <summary>
    /// Книга (копия), которая была взята пользователем.
    /// </summary>
    public BookCopy? BookCopy { get; init; } 
}