using SaaS_BLL.Models;

namespace SaaS_PR.Core;

/// <summary>
/// Holds the currently authenticated user for the duration of the session.
/// Registered as a singleton in DI and injected into ViewModels that need user context.
/// </summary>
public class SessionContext
{
    public UserDto? CurrentUser { get; private set; }

    public bool IsAuthenticated => this.CurrentUser is not null;

    public bool IsAdmin => this.CurrentUser?.RoleName == "Admin";

    public void SetUser(UserDto user)
    {
        this.CurrentUser = user;
    }

    public void Clear()
    {
        this.CurrentUser = null;
    }
}