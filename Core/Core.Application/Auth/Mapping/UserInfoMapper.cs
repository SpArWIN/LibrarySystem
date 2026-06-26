using Common.Contracts.User;
using Core.Domain.Models;

namespace Core.Application.Auth.Mapping;

/// <summary>
/// Маппинг модели пользователя в DTO ответа авторизации.
/// </summary>
internal static class UserInfoMapper
{
    /// <summary>
    /// Собрать <see cref="UserInfoDto"/> из сущности и ролей.
    /// </summary>
    /// <param name="user">Пользователь.</param>
    /// <param name="roles">Имена ролей.</param>
    /// <param name="permissions">Permissions (scopes).</param>
    /// <returns><see cref="UserInfoDto"/>.</returns>
    public static UserInfoDto ToUserInfoDto(User user, string?[] roles, IReadOnlyCollection<string> permissions) =>
        new()
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
            Permissions = permissions,
        };
}
