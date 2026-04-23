using System.Collections.ObjectModel;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_Domain.Enums;
using SaaS_PR.Core;

namespace SaaS_PR.ViewModels.Admin;

public class PlansViewModel : ViewModelBase
{
    private readonly IPlanService _planService;

    private ObservableCollection<PlanDto> _plans = new();
    private PlanDto? _selectedPlan;
    private string _searchText = string.Empty;
    private bool _isBusy;
    private bool _isEditing;
    private string? _errorMessage;

    private List<PlanDto> _allPlans = new();

    // Form fields
    private string _formName = string.Empty;
    private string? _formDescription;
    private decimal _formBasePrice;
    private PlanType _formPlanType;
    private Currency _formCurrency;
    private BillingInterval _formBillingInterval;
    private int _formTrialDays;

    public ObservableCollection<PlanDto> Plans { get => this._plans; set => this.SetProperty(ref this._plans, value); }

    public PlanDto? SelectedPlan
    {
        get => this._selectedPlan;
        set
        {
            this.SetProperty(ref this._selectedPlan, value);
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
            this.FilterPlans();
        }
    }

    public bool IsBusy { get => this._isBusy; set => this.SetProperty(ref this._isBusy, value); }

    public bool IsEditing { get => this._isEditing; set => this.SetProperty(ref this._isEditing, value); }

    public string? ErrorMessage { get => this._errorMessage; set => this.SetProperty(ref this._errorMessage, value); }

    public string FormName { get => this._formName; set => this.SetProperty(ref this._formName, value); }

    public string? FormDescription { get => this._formDescription; set => this.SetProperty(ref this._formDescription, value); }

    public decimal FormBasePrice { get => this._formBasePrice; set => this.SetProperty(ref this._formBasePrice, value); }

    public PlanType FormPlanType { get => this._formPlanType; set => this.SetProperty(ref this._formPlanType, value); }

    public Currency FormCurrency { get => this._formCurrency; set => this.SetProperty(ref this._formCurrency, value); }

    public BillingInterval FormBillingInterval { get => this._formBillingInterval; set => this.SetProperty(ref this._formBillingInterval, value); }

    public int FormTrialDays { get => this._formTrialDays; set => this.SetProperty(ref this._formTrialDays, value); }

    public IEnumerable<PlanType> PlanTypes => Enum.GetValues<PlanType>();

    public IEnumerable<Currency> Currencies => Enum.GetValues<Currency>();

    public IEnumerable<BillingInterval> BillingIntervals => Enum.GetValues<BillingInterval>();

    public AsyncRelayCommand LoadCommand { get; }

    public AsyncRelayCommand SaveCommand { get; }

    public AsyncRelayCommand DeleteCommand { get; }

    public RelayCommand NewPlanCommand { get; }

    public RelayCommand CancelEditCommand { get; }

    public PlansViewModel(IPlanService planService)
    {
        this._planService = planService;

        this.LoadCommand = new AsyncRelayCommand(this.LoadAsync);
        this.SaveCommand = new AsyncRelayCommand(this.SaveAsync, _ => !this.IsBusy);
        this.DeleteCommand = new AsyncRelayCommand(this.DeleteAsync, _ => this.SelectedPlan is not null);
        this.NewPlanCommand = new RelayCommand(this.StartNewPlan);
        this.CancelEditCommand = new RelayCommand(this.CancelEdit);
    }

    private async Task LoadAsync(object? _)
    {
        this.IsBusy = true;
        this.ErrorMessage = null;

        var result = await this._planService.GetAllAsync();
        if (result.Success)
        {
            this._allPlans = result.Data!.ToList();
            this.Plans = new ObservableCollection<PlanDto>(this._allPlans);
        }
        else
        {
            this.ErrorMessage = result.Error;
        }

        this.IsBusy = false;
    }

    private void FilterPlans()
    {
        if (string.IsNullOrWhiteSpace(this.SearchText))
        {
            this.Plans = new ObservableCollection<PlanDto>(this._allPlans);
            return;
        }

        var filtered = this._allPlans.Where(p =>
            p.Name.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
            p.PlanType.ToString().Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
            (p.Description?.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ?? false));

        this.Plans = new ObservableCollection<PlanDto>(filtered);
    }

    private async Task SaveAsync(object? _)
    {
        this.IsBusy = true;
        this.ErrorMessage = null;

        if (this.SelectedPlan is null)
        {
            var result = await this._planService.CreateAsync(new CreatePlanRequest
            {
                Name = this.FormName,
                Description = this.FormDescription,
                BasePrice = this.FormBasePrice,
                PlanType = this.FormPlanType,
                Currency = this.FormCurrency,
                BillingInterval = this.FormBillingInterval,
                TrialDays = this.FormTrialDays
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
            var result = await this._planService.UpdateAsync(this.SelectedPlan.Id, new UpdatePlanRequest
            {
                Name = this.FormName,
                Description = this.FormDescription,
                BasePrice = this.FormBasePrice,
                PlanType = this.FormPlanType,
                Currency = this.FormCurrency,
                BillingInterval = this.FormBillingInterval,
                TrialDays = this.FormTrialDays
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

    private async Task DeleteAsync(object? _)
    {
        if (this.SelectedPlan is null)
        {
            return;
        }

        var result = await this._planService.DeleteAsync(this.SelectedPlan.Id);
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

    private void StartNewPlan()
    {
        this.SelectedPlan = null;
        this.ClearForm();
        this.IsEditing = true;
    }

    private void CancelEdit()
    {
        this.IsEditing = false;
        this.ClearForm();
    }

    private void PopulateForm(PlanDto plan)
    {
        this.FormName = plan.Name;
        this.FormDescription = plan.Description;
        this.FormBasePrice = plan.BasePrice;
        this.FormPlanType = plan.PlanType;
        this.FormCurrency = plan.Currency;
        this.FormBillingInterval = plan.BillingInterval;
        this.FormTrialDays = plan.TrialDays;
        this.IsEditing = true;
    }

    private void ClearForm()
    {
        this.FormName = string.Empty;
        this.FormDescription = null;
        this.FormBasePrice = 0;
        this.FormPlanType = PlanType.Basic;
        this.FormCurrency = Currency.USD;
        this.FormBillingInterval = BillingInterval.Monthly;
        this.FormTrialDays = 0;
    }
}