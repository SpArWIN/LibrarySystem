namespace Common.Http.Accessors;

public interface ITenantContextAccessor
{
    /// <summary>
    /// Текущий tentant- контекст.
    /// </summary>
    ITenantContext? CurrentContext { get; set; }
}