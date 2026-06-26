using Common.Contracts.Auth;
using Common.Validation.Api.CustomException;
using Common.Validation.Api.Errors;
using Core.Application.Auth.Requests;
using Core.Domain.Models;
using Core.Domain.Repository;
using Core.Infrastructure.Extensions.UnitOfWorks;
using MediatR;

namespace Core.Application.Auth.Behaviors;

/// <summary>
/// Создание пользователя в central DB и добавление в Bloom filter.
/// </summary>
public sealed class RegistrationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : RegisterUserRequest
    where TResponse : AuthorizeResponse
{
    private readonly IUserPresenceCache _userPresenceCache;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userPresenceCache"><see cref="IUserPresenceCache"/>.</param>
    public RegistrationPipelineBehavior(IUserPresenceCache userPresenceCache)
    {
        _userPresenceCache = userPresenceCache;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var repository = request.UnitOfWork.GetAuthorizationRepository();
        var userRepository = request.UnitOfWork.GetUserRepository();

        var existingUser = await repository.FindUserByUsernameAsync(request.Username, cancellationToken);
        if (existingUser is not null)
        {
            throw LibraryException.Create(ApiErrors.Auth.UserError.UserAlreadyExists);
        }

        var defaultRoleIds = await userRepository.GetDefaultRoleIdsAsync(cancellationToken);
        var roleIds = request.RoleIds?.Any() == true ? request.RoleIds : defaultRoleIds;
        var roleNames = await userRepository.GetRoleNamesByIdsAsync(roleIds, cancellationToken);
        var user = MapToUser(request);
        var userRoles = roleIds.Select(roleId => new UserRole
        {
            UserId = user.Id,
            RoleId = roleId
        });

        await userRepository.AddUsersAsync([user], cancellationToken);
        await repository.AddUserRolesAsync(userRoles, cancellationToken);
        await _userPresenceCache.AddAsync(request.Username, cancellationToken);

        request.User = user;
        request.Roles = roleNames.ToArray();

        return await next();
    }

    private static User MapToUser(RegisterUserRequest request) =>
        new()
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Password = request.Password,
            LastName = request.LastName,
            Name = request.Name,
            SurName = request.SurName,
            DateOfBirth = request.DateOfBirth,
            Phone = request.Phone,
            Address = request.Address,
            RegistrationDate = DateTime.UtcNow,
        };
}
