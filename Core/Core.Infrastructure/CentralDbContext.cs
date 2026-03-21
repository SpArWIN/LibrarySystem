using Core.Domain.Models;
using Core.Domain.Models.Instanse;
using Core.Infrastructure.Constaints;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure;


public sealed class CentralDbContext(DbContextOptions<CentralDbContext> options) : DbContext(options)
{
    /// <summary>Центральная таблица, которая будет хранить все остальные.</summary>
    public DbSet<LibraryInstance> LibraryInstances { get; init; }
    
    /// <summary>Пользователи.</summary>
    public DbSet<User> Users { get; init; }
    
    /// <summary>Роли пользователей.</summary>
    public DbSet<UserRole> UserRoles { get; init; }
    
    /// <summary>Роли.</summary>
    public DbSet<Role> Roles { get; init; }
    
    /// <summary>Refresh-сессии.</summary>
    public DbSet<RefreshSession> RefreshSessions { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CentralDbContext).Assembly,
            type =>type.FullName!.StartsWith("Core.Infrastructure.Context.Central.Config", StringComparison.Ordinal)
        );
        
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = RoleIds.Reader, Name = nameof(Domain.Enum.Roles.Roles.Reader), Description = "Читатель" },
            new Role { Id = RoleIds.Librarian, Name = nameof(Domain.Enum.Roles.Roles.Librarian), Description = "Библиотекарь" },
            new Role { Id = RoleIds.Administrator, Name = nameof(Domain.Enum.Roles.Roles.Administrator), Description = "Администратор" }
        );
        base.OnModelCreating(modelBuilder);
    }
}