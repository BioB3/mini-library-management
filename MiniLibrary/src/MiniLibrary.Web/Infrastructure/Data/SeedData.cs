using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MiniLibrary.Web.Infrastructure.Data;

public static class SeedData
{

  public static async Task InitializeAsync(AppDbContext dbContext, ILogger logger)
  {

  }

  public static async Task PopulateTestDataAsync(AppDbContext dbContext, ILogger logger)
  {
    logger.LogInformation("Seeding database with sample data.");

    await dbContext.SaveChangesAsync();
  }
}
