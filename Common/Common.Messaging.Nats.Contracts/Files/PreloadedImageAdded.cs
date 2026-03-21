using Common.Messaging.Nats.Contracts.Based;
using ProtoBuf;

namespace Common.Messaging.Nats.Contracts.Files;

/// <summary>
/// Событие: в бакет загружено предопределённое (preload) изображение
/// </summary>
[ProtoContract]
public sealed class PreloadedImageAdded : ILibraryMessage
{
    /// <summary>
    /// Имя бакета.
    /// </summary>
    [ProtoMember(1)]
    public required string Bucket { get; init; }
    
    /// <summary>
    /// Полный ключ объекта в MinIO (включая папки, если есть)
    /// </summary>
    [ProtoMember(2)]
    public required string ObjectKey { get; init; }
    
    /// <summary>
    /// Публичная ссылка на изображение.
    /// </summary>
    [ProtoMember(3)]
    public required string PublicUrl { get; init; }
    
    /// <summary>
    /// MIME-тип файла
    /// </summary>
    [ProtoMember(4)]
    public string? ContentType { get; init; }
    
    /// <summary>
    /// Размер.
    /// </summary>
    [ProtoMember(5)]
    public long? SizeBytes { get; init; }
    
    /// <summary>
    /// Когда файл был успешно загружен (UTC)
    /// </summary>
    [ProtoMember(10)]
    public DateTime UploadedAtUtc { get; init; } = DateTime.UtcNow;
    
    /// <summary>
    /// откуда взято изображение (имя файла без пути)
    /// </summary>
    [ProtoMember(12)]
    public string? SourceFileName { get; init; }
    
}