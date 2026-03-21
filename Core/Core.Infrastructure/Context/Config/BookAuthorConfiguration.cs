using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Context.Config;

/// <summary>
/// Конфигурация настроек промежуточной таблицы между автором и книгой.
/// </summary>
public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BookAuthor> builder)
    {
        builder.ToTable("BookAuthors");
        builder.HasKey(x => new { x.BookId, x.AuthorId });
        
        builder.HasOne(x => x.Book)
            .WithMany(x => x.BookAuthors)
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Author)
            .WithMany(x => x.BookAuthors)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}