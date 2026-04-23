using SaaS_BLL.Interfaces;
using SaaS_BLL.Models.Requests;
using SaaS_PR.Core;

namespace SaaS_PR.ViewModels.Shared;

public abstract class ProfileViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    protected readonly SessionContext SessionContext;

    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private string _currentPassword = string.Empty;
    private string _newPassword = string.Empty;
    private string _confirmNewPassword = string.Empty;
    private string? _errorMessage;
    private string? _successMessage;
    private bool _isBusy;

    public string FirstName { get => this._firstName; set => this.SetProperty(ref this._firstName, value); }

    public string LastName { get => this._lastName; set => this.SetProperty(ref this._lastName, value); }

    public string Email { get => this._email; set => this.SetProperty(ref this._email, value); }

    public string CurrentPassword { get => this._currentPassword; set => this.SetProperty(ref this._currentPassword, value); }

    public string NewPassword { get => this._newPassword; set => this.SetProperty(ref this._newPassword, value); }

    public string ConfirmNewPassword { get => this._confirmNewPassword; set => this.SetProperty(ref this._confirmNewPassword, value); }

    public string? ErrorMessage { get => this._errorMessage; set => this.SetProperty(ref this._errorMessage, value); }

    public string? SuccessMessage { get => this._successMessage; set => this.SetProperty(ref this._successMessage, value); }

    public bool IsBusy { get => this._isBusy; set => this.SetProperty(ref this._isBusy, value); }

    public AsyncRelayCommand UpdateProfileCommand { get; }

    public AsyncRelayCommand ChangePasswordCommand { get; }

    protected ProfileViewModel(IAuthService authService, IUserService userService, SessionContext sessionContext)
    {
        this._authService = authService;
        this._userService = userService;
        this.SessionContext = sessionContext;

        this.UpdateProfileCommand = new AsyncRelayCommand(this.UpdateProfileAsync, _ => !this.IsBusy);
        this.ChangePasswordCommand = new AsyncRelayCommand(this.ChangePasswordAsync, _ => !this.IsBusy);

        this.LoadCurrentUser();
    }

    private void LoadCurrentUser()
    {
        if (this.SessionContext.CurrentUser is not { } user)
        {
            return;
        }

        this.FirstName = user.FirstName;
        this.LastName = user.LastName;
        this.Email = user.Email;
    }

    private async Task UpdateProfileAsync(object? _)
    {
        this.IsBusy = true;
        this.ErrorMessage = null;
        this.SuccessMessage = null;

        var result = await this._userService.UpdateAsync(this.SessionContext.CurrentUser!.Id, new UpdateUserRequest
        {
            FirstName = this.FirstName,
            LastName = this.LastName,
            Email = this.Email
        });

        if (result.Success)
        {
            this.SessionContext.SetUser(result.Data!);
            this.SuccessMessage = "Profile updated successfully.";
        }
        else
        {
            this.ErrorMessage = result.Error;
        }

        this.IsBusy = false;
    }

    private async Task ChangePasswordAsync(object? _)
    {
        this.ErrorMessage = null;
        this.SuccessMessage = null;

        if (this.NewPassword != this.ConfirmNewPassword)
        {
            this.ErrorMessage = "New passwords do not match.";
            return;
        }

        this.IsBusy = true;

        var result = await this._authService.ChangePasswordAsync(this.SessionContext.CurrentUser!.Id, new ChangePasswordRequest
        {
            CurrentPassword = this.CurrentPassword,
            NewPassword = this.NewPassword
        });

        if (result.Success)
        {
            this.SuccessMessage = "Password changed successfully.";
            this.CurrentPassword = string.Empty;
            this.NewPassword = string.Empty;
            this.ConfirmNewPassword = string.Empty;
        }
        else
        {
            this.ErrorMessage = result.Error;
        }

        this.IsBusy = false;
    }
}