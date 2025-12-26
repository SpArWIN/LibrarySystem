namespace Common.Contracts.Permission;

/// <summary>
/// Полномочия.
/// </summary>
public static class Permissions
{
    /// <summary>
    /// Просматривать Бд.
    /// </summary>
    public const string ViewDb = "view_db";
    
    /// <summary>
    /// Создаать БД.
    /// </summary>
    public const string CreateDb = "create_db";
    
    /// <summary>
    /// Выдавать книгу.
    /// </summary>
    public const string IssueBook = "issue_book";
    
    /// <summary>
    /// Получить книгу.
    /// </summary>
    public const string GetBook = "get_book";
}