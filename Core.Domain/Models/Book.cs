namespace Core.Domain.Models
{
    /// <summary>
    /// Сущность книги.
    /// </summary>
    public sealed class Book
    {
        /// <summary>
        /// Идентификатор книги.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Название книги.
        /// </summary>
        public string Title { get; init; } = null!;

        /// <summary>
        /// Описание книги.
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// Страна издания книги.
        /// </summary>
        public string? Country { get; init; }

        /// <summary>
        /// Идентификатор издателя.
        /// </summary>
        public Guid PublisherId { get; init; }

        /// <summary>
        /// Издатель книги.
        /// </summary>
        public Publisher Publisher { get; init; } = null!;
    
        /// <summary>
        /// Уникальный ключ книги.
        /// </summary>
        public string BookKey { get; set; } = null!;
    
        /// <summary>
        /// Навигационное свойство для связи с книгами.
        /// </summary>
        public List<BookAuthor> BookAuthors { get; } = [];
    
        /// <summary>
        /// Навигационное свойство для связи с жанрами.
        /// </summary>
        public List<BookGenre> BookGenres { get; } = [];
    
        /// <summary>Общее количество экземпляров.</summary>
        public int TotalCopies { get; init; }  
    }
}