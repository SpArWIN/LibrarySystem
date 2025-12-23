using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Context.Config;

/// <summary>
/// Конфигурация настроек сущности автор.
/// </summary>
public sealed class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x=>x.LastName)
            .HasMaxLength(100);
        
        builder.Property(x=> x.SurName)
            .HasMaxLength(100);
        
        builder.Property(x=> x.Country)
            .HasMaxLength(100);
        
        builder.HasIndex(x => new { x.Name, x.LastName });
        
        builder.HasMany(x => x.BookAuthors)
            .WithOne(x => x.Author)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}