using SaaS_PR.Core;
using SaaS_PR.ViewModels.Auth;

namespace SaaS_PR.ViewModels.Admin;

public class AdminShellViewModel : ViewModelBase
{
    private readonly ShellNavigationService _shellNav;
    private readonly RootNavigationService _rootNav;
    private readonly SessionContext _sessionContext;

    public string CurrentUserName => this._sessionContext.CurrentUser is { } user
        ? $"{user.FirstName} {user.LastName}"
        : string.Empty;

    // Shell views bind their ContentControl to this
    public ShellNavigationService ShellNav => this._shellNav;

    public RelayCommand GoToDashboardCommand { get; }

    public RelayCommand GoToUsersCommand { get; }

    public RelayCommand GoToPlansCommand { get; }

    public RelayCommand GoToPlanAddonsCommand { get; }

    public RelayCommand GoToSubscriptionsCommand { get; }

    public RelayCommand GoToSubscriptionAddonsCommand { get; }

    public RelayCommand GoToPaymentsCommand { get; }

    public RelayCommand GoToProfileCommand { get; }

    public RelayCommand LogoutCommand { get; }

    public AdminShellViewModel(
        ShellNavigationService shellNav,
        RootNavigationService rootNav,
        SessionContext sessionContext)
    {
        this._shellNav = shellNav;
        this._rootNav = rootNav;
        this._sessionContext = sessionContext;

        this.GoToDashboardCommand = new RelayCommand(() => this._shellNav.NavigateTo<AdminDashboardViewModel>());
        this.GoToUsersCommand = new RelayCommand(() => this._shellNav.NavigateTo<UsersViewModel>());
        this.GoToPlansCommand = new RelayCommand(() => this._shellNav.NavigateTo<PlansViewModel>());
        this.GoToPlanAddonsCommand = new RelayCommand(() => this._shellNav.NavigateTo<PlanAddonsViewModel>());
        this.GoToSubscriptionsCommand = new RelayCommand(() => this._shellNav.NavigateTo<SubscriptionsViewModel>());
        this.GoToSubscriptionAddonsCommand = new RelayCommand(() => this._shellNav.NavigateTo<SubscriptionAddonsViewModel>());
        this.GoToPaymentsCommand = new RelayCommand(() => this._shellNav.NavigateTo<PaymentsViewModel>());
        this.GoToProfileCommand = new RelayCommand(() => this._shellNav.NavigateTo<AdminProfileViewModel>());
        this.LogoutCommand = new RelayCommand(this.ExecuteLogout);

        this._shellNav.NavigateTo<AdminDashboardViewModel>();
    }

    private void ExecuteLogout()
    {
        this._sessionContext.Clear();
        this._rootNav.NavigateTo<LoginViewModel>();
    }
}