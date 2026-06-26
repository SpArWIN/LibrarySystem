using Common.Validation.Api.CustomException;
using Microsoft.AspNetCore.Http;

namespace Common.Validation.Api.Handle;

/// <summary>
/// Контекст внутренних ошибок Library System.
/// </summary>
public record HandleLibraryExceptionContext : IHandleErrorContext
{
    /// <summary>
    /// Внутренние ошибки <see cref="LibraryException"/>.
    /// </summary>
    public required LibraryException Exception { get; init; }
    
    /// <inheritdoc />
    public required HttpContext HttpContext { get; init; }
    
    /// <inheritdoc />
    Exception IHandleErrorContext.Exception => Exception;
}