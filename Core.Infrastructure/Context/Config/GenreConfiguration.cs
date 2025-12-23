using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Context.Config;

/// <summary>
/// Конфигурация сущности жанров.
/// </summary>
public class GenreConfiguration :IEntityTypeConfiguration<Genre>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("Genres");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.NameGenre)
            .IsRequired()
            .HasMaxLength(100);
    }
}