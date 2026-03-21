namespace Core.Domain.Enum.BookEnum;

/// <summary>
/// Перечисление статусов книги.
/// </summary>
public enum BookStatus
{
    /// <summary>
    /// Книга доступна для выдачи.
    /// </summary>
    Available = 0,

    /// <summary>
    /// Книга выдана пользователю.
    /// </summary>
    Borrowed = 1,

    /// <summary>
    /// Книга просрочена.
    /// </summary>
    Overdue = 2,

    /// <summary>
    /// Книга утеряна.
    /// </summary>
    Lost = 3,
    
    /// <summary>
    /// Книга забронирована.
    /// </summary>
    Booked = 4
}