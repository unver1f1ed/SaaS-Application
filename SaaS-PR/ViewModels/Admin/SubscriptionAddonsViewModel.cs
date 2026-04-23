using System.Collections.ObjectModel;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_PR.Core;

namespace SaaS_PR.ViewModels.Admin;

public class SubscriptionAddonsViewModel : ViewModelBase
{
    private readonly ISubscriptionAddonService _subscriptionAddonService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUserService _userService;
    private readonly IPlanAddonService _planAddonService;

    private ObservableCollection<UserDto> _users = new();
    private ObservableCollection<SubscriptionDto> _subscriptions = new();
    private ObservableCollection<SubscriptionAddonDto> _currentAddons = new();
    private ObservableCollection<PlanAddonDto> _availableAddons = new();
    private UserDto? _selectedUser;
    private SubscriptionDto? _selectedSubscription;
    private PlanAddonDto? _selectedAvailableAddon;
    private int _formQuantity = 1;
    private bool _isBusy;
    private string? _errorMessage;

    public ObservableCollection<UserDto> Users { get => this._users; set => this.SetProperty(ref this._users, value); }

    public ObservableCollection<SubscriptionDto> Subscriptions { get => this._subscriptions; set => this.SetProperty(ref this._subscriptions, value); }

    public ObservableCollection<SubscriptionAddonDto> CurrentAddons { get => this._currentAddons; set => this.SetProperty(ref this._currentAddons, value); }

    public ObservableCollection<PlanAddonDto> AvailableAddons { get => this._availableAddons; set => this.SetProperty(ref this._availableAddons, value); }

    public UserDto? SelectedUser
    {
        get => this._selectedUser;
        set
        {
            this.SetProperty(ref this._selectedUser, value);
            this.Subscriptions = new ObservableCollection<SubscriptionDto>();
            this.CurrentAddons = new ObservableCollection<SubscriptionAddonDto>();
            this.AvailableAddons = new ObservableCollection<PlanAddonDto>();
            if (value is not null)
            {
                _ = this.LoadSubscriptionsAsync(value.Id);
            }
        }
    }

    public SubscriptionDto? SelectedSubscription
    {
        get => this._selectedSubscription;
        set
        {
            this.SetProperty(ref this._selectedSubscription, value);
            this.CurrentAddons = new ObservableCollection<SubscriptionAddonDto>();
            this.AvailableAddons = new ObservableCollection<PlanAddonDto>();
            if (value is not null)
            {
                _ = this.LoadSubscriptionDetailsAsync(value);
            }
        }
    }

    public PlanAddonDto? SelectedAvailableAddon { get => this._selectedAvailableAddon; set => this.SetProperty(ref this._selectedAvailableAddon, value); }

    public int FormQuantity { get => this._formQuantity; set => this.SetProperty(ref this._formQuantity, value); }

    public bool IsBusy { get => this._isBusy; set => this.SetProperty(ref this._isBusy, value); }

    public string? ErrorMessage { get => this._errorMessage; set => this.SetProperty(ref this._errorMessage, value); }

    public AsyncRelayCommand LoadUsersCommand { get; }

    public AsyncRelayCommand AddAddonCommand { get; }

    public AsyncRelayCommand RemoveAddonCommand { get; }

    public SubscriptionAddonsViewModel(
        ISubscriptionAddonService subscriptionAddonService,
        ISubscriptionService subscriptionService,
        IUserService userService,
        IPlanAddonService planAddonService)
    {
        this._subscriptionAddonService = subscriptionAddonService;
        this._subscriptionService = subscriptionService;
        this._userService = userService;
        this._planAddonService = planAddonService;

        this.LoadUsersCommand = new AsyncRelayCommand(this.LoadUsersAsync);
        this.AddAddonCommand = new AsyncRelayCommand(
            this.AddAddonAsync,
            _ => this.SelectedSubscription is not null && this.SelectedAvailableAddon is not null && !this.IsBusy);
        this.RemoveAddonCommand = new AsyncRelayCommand(
            this.RemoveAddonAsync,
            _ => this.SelectedSubscription is not null);

        _ = this.LoadUsersAsync(null);
    }

    private async Task LoadUsersAsync(object? _)
    {
        this.IsBusy = true;
        var result = await this._userService.GetAllAsync();
        if (result.Success)
        {
            this.Users = new ObservableCollection<UserDto>(result.Data!);
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

    private async Task LoadSubscriptionDetailsAsync(SubscriptionDto subscription)
    {
        this.IsBusy = true;

        var currentResult = await this._subscriptionAddonService.GetBySubscriptionIdAsync(subscription.Id);
        if (currentResult.Success)
        {
            this.CurrentAddons = new ObservableCollection<SubscriptionAddonDto>(currentResult.Data!);
        }

        var availableResult = await this._planAddonService.GetByPlanIdAsync(subscription.PlanId);
        if (availableResult.Success)
        {
            var currentIds = this.CurrentAddons.Select(a => a.PlanAddonId).ToHashSet();
            this.AvailableAddons = new ObservableCollection<PlanAddonDto>(
                availableResult.Data!.Where(a => !currentIds.Contains(a.Id)));
        }

        this.IsBusy = false;
    }

    private async Task AddAddonAsync(object? _)
    {
        if (this.SelectedSubscription is null || this.SelectedAvailableAddon is null)
        {
            return;
        }

        this.IsBusy = true;
        this.ErrorMessage = null;

        var result = await this._subscriptionAddonService.AddAddonAsync(new AddSubscriptionAddonRequest
        {
            SubscriptionId = this.SelectedSubscription.Id,
            PlanAddonId = this.SelectedAvailableAddon.Id,
            Quantity = this.FormQuantity
        });

        if (result.Success)
        {
            this.FormQuantity = 1;
            this.SelectedAvailableAddon = null;
            await this.LoadSubscriptionDetailsAsync(this.SelectedSubscription);
        }
        else
        {
            this.ErrorMessage = result.Error;
        }

        this.IsBusy = false;
    }

    private async Task RemoveAddonAsync(object? parameter)
    {
        if (parameter is not SubscriptionAddonDto addon || this.SelectedSubscription is null)
        {
            return;
        }

        this.IsBusy = true;
        this.ErrorMessage = null;

        var result = await this._subscriptionAddonService.RemoveAddonAsync(addon.Id);
        if (result.Success)
        {
            await this.LoadSubscriptionDetailsAsync(this.SelectedSubscription);
        }
        else
        {
            this.ErrorMessage = result.Error;
        }

        this.IsBusy = false;
    }
}