using SaaS_BLL.Interfaces;
using SaaS_BLL.Models.Requests;
using SaaS_PR.Core;
using SaaS_PR.ViewModels.Admin;
using SaaS_PR.ViewModels.User;

namespace SaaS_PR.ViewModels.Auth;

public class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly RootNavigationService _rootNav;
    private readonly SessionContext _sessionContext;

    private string _email = string.Empty;
    private string _password = string.Empty;
    private string? _errorMessage;
    private bool _isBusy;

    public string Email
    {
        get => this._email;
        set => this.SetProperty(ref this._email, value);
    }

    public string Password
    {
        get => this._password;
        set => this.SetProperty(ref this._password, value);
    }

    public string? ErrorMessage
    {
        get => this._errorMessage;
        set => this.SetProperty(ref this._errorMessage, value);
    }

    public bool IsBusy
    {
        get => this._isBusy;
        set => this.SetProperty(ref this._isBusy, value);
    }

    public AsyncRelayCommand LoginCommand { get; }

    public RelayCommand GoToRegisterCommand { get; }

    public LoginViewModel(
        IAuthService authService,
        RootNavigationService rootNav,
        SessionContext sessionContext)
    {
        this._authService = authService;
        this._rootNav = rootNav;
        this._sessionContext = sessionContext;

        this.LoginCommand = new AsyncRelayCommand(this.ExecuteLoginAsync, _ => !this.IsBusy);
        this.GoToRegisterCommand = new RelayCommand(() => this._rootNav.NavigateTo<RegisterViewModel>());
    }

    private async Task ExecuteLoginAsync(object? _)
    {
        this.ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(this.Email) || string.IsNullOrWhiteSpace(this.Password))
        {
            this.ErrorMessage = "Please enter your email and password.";
            return;
        }

        this.IsBusy = true;

        var result = await this._authService.LoginAsync(new LoginRequest
        {
            Email = this.Email,
            Password = this.Password
        });

        this.IsBusy = false;

        if (!result.Success)
        {
            this.ErrorMessage = result.Error;
            return;
        }

        this._sessionContext.SetUser(result.Data!);

        if (this._sessionContext.IsAdmin)
        {
            this._rootNav.NavigateTo<AdminShellViewModel>();
        }
        else
        {
            this._rootNav.NavigateTo<UserShellViewModel>();
        }
    }
}