using Microsoft.AspNetCore.Http;

namespace Common.Validation.Api.Handle;

public sealed record HandleGenericExceptionContext : IHandleErrorContext
{
    /// <summary>
    /// Ошибки связанные с Generic.
    /// </summary>
    public required Exception Exception { get; init; }
    
    /// <inheritdoc />
    public required HttpContext HttpContext { get; init; }
    
}