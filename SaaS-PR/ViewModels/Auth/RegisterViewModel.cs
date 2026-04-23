using SaaS_BLL.Interfaces;
using SaaS_BLL.Models.Requests;
using SaaS_PR.Core;

namespace SaaS_PR.ViewModels.Auth;

public class RegisterViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly RootNavigationService _rootNav;
    private readonly SessionContext _sessionContext;

    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string? _errorMessage;
    private bool _isBusy;

    public string FirstName
    {
        get => this._firstName;
        set => this.SetProperty(ref this._firstName, value);
    }

    public string LastName
    {
        get => this._lastName;
        set => this.SetProperty(ref this._lastName, value);
    }

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

    public string ConfirmPassword
    {
        get => this._confirmPassword;
        set => this.SetProperty(ref this._confirmPassword, value);
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

    public AsyncRelayCommand RegisterCommand { get; }

    public RelayCommand GoToLoginCommand { get; }

    public RegisterViewModel(
        IAuthService authService,
        RootNavigationService rootNav,
        SessionContext sessionContext)
    {
        this._authService = authService;
        this._rootNav = rootNav;
        this._sessionContext = sessionContext;

        this.RegisterCommand = new AsyncRelayCommand(this.ExecuteRegisterAsync, _ => !this.IsBusy);
        this.GoToLoginCommand = new RelayCommand(() => this._rootNav.NavigateTo<LoginViewModel>());
    }

    private async Task ExecuteRegisterAsync(object? _)
    {
        this.ErrorMessage = null;

        if (this.Password != this.ConfirmPassword)
        {
            this.ErrorMessage = "Passwords do not match.";
            return;
        }

        this.IsBusy = true;

        var result = await this._authService.RegisterAsync(new RegisterRequest
        {
            FirstName = this.FirstName,
            LastName = this.LastName,
            Email = this.Email,
            Password = this.Password
        });

        this.IsBusy = false;

        if (!result.Success)
        {
            this.ErrorMessage = result.Error;
            return;
        }

        // After registration navigate back to login
        this._rootNav.NavigateTo<LoginViewModel>();
    }
}