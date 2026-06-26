namespace Common.Localization.Services;

/// <summary>
/// Сервис локализации.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Текущий язык.
    /// </summary>
    string CurrentLanguage { get; }
    
    /// <summary>
    /// Подписка на изменение.
    /// </summary>
    IObservable<string> OnCurrentLanguageChanged { get; }
    
    /// <summary>
    /// Получить локализованную строку по ключу.
    /// </summary>
    string GetString(string key);
    
    /// <summary>
    /// Получить локализованную строку с подстановкой параметров.
    /// </summary>
    string GetString(string key, Dictionary<string, object>? parameters);
    
    /// <summary>
    /// Установить язык.
    /// </summary>
    Task SetLanguageAsync(string language, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить список поддерживаемых языков.
    /// </summary>
    List<string?> GetSupportedLanguages();
}