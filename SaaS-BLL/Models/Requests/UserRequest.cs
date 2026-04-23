// SaaS-BLL/Models/Requests/UserRequests.cs
// ADD CreateUserRequest to the existing file

namespace SaaS_BLL.Models.Requests;

public class UpdateUserRequest
{
    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public string Email { get; init; } = null!;
}

public class CreateUserRequest
{
    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public string Email { get; init; } = null!;

    public string Password { get; init; } = null!;

    public int UserRoleId { get; init; }
}