using Core.Domain.Enum.BookEnum;

namespace Core.Domain.Models.Loans
{
    /// <summary>
    /// Частичное обновление полей записи о выдаче.
    /// </summary>
    public sealed record LoanPatch
    {
        /// <summary>
        /// Идентификатор записи о выдаче.
        /// </summary>
        public required Guid Id { get; init; }

        /// <summary>
        /// Идентификатор книги.
        /// </summary>
        public Guid? BookId { get; init; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid? UserId { get; init; }

        /// <summary>
        /// Дата начала выдачи (UTC).
        /// </summary>
        public DateTime? LoanDate { get; init; }

        /// <summary>
        /// Дата окончания/срок возврата (UTC).
        /// </summary>
        public DateTime? ExpiryDate { get; init; }

        /// <summary>
        /// Признак возврата книги.
        /// </summary>
        public bool? IsReturned { get; init; }

        /// <summary>
        /// Количество дней просрочки.
        /// </summary>
        public int? OverdueDays { get; init; }

        /// <summary>
        /// Статус экземпляра в рамках данной выдачи.
        /// </summary>
        public BookStatus? BookStatus { get; init; }
    }
}