using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Common.Validation.Api.Handle;

public sealed record HandleValidationExceptionContext : IHandleErrorContext
{
    /// <summary>
    /// Ошибки связанные с валидацией.
    /// </summary>
    public required ValidationException Exception { get; init; }
    
    /// <inheritdoc />
    public required HttpContext HttpContext { get; init; }
    
    /// <inheritdoc />
    Exception IHandleErrorContext.Exception => Exception;
}