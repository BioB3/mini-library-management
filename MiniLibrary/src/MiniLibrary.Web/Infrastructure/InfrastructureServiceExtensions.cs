using Ardalis.GuardClauses;
using MiniLibrary.Web.Infrastructure.Data;
using MiniLibrary.Web.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MiniLibrary.Web.Infrastructure;

public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    // Always use SQL Server from Aspire
    string? connectionString = config.GetConnectionString("AppDb");
    Guard.Against.Null(connectionString, "AppDb connection string is required. Make sure the application is running with Aspire.");

    services.AddScoped<EventDispatchInterceptor>();
    services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();

    services.AddDbContext<AppDbContext>((provider, options) =>
    {
      var eventDispatchInterceptor = provider.GetRequiredService<EventDispatchInterceptor>();

      options.UseNpgsql(connectionString,
      b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
      options.AddInterceptors(eventDispatchInterceptor);
    });

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
          .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

    // Add ASP.NET Core Identity
    services.AddIdentityCore<ApplicationUser>(options =>
    {
      options.Password.RequiredLength = 8;
      options.Password.RequireDigit = true;
      options.Password.RequireUppercase = true;
      options.Password.RequireNonAlphanumeric = false;
      options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
