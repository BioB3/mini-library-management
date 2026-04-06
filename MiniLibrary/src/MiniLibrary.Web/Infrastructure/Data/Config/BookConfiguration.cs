using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniLibrary.Web.Domain.BookAggregate;

namespace MiniLibrary.Web.Infrastructure.Data.Config;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
  public void Configure(EntityTypeBuilder<Book> builder)
  {
    builder.Property(entity => entity.Id)
      .HasValueGenerator<VogenGuidIdValueGenerator<AppDbContext, Book, BookId>>()
      .HasVogenConversion()
      .IsRequired();

    builder.Property(entity => entity.Title)
      .HasMaxLength(200)
      .IsRequired();

    builder.Property(entity => entity.Author)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(entity => entity.Isbn)
    .HasMaxLength(20)
    .IsRequired();

    builder.HasIndex(entity => entity.Isbn)
    .IsUnique();

    builder.HasMany(b => b.Copies)
    .WithOne()
    .HasForeignKey("BookId")
    .OnDelete(DeleteBehavior.Cascade);

    builder.Navigation(b => b.Copies)
    .UsePropertyAccessMode(PropertyAccessMode.Field);
  }
}
