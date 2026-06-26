using Common.Db.Abstractions;
using Core.Application.Auth.Context;
using Core.Domain.Models;

namespace Core.Application.Services.TokenIssuer;

/// <summary>
/// Выдача access/refresh токенов и ротация refresh-сессий.
/// </summary>
public interface IAuthTokenIssuer
{
    /// <summary>
    /// Создать новую refresh-сессию и заполнить поля токенов в контексте (login / register).
    /// </summary>
    /// <param name="context">Контекст запроса с полями для выданных токенов.</param>
    /// <param name="user">Пользователь, для которого выдаются токены.</param>
    /// <param name="libraryId">Опциональный tenant для scope в JWT.</param>
    /// <param name="unitOfWork">Единица работы.</param>
    /// <param name="submittedRefreshToken">Refresh клиента (login): повторная выдача сессии, если ещё активна.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task IssueForUserAsync(
        IAuthTokenIssuanceContext context,
        User user,
        Guid? libraryId,
        IUnitOfWork unitOfWork,
        string? submittedRefreshToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ротация refresh-сессии и заполнение полей токенов в контексте.
    /// </summary>
    /// <param name="context">Контекст refresh-запроса.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    Task RotateRefreshAsync(
        IRefreshTokenOperationContext context,
        CancellationToken cancellationToken = default);
}
