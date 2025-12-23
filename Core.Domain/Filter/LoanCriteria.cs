using Core.Domain.Enum.BookEnum;

namespace Core.Domain.Filter
{
    /// <summary>
    /// Критерии фильтрации записей о выдаче/бронировании.
    /// </summary>
    public sealed record LoanCriteria
    {
        /// <summary>
        /// Идентификатор пользователя (фильтр по пользователю).
        /// </summary>
        public Guid? UserId { get; init; }
    
        /// <summary>
        /// Идентификатор книги (фильтр по книге).
        /// </summary>
        public Guid? BookId { get; init; }
    
        /// <summary>
        /// Оставить только активные записи ( не возвращены)
        /// </summary>
        public bool IsActiveOnly { get; init; }
    
        /// <summary>
        /// Оставить только просроченные записи.
        /// </summary>
        public bool? OverdueOnly { get; init; }
    
        /// <summary>
        /// Набор статусов, по которым требуется фильтрация.
        /// </summary>
        public IReadOnlyCollection<BookStatus>? Statuses { get; init; }
    
        /// <summary>
        /// Нижняя граница даты создания/начала выдачи (UTC).
        /// </summary>
        public DateTime? FromUtc { get; init; }

        /// <summary>
        /// Верхняя граница даты создания/начала выдачи (UTC).
        /// </summary>
        public DateTime? ToUtc { get; init; }
    }
}