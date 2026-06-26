namespace Common.Http.Accessors;

/// <inheritdoc />
public sealed class CorrelationContextAccessor : ICorrelationContextAccessor
{
    private static readonly AsyncLocal<CorrelationContextHolder> Current = new AsyncLocal<CorrelationContextHolder>();

    /// <inheritdoc />
    public ICorrelationContext? CorrelationContext
    {
        get => Current.Value?.Context;
        set
        {
            var holder = Current.Value;
            if (holder is not null)
            {
                holder.Context = null;
            }

            if (value is not null)
            {
                Current.Value = new CorrelationContextHolder { Context = value };
            }
        }
    }
}

/// <summary>
/// Внутреннее хранилище между пайплайнами.
/// </summary>
internal sealed class CorrelationContextHolder
{
    public ICorrelationContext? Context { get; set; }
}