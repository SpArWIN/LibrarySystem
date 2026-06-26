using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Common.Validation.Api.CustomException;
using Common.Validation.Api.Errors;
using Core.Application.Auth.Context;
using Core.Application.Builder;
using Core.Application.Services.JWt;
using Core.Application.Services.Mappings;
using Core.Application.Utils;
using Core.Domain.Enum.Roles;
using Core.Domain.Models;
using Core.Domain.Models.Instanse;
using Core.Infrastructure.Extensions.UnitOfWorks;
using Microsoft.Extensions.Options;

namespace Core.Application.Services.TokenIssuer;

/// <inheritdoc cref="IAuthTokenIssuer"/>
public sealed class AuthTokenIssuer : IAuthTokenIssuer
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPermissionMapper _permissionMapper;
    private readonly IClaimBuilder _claimBuilder;
    private readonly IOptions<RefreshTokenOptions> _refreshTokenOptions;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="jwtTokenService"><see cref="IJwtTokenService"/>.</param>
    /// <param name="permissionMapper"><see cref="IPermissionMapper"/>.</param>
    /// <param name="claimBuilder"><see cref="IClaimBuilder"/>.</param>
    /// <param name="refreshTokenOptions">Настройки refresh-токена.</param>
    public AuthTokenIssuer(
        IJwtTokenService jwtTokenService,
        IPermissionMapper permissionMapper,
        IClaimBuilder claimBuilder,
        IOptions<RefreshTokenOptions> refreshTokenOptions)
    {
        _jwtTokenService = jwtTokenService;
        _permissionMapper = permissionMapper;
        _claimBuilder = claimBuilder;
        _refreshTokenOptions = refreshTokenOptions;
    }

    /// <inheritdoc />
    public async Task IssueForUserAsync(
        IAuthTokenIssuanceContext context,
        User user,
        Guid? libraryId,
        IUnitOfWork unitOfWork,
        string? submittedRefreshToken = null,
        CancellationToken cancellationToken = default)
    {
        if (await TryReuseExistingSessionAsync(

                context,
                user,
                libraryId,
                unitOfWork,
                submittedRefreshToken,
                cancellationToken))
        {
            return;
        }

        var (accessToken, roles, scopes, accessExpUtc) =
            await CreateAccessTokenAsync(user, context.Roles, libraryId, unitOfWork, cancellationToken);

        var nowUtc = DateTimeOffset.UtcNow;
        var refreshToken = RefreshTokenCrypto.GenerateToken();
        var refreshHash = RefreshTokenCrypto.ComputeHash(refreshToken, _refreshTokenOptions.Value.Pepper);
        var refreshExpUtc = nowUtc.AddDays(_refreshTokenOptions.Value.LifetimeDays);
        
        await unitOfWork.GetRefreshSessionRepository().AddAsync(new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAtUtc = nowUtc,
            ExpiresAtUtc = refreshExpUtc,
            AccessToken = accessToken,
            AccessExpiresAtUtc = accessExpUtc,

        }, cancellationToken);


        context.AccessToken = accessToken;
        context.AccessExpiresAtUtc = accessExpUtc;
        context.RefreshToken = refreshToken;
        context.RefreshExpiresAtUtc = refreshExpUtc;
        context.Roles = roles;
        context.Permissions = scopes;
    }
    
    /// <inheritdoc />
    public async Task RotateRefreshAsync(
        IRefreshTokenOperationContext context,
        CancellationToken cancellationToken = default)
    {
        var repository = context.UnitOfWork.GetAuthorizationRepository();
        var sessionRepository = context.UnitOfWork.GetRefreshSessionRepository();
        var nowUtc = DateTimeOffset.UtcNow;
        var oldHash = RefreshTokenCrypto.ComputeHash(
            context.SubmittedRefreshToken,
            _refreshTokenOptions.Value.Pepper);
        var oldSession = await repository.FindRefreshSessionByHashAsync(oldHash, cancellationToken)
                         ?? throw LibraryException.Create(
                             ApiErrors.Auth.InvalidToken,
                             context.SubmittedRefreshToken);

        if (!oldSession.IsActive(nowUtc))
        {
            throw LibraryException.Create(ApiErrors.Auth.TokenExpired);
        }
        var user = await repository.FindUserByIdAsync(oldSession.UserId, cancellationToken)
                   ?? throw LibraryException.Create(ApiErrors.Auth.UserNotFound);
        context.User = user;

        
        var (accessToken, roles, scopes, accessExpUtc) = await CreateAccessTokenAsync(
            user,
            context.Roles,
            context.LibraryId,
            context.UnitOfWork,
            cancellationToken);

        var newRefreshToken = RefreshTokenCrypto.GenerateToken();
        var newHash = RefreshTokenCrypto.ComputeHash(newRefreshToken, _refreshTokenOptions.Value.Pepper);
        var newExpUtc = nowUtc.AddDays(_refreshTokenOptions.Value.LifetimeDays);
        var newSessionId = Guid.NewGuid();


        await sessionRepository.RevokeAsync(oldSession.Id, nowUtc, newSessionId, cancellationToken);
        await sessionRepository.AddAsync(new RefreshSession

        {
            Id = newSessionId,
            UserId = user.Id,

            TokenHash = newHash,

            CreatedAtUtc = nowUtc,
            ExpiresAtUtc = newExpUtc,
            AccessToken = accessToken,
            AccessExpiresAtUtc = accessExpUtc,

        }, cancellationToken);

        
        context.AccessToken = accessToken;
        context.AccessExpiresAtUtc = accessExpUtc;
        context.RefreshToken = newRefreshToken;
        context.RefreshExpiresAtUtc = newExpUtc;
        context.Roles = roles;
        context.Permissions = scopes;
    }
    
    private async Task<bool> TryReuseExistingSessionAsync(
        IAuthTokenIssuanceContext context,
        User user,
        Guid? libraryId,
        IUnitOfWork unitOfWork,
        string? submittedRefreshToken,
        CancellationToken cancellationToken)

    {
        var nowUtc = DateTimeOffset.UtcNow;
        var sessionRepository = unitOfWork.GetRefreshSessionRepository();

        RefreshSession? session;

        if (!string.IsNullOrWhiteSpace(submittedRefreshToken))
        {

            var hash = RefreshTokenCrypto.ComputeHash(submittedRefreshToken, _refreshTokenOptions.Value.Pepper);

            session = await sessionRepository.FindByHashAsync(hash, cancellationToken);

            if (session is null || session.UserId != user.Id || !session.IsActive(nowUtc))
            {
                return false;
            }

        }
        else
        {
            session = await sessionRepository.FindLatestActiveByUserIdAsync(user.Id, nowUtc, cancellationToken);
            if (session is null)
            {
                return false;
            }
        }
        
        var (roleNames, scopes) = 
            await LoadRolesAndScopesAsync(user, context.Roles, unitOfWork, cancellationToken);
        
        if (!string.IsNullOrEmpty(session.AccessToken)
            && session.AccessExpiresAtUtc.HasValue
            && session.AccessExpiresAtUtc.Value > nowUtc)
        {

            FillContext(context, 
                session.AccessToken, session.AccessExpiresAtUtc.Value,
                submittedRefreshToken ?? 
                string.Empty, 
                session.ExpiresAtUtc, roleNames, scopes);

            return true;
        }
        
        if (!session.IsActive(nowUtc))
        {
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(submittedRefreshToken))
        {

            return false;

        }
        
        var (accessToken, roles, newScopes, accessExpUtc) = 
            await CreateAccessTokenAsync(
            user,
            context.Roles,
            libraryId,
            unitOfWork,
            cancellationToken);

        await sessionRepository.UpdateAccessAsync(session.Id, accessToken, accessExpUtc, cancellationToken);

        FillContext(context, accessToken, accessExpUtc, submittedRefreshToken, session.ExpiresAtUtc, roles, newScopes);

        return true;

    }



    private static void FillContext(
        IAuthTokenIssuanceContext context,
        string accessToken,
        DateTimeOffset accessExpUtc,
        string refreshToken,
        DateTimeOffset refreshExpUtc,
        string?[] roles,
        IReadOnlyCollection<string> scopes)

    {
        context.AccessToken = accessToken;
        context.AccessExpiresAtUtc = accessExpUtc;
        context.RefreshToken = refreshToken;
        context.RefreshExpiresAtUtc = refreshExpUtc;
        context.Roles = roles;
        context.Permissions = scopes;

    }



    private async Task<(string?[] Roles, IReadOnlyCollection<string> Scopes)> LoadRolesAndScopesAsync(
        User user,
        string?[]? roles,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)

    {

        var userRepository = unitOfWork.GetUserRepository();
        var roleNames = roles is { Length: > 0 }
            ? roles
            : (await userRepository.GetUserRolesName(user, cancellationToken)).ToArray();

        var roleEnums = GetRoleEnums(roleNames);

        var scopes = _permissionMapper.MapScopes(roleEnums);

        return (roleNames, scopes);

    }



    private async Task<(string AccessToken, string?[] Roles, IReadOnlyCollection<string> Scopes, DateTimeOffset AccessExpUtc)> CreateAccessTokenAsync(
        User user,
        string?[]? roles,
        Guid? libraryId,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {

        var (roleNames, scopes) = 
            await LoadRolesAndScopesAsync(user, roles, unitOfWork, cancellationToken);

        var claims = _claimBuilder.BuildAccessClaims(
            user.Id,
            user.Username,
            roleNames,
            scopes,
            libraryId);
        

        var (token, expires) = _jwtTokenService.CreateAccessToken(claims);

        return (token, roleNames, scopes, expires);

    }



    private static Roles[] GetRoleEnums(string?[] roleNames) =>

        roleNames

            .Select(n => Enum.TryParse<Roles>(n, ignoreCase: true, out var r) ? (Roles?)r : null)

            .Where(r => r is not null)

            .Select(r => r!.Value)

            .Distinct()

            .ToArray();

}


