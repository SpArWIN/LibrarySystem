using Common.Contracts.Auth;
using Common.Db.Abstractions;

namespace Core.Application.Services.AuthorizeService;

/// <summary>
/// Сервис авторизации и выхода пользователей из системы.
/// Делегирует register/login/refresh в MediatR-пайплайны.
/// </summary>
public interface IAuthorizeService
{
    /// <summary>
    /// Регистрация пользователя (central DB, без обязательной привязки к библиотеке).
    /// </summary>
    /// <param name="request">Данные регистрации.</param>
    /// <param name="uow">Единица работы.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    Task<AuthorizeResponse> RegisterAsync(RegisterRequestDto request, IUnitOfWork uow, CancellationToken ct = default);

    /// <summary>
    /// Выход (отзыв refresh-сессии).
    /// </summary>
    /// <param name="request">Refresh-токен для отзыва.</param>
    /// <param name="uow">Единица работы.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    Task LogoutAsync(LogoutRequestDto request, IUnitOfWork uow, CancellationToken ct = default);

    /// <summary>
    /// Обновить access-токен по refresh-токену.
    /// </summary>
    /// <param name="request">Refresh-запрос.</param>
    /// <param name="uow">Единица работы.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    Task<RefreshResponseDto> RefreshAsync(RefreshRequestDto request, IUnitOfWork uow, CancellationToken ct = default);

    /// <summary>
    /// Авторизация по логину и паролю.
    /// </summary>
    /// <param name="request">Данные входа.</param>
    /// <param name="uow">Единица работы.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    Task<AuthorizeResponse> LoginAsync(LoginRequestDto request, IUnitOfWork uow, CancellationToken ct = default);
}
