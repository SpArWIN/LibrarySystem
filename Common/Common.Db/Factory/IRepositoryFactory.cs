using Microsoft.EntityFrameworkCore;

namespace Common.Db.Factory;

public interface IRepositoryFactory<in TDbContext> where TDbContext : DbContext
{
    /// <summary>
    /// Создать репозиторий на основании переданного контекста.
    /// </summary>
    /// <param name="dbContext"><see cref="DbContext"/>.</param>
    /// <typeparam name="T">Тип репозитория.</typeparam>
    /// <returns>Созданный репозиторий.</returns>
    T Create<T>(TDbContext dbContext)
    where T : class;
}