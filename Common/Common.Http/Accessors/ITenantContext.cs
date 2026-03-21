using Common.Contracts.Settings;

namespace Common.Http.Accessors;

/// <summary>
/// Контекст запросов.
/// </summary>
public interface ITenantContext
{
    /// <summary>Идентификатор текущей библиотеки.</summary>
    Guid LibraryId { get; }

    /// <summary>Настройки БД текущей библиотеки.</summary>
    DataBaseSettings DbSettings { get; }
}