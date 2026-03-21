namespace Common.Contracts.Models.Pagination;

/// <summary>
/// Обёртка пагинации.
/// </summary>
/// <typeparam name="T">Тип.</typeparam>
public sealed record Paginated<T>
{
    /// <summary>
    /// Массив элементов, для пагинации.
    /// </summary>
    public required IReadOnlyList<T> Items { get; init; }
    
    /// <summary>
    /// Страница.
    /// </summary>
    public required int Page { get; init; }
    
    /// <summary>
    /// Количество элементов на странице.
    /// </summary>
    public required int PageSize { get; init; }
    
    /// <summary>
    /// Общее количество элементов.
    /// </summary>
    public required long TotalCount { get; init; }
}