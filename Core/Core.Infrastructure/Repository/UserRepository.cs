using Common.Extensions;
using Core.Domain.Enum.Roles;
using Core.Domain.Models;
using Core.Domain.Models.Pagination;
using Core.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository;

/// <inheritdoc />
public sealed class UserRepository : IUserRepository
{
    private readonly CentralDbContext _context;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="context"><see cref="CentralDbContext"/>.</param>
    public UserRepository(CentralDbContext context)
    {
        _context = context;
    }
    
    /// <inheritdoc />
    public async Task<(IEnumerable<User?> Items, int TotalCount)> GetAllUsersAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {
        var pageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
        var pageSize = pagination.PageSize <= 0 ? 1 : pagination.PageSize;
        var skip = (pageNumber - 1) * pageSize;
        var query = _context.Users
            .AsQueryable()
            .AsNoTracking()
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role);
        var totalCount = await query.CountAsync(cancellationToken);
        
        var pageIds = await query
            .AsNoTracking()
            .Skip(skip)
            .Take(pageSize)
            .Select(x=>x.Id)
            .ToListAsync(cancellationToken);
        if (!pageIds.Any())
        {
            return ([], 0);
        }

        var items =  await query
            .AsQueryable()
            .Where(x=> pageIds.Contains(x.Id))
            .Select(user =>  new User()
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                SurName = user.SurName,
                LastName = user.LastName,
                Address = user.Address,
                RegistrationDate = user.RegistrationDate,
                Password = user.Password,
                DateOfBirth = user.DateOfBirth,
               UserRoles =  user.UserRoles.Select(x=> new UserRole()
               {
                   UserId = x.UserId,
                   Role = x.Role,
                   User = x.User
               }).ToList()
            }).ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<Guid> usersIds, CancellationToken cancellationToken = default)
    {
        var ids = usersIds as Guid[] ?? usersIds.ToArray();

        if (!ids.Any())
        {
            return [];
        }
        return await _context
            .Users
            .AsQueryable()
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Include(x=>x.UserRoles)
            .ThenInclude(x => x.Role)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
        
    }

    /// <inheritdoc />
    public async Task<User?> GetUserByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task UpdateUsersAsync(IEnumerable<User> users)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task DeleteUsersAsync(IEnumerable<User> users)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<Dictionary<Guid, string>> GetRolesUser(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<List<Role>> GetUsersRoles(Guid userId, CancellationToken cancellationToken = default)
    {
       return await _context.Roles
           .AsNoTracking()
           .AsQueryable()
           .Where(x=>x.UserRoles.All(x=>x.UserId == userId))
           .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<string?>> GetUserRolesName(User user, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Include(ur => ur.Role)
            .Where(ur => ur.Role != null &&! string.IsNullOrEmpty(ur.Role.Name))
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    //TODO Подумать нал созданием отдельного репозитория ролей.
    /// <inheritdoc />
    public async Task<List<Role>> GetDefaultRole()
    {
        return await _context.Roles
            .AsNoTracking()
            .AsQueryable()
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Guid>> GetDefaultRoleIdsAsync(CancellationToken cancellationToken = default)
    {
        var defaultRole = await _context.Roles
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(r =>r.Name == "Reader", cancellationToken);
        
        return defaultRole is not null
            ? [defaultRole.Id]
            : Array.Empty<Guid>();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>> GetRoleNamesByIdsAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        if (roleIds is null || !roleIds.Any())
        {
            return Array.Empty<string>();
        }
        
        return await _context.Roles
            .AsNoTracking()
            .Where(r => roleIds!.Contains(r.Id))
            .Select(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddUsersAsync(IEnumerable<User> users, CancellationToken ct = default)
    {
        var userList = users as User[] ?? users.ToArray();
        if (!userList.Any())
        {
            return;
        }
        await _context.Users.AddRangeAsync(userList, ct);
    }
}