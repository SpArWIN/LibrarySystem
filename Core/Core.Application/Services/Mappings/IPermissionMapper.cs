using Core.Domain.Enum.Roles;

namespace Core.Application.Services.Mappings;

/// <summary>
/// Сервис мапинга прав.
/// </summary>
public interface IPermissionMapper
{
    /// <summary>
    /// Построить список прав для ролей.
    /// </summary>
    /// <param name="roles"><see cref="Roles"/>.</param>
    /// <returns>Набор прав.</returns>
    IReadOnlyCollection<string> MapScopes(IReadOnlyCollection<Roles> roles);
}