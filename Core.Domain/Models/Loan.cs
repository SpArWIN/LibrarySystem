using Core.Domain.Enum.BookEnum;

namespace Core.Domain.Models
{
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
        /// Идентификатор книги.
        /// </summary>
        public Guid? BookId { get; init; }

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
        /// Пользователь, взявший книгу.
        /// </summary>
        public User? User { get; init; }
    
        /// <summary>
        /// Книга, которая была взята пользователем.
        /// </summary>
        public Book? Book { get; init; } 
    }
}