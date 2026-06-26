namespace Common.Http.Context;

/// <summary>
/// Возврат контекста в исходное состояние.
/// </summary>
public sealed class ContextRollBack<T> : IDisposable
{
    private readonly T _originalValue;
    private readonly AsyncLocal<T> _valueContainer;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="valueContainer"><see cref="AsyncLocal{T}"/>.</param>
    public ContextRollBack(AsyncLocal<T> valueContainer)
    {
        _valueContainer = valueContainer;
        _originalValue = _valueContainer.Value!;
    }
    
    ///<inheritdoc/>
    public void Dispose() => _valueContainer.Value = _originalValue;
}