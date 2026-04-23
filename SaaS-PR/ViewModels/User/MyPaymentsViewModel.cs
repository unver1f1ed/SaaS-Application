using System.Collections.ObjectModel;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_PR.Core;

namespace SaaS_PR.ViewModels.User;

public class MyPaymentsViewModel : ViewModelBase
{
    private readonly IPaymentService _paymentService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly SessionContext _sessionContext;

    private ObservableCollection<SubscriptionDto> _subscriptions = new();
    private ObservableCollection<PaymentDto> _payments = new();
    private SubscriptionDto? _selectedSubscription;
    private bool _isBusy;
    private string? _errorMessage;

    public ObservableCollection<SubscriptionDto> Subscriptions { get => this._subscriptions; set => this.SetProperty(ref this._subscriptions, value); }

    public ObservableCollection<PaymentDto> Payments
    {
        get => this._payments;
        set
        {
            this.SetProperty(ref this._payments, value);
            this.OnPropertyChanged(nameof(this.HasPayments));
        }
    }

    public bool HasPayments => this._payments.Count > 0;

    public SubscriptionDto? SelectedSubscription
    {
        get => this._selectedSubscription;
        set
        {
            this.SetProperty(ref this._selectedSubscription, value);
            if (value is not null)
            {
                _ = this.LoadPaymentsAsync(value.Id);
            }
        }
    }

    public bool IsBusy { get => this._isBusy; set => this.SetProperty(ref this._isBusy, value); }

    public string? ErrorMessage { get => this._errorMessage; set => this.SetProperty(ref this._errorMessage, value); }

    public AsyncRelayCommand LoadCommand { get; }

    public MyPaymentsViewModel(IPaymentService paymentService, ISubscriptionService subscriptionService, SessionContext sessionContext)
    {
        this._paymentService = paymentService;
        this._subscriptionService = subscriptionService;
        this._sessionContext = sessionContext;

        this.LoadCommand = new AsyncRelayCommand(this.LoadAsync);

        _ = this.LoadAsync(null);
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

    private async Task LoadPaymentsAsync(int subscriptionId)
    {
        this.IsBusy = true;
        var result = await this._paymentService.GetBySubscriptionIdAsync(subscriptionId);
        if (result.Success)
        {
            this.Payments = new ObservableCollection<PaymentDto>(result.Data!);
        }

        this.IsBusy = false;
    }
}