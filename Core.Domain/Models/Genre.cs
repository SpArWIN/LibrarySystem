namespace Core.Domain.Models
{
    /// <summary>
    /// Сущность жанра книги.
    /// </summary>
    public sealed class Genre
    {
        /// <summary>
        /// Идентификатор жанра.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Название жанра.
        /// </summary>
        public string NameGenre { get; init; } = null!;
    
        /// <summary>
        /// Навигационное свойство для связи с книгами.
        /// </summary>
        public List<BookGenre> BookGenres { get; } = [];
    }
}