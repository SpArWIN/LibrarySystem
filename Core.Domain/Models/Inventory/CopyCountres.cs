namespace Core.Domain.Models.Inventory;

/// <summary>
/// Агрегированные счётчики экземпляров одной книги.
/// </summary>
public sealed record CopyCountres
{
    /// <summary>
    /// Идентификатор книги.
    /// </summary>
    public required Guid BookId { get; init; }

    /// <summary>
    /// Общее количество экземпляров книги (логический запас).
    /// </summary>
    public required int Total { get; init; }

    /// <summary>
    /// Количество доступных к выдаче экземпляров.
    /// </summary>
    public required int Available { get; init; }

    /// <summary>
    /// Количество выданных экземпляров (активные выдачи).
    /// </summary>
    public required int Borrowed { get; init; }

    /// <summary>
    /// Количество забронированных экземпляров (активные брони).
    /// </summary>
    public required int Booked { get; init; }

    /// <summary>
    /// Количество списанных/утерянных экземпляров.
    /// </summary>
    public required int Lost { get; init; }
}