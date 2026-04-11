using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniLibrary.Web.Domain.BookAggregate;
using MiniLibrary.Web.Domain.MemberAggregate;
using MiniLibrary.Web.Infrastructure.Identity;

namespace MiniLibrary.Web.Infrastructure.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : 
  IdentityDbContext<ApplicationUser>(options)
{
  public DbSet<Member> Members => Set<Member>();
  public DbSet<Book> Books => Set<Book>();
  public DbSet<BookCopy> BookCopies => Set<BookCopy>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override int SaveChanges() =>
        SaveChangesAsync().GetAwaiter().GetResult();
}
