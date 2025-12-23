namespace Core.Domain.Models
{
    /// <summary>
    /// Сущность автора.
    /// </summary>
    public sealed class Author
    {
        /// <summary>
        /// Идентификатор автора.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Фамилия автора.
        /// </summary>
        public string LastName { get; init; } = null!;

        /// <summary>
        /// Имя автора.
        /// </summary>
        public string Name { get; init; } = null!;

        /// <summary>
        /// Отчество автора.
        /// </summary>
        public string? SurName { get; init; }

        /// <summary>
        /// Дата рождения автора.
        /// </summary>
        public DateTime? DateOfBirth { get; init; }

        /// <summary>
        /// Страна происхождения автора.
        /// </summary>
        public string? Country { get; init; }

        /// <summary>
        /// Навигационное свойство для связи с книгами.
        /// </summary>
        public List<BookAuthor> BookAuthors { get; } = [];

    }
}