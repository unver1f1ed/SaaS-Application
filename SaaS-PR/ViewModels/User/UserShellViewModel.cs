using SaaS_PR.Core;
using SaaS_PR.ViewModels.Auth;

namespace SaaS_PR.ViewModels.User;

public class UserShellViewModel : ViewModelBase
{
    private readonly ShellNavigationService _shellNav;
    private readonly RootNavigationService _rootNav;
    private readonly SessionContext _sessionContext;

    public string CurrentUserName => this._sessionContext.CurrentUser is { } user
        ? $"{user.FirstName} {user.LastName}"
        : string.Empty;

    public ShellNavigationService ShellNav => this._shellNav;

    public RelayCommand GoToDashboardCommand { get; }

    public RelayCommand GoToBrowsePlansCommand { get; }

    public RelayCommand GoToMySubscriptionCommand { get; }

    public RelayCommand GoToMyPaymentsCommand { get; }

    public RelayCommand GoToProfileCommand { get; }

    public RelayCommand LogoutCommand { get; }

    public RelayCommand GoBackCommand { get; }

    public RelayCommand GoForwardCommand { get; }

    public UserShellViewModel(
        ShellNavigationService shellNav,
        RootNavigationService rootNav,
        SessionContext sessionContext)
    {
        this._shellNav = shellNav;
        this._rootNav = rootNav;
        this._sessionContext = sessionContext;

        this.GoToDashboardCommand = new RelayCommand(() => this._shellNav.NavigateTo<UserDashboardViewModel>());
        this.GoToBrowsePlansCommand = new RelayCommand(() => this._shellNav.NavigateTo<BrowsePlansViewModel>());
        this.GoToMySubscriptionCommand = new RelayCommand(() => this._shellNav.NavigateTo<MySubscriptionViewModel>());
        this.GoToMyPaymentsCommand = new RelayCommand(() => this._shellNav.NavigateTo<MyPaymentsViewModel>());
        this.GoToProfileCommand = new RelayCommand(() => this._shellNav.NavigateTo<UserProfileViewModel>());
        this.LogoutCommand = new RelayCommand(this.ExecuteLogout);
        this.GoBackCommand = new RelayCommand(() => this._shellNav.GoBack(), () => this._shellNav.CanGoBack);
        this.GoForwardCommand = new RelayCommand(() => this._shellNav.GoForward(), () => this._shellNav.CanGoForward);

        this._shellNav.NavigateTo<UserDashboardViewModel>();
    }

    private void ExecuteLogout()
    {
        this._sessionContext.Clear();
        this._rootNav.NavigateTo<LoginViewModel>();
    }
}