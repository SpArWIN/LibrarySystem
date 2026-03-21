namespace Common.Contracts.Storage.Files;

/// <summary>
/// Описание объекта хранилища.
/// </summary>
public sealed record FileDescriptor
{
    /// <summary>
    /// Имя бакета.
    /// </summary>
    public required string Bucket {get; init; }
    
    /// <summary>
    /// Имя файла или объекта.
    /// </summary>
    public required string ObjectName {get; init; }
    
    /// <summary>
    /// Тип содержимого.
    /// </summary>
    public string? ContentType { get; init; }
    
    /// <summary>
    /// Размер файла.
    /// </summary>
    public double? Size { get; init; }
    
    /// <summary>
    /// Последнее изменение файла.
    /// </summary>
    public DateTime? LastModified { get; init; }
}