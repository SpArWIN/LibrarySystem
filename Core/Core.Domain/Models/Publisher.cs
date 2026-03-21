namespace Core.Domain.Models;

/// <summary>
/// Сущность издателя.
/// </summary>
public sealed class Publisher
{
    /// <summary>
    /// Идентификатор издателя.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Название издательства.
    /// </summary>
    public required string PublisherName { get; init; }

    /// <summary>
    /// Адрес издательства.
    /// </summary>
    public string? Address { get; init; }

    /// <summary>
    /// Дата основания издательства.
    /// </summary>
    public DateTime? DateOfFound { get; init; }
}