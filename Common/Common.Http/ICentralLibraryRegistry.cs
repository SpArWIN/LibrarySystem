using Common.Contracts.Settings;

namespace Common.Http;

/// <summary>
/// Сервис настроек регистрации баз данных.
/// </summary>
public interface ICentralLibraryRegistry
{
    /// <summary>
    /// Получить настройки БД по идентификатору.
    /// </summary>
    /// <param name="libraryId">Идентификатор библиотеки.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="DataBaseSettings"/>.</returns>
    Task<DataBaseSettings> GetDbSettingsAsync(Guid libraryId, CancellationToken ct = default);
}