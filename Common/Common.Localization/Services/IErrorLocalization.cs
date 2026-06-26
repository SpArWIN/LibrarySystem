namespace Common.Localization.Services;

/// <summary>
/// Интерфейс локализации ошибок.
/// </summary>
public interface IErrorLocalization
{
    /// <summary>
    /// Получить локализованную ошиюку.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <returns>.</returns>
    string GetString(string key);
    
    /// <summary>
    /// Получить локализованную ошибку, подставляя параметры.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <param name="parameters">Параметры.</param>
    /// <returns>.</returns>
    string GetString(string key, Dictionary<string, object>? parameters);
}