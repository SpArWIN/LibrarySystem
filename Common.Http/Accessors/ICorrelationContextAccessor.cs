namespace Common.Http.Accessors;

public interface ICorrelationContextAccessor
{
    /// <summary>
    /// Текущий контекст запроса.
    /// </summary>
    public ICorrelationContext CorrelationContext { get; set; }
}