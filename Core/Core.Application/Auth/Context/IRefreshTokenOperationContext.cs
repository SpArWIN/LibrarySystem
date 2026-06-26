using Common.Contracts.Auth;
using Common.Db.Abstractions;
using Core.Domain.Models;
using MediatR;

namespace Core.Application.Auth.Context;

/// <summary>
/// Контекст обновления пары токенов по refresh.
/// </summary>
public interface IRefreshTokenOperationContext : IRequest<RefreshResponseDto>, IAuthTokenIssuanceContext
{
    /// <summary>Единица работы central DB.</summary>
    IUnitOfWork UnitOfWork { get; }

    /// <summary>Refresh-токен, переданный клиентом.</summary>
    string SubmittedRefreshToken { get; }

    /// <summary>
    /// Идентификатор библиотеки (tenant) для scope в новом access-токене.
    /// </summary>
    Guid? LibraryId { get; }

    /// <summary>
    /// Пользователь, найденный по активной refresh-сессии.
    /// </summary>
    User? User { get; set; }
}
