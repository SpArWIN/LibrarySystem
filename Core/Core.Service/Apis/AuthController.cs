using Common.Contracts.Auth;
using Common.Db.Extensions;
using Common.Db.Factory;
using Common.Policies.Services;
using Core.Application.Services.AuthorizeService;
using Core.Application.Services.Permissions;
using Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Service.Apis;

/// <summary>
/// Авторизация, регистрация и права текущего пользователя.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthorizeService _authorizeService;
    private readonly IPermissionEvaluator _permissionEvaluator;
    private readonly IUnitOfWorkFactory<CentralDbContext> _unitOfWorkFactory;
    private readonly IDbResilience _dbResilience;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public AuthController(
        IAuthorizeService authorizeService,
        IPermissionEvaluator permissionEvaluator,
        IUnitOfWorkFactory<CentralDbContext> unitOfWorkFactory,
        IDbResilience dbResilience)
    {
        _authorizeService = authorizeService;
        _permissionEvaluator = permissionEvaluator;
        _unitOfWorkFactory = unitOfWorkFactory;
        _dbResilience = dbResilience;
    }

    /// <summary>
    /// Регистрация пользователя (central DB, без обязательной привязки к библиотеке).
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthorizeResponse>> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWorkFactory.CreateWithRetryAsync(
            uow => _authorizeService.RegisterAsync(request, uow, cancellationToken),
            _dbResilience.Write,
            cancellationToken);
        return result;
    }

    /// <summary>
    /// Вход по логину и паролю.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthorizeResponse>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWorkFactory.CreateWithRetryAsync(
            uow => _authorizeService.LoginAsync(request, uow, cancellationToken),
            _dbResilience.Write,
            cancellationToken);
        return result;
    }

    /// <summary>
    /// Обновить пару токенов по refresh-токену.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshResponseDto>> Refresh(
        [FromBody] RefreshRequestDto refreshRequestDto,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWorkFactory.CreateWithRetryAsync(
            uow => _authorizeService.RefreshAsync(refreshRequestDto, uow, cancellationToken),
            _dbResilience.Write,
            cancellationToken);
        return result;
    }

    /// <summary>
    /// Выход (отзыв refresh-сессии).
    /// </summary>
    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequestDto request,
        CancellationToken cancellationToken = default)
    {
        await _unitOfWorkFactory.CreateWithRetryAsync(
            async uow =>
            {
                await _authorizeService.LogoutAsync(request, uow, cancellationToken);
                return 0;
            },
            _dbResilience.Write,
            cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Права текущего пользователя из JWT (роли, permissions, политики) — для фронта.
    /// </summary>
    [Authorize]
    [HttpGet("me/permissions")]
    public ActionResult<CurrentUserPermissionsDto> GetMyPermissions() =>
        _permissionEvaluator.GetCurrentAccess(User);

    /// <summary>
    /// Проверка permission по JWT (удобно для фронта / отладки).
    /// </summary>
    [Authorize]
    [HttpGet("me/can/{permission}")]
    public ActionResult<bool> Can(string permission) =>
        _permissionEvaluator.HasPermission(User, permission);

    /// <summary>
    /// Проверка политики ASP.NET по JWT.
    /// </summary>
    [Authorize]
    [HttpGet("me/policy/{policyName}")]
    public ActionResult<bool> HasPolicy(string policyName) =>
        _permissionEvaluator.HasPolicy(User, policyName);
}
