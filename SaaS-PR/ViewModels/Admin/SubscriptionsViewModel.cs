using System.Collections.ObjectModel;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_PR.Core;

namespace SaaS_PR.ViewModels.Admin;

public class SubscriptionsViewModel : ViewModelBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUserService _userService;
    private readonly IPlanService _planService;

    private ObservableCollection<UserDto> _users = new();
    private ObservableCollection<SubscriptionDto> _subscriptions = new();
    private ObservableCollection<PlanDto> _plans = new();
    private UserDto? _selectedUser;
    private SubscriptionDto? _selectedSubscription;
    private PlanDto? _selectedPlan;
    private bool _autoRenew;
    private bool _isBusy;
    private string? _errorMessage;
    private string? _successMessage;
    private string _cancellationReason = string.Empty;

    public ObservableCollection<UserDto> Users { get => this._users; set => this.SetProperty(ref this._users, value); }

    public ObservableCollection<SubscriptionDto> Subscriptions { get => this._subscriptions; set => this.SetProperty(ref this._subscriptions, value); }

    public ObservableCollection<PlanDto> Plans { get => this._plans; set => this.SetProperty(ref this._plans, value); }

    public UserDto? SelectedUser
    {
        get => this._selectedUser;
        set
        {
            this.SetProperty(ref this._selectedUser, value);
            if (value is not null)
            {
                _ = this.LoadSubscriptionsAsync(value.Id);
            }
        }
    }

    public SubscriptionDto? SelectedSubscription
    {
        get => this._selectedSubscription;
        set => this.SetProperty(ref this._selectedSubscription, value);
    }

    public PlanDto? SelectedPlan { get => this._selectedPlan; set => this.SetProperty(ref this._selectedPlan, value); }

    public bool AutoRenew { get => this._autoRenew; set => this.SetProperty(ref this._autoRenew, value); }

    public bool IsBusy { get => this._isBusy; set => this.SetProperty(ref this._isBusy, value); }

    public string? ErrorMessage { get => this._errorMessage; set => this.SetProperty(ref this._errorMessage, value); }

    public string? SuccessMessage { get => this._successMessage; set => this.SetProperty(ref this._successMessage, value); }

    public string CancellationReason { get => this._cancellationReason; set => this.SetProperty(ref this._cancellationReason, value); }

    public AsyncRelayCommand LoadUsersCommand { get; }

    public AsyncRelayCommand CreateSubscriptionCommand { get; }

    public AsyncRelayCommand CancelSubscriptionCommand { get; }

    public AsyncRelayCommand RenewSubscriptionCommand { get; }

    public AsyncRelayCommand DeleteSubscriptionCommand { get; }

    public SubscriptionsViewModel(ISubscriptionService subscriptionService, IUserService userService, IPlanService planService)
    {
        this._subscriptionService = subscriptionService;
        this._userService = userService;
        this._planService = planService;

        this.LoadUsersCommand = new AsyncRelayCommand(this.LoadUsersAsync);
        this.CreateSubscriptionCommand = new AsyncRelayCommand(this.CreateSubscriptionAsync, _ => this.SelectedUser is not null && this.SelectedPlan is not null && !this.IsBusy);
        this.CancelSubscriptionCommand = new AsyncRelayCommand(this.CancelSubscriptionAsync, _ => this.SelectedSubscription is not null);
        this.RenewSubscriptionCommand = new AsyncRelayCommand(this.RenewSubscriptionAsync, _ => this.SelectedSubscription is not null);
        this.DeleteSubscriptionCommand = new AsyncRelayCommand(this.DeleteSubscriptionAsync, _ => this.SelectedSubscription is not null);
    }

    private async Task LoadUsersAsync(object? _)
    {
        this.IsBusy = true;

        var usersResult = await this._userService.GetAllAsync();
        if (usersResult.Success)
        {
            this.Users = new ObservableCollection<UserDto>(usersResult.Data!);
        }

        var plansResult = await this._planService.GetAllAsync();
        if (plansResult.Success)
        {
            this.Plans = new ObservableCollection<PlanDto>(plansResult.Data!);
        }

        this.IsBusy = false;
    }

    private async Task LoadSubscriptionsAsync(int userId)
    {
        this.IsBusy = true;
        var result = await this._subscriptionService.GetByUserIdAsync(userId);
        if (result.Success)
        {
            this.Subscriptions = new ObservableCollection<SubscriptionDto>(result.Data!);
        }

        this.IsBusy = false;
    }

    private async Task CreateSubscriptionAsync(object? _)
    {
        if (this.SelectedUser is null || this.SelectedPlan is null)
        {
            return;
        }

        this.IsBusy = true;
        this.ErrorMessage = null;
        this.SuccessMessage = null;

        var result = await this._subscriptionService.SubscribeAsync(new CreateSubscriptionRequest
        {
            UserId = this.SelectedUser.Id,
            PlanId = this.SelectedPlan.Id,
            AutoRenew = this.AutoRenew
        });

        if (result.Success)
        {
            this.SuccessMessage = $"Successfully created subscription to {this.SelectedPlan.Name} for {this.SelectedUser.FirstName} {this.SelectedUser.LastName}";
            await this.LoadSubscriptionsAsync(this.SelectedUser.Id);
            this.SelectedPlan = null;
            this.AutoRenew = false;
        }
        else
        {
            this.ErrorMessage = result.Error;
        }

        this.IsBusy = false;
    }

    private async Task CancelSubscriptionAsync(object? _)
    {
        if (this.SelectedSubscription is null)
        {
            return;
        }

        var result = await this._subscriptionService.CancelAsync(
            this.SelectedSubscription.Id,
            new CancelSubscriptionRequest { CancellationReason = this.CancellationReason });

        if (result.Success && this.SelectedUser is not null)
        {
            await this.LoadSubscriptionsAsync(this.SelectedUser.Id);
        }
        else
        {
            this.ErrorMessage = result.Error;
        }
    }

    private async Task RenewSubscriptionAsync(object? _)
    {
        if (this.SelectedSubscription is null)
        {
            return;
        }

        var result = await this._subscriptionService.RenewAsync(this.SelectedSubscription.Id);

        if (result.Success && this.SelectedUser is not null)
        {
            await this.LoadSubscriptionsAsync(this.SelectedUser.Id);
        }
        else
        {
            this.ErrorMessage = result.Error;
        }
    }

    private async Task DeleteSubscriptionAsync(object? _)
    {
        if (this.SelectedSubscription is null)
        {
            return;
        }

        this.IsBusy = true;
        this.ErrorMessage = null;
        this.SuccessMessage = null;

        var result = await this._subscriptionService.DeleteAsync(this.SelectedSubscription.Id);

        if (result.Success && this.SelectedUser is not null)
        {
            this.SuccessMessage = "Subscription deleted successfully";
            await this.LoadSubscriptionsAsync(this.SelectedUser.Id);
            this.SelectedSubscription = null;
        }
        else
        {
            this.ErrorMessage = result.Error;
        }

        this.IsBusy = false;
    }
}