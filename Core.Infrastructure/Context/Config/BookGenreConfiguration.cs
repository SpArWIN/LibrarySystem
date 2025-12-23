using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Context.Config;

/// <summary>
/// Конфигурация сущностей жанр и книги.
/// </summary>
public class BookGenreConfiguration : IEntityTypeConfiguration<BookGenre>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BookGenre> builder)
    {
        builder.ToTable("BookGenres");
        builder.HasKey(x => new { x.BookId, x.GenreId });
        
        builder.HasOne(bg => bg.Book) 
            .WithMany(b => b.BookGenres)
            .HasForeignKey(bg => bg.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bg => bg.Genre) 
            .WithMany(g => g.BookGenres)
            .HasForeignKey(bg => bg.GenreId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}