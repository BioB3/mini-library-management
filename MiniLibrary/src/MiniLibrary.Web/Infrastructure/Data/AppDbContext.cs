using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MiniLibrary.Web.Domain.BookAggregate;

namespace MiniLibrary.Web.Infrastructure.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : 
  DbContext(options)
{
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
