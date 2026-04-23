using System.Collections.ObjectModel;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_Domain.Enums;
using SaaS_PR.Core;

namespace SaaS_PR.ViewModels.Admin;

public class PaymentsViewModel : ViewModelBase
{
    private readonly IPaymentService _paymentService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUserService _userService;

    private ObservableCollection<UserDto> _users = new();
    private ObservableCollection<SubscriptionDto> _subscriptions = new();
    private ObservableCollection<PaymentDto> _payments = new();
    private UserDto? _selectedUser;
    private SubscriptionDto? _selectedSubscription;
    private PaymentDto? _selectedPayment;
    private bool _isBusy;
    private string? _errorMessage;

    // Record payment form
    private decimal _formAmount;
    private Currency _formCurrency;
    private PaymentMethod _formPaymentMethod;

    public ObservableCollection<UserDto> Users { get => this._users; set => this.SetProperty(ref this._users, value); }

    public ObservableCollection<SubscriptionDto> Subscriptions { get => this._subscriptions; set => this.SetProperty(ref this._subscriptions, value); }

    public ObservableCollection<PaymentDto> Payments { get => this._payments; set => this.SetProperty(ref this._payments, value); }

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
        set
        {
            this.SetProperty(ref this._selectedSubscription, value);
            if (value is not null)
            {
                _ = this.LoadPaymentsAsync(value.Id);
            }
        }
    }

    public PaymentDto? SelectedPayment { get => this._selectedPayment; set => this.SetProperty(ref this._selectedPayment, value); }

    public bool IsBusy { get => this._isBusy; set => this.SetProperty(ref this._isBusy, value); }

    public string? ErrorMessage { get => this._errorMessage; set => this.SetProperty(ref this._errorMessage, value); }

    public decimal FormAmount { get => this._formAmount; set => this.SetProperty(ref this._formAmount, value); }

    public Currency FormCurrency { get => this._formCurrency; set => this.SetProperty(ref this._formCurrency, value); }

    public PaymentMethod FormPaymentMethod { get => this._formPaymentMethod; set => this.SetProperty(ref this._formPaymentMethod, value); }

    public IEnumerable<PaymentStatus> PaymentStatuses => Enum.GetValues<PaymentStatus>();

    public IEnumerable<Currency> Currencies => Enum.GetValues<Currency>();

    public IEnumerable<PaymentMethod> PaymentMethods => Enum.GetValues<PaymentMethod>();

    public AsyncRelayCommand LoadUsersCommand { get; }

    public AsyncRelayCommand UpdatePaymentStatusCommand { get; }

    public AsyncRelayCommand RecordPaymentCommand { get; }

    public PaymentsViewModel(IPaymentService paymentService, ISubscriptionService subscriptionService, IUserService userService)
    {
        this._paymentService = paymentService;
        this._subscriptionService = subscriptionService;
        this._userService = userService;

        this.LoadUsersCommand = new AsyncRelayCommand(this.LoadUsersAsync);
        this.UpdatePaymentStatusCommand = new AsyncRelayCommand(this.UpdateStatusAsync, _ => this.SelectedPayment is not null);
        this.RecordPaymentCommand = new AsyncRelayCommand(this.RecordPaymentAsync, _ => this.SelectedSubscription is not null && !this.IsBusy);
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

    private async Task UpdateStatusAsync(object? parameter)
    {
        if (this.SelectedPayment is null || parameter is not PaymentStatus status)
        {
            return;
        }

        var result = await this._paymentService.UpdateStatusAsync(this.SelectedPayment.Id, status);

        if (result.Success && this.SelectedSubscription is not null)
        {
            await this.LoadPaymentsAsync(this.SelectedSubscription.Id);
        }
        else
        {
            this.ErrorMessage = result.Error;
        }
    }

    private async Task RecordPaymentAsync(object? _)
    {
        if (this.SelectedSubscription is null)
        {
            return;
        }

        this.IsBusy = true;
        this.ErrorMessage = null;

        var result = await this._paymentService.RecordPaymentAsync(new RecordPaymentRequest
        {
            SubscriptionId = this.SelectedSubscription.Id,
            Amount = this.FormAmount,
            Currency = this.FormCurrency,
            PaymentMethod = this.FormPaymentMethod
        });

        if (result.Success)
        {
            this.FormAmount = 0;
            await this.LoadPaymentsAsync(this.SelectedSubscription.Id);
        }
        else
        {
            this.ErrorMessage = result.Error;
        }

        this.IsBusy = false;
    }
}