using System.Collections.ObjectModel;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_Domain.Enums;
using SaaS_PR.Core;

namespace SaaS_PR.ViewModels.User;

public class BrowsePlansViewModel : ViewModelBase
{
    private readonly IPlanService _planService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly SessionContext _sessionContext;

    private ObservableCollection<PlanDto> _plans = new();
    private PlanDto? _selectedPlan;
    private bool _autoRenew;
    private bool _isBusy;
    private string? _errorMessage;
    private string? _successMessage;

    public ObservableCollection<PlanDto> Plans { get => this._plans; set => this.SetProperty(ref this._plans, value); }

    public PlanDto? SelectedPlan { get => this._selectedPlan; set => this.SetProperty(ref this._selectedPlan, value); }

    public bool AutoRenew { get => this._autoRenew; set => this.SetProperty(ref this._autoRenew, value); }

    public bool IsBusy { get => this._isBusy; set => this.SetProperty(ref this._isBusy, value); }

    public string? ErrorMessage { get => this._errorMessage; set => this.SetProperty(ref this._errorMessage, value); }

    public string? SuccessMessage { get => this._successMessage; set => this.SetProperty(ref this._successMessage, value); }

    public AsyncRelayCommand LoadCommand { get; }

    public AsyncRelayCommand SubscribeCommand { get; }

    public BrowsePlansViewModel(IPlanService planService, ISubscriptionService subscriptionService, SessionContext sessionContext)
    {
        this._planService = planService;
        this._subscriptionService = subscriptionService;
        this._sessionContext = sessionContext;

        this.LoadCommand = new AsyncRelayCommand(this.LoadAsync);
        this.SubscribeCommand = new AsyncRelayCommand(this.SubscribeAsync, _ => !this.IsBusy);
    }

    private async Task LoadAsync(object? _)
    {
        this.IsBusy = true;

        if (this._sessionContext.CurrentUser is null)
        {
            this.IsBusy = false;
            return;
        }

        var plansResult = await this._planService.GetAllAsync();
        var subscriptionsResult = await this._subscriptionService.GetByUserIdAsync(this._sessionContext.CurrentUser.Id);

        if (plansResult.Success && subscriptionsResult.Success)
        {
            // Get IDs of plans the user already has active subscriptions to
            var subscribedPlanIds = subscriptionsResult.Data!
                .Where(s => s.State == SubscriptionState.Active || s.State == SubscriptionState.Trial)
                .Select(s => s.PlanId)
                .ToHashSet();

            // Filter out plans user is already subscribed to
            var availablePlans = plansResult.Data!
                .Where(p => !subscribedPlanIds.Contains(p.Id));

            this.Plans = new ObservableCollection<PlanDto>(availablePlans);
        }

        this.IsBusy = false;
    }

    private async Task SubscribeAsync(object? parameter)
    {
        if (parameter is not PlanDto plan || this._sessionContext.CurrentUser is null)
        {
            return;
        }

        this.IsBusy = true;
        this.ErrorMessage = null;
        this.SuccessMessage = null;

        var result = await this._subscriptionService.SubscribeAsync(new CreateSubscriptionRequest
        {
            UserId = this._sessionContext.CurrentUser.Id,
            PlanId = plan.Id,
            AutoRenew = this.AutoRenew
        });

        if (result.Success)
        {
            this.SuccessMessage = $"Successfully subscribed to {plan.Name}!";
            // Reload plans to filter out the subscribed plan
            await this.LoadAsync(null);
        }
        else
        {
            this.ErrorMessage = result.Error;
        }

        this.IsBusy = false;
    }
}