namespace Core.Domain.Models
{
    /// <summary>
    /// Связующая сущность книги и автора.
    /// </summary>
    public sealed class BookAuthor
    {
        /// <summary>
        /// Идентификатор записи.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Идентификатор автора.
        /// </summary>
        public Guid AuthorId { get; init; }

        /// <summary>
        /// Идентификатор книги.
        /// </summary>
        public Guid BookId { get; init; }
    
        /// <summary>
        /// Навигационное свойство Автора..
        /// </summary>
        public Author? Author { get; init; }

        /// <summary>
        ///  Навигационное свойство к сущности Book
        /// </summary>
        public Book? Book { get; init; }
    }
}