using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using MiniLibrary.Web.Domain.MemberAggregate;
using MiniLibrary.Web.Infrastructure.Data;
using MiniLibrary.Web.Infrastructure.Identity;

namespace MiniLibrary.Web.UserFeatures.Register;

public sealed class RegisterUserRequest
{
  public string Email { get; init; } = string.Empty;
  public string FirstName { get; init; } = string.Empty;
  public string LastName { get; init; } = string.Empty;
  public string Password { get; init; } = string.Empty;
}

public class RegisterEndpoint(UserManager<ApplicationUser> userManager, IRepository<Member> memberRepository) : 
  Endpoint<RegisterUserRequest, 
          Results<Created<UserResponse>, ValidationProblem, ProblemHttpResult>>
{
  public override void Configure()
  {
    Post("/auth/register");
    AllowAnonymous();

    Summary(s =>
    {
      s.Summary = "Register a new user";
      s.Description = "Creates a new user account with the specified email and password.";
      s.ExampleRequest = new RegisterUserRequest 
      { 
        Email = "user@example.com", 
        FirstName = "John", 
        LastName = "Doe",
        Password = "SecurePass123!" 
      };
      s.ResponseExamples[201] = new UserResponse(
        Guid.NewGuid().ToString(), 
        "user@example.com", 
        "John", 
        "Doe", 
        new List<string> { "Member" }
      );

      s.Responses[201] = "User registered successfully";
      s.Responses[400] = "Invalid request data or user already exists";
    });

    Tags("Auth");
  }

  public override async Task<Results<Created<UserResponse>, ValidationProblem, ProblemHttpResult>> 
    ExecuteAsync(RegisterUserRequest request, CancellationToken cancellationToken)
  {
    var user = new ApplicationUser 
    { 
      UserName = request.Email,
      Email = request.Email,
      FirstName = request.FirstName,
      LastName = request.LastName
    };

    var result = await userManager.CreateAsync(user, request.Password);
    
    if (!result.Succeeded)
    {
      var errors = result.Errors
        .GroupBy(e => "User")
        .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

      return TypedResults.ValidationProblem(errors);
    }

    // Assign Member role by default
    await userManager.AddToRoleAsync(user, "Member");

    // Create Member domain entity linked to the user
    var member = Member.Create(user.Id, request.FirstName, request.LastName);
    await memberRepository.AddAsync(member, cancellationToken);
    await memberRepository.SaveChangesAsync(cancellationToken);

    var response = new UserResponse(user.Id, user.Email ?? "", user.FirstName, user.LastName, new List<string> { "Member" });
    return TypedResults.Created($"/users/{user.Id}", response);
  }
}

public sealed class RegisterUserValidator : Validator<RegisterUserRequest>
{
  public RegisterUserValidator()
  {
    RuleFor(x => x.Email)
      .NotEmpty()
      .WithMessage("Email is required")
      .EmailAddress()
      .WithMessage("Email must be valid");

    RuleFor(x => x.FirstName)
      .NotEmpty()
      .WithMessage("First name is required")
      .MaximumLength(100)
      .WithMessage("First name must not exceed 100 characters");

    RuleFor(x => x.LastName)
      .NotEmpty()
      .WithMessage("Last name is required")
      .MaximumLength(100)
      .WithMessage("Last name must not exceed 100 characters");

    RuleFor(x => x.Password)
      .NotEmpty()
      .WithMessage("Password is required")
      .MinimumLength(8)
      .WithMessage("Password must be at least 8 characters");
  }
}
