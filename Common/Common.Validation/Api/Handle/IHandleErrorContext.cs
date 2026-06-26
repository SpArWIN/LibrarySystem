using MediatR;
using Microsoft.AspNetCore.Http;

namespace Common.Validation.Api.Handle;

/// <summary>
/// Контекст фатальных ошибок.
/// </summary>
public interface IHandleErrorContext : IRequest<HandleErrorResponse>
{
    /// <summary>
    /// Все ошибки.
    /// </summary>
    Exception Exception { get; }
    
    /// <summary>
    /// Ошибки Http контекста.
    /// </summary>
    HttpContext HttpContext { get; }
}