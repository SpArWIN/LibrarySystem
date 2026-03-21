using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Context.Config;

/// <summary>
/// Конфигурация инвентарных книг.
/// </summary>
public sealed class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        builder.ToTable("BookCopies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BookKey)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.BookKey)
            .IsUnique();

        builder.HasIndex(x => new { x.BookId, x.BookStatus });
        
        builder.HasOne(x => x.Book)
            .WithMany()
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}