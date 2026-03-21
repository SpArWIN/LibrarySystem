namespace Common.Http.Accessors;

/// <inheritdoc />
public sealed class CorrelationContextAccessor : ICorrelationContextAccessor
{
    /// <inheritdoc />
    public ICorrelationContext CorrelationContext { get; set; }
}