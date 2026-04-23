using System.Collections.ObjectModel;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_PR.Core;

namespace SaaS_PR.ViewModels.Admin;

public class UsersViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    private readonly IUserRoleService _userRoleService;

    private ObservableCollection<UserDto> _users = new();
    private ObservableCollection<UserRoleDto> _availableRoles = new();
    private UserDto? _selectedUser;
    private string _searchText = string.Empty;
    private bool _isBusy;
    private bool _isEditing;
    private string? _errorMessage;

    // Form fields
    private string _formFirstName = string.Empty;
    private string _formLastName = string.Empty;
    private string _formEmail = string.Empty;
    private string _formPassword = string.Empty;
    private UserRoleDto? _formRole;

    private List<UserDto> _allUsers = new();

    public ObservableCollection<UserDto> Users
    {
        get => this._users;
        set => this.SetProperty(ref this._users, value);
    }

    public ObservableCollection<UserRoleDto> AvailableRoles
    {
        get => this._availableRoles;
        set => this.SetProperty(ref this._availableRoles, value);
    }

    public UserDto? SelectedUser
    {
        get => this._selectedUser;
        set
        {
            this.SetProperty(ref this._selectedUser, value);
            if (value is not null)
            {
                this.PopulateForm(value);
            }
        }
    }

    public string SearchText
    {
        get => this._searchText;
        set
        {
            this.SetProperty(ref this._searchText, value);
            this.FilterUsers();
        }
    }

    public bool IsBusy { get => this._isBusy; set => this.SetProperty(ref this._isBusy, value); }

    public bool IsEditing { get => this._isEditing; set => this.SetProperty(ref this._isEditing, value); }

    public string? ErrorMessage { get => this._errorMessage; set => this.SetProperty(ref this._errorMessage, value); }

    public string FormFirstName { get => this._formFirstName; set => this.SetProperty(ref this._formFirstName, value); }

    public string FormLastName { get => this._formLastName; set => this.SetProperty(ref this._formLastName, value); }

    public string FormEmail { get => this._formEmail; set => this.SetProperty(ref this._formEmail, value); }

    public string FormPassword { get => this._formPassword; set => this.SetProperty(ref this._formPassword, value); }

    public UserRoleDto? FormRole { get => this._formRole; set => this.SetProperty(ref this._formRole, value); }

    public AsyncRelayCommand LoadCommand { get; }

    public AsyncRelayCommand SaveCommand { get; }

    public AsyncRelayCommand DeleteUserCommand { get; }

    public RelayCommand NewUserCommand { get; }

    public RelayCommand CancelEditCommand { get; }

    public UsersViewModel(IUserService userService, IUserRoleService userRoleService)
    {
        this._userService = userService;
        this._userRoleService = userRoleService;

        this.LoadCommand = new AsyncRelayCommand(this.LoadAsync);
        this.SaveCommand = new AsyncRelayCommand(this.SaveAsync, _ => !this.IsBusy);
        this.DeleteUserCommand = new AsyncRelayCommand(this.DeleteUserAsync, _ => this.SelectedUser is not null);
        this.NewUserCommand = new RelayCommand(this.StartNew);
        this.CancelEditCommand = new RelayCommand(this.CancelEdit);
    }

    private async Task LoadAsync(object? _)
    {
        this.IsBusy = true;
        this.ErrorMessage = null;

        var usersResult = await this._userService.GetAllAsync();
        if (usersResult.Success)
        {
            this._allUsers = usersResult.Data!.ToList();
            this.Users = new ObservableCollection<UserDto>(this._allUsers);
        }
        else
        {
            this.ErrorMessage = usersResult.Error;
        }

        var rolesResult = await this._userRoleService.GetAllAsync();
        if (rolesResult.Success)
        {
            this.AvailableRoles = new ObservableCollection<UserRoleDto>(rolesResult.Data!);
        }

        this.IsBusy = false;
    }

    private async Task SaveAsync(object? _)
    {
        if (this.FormRole is null)
        {
            this.ErrorMessage = "Please select a role.";
            return;
        }

        this.IsBusy = true;
        this.ErrorMessage = null;

        if (this.SelectedUser is null)
        {
            var result = await this._userService.CreateAsync(new CreateUserRequest
            {
                FirstName = this.FormFirstName,
                LastName = this.FormLastName,
                Email = this.FormEmail,
                Password = this.FormPassword,
                UserRoleId = this.FormRole.Id
            });

            if (!result.Success)
            {
                this.ErrorMessage = result.Error;
                this.IsBusy = false;
                return;
            }
        }
        else
        {
            var result = await this._userService.UpdateAsync(this.SelectedUser.Id, new UpdateUserRequest
            {
                FirstName = this.FormFirstName,
                LastName = this.FormLastName,
                Email = this.FormEmail
            });

            if (!result.Success)
            {
                this.ErrorMessage = result.Error;
                this.IsBusy = false;
                return;
            }
        }

        this.IsEditing = false;
        this.IsBusy = false;
        await this.LoadAsync(null);
    }

    private async Task DeleteUserAsync(object? _)
    {
        if (this.SelectedUser is null)
        {
            return;
        }

        var result = await this._userService.DeleteAsync(this.SelectedUser.Id);
        if (result.Success)
        {
            this.IsEditing = false;
            await this.LoadAsync(null);
        }
        else
        {
            this.ErrorMessage = result.Error;
        }
    }

    private void StartNew()
    {
        this.SelectedUser = null;
        this.ClearForm();
        this.IsEditing = true;
    }

    private void CancelEdit()
    {
        this.IsEditing = false;
        this.ClearForm();
    }

    private void PopulateForm(UserDto user)
    {
        this.FormFirstName = user.FirstName;
        this.FormLastName = user.LastName;
        this.FormEmail = user.Email;
        this.FormPassword = string.Empty;
        this.FormRole = this.AvailableRoles.FirstOrDefault(r => r.Id == user.UserRoleId);
        this.IsEditing = true;
    }

    private void ClearForm()
    {
        this.FormFirstName = string.Empty;
        this.FormLastName = string.Empty;
        this.FormEmail = string.Empty;
        this.FormPassword = string.Empty;
        this.FormRole = null;
    }

    private void FilterUsers()
    {
        if (string.IsNullOrWhiteSpace(this.SearchText))
        {
            this.Users = new ObservableCollection<UserDto>(this._allUsers);
            return;
        }

        var filtered = this._allUsers.Where(u =>
            u.FirstName.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
            u.LastName.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
            u.Email.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase));

        this.Users = new ObservableCollection<UserDto>(filtered);
    }
}