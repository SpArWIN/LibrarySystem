using Common.Contracts.Auth;
using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Core.Application.Auth.Requests;
using Core.Application.Utils;
using Core.Infrastructure.Extensions.UnitOfWorks;
using MediatR;
using Microsoft.Extensions.Options;

namespace Core.Application.Services.AuthorizeService;

/// <inheritdoc cref="IAuthorizeService"/>
public sealed class AuthorizeService : IAuthorizeService
{
    private readonly IMediator _mediator;
    private readonly IOptions<RefreshTokenOptions> _refreshTokenOptions;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="mediator"><see cref="IMediator"/>.</param>
    /// <param name="refreshTokenOptions">Настройки refresh-токена.</param>
    public AuthorizeService(IMediator mediator, IOptions<RefreshTokenOptions> refreshTokenOptions)
    {
        _mediator = mediator;
        _refreshTokenOptions = refreshTokenOptions;
    }

    /// <inheritdoc />
    public Task<AuthorizeResponse> RegisterAsync(
        RegisterRequestDto request,
        IUnitOfWork uow,
        CancellationToken ct = default) =>
        _mediator.Send(new RegisterUserRequest
        {
            Username = request.Username,
            Password = request.Password,
            LastName = request.LastName,
            Name = request.Name,
            SurName = request.SurName,
            DateOfBirth = request.DateOfBirth,
            Phone = request.Phone,
            Address = request.Address,
            LibraryId = request.LibraryId,
            RoleIds = request.RoleIds,
            UnitOfWork = uow,
        }, ct);

    /// <inheritdoc />
    public Task<AuthorizeResponse> LoginAsync(LoginRequestDto request, IUnitOfWork uow, CancellationToken ct = default) =>
        _mediator.Send(new LoginUserRequest
        {
            Username = request.Username,
            Password = request.Password,
            LibraryId = request.LibraryId,
            SubmittedRefreshToken = request.RefreshToken,
            UnitOfWork = uow,
        }, ct);

    /// <inheritdoc />
    public Task<RefreshResponseDto> RefreshAsync(RefreshRequestDto request, IUnitOfWork uow, CancellationToken ct = default) =>
        _mediator.Send(new RefreshTokensRequest
        {
            SubmittedRefreshToken = request.RefreshToken,
            LibraryId = request.LibraryId,
            UnitOfWork = uow,
        }, ct);

    /// <inheritdoc />
    public async Task LogoutAsync(LogoutRequestDto request, IUnitOfWork uow, CancellationToken ct = default)
    {
        var repository = uow.GetRefreshSessionRepository();
        var nowUtc = DateTimeOffset.UtcNow;
        var hash = RefreshTokenCrypto.ComputeHash(request.RefreshToken, _refreshTokenOptions.Value.Pepper);
        var session = await repository.FindByHashAsync(hash, ct);
        if (session is null)
        {
            return;
        }

        if (!session.IsActive(nowUtc))
        {
            return;
        }

        await repository.RevokeAsync(session.Id, nowUtc, null, ct);
    }
}
