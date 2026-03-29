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
    
    /// <summary>
    /// Получить репозиторий.
    /// </summary>
    /// <param name="unitOfWork">uow.</param>
    /// <returns>Репозиторий <see cref="IPublisherRepository"/>.</returns>
    public static IPublisherRepository GetPublisherRepository(this IUnitOfWork unitOfWork) 
        => unitOfWork.GetRepository<IPublisherRepository>();
    
    /// <summary>
    /// Получить репозиторий.
    /// </summary>
    /// <param name="unitOfWork">uow.</param>
    /// <returns>Репозиторий <see cref="IInventoryReadRepository"/>.</returns>
    public static IInventoryReadRepository GetInventoryReadRepository(this IUnitOfWork unitOfWork) =>
    unitOfWork.GetRepository<IInventoryReadRepository>();
    
    /// <summary>
    /// Получить репозиторий.
    /// </summary>
    /// <param name="unitOfWork">.</param>
    /// <returns><see cref="IRefreshSessionRepository"/>.</returns>
    public static IRefreshSessionRepository GetRefreshSessionRepository(this IUnitOfWork unitOfWork) =>
    unitOfWork.GetRepository<IRefreshSessionRepository>();
    
    /// <summary>
    /// Получить репозиторий.
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <returns><see cref="ILoanRepository"/>.</returns>
    public static ILoanRepository GetLoanRepository(this IUnitOfWork unitOfWork) =>
    unitOfWork.GetRepository<ILoanRepository>();
    
    /// <summary>
    /// Получить репозиторий.
    /// </summary>
    /// <param name="unitOfWork"><see cref="IUnitOfWork"/>.</param>
    /// <returns><see cref="IUserRepository"/>.</returns>
    public static IUserRepository GetUserRepository(this IUnitOfWork unitOfWork) =>
    unitOfWork.GetRepository<IUserRepository>();
    
    /// <summary>
    /// Получить репозиторий.
    /// </summary>
    /// <param name="unitOfWork"><see cref="IUnitOfWork"/>.</param>
    /// <returns><see cref="IBookCopyRepository"/>.</returns>
    public static IBookCopyRepository GetBookCopyRepository(this IUnitOfWork unitOfWork) =>
    unitOfWork.GetRepository<IBookCopyRepository>();
}