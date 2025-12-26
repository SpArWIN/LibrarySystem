using Common.Db.Abstractions;
using Core.Domain.Repository;

namespace Core.Infrastructure.Extensions.UnitOfWorks;

public static class UnitOfWorkExtensions
{
    /// <summary>
    /// Получить репозиторий.
    /// </summary>
    /// <param name="unitOfWork">uow.</param>
    /// <returns>Репозиторий <see cref="IBookRepository"/>.</returns>
    public static IBookRepository GetBookRepository(this IUnitOfWork unitOfWork) =>
        unitOfWork.GetRepository<IBookRepository>();
    
    /// <summary>
    /// Получить репозиторий.
    /// </summary>
    /// <param name="unitOfWork">uow.</param>
    /// <returns>Репозиторий <see cref="IAuthorizationRepository"/>.</returns>
    public static IAuthorizationRepository GetAuthorizationRepository(this IUnitOfWork unitOfWork) =>
    unitOfWork.GetRepository<IAuthorizationRepository>();
    
    /// <summary>
    /// Получить репозиторий.
    /// </summary>
    /// <param name="unitOfWork">uow.</param>
    /// <returns>Репозиторий <see cref="ILibraryInstanceRepository"/>.</returns>
    public static ILibraryInstanceRepository GetLibraryInstanceRepository(this IUnitOfWork unitOfWork) =>
    unitOfWork.GetRepository<ILibraryInstanceRepository>();
}