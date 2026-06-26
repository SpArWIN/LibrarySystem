namespace Common.Validation.Api.Handle;

/// <summary>
/// Система перехвата фатальных ошибок.
/// </summary>
public record HandleErrorResponse
{
    /// <summary>
    /// Признак того, что ошибка обработана.
    /// </summary>
    public bool Handled { get; init; } = true;
    
    /// <summary>
    /// Код ошибки.
    /// </summary>
    public int StatusCode { get; init; }
    
    /// <summary>
    /// Ответ.
    /// </summary>
    public object? Response { get; init; }
}