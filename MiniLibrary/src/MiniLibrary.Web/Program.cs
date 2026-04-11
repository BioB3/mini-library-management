using FastEndpoints;
using FastEndpoints.Swagger;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;
using MiniLibrary.Web.Configurations;
using MiniLibrary.Web.Infrastructure.Data;
using MiniLibrary.Web.Infrastructure.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults()    // This sets up OpenTelemetry logging
       .AddLoggerConfigs();     // This adds Serilog for console formatting

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

startupLogger.LogInformation("Starting web host");

builder.Services.AddOptionConfigs(builder.Configuration, startupLogger, builder);
builder.Services.AddServiceConfigs(startupLogger, builder);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["Jwt:SecretKey"]);
builder.Services.AddAuthorization();

builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                  o.ShortSchemaNames = true;
                  o.EnableJWTBearerAuth = true;
                });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

await app.UseAppMiddlewareAndSeedDatabase();

app.MapDefaultEndpoints(); // Aspire health checks and metrics

app.Run();

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program { }
