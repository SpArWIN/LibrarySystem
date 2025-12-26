using Common.Contracts.Database;
using Common.Db.Abstractions;

namespace Core.Application.Services.Abstractions;

/// <summary>
/// Сервис создания библиотек - новых баз.
/// </summary>
public interface ILibraryProvisioningService
{
    /// <summary>
    /// Создать библиотеку, физическая база данных +  запись в основную.
    /// </summary>
    /// <param name="centralUow"><see cref="IUnitOfWork"/>.</param>
    /// <param name="request"><see cref="CreateLibraryRequestDto"/>.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="CreateLibraryResponseDto"/>.</returns>
    Task<CreateLibraryResponseDto> CreateAsync(IUnitOfWork centralUow, CreateLibraryRequestDto request, 
        CancellationToken ct = default);
    
    /// <summary>
    /// Удалить библиотеку.
    /// </summary>
    /// <param name="centralUow"><see cref="IUnitOfWork"/>.</param>
    /// <param name="libraryId">Идентификатор библиотеки.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    Task DeleteAsync(IUnitOfWork centralUow, Guid libraryId, CancellationToken ct = default);
}