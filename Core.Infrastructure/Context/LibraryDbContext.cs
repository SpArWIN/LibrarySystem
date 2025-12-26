using Core.Domain.Enum.Roles;
using Core.Domain.Models;
using Core.Infrastructure.Constaints;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Context;

/// <summary>
/// Контекст Библиотеки.
/// </summary>
/// <param name="options"><see cref="DbContextOptions{TContext}"/>.</param>
public sealed class LibraryDbContext (DbContextOptions<LibraryDbContext> options)
: DbContext(options)
{
    
    /// <summary>Авторы.</summary>
    public DbSet<Author> Authors { get; init; }
    
    /// <summary>Книги.</summary>
    public DbSet<Book> Books { get; init; }
    
    /// <summary>Жанры.</summary>
    public DbSet<Genre> Genres { get; init; }
    
    /// <summary>Связь книга-жанр.</summary>
    public DbSet<BookGenre> BookGenres { get; init; }
    
    /// <summary>Выдачи/брони.</summary>
    public DbSet<Loan> Loans { get; init; }
    
    /// <summary>Издатели.</summary>
    public DbSet<Publisher> Publishers { get; init; }
    
    /// <summary>Связь книга-автор.</summary>
    public DbSet<BookAuthor> AuthorBooks { get; init; }
    
    /// <summary>
    /// Построение модели и применение конфигураций.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}