using Core.Domain.Enum.BookEnum;

namespace Core.Domain.Models;

/// <summary>
/// Физический экземпляр книги (инвентарная единица).
/// </summary>
public sealed class BookCopy
{
    /// <summary>
    /// Идентификатор копии книги.
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Идентификатор книги, к которой это относится.
    /// </summary>
    public Guid BookId { get; init; }
    
    /// <summary>
    /// Инвентарный ключ для поиска и отображения на UI.
    /// </summary>
    public string BookKey { get; init; } = string.Empty;
    
    /// <summary>
    /// Статус экземпляра.
    /// </summary>
    public BookStatus BookStatus { get; set; }
    
    /// <summary>
    /// Каталожная книга.
    /// </summary>
    public Book Book { get; init; } = null!;
}