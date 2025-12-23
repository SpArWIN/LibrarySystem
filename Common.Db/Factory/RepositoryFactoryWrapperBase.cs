using Microsoft.EntityFrameworkCore;

namespace Common.Db.Factory;

/// <summary>
/// Враппер фабрики репозиториев.
/// </summary>
public abstract class RepositoryFactoryWrapperBase<TDbContext>
where TDbContext : DbContext
{
    /// <summary>
    /// Создать репозиторий.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных.</param>
    /// <typeparam name="TDbContext">Тип контекстаю.</typeparam>
    /// <returns>Репозиторий.</returns>
    public abstract object Create (TDbContext dbContext);
}