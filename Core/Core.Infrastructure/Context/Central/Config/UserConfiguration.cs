using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Context.Central.Config;

/// <summary>
/// Конфигурация сущности пользователя.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        
        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Username).IsUnique();

        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SurName).HasMaxLength(100);
        
        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(u => u.Address)
            .HasMaxLength(300);
        builder.Property(u => u.Phone)
            .HasMaxLength(12);

        builder.Property(u => u.RegistrationDate)
            .IsRequired();

        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}