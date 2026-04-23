using System.Collections.ObjectModel;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_PR.Core;

namespace SaaS_PR.ViewModels.User;

public class MySubscriptionViewModel : ViewModelBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly ISubscriptionAddonService _subscriptionAddonService;
    private readonly IPlanAddonService _planAddonService;
    private readonly SessionContext _sessionContext;

    private ObservableCollection<SubscriptionDto> _subscriptions = new();
    private ObservableCollection<SubscriptionAddonDto> _activeAddons = new();
    private ObservableCollection<PlanAddonDto> _availableAddons = new();
    private SubscriptionDto? _selectedSubscription;
    private PlanAddonDto? _selectedAvailableAddon;
    private bool _isBusy;
    private string? _errorMessage;
    private string? _cancellationReason;

    public ObservableCollection<SubscriptionDto> Subscriptions { get => this._subscriptions; set => this.SetProperty(ref this._subscriptions, value); }

    public ObservableCollection<SubscriptionAddonDto> ActiveAddons { get => this._activeAddons; set => this.SetProperty(ref this._activeAddons, value); }

    public ObservableCollection<PlanAddonDto> AvailableAddons { get => this._availableAddons; set => this.SetProperty(ref this._availableAddons, value); }

    public SubscriptionDto? SelectedSubscription
    {
        get => this._selectedSubscription;
        set
        {
            this.SetProperty(ref this._selectedSubscription, value);
            if (value is not null)
            {
                _ = this.LoadSubscriptionDetailsAsync(value);
            }
        }
    }

    public PlanAddonDto? SelectedAvailableAddon { get => this._selectedAvailableAddon; set => this.SetProperty(ref this._selectedAvailableAddon, value); }

    public bool IsBusy { get => this._isBusy; set => this.SetProperty(ref this._isBusy, value); }

    public string? ErrorMessage { get => this._errorMessage; set => this.SetProperty(ref this._errorMessage, value); }

    public string? CancellationReason { get => this._cancellationReason; set => this.SetProperty(ref this._cancellationReason, value); }

    public AsyncRelayCommand LoadCommand { get; }

    public AsyncRelayCommand CancelSubscriptionCommand { get; }

    public AsyncRelayCommand AddAddonCommand { get; }

    public AsyncRelayCommand RemoveAddonCommand { get; }

    public MySubscriptionViewModel(
        ISubscriptionService subscriptionService,
        ISubscriptionAddonService subscriptionAddonService,
        IPlanAddonService planAddonService,
        SessionContext sessionContext)
    {
        this._subscriptionService = subscriptionService;
        this._subscriptionAddonService = subscriptionAddonService;
        this._planAddonService = planAddonService;
        this._sessionContext = sessionContext;

        this.LoadCommand = new AsyncRelayCommand(this.LoadAsync);
        this.CancelSubscriptionCommand = new AsyncRelayCommand(this.CancelAsync, _ => this.SelectedSubscription is not null);
        this.AddAddonCommand = new AsyncRelayCommand(this.AddAddonAsync, _ => this.SelectedSubscription is not null && this.SelectedAvailableAddon is not null);
        this.RemoveAddonCommand = new AsyncRelayCommand(this.RemoveAddonAsync, _ => this.SelectedSubscription is not null);
    }

    private async Task LoadAsync(object? _)
    {
        if (this._sessionContext.CurrentUser is null)
        {
            return;
        }

        this.IsBusy = true;
        var result = await this._subscriptionService.GetByUserIdAsync(this._sessionContext.CurrentUser.Id);
        if (result.Success)
        {
            this.Subscriptions = new ObservableCollection<SubscriptionDto>(result.Data!);
        }

        this.IsBusy = false;
    }

    private async Task LoadSubscriptionDetailsAsync(SubscriptionDto subscription)
    {
        this.IsBusy = true;

        var addonsResult = await this._subscriptionAddonService.GetBySubscriptionIdAsync(subscription.Id);
        if (addonsResult.Success)
        {
            this.ActiveAddons = new ObservableCollection<SubscriptionAddonDto>(addonsResult.Data!);
        }

        var availableResult = await this._planAddonService.GetByPlanIdAsync(subscription.PlanId);
        if (availableResult.Success)
        {
            this.AvailableAddons = new ObservableCollection<PlanAddonDto>(availableResult.Data!);
        }

        this.IsBusy = false;
    }

    private async Task CancelAsync(object? _)
    {
        if (this.SelectedSubscription is null)
        {
            return;
        }

        var result = await this._subscriptionService.CancelAsync(
            this.SelectedSubscription.Id,
            new CancelSubscriptionRequest { CancellationReason = this.CancellationReason });

        if (result.Success)
        {
            await this.LoadAsync(null);
        }
        else
        {
            this.ErrorMessage = result.Error;
        }
    }

    private async Task AddAddonAsync(object? _)
    {
        if (this.SelectedSubscription is null || this.SelectedAvailableAddon is null)
        {
            return;
        }

        var result = await this._subscriptionAddonService.AddAddonAsync(new AddSubscriptionAddonRequest
        {
            SubscriptionId = this.SelectedSubscription.Id,
            PlanAddonId = this.SelectedAvailableAddon.Id,
            Quantity = 1
        });

        if (result.Success)
        {
            await this.LoadSubscriptionDetailsAsync(this.SelectedSubscription);
        }
        else
        {
            this.ErrorMessage = result.Error;
        }
    }

    private async Task RemoveAddonAsync(object? parameter)
    {
        if (parameter is not SubscriptionAddonDto addon)
        {
            return;
        }

        var result = await this._subscriptionAddonService.RemoveAddonAsync(addon.Id);

        if (result.Success && this.SelectedSubscription is not null)
        {
            await this.LoadSubscriptionDetailsAsync(this.SelectedSubscription);
        }
        else
        {
            this.ErrorMessage = result.Error;
        }
    }
}