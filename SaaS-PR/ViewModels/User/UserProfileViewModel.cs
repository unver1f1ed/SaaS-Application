using SaaS_BLL.Interfaces;
using SaaS_PR.Core;
using SaaS_PR.ViewModels.Shared;

namespace SaaS_PR.ViewModels.User;

public class UserProfileViewModel : ProfileViewModel
{
    public UserProfileViewModel(IAuthService authService, IUserService userService, SessionContext sessionContext)
        : base(authService, userService, sessionContext)
    {
    }
}