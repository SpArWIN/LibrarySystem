namespace Common.Messaging.Nats.Subjects;

/// <summary>
/// Константы тем публикаций событий в Nats.
/// </summary>
public static class NatsSubjects
{
    private const string BasePrefix = "events";
    
    /// <summary>
    /// успешно добавлено/загружено предопределённое (preload) изображение
    /// </summary>
    public const string FileMetaImagePreloadedAdded = 
        $"{BasePrefix}.file-storage.meta-image.preloaded.added";
    
    /// <summary>
    /// Событие: изображение было удалено из бакета
    /// </summary>
    public const string FileMetaImageDeleted = 
        $"{BasePrefix}.file-storage.meta-image.deleted";
    
}