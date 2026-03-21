using Core.Domain.Models.Instanse;

namespace Core.Domain.Repository;

/// <summary>
/// Репозиторий сессии.
/// </summary>
public interface IRefreshSessionRepository
{
    /// <summary>Добавить refresh-сессию.</summary>
    Task AddAsync(RefreshSession session, CancellationToken ct = default);

    /// <summary>Найти сессию по TokenHash.</summary>
    Task<RefreshSession?> FindByHashAsync(string tokenHash, CancellationToken ct = default);

    /// <summary>
    /// Отозвать сессию.
    /// </summary>
    /// <param name="sessionId">Id сессии.</param>
    /// <param name="revokedAtUtc">Дата установки, когда сессию отозвали.</param>
    /// <param name="replacedBySessionId">Заменямый Id сессии.</param>
    /// <param name="ct"><see cref="CancellationToken"/>,</param>
    /// <returns></returns>
    Task RevokeAsync(Guid sessionId, DateTimeOffset revokedAtUtc, Guid? replacedBySessionId, CancellationToken ct = default);
}