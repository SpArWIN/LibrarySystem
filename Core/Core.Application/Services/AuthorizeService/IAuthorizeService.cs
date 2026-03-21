using Common.Contracts.Auth;
using Common.Db.Abstractions;

namespace Core.Application.Services.AuthorizeService;

/// <summary>
/// Сервис авторизации и выхода пользователей из системы.
/// </summary>
public interface IAuthorizeService
{
    /// <summary>
    /// Выход.
    /// </summary>
    /// <param name="request"><see cref="LoginRequestDto"/>.</param>
    /// <param name="uow"><see cref="IUnitOfWork"/>,</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    Task LogoutAsync(LogoutRequestDto request, IUnitOfWork uow, CancellationToken ct = default);

    /// <summary>
    /// Обновить Access токен по refresh токену.
    /// </summary>
    /// <param name="request"><see cref="RefreshRequestDto"/>.</param>
    /// <param name="uow"><see cref="IUnitOfWork"/>.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="RefreshResponseDto"/>.</returns>
    Task<RefreshResponseDto> RefreshAsync(RefreshRequestDto request, IUnitOfWork uow , CancellationToken ct = default);

    /// <summary>
    /// Авторизация.
    /// </summary>
    /// <param name="request"><see cref="LoginRequestDto"/>.</param>
    /// <param name="uow"><see cref="IUnitOfWork"/>.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="AuthorizeResponse"/>.</returns>
    Task<AuthorizeResponse> LoginAsync(LoginRequestDto request, IUnitOfWork uow, CancellationToken ct = default);
}