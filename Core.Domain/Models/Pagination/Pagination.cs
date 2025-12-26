namespace Core.Domain.Models.Pagination;

/// <summary>
/// Пагинация.
/// </summary>
public sealed class Pagination
{
    /// <summary>
    /// Номер страницы.
    /// </summary>
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// Количество элементов на странице.
    /// </summary>
    public int PageSize { get; set; } = 10;
}