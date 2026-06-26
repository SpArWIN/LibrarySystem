using System.Reflection;
using Common.Extensions;
using Common.Validation.Api.Errors;
using Common.Validation.Attribute;
using Common.Validation.ErrorTypes;

namespace Common.Validation.Api.CustomException;

/// <summary>
/// Бизнес исключение Library System.
/// </summary>
public sealed class LibraryException : Exception
{
    /// <summary>
    /// Код ошибки.
    /// </summary>
    public string ErrorKey { get; }
    
    /// <summary>
    /// Тип Http Ошибки.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Параметры для подстановки в сообщение.
    /// </summary>
    public Dictionary<string, object> Parameters { get; } = new();

    private LibraryException(string errorKey, ErrorType type, string? message = null)
        : base(message ?? errorKey)
    {
        ErrorKey = errorKey;
        Type = type;
    }

    /// <summary>
    /// Создать исключение с указанием ошибки.
    /// </summary>
    /// <param name="errorKey">Ключ ошибки. (Из Api*.Errors)</param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static LibraryException Create(string errorKey, object? parameters = null)
    {
        var typeAtr = GetErrorType(errorKey);
          var exception =  new LibraryException(errorKey, typeAtr);
          if (parameters is not null)
          {
              var paramName = parameters.GetType().Name.ToLowerInvariant();
              exception.WithParameter(paramName, parameters);
          }
          return exception;
    }
    
    /// <summary>
    /// Создать исключение с указанием ошибки и явным именем параметра.
    /// </summary>
    /// <param name="errorKey">Ключ ошибки (из Api.*.Errors).</param>
    /// <param name="parameterName">Имя параметра.</param>
    /// <param name="parameterValue">Значение параметра.</param>
    public static LibraryException Create(string errorKey, string parameterName, object parameterValue)
    {
        var type = GetErrorType(errorKey);
        return new LibraryException(errorKey, type)
            .WithParameter(parameterName, parameterValue);
    }

    public LibraryException WithParameter(string key, object value)
    {
        Parameters.Add(key, value);
        return this;
    }

    private static ErrorType GetErrorType(string errorKey)
    {
        var field = FindFieldByConstantValue(errorKey);
        var attribute = field?.GetCustomAttribute<ErrorCodeAttribute>();
        return attribute?.Type ?? ErrorType.InternalServerError;
    }

    private static FieldInfo? FindFieldByConstantValue(string fieldName)
    {
        var assembly = typeof(ApiErrors).Assembly;
        return assembly.GetTypes()
            .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static))
            .FirstOrDefault(field =>
            {
                try
                {
                    return field.GetValue(null)?.ToString() == fieldName;
                }
                catch
                {
                    return false;
                }
            });
    }
    
}