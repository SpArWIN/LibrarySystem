using Common.Contracts.Auth;
using Common.Db.Abstractions;
using Core.Domain.Models;
using MediatR;

namespace Core.Application.Auth.Context;

/// <summary>
/// Контекст операции с пользователем (регистрация / логин).
/// </summary>
public interface IUserOperationContext : IRequest<AuthorizeResponse>, IAuthTokenIssuanceContext
{
    /// <summary>Единица работы central DB.</summary>
    IUnitOfWork UnitOfWork { get; }

    /// <summary>
    /// Загруженный или созданный пользователь.
    /// Заполняется behavior регистрации/логина, используется при выдаче токенов.
    /// </summary>
    User? User { get; set; }

    /// <summary>
    /// Идентификатор библиотеки (tenant) для scope в JWT.
    /// При регистрации обычно <c>null</c> — пользователь ещё не привязан к библиотеке.
    /// </summary>
    Guid? LibraryId { get; }
}
