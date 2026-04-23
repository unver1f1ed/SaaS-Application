using System.Collections.ObjectModel;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_PR.Core;

namespace SaaS_PR.ViewModels.User;

public class UserDashboardViewModel : ViewModelBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly SessionContext _sessionContext;

    private ObservableCollection<SubscriptionDto> _subscriptions = new();
    private bool _isBusy;
    private string? _errorMessage;

    public ObservableCollection<SubscriptionDto> Subscriptions
    {
        get => this._subscriptions;
        set => this.SetProperty(ref this._subscriptions, value);
    }

    public bool IsBusy { get => this._isBusy; set => this.SetProperty(ref this._isBusy, value); }

    public string? ErrorMessage { get => this._errorMessage; set => this.SetProperty(ref this._errorMessage, value); }

    public bool HasActiveSubscription => this.Subscriptions.Any(s =>
        s.State is SaaS_Domain.Enums.SubscriptionState.Active or SaaS_Domain.Enums.SubscriptionState.Trial);

    public AsyncRelayCommand LoadCommand { get; }

    public UserDashboardViewModel(ISubscriptionService subscriptionService, SessionContext sessionContext)
    {
        this._subscriptionService = subscriptionService;
        this._sessionContext = sessionContext;

        this.LoadCommand = new AsyncRelayCommand(this.LoadAsync);
    }

    private async Task LoadAsync(object? _)
    {
        if (this._sessionContext.CurrentUser is null)
        {
            return;
        }

        this.IsBusy = true;
        this.ErrorMessage = null;

        var result = await this._subscriptionService.GetByUserIdAsync(this._sessionContext.CurrentUser.Id);

        if (result.Success)
        {
            this.Subscriptions = new ObservableCollection<SubscriptionDto>(result.Data!);
            this.OnPropertyChanged(nameof(this.HasActiveSubscription));
        }
        else
        {
            this.ErrorMessage = result.Error;
        }

        this.IsBusy = false;
    }
}