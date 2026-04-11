using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using MiniLibrary.Web.Infrastructure.Identity;

namespace MiniLibrary.Web.UserFeatures.Login;

public sealed class LoginRequest
{
  public string Email { get; init; } = string.Empty;
  public string Password { get; init; } = string.Empty;
}

public sealed class LoginResponse
{
  public string UserId { get; init; } = string.Empty;
  public string Email { get; init; } = string.Empty;
  public string FirstName { get; init; } = string.Empty;
  public string LastName { get; init; } = string.Empty;
  public IReadOnlyList<string> Roles { get; init; } = new List<string>();
  public string Token { get; init; } = string.Empty;
}

public class LoginEndpoint(UserManager<ApplicationUser> userManager) : 
  Endpoint<LoginRequest, Results<Ok<LoginResponse>, UnauthorizedHttpResult, ProblemHttpResult>>
{
  public override void Configure()
  {
    Post("/auth/login");
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Login user";
      s.Description = "Authenticates a user and returns their information with assigned roles.";
      s.ExampleRequest = new LoginRequest
      {
        Email = "user@example.com",
        Password = "SecurePass123!"
      };
      s.ResponseExamples[200] = new LoginResponse
      {
        UserId = Guid.NewGuid().ToString(),
        Email = "user@example.com",
        FirstName = "John",
        LastName = "Doe",
        Roles = new List<string> { "Member" },
        Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
      };

      s.Responses[200] = "Login successful";
      s.Responses[401] = "Invalid credentials";
    });

    Tags("Auth");
  }

  public override async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult, ProblemHttpResult>>
  ExecuteAsync(LoginRequest request, CancellationToken ct)
  {
    var user = await userManager.FindByEmailAsync(request.Email);
    
    if (user == null)
      return TypedResults.Unauthorized();

    var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
    if (!passwordValid)
      return TypedResults.Unauthorized();

    var roles = (await userManager.GetRolesAsync(user)).ToList();

    var token = JwtBearer.CreateToken(o =>
    {
      o.SigningKey = Config["Jwt:SecretKey"]!;
      o.ExpireAt = DateTime.UtcNow.AddHours(24);

      if (roles.Count > 0)
        o.User.Roles.Add(roles.ToArray());

      o.User.Claims.Add(("Email", user.Email ?? ""));
      o.User["UserId"] = user.Id;
      o.User["FirstName"] = user.FirstName;
      o.User["LastName"] = user.LastName;
    });

    var response = new LoginResponse
    {
      UserId = user.Id,
      Email = user.Email ?? "",
      FirstName = user.FirstName,
      LastName = user.LastName,
      Roles = roles,
      Token = token
    };

    return TypedResults.Ok(response);
  }
}

public sealed class LoginValidator : Validator<LoginRequest>
{
  public LoginValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty()
      .WithMessage("Email is required")
      .EmailAddress()
      .WithMessage("Email must be valid");

    RuleFor(x => x.Password)
      .NotEmpty()
      .WithMessage("Password is required");
  }
}
