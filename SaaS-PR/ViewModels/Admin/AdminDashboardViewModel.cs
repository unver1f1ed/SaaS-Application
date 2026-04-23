using SaaS_BLL.Interfaces;
using SaaS_Domain.Enums;
using SaaS_PR.Core;

namespace SaaS_PR.ViewModels.Admin;

public class AdminDashboardViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IPaymentService _paymentService;
    private readonly IPlanService _planService;

    private int _totalUsers;
    private int _totalPlans;
    private int _activeSubscriptions;
    private decimal _totalRevenue;
    private bool _isBusy;

    public int TotalUsers
    {
        get => this._totalUsers;
        set => this.SetProperty(ref this._totalUsers, value);
    }

    public int TotalPlans
    {
        get => this._totalPlans;
        set => this.SetProperty(ref this._totalPlans, value);
    }

    public int ActiveSubscriptions
    {
        get => this._activeSubscriptions;
        set => this.SetProperty(ref this._activeSubscriptions, value);
    }

    public decimal TotalRevenue
    {
        get => this._totalRevenue;
        set => this.SetProperty(ref this._totalRevenue, value);
    }

    public bool IsBusy
    {
        get => this._isBusy;
        set => this.SetProperty(ref this._isBusy, value);
    }

    public AsyncRelayCommand LoadCommand { get; }

    public AdminDashboardViewModel(
        IUserService userService,
        ISubscriptionService subscriptionService,
        IPaymentService paymentService,
        IPlanService planService)
    {
        this._userService = userService;
        this._subscriptionService = subscriptionService;
        this._paymentService = paymentService;
        this._planService = planService;

        this.LoadCommand = new AsyncRelayCommand(this.LoadAsync);
    }

    private async Task LoadAsync(object? _)
    {
        this.IsBusy = true;

        var usersResult = await this._userService.GetAllAsync();
        var plansResult = await this._planService.GetAllAsync();
        var subscriptionsResult = await this._subscriptionService.GetAllAsync();

        if (usersResult.Success)
        {
            this.TotalUsers = usersResult.Data!.Count();
        }

        if (plansResult.Success)
        {
            this.TotalPlans = plansResult.Data!.Count();
        }

        if (subscriptionsResult.Success)
        {
            this.ActiveSubscriptions = subscriptionsResult.Data!
                .Count(s => s.State == SubscriptionState.Active || s.State == SubscriptionState.Trial);

            // Calculate total revenue from all subscriptions
            var subscriptionIds = subscriptionsResult.Data!.Select(s => s.Id).ToList();
            decimal totalRevenue = 0;

            foreach (var subId in subscriptionIds)
            {
                var paymentsResult = await this._paymentService.GetBySubscriptionIdAsync(subId);
                if (paymentsResult.Success)
                {
                    totalRevenue += paymentsResult.Data!
                        .Where(p => p.Status == PaymentStatus.Completed)
                        .Sum(p => p.Amount);
                }
            }

            this.TotalRevenue = totalRevenue;
        }

        this.IsBusy = false;
    }
}