using Common.Contracts.Auth;
using Common.Contracts.Settings;
using Common.Contracts.User;
using Common.Db.Abstractions;
using Core.Application.Builder;
using Core.Application.Services.Hash;
using Core.Application.Services.JWt;
using Core.Application.Services.Mappings;
using Core.Application.Utils;
using Core.Domain.Enum.Roles;
using Core.Domain.Models;
using Core.Domain.Models.Instanse;
using Core.Infrastructure.Extensions.UnitOfWorks;
using Microsoft.Extensions.Options;
using Serilog;

namespace Core.Application.Services.AuthorizeService;

/// <inheritdoc />
public sealed class AuthorizeService : IAuthorizeService
{
    private static readonly ILogger Logger = Log.ForContext<AuthorizeService>();
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPermissionMapper _permissionMapper;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IClaimBuilder _claimBuilder;
    private readonly IOptions<RefreshTokenOptions> _refreshTokenOptions;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="passwordHasher"><see cref="IPasswordHasher"/>.</param>
    /// <param name="permissionMapper"><see cref="IPermissionMapper"/>.</param>
    /// <param name="jwtTokenService"><see cref="IJwtTokenService"/>.</param>
    /// <param name="refreshTokenOptions"><see cref="RefreshTokenOptions"/>.</param>
    /// <param name="claimBuilder"><see cref="IClaimBuilder"/>.</param>
    public AuthorizeService(IPasswordHasher passwordHasher, 
        IPermissionMapper permissionMapper, 
        IJwtTokenService jwtTokenService,
        IOptions<RefreshTokenOptions> refreshTokenOptions, 
        IClaimBuilder claimBuilder)
    {
        _passwordHasher = passwordHasher;
        _permissionMapper = permissionMapper;
        _jwtTokenService = jwtTokenService;
        _refreshTokenOptions = refreshTokenOptions;
        _claimBuilder = claimBuilder;
    }

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

    /// <inheritdoc />
    public async Task<RefreshResponseDto> RefreshAsync(RefreshRequestDto request, IUnitOfWork uow, CancellationToken ct = default)
    {
        Logger.Debug("-> Initial refresh request");
        var repository = uow.GetAuthorizationRepository();
        var sessionRepository = uow.GetRefreshSessionRepository();
        var nowUtc = DateTimeOffset.UtcNow;
        var oldHash = RefreshTokenCrypto.ComputeHash(request.RefreshToken, _refreshTokenOptions.Value.Pepper);
        var oldSession = await repository.FindRefreshSessionByHashAsync(oldHash, ct)
                         ?? throw new InvalidOperationException("Refresh token is invalid.");
        
        Logger.Debug("Find Old Session : {session}", oldSession);

        if (!oldSession.IsActive(nowUtc))
        {
            throw new UnauthorizedAccessException("Refresh token is expired or revoked.");
        }
        var user = await repository.FindUserByIdAsync(oldSession.UserId, ct)
            ?? throw new InvalidOperationException("User not Found.");
        
        Logger.Debug("Find User {us}",user);
    
        var (accessToken, _, accessExpUtc) = CreateUserAccessToken(user, request.LibraryId);
      
        var newRefreshToken = RefreshTokenCrypto.GenerateToken();
        var newHash = RefreshTokenCrypto.ComputeHash(newRefreshToken, _refreshTokenOptions.Value.Pepper);
        var newExpUtc = nowUtc.AddDays(_refreshTokenOptions.Value.LifetimeDays);
        var newSessionId = Guid.NewGuid();
        
        await sessionRepository.RevokeAsync(oldSession.Id, nowUtc, newSessionId, ct);
        
        await sessionRepository.AddAsync(new RefreshSession()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = newHash,
            CreatedAtUtc = nowUtc,
            ExpiresAtUtc = newExpUtc
        }, ct);

        return new RefreshResponseDto()
        {
            AccessToken = accessToken,
            AccessExpiresAtUtc = accessExpUtc,
            RefreshToken = newRefreshToken,
            RefreshExpiresAtUtc = newExpUtc
        };
    }
    
    /// <inheritdoc />
    public async Task<AuthorizeResponse> LoginAsync(LoginRequestDto request, IUnitOfWork uow, CancellationToken ct = default)
    {
        //TODO Закешировать данные, чтоб не отправлять постоянно в бд.
        var repository = uow.GetAuthorizationRepository();
        var sessionRepository = uow.GetRefreshSessionRepository();
        var user = await repository.FindUserByUsernameAsync(request.Username, ct)
                   ?? throw new InvalidOperationException("Invalid credentials.");
        if (!_passwordHasher.Verify(request.Password, user.Password))
        {
            //TODO заменить потом на кастомные ошибки. а также проверки на существование пользователя.
            throw new UnauthorizedAccessException("Invalid credentials.");
        }
        
        var (accessToken,roles, accessExpUtc) = CreateUserAccessToken(user, request.LibraryId);
        
        var nowUtc = DateTimeOffset.UtcNow;
        var refreshToken = RefreshTokenCrypto.GenerateToken();
        var refreshHash = RefreshTokenCrypto.ComputeHash(refreshToken, _refreshTokenOptions.Value.Pepper);
        var refreshExpUtc = nowUtc.AddDays(_refreshTokenOptions.Value.LifetimeDays);

        await sessionRepository.AddAsync(new RefreshSession()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAtUtc = nowUtc,
            ExpiresAtUtc = refreshExpUtc
        }, ct);

        return new AuthorizeResponse()
        {
            AccessToken = accessToken,
            AccessExpiresAtUtc = accessExpUtc,
            RefreshToken = refreshToken,
            RefreshExpiresAtUtc = refreshExpUtc,
            UserInfo = new UserInfoDto()
            {
                Id = user.Id,
                Username = user.Username,
                LastName = user.LastName,
                Name = user.Name,
                SurName = user.SurName,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                Phone = user.Phone,
                RegistrationDate = user.RegistrationDate,
                Roles = roles,
            }
        };
    }
    
    private (string AccessToken, string?[] Roles, DateTimeOffset AccessExpUtc) CreateUserAccessToken(
        User user, 
        Guid? libraryId)
    {
        var rolesNames = GetUserRolesNames(user);
        var rolesEnums = GetRoleEnums(rolesNames);
        var scopes = _permissionMapper.MapScopes(rolesEnums);
        var claims = _claimBuilder.BuildAccessClaims(
            user.Id,
            user.Username,
            rolesNames,
            scopes,
            libraryId
        );
        var (token, expires) = _jwtTokenService.CreateAccessToken(claims);
        return (token, rolesNames, expires);
    }

    private string?[] GetUserRolesNames(User user)
    {
      var rolesNames =  user.UserRoles
            .Select(x => x.Role?.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
      
      return rolesNames;
    }

    private Roles [] GetRoleEnums(string?[] roleNames)
    {
        var roleEnums = roleNames
            .Select(n => Enum.TryParse<Roles>(n, ignoreCase: true, out var r) ? (Roles?)r : null)
            .Where(r => r is not null)
            .Select(x => x.Value)
            .Distinct()
            .ToArray();
        
        return roleEnums;
    }
    
}