using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Context.Config;

/// <summary>
/// Конфигурация сущности книги.
/// </summary>
public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(b => b.Id);
        
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(512);
        
        builder.Property(x => x.Description)
            .HasMaxLength(4000);
        
        builder.Property(x => x.Country)
            .HasMaxLength(64);
        
        builder.Property(x => x.BookKey)
            .IsRequired()
            .HasMaxLength(128);
        
        builder.HasOne(x => x.Publisher)
            .WithMany()
            .HasForeignKey(x => x.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasMany(x => x.BookGenres)
            .WithOne(x => x.Book)
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x=> x.BookAuthors)
            .WithOne(x => x.Book)
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}