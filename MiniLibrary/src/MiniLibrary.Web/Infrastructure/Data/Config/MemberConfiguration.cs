using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniLibrary.Web.Domain.MemberAggregate;

namespace MiniLibrary.Web.Infrastructure.Data.Config;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
  public void Configure(EntityTypeBuilder<Member> builder)
  {
    builder.Property(entity => entity.Id)
      .HasValueGenerator<VogenGuidIdValueGenerator<AppDbContext, Member, MemberId>>()
      .HasVogenConversion()
      .IsRequired();

    builder.Property(entity => entity.UserId)
      .HasMaxLength(450)
      .IsRequired();

    builder.HasIndex(entity => entity.UserId)
      .IsUnique();

    builder.Property(entity => entity.FirstName)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(entity => entity.LastName)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(entity => entity.JoinedAt)
      .IsRequired();
  }
}
