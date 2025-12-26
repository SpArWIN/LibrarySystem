using Core.Domain.Models.Instanse;
using Core.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository;

/// <inheritdoc />
public sealed class LibraryInstanceRepository(CentralDbContext context) : ILibraryInstanceRepository
{
    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    => await context.LibraryInstances.AnyAsync(x => x.Name == name, ct);

    /// <inheritdoc />
    public async Task AddAsync(LibraryInstance instance, CancellationToken ct = default)
     => await context.LibraryInstances.AddAsync(instance, ct);

    /// <inheritdoc />
    public async Task<LibraryInstance?> FindAsync(Guid id, CancellationToken ct = default)
     => await context.LibraryInstances.FirstOrDefaultAsync(x=> x.Id == id, ct);

    /// <inheritdoc />
    public Task RemoveAsync(LibraryInstance instance, CancellationToken ct = default)
    {
        context.LibraryInstances.Remove(instance);
        return Task.CompletedTask;
    }
}