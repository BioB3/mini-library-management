using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniLibrary.Web.Domain.BookAggregate;

namespace MiniLibrary.Web.Infrastructure.Data.Config;

public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
{
    public void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        builder.Property(entity => entity.Id)
        .HasValueGenerator<VogenGuidIdValueGenerator<AppDbContext, BookCopy, BookCopyId>>()
        .HasVogenConversion()
        .IsRequired();

        builder.Property(entity => entity.SerialNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(entity => entity.SerialNumber)
            .IsUnique();

        builder.Property(entity => entity.IsAvailable)
            .IsRequired()
            .HasDefaultValue(true);
    }
}