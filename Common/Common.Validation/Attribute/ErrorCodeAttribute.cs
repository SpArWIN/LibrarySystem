using Common.Validation.ErrorTypes;

namespace Common.Validation.Attribute;

/// <summary>
/// Атрибут для маркировки констант ошибок.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class ErrorCodeAttribute : System.Attribute
{
    /// <summary>
    /// Тип ошибок со статус кодом.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="type"><see cref="ErrorType"/>.</param>
    public ErrorCodeAttribute(ErrorType type)
    {
        Type = type;
    }
}