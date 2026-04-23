namespace SaaS_BLL.Models.Requests;

public class LoginRequest
{
    public string Email { get; init; } = null!;

    public string Password { get; init; } = null!;
}

public class RegisterRequest
{
    public string FirstName { get; init; } = null!;

    public string LastName { get; init; } = null!;

    public string Email { get; init; } = null!;

    public string Password { get; init; } = null!;
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; init; } = null!;

    public string NewPassword { get; init; } = null!;
}