namespace Core.Domain.Models;

/// <summary>
/// Связующая сущность жанра и книгу.
/// </summary>
public sealed class BookGenre
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public Guid Id {get; init;}
    
    /// <summary>
    /// Идентификатор книги.
    /// </summary>
    public Guid BookId { get; init; }
    
    /// <summary>
    /// Идентификатор жанра.
    /// </summary>
    public Guid GenreId { get; init; }
    
    /// <summary>
    /// Навигационное свойство Книги.
    /// </summary>
    public Book? Book { get; init; } 
    
    /// <summary>
    /// Навигационное свойство жанра.
    /// </summary>
    public Genre? Genre { get; init; }
}