namespace Core.Domain.Models.Inventory;

/// <summary>
/// Элемент списка каталога книг со счётчиками экземпляров.
/// </summary>
public sealed record BookListItem
{
    /// <summary>Идентификатор книги в каталоге.</summary>
    public required Guid BookId { get; init; }

    /// <summary>Название.</summary>
    public required string Title { get; init; }

    /// <summary>Идентификатор издателя.</summary>
    public required Guid PublisherId { get; init; }

    /// <summary>Название издателя.</summary>
    public required string PublisherName { get; init; }
    
    /// <summary>Жанры книги.</summary>
    public List<string>? Genres { get; init; }

    /// <summary>Общее количество экземпляров.</summary>
    public required int TotalCopies { get; init; }

    /// <summary>Доступно к выдаче.</summary>
    public required int AvailableCopies { get; init; }
}