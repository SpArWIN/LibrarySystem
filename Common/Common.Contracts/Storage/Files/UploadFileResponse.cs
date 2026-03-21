namespace Common.Contracts.Storage.Files;

/// <summary>
/// Результат загрузки файла.
/// </summary>
public sealed record UploadFileResponse
{
    /// <summary>Ключ файла (bucket:object).</summary>
    public required string Key { get; init; }
    
    /// <summary>Имя бакета.</summary>
    public required string Bucket { get; init; }
    
    /// <summary>Имя файла.</summary>
    public required string ObjectName { get; init; }
}