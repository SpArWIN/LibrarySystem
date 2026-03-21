using Core.Domain.Models.Instanse;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Context.Central.Config;

/// <summary>
/// Конфигурация баз данных.
/// </summary>
public class LibraryInstanceConfiguration : IEntityTypeConfiguration<LibraryInstance>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LibraryInstance> builder)
    {
        builder.ToTable("LibraryInstances");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.ConnectionString)
            .IsRequired()
            .HasMaxLength(1000);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}