using Core.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Context.Config;

/// <summary>
/// Конфигурация промежуточный таблицы выдачи пользователю книгу.
/// </summary>
public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BookId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.BookId, x.IsReturned });
        builder.HasIndex(x => new { x.UserId, x.IsReturned });
        builder.HasIndex(x => x.ExpiryDate);
        builder.Property(x => x.BookStatus)
            .HasConversion<int>()
            .IsRequired();
        
        builder.HasOne(x => x.Book)
            .WithMany()
            .HasForeignKey(x => x.BookId);

    }
}