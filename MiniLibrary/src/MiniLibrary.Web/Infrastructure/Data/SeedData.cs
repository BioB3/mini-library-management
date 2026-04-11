using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MiniLibrary.Web.Infrastructure.Data;

public static class SeedData
{
  private const string AdminRole = "Admin";
  private const string LibrarianRole = "Librarian";
  private const string MemberRole = "Member";

  public static async Task InitializeAsync(AppDbContext dbContext, RoleManager<IdentityRole> roleManager, ILogger logger)
  {
    await CreateRolesAsync(roleManager, logger);
  }

  public static async Task PopulateTestDataAsync(AppDbContext dbContext, ILogger logger)
  {
    logger.LogInformation("Seeding database with sample data.");

    await dbContext.SaveChangesAsync();
  }

  private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
  {
    var roles = new[] { AdminRole, LibrarianRole, MemberRole };

    foreach (var roleName in roles)
    {
      var roleExists = await roleManager.RoleExistsAsync(roleName);
      if (!roleExists)
      {
        var result = await roleManager.CreateAsync(new IdentityRole { Name = roleName });
        if (result.Succeeded)
        {
          logger.LogInformation("Role {RoleName} created successfully", roleName);
        }
        else
        {
          logger.LogError("Failed to create role {RoleName}: {Errors}", roleName, string.Join(", ", result.Errors));
        }
      }
    }
  }
}

