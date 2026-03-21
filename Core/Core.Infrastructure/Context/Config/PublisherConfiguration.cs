using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Context.Config;

/// <summary>
/// Конфигурация сузности издательства.
/// </summary>
public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.ToTable("Publishers");

        builder.HasKey(p => p.Id);
        
        builder.Property(x => x.PublisherName)
            .IsRequired()
            .HasMaxLength(256);
        builder.HasIndex(x => x.PublisherName).IsUnique();
        
        builder.Property(x=>x.Address).HasMaxLength(512);
        
    }
}