namespace Common.Localization.Services;

/// <inheritdoc />
public sealed class ErrorLocalization : IErrorLocalization
{
    private readonly ILocalizationService _localization;
    private const string Category = "localization";
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="localization"><see cref="ILocalizationService"/>.</param>
    public ErrorLocalization(ILocalizationService localization)
    {
        _localization = localization;
    }

    /// <inheritdoc />
    public string GetString(string key) => _localization.GetString($"{Category}.{key}");

    /// <inheritdoc />
    public string GetString(string key, Dictionary<string, object>? parameters)
   => _localization.GetString($"{Category}.{key}", parameters);
}