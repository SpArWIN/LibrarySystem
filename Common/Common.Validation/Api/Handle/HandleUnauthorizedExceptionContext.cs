using Microsoft.AspNetCore.Http;

namespace Common.Validation.Api.Handle;

public sealed record HandleUnauthorizedExceptionContext : IHandleErrorContext
{
    /// <summary>
    /// Ошибки связанные с аутентификацией и авторизацией.
    /// </summary>
    public required UnauthorizedAccessException Exception { get; init; }
    
    /// <inheritdoc />
    public required HttpContext HttpContext { get; init; }
    
    /// <inheritdoc />
    Exception IHandleErrorContext.Exception => Exception;
}