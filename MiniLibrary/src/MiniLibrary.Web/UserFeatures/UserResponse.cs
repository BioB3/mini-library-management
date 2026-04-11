namespace MiniLibrary.Web.UserFeatures;

public record UserResponse(string Id, string Email, string FirstName, string LastName, IReadOnlyList<string> Roles);

public record UserDto(string Id, string Email, string FirstName, string LastName, IReadOnlyList<string> Roles);
