namespace Common.Contracts.Storage.Files;

/// <summary>
/// Результат удаления.
/// </summary>
public sealed record DeleteFileResponse
{
    /// <summary>Удалённый ключ.</summary>
    public required string Key { get; init; }
    
    /// <summary>Был ли удалён объект.</summary>
    public bool Deleted { get; init; }
}