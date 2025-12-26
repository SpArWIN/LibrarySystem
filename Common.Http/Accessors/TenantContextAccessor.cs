namespace Common.Http.Accessors;

/// <inheritdoc />
public sealed class TenantContextAccessor : ITenantContextAccessor
{
    public ITenantContext? CurrentContext { get; set; }
}