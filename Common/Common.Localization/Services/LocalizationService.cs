using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using Common.Extensions;
using Microsoft.Extensions.Logging;

namespace Common.Localization.Services;

/// <inheritdoc />
public sealed class LocalizationService : ILocalizationService
{
    private readonly ILogger<LocalizationService> _logger;
    private readonly BehaviorSubject<string> _languageSubject;
    private readonly Dictionary<string, Dictionary<string, string>> _resources;
    private readonly List<string> _supportedLanguages;
    private Dictionary<string, string> _currentResources;
    private const string ResourcesPath = "Resources";
    private const string DefaultLanguage = "ru-RU";

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="logger"></param>
    public LocalizationService(ILogger<LocalizationService> logger)
    {
        _logger = logger;
        _resources = new Dictionary<string, Dictionary<string, string>>();
        _supportedLanguages = new List<string>();
        _currentResources = new Dictionary<string, string>();
        _languageSubject = new BehaviorSubject<string>(DefaultLanguage);
        CurrentLanguage = DefaultLanguage;
        
        LoadResourcesAsync(DefaultLanguage, CancellationToken.None).GetResultSync();
    }

    /// <inheritdoc />
    public string CurrentLanguage { get;  private set; }

    /// <inheritdoc />
    public IObservable<string> OnCurrentLanguageChanged => _languageSubject.AsObservable();

    /// <inheritdoc />
    public string GetString(string key)
    {
        if (_currentResources.TryGetValue(key, out var value))
        {
            return value;
        }
        _logger.LogWarning("Key not found: {Key}", key);
        return key;
    }

    /// <inheritdoc />
    public string GetString(string key, Dictionary<string, object>? parameters)
    {
        var text = GetString(key);
        if (parameters is not null && parameters.Any())
        {
            parameters
                .ForEach(param =>
                {
                    text = text.Replace($"{{{param.Key}}}", param.Value?.ToString());
                });
        }
        return text;
    }

    /// <inheritdoc />
    public async Task SetLanguageAsync(string language, CancellationToken cancellationToken = default)
    {
        if (CurrentLanguage == language)
        {
            return;
        }
        await LoadResourcesAsync(language, cancellationToken);
        CurrentLanguage = language;
        _languageSubject.OnNext(language);
        _logger.LogInformation("Language changed to: {Language}", language);
    }

    /// <inheritdoc />
    public List<string?> GetSupportedLanguages()
    {
        var resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ResourcesPath);
        if (!Directory.Exists(resourcesPath))
        {
            return [DefaultLanguage];
        }

        var languages = Directory.GetFiles(resourcesPath, "*.json")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(fileName => fileName?.Contains('.') == true)
            .Select(fileName => fileName?.Split('.').Last())
            .Distinct()
            .ToList();
        return languages.Count == 0 ? [DefaultLanguage] : languages;
    }

    private async Task LoadResourcesAsync(string language, CancellationToken cancellationToken = default)
    {
        if (_resources.TryGetValue(language, out var resources))
        {
            _currentResources = resources;
            return;
        }
        var newResources = await LoadFromFilesAsync(language, cancellationToken);
        _resources[language] = newResources;
        _currentResources = newResources;
        _logger.LogInformation("Loaded localization for language: {Language}", language);
    }

    private async Task<Dictionary<string, string>> LoadFromFilesAsync(string language, 
        CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, string>();
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var resourcesPath = Path.Combine(basePath, ResourcesPath);
        if (!Directory.Exists(resourcesPath))
        {
            _logger.LogWarning("Resources directory not found: {Path}", resourcesPath);
            return result;
        }
        var files = Directory.GetFiles(resourcesPath, $"*.{language}.json");
        await files.ForEachAsync(async (file, ct) =>
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var category = fileName.Split('.').First();
            try
            {
                var json = await File.ReadAllTextAsync(file, ct);
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                if (data != null)
                {
                    var flatten = FlattenDictionary(data, category);
                    flatten.ForEach(entry => result.Add(entry.Key, entry.Value));
                }

                _logger.LogDebug("Loaded category {Category} for language {Language}", category, language);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load file: {File}", file);
            }

        }, cancellationToken: cancellationToken);
        return result;
    }

    private static Dictionary<string, string> FlattenDictionary(
        Dictionary<string, object> source, 
        string category, 
        string currentPath = "")
    {
        var result = new Dictionary<string, string>();
        source.ForEach(kv =>
        {
            var newPath = string.IsNullOrEmpty(currentPath) ? kv.Key : $"{currentPath}.{kv.Key}";
            ProcessValue(kv.Value, newPath, category, result);
        });
        return result;
    }
    

    private static void ProcessValue(object value, string key, string category, Dictionary<string, string> result)
    {
        if (value is JsonElement element)
        {
            ProcessJsonElement(element, key, category, result);
        }
        else if (value is string strValue)
        {
            var fullKey = BuildKey(category, key);
            result[fullKey] = strValue;
        }
        else
        {
            var fullKey = BuildKey(category, key);
            result[fullKey] = value?.ToString() ?? key;
        }
    }
    
    private static string BuildKey(string category, string key) => $"{category}.{key}";

    private static void ProcessJsonElement(JsonElement element, string key, string category, Dictionary<string, string> result)
    {
        var fullKey =  BuildKey(category, key);
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var nested = JsonSerializer.Deserialize<Dictionary<string, object>>(element.GetRawText());
                if (nested != null)
                {
                    var flattened = FlattenDictionary(nested, category, key);
                    flattened.ForEach(item =>
                    {
                        result[item.Key] = item.Value;
                    });
                }
                break;
            
            case JsonValueKind.String:
                
                result[fullKey] = element.GetString() ?? key;
                break;
            
            case JsonValueKind.Number:
                result[fullKey] = element.GetRawText();
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
                result[fullKey] = element.GetBoolean().ToString();
                break;
            
            case JsonValueKind.Null:
                result[fullKey] = "null";
                break;
            
            default:
                result[fullKey] = element.ToString();
                break;
        }
    }
}