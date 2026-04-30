using System.Collections.ObjectModel;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Models;
using SaaS_BLL.Models.Requests;
using SaaS_Domain.Enums;
using SaaS_PR.Core;

namespace SaaS_PR.ViewModels.Admin;

public class PlanAddonsViewModel : ViewModelBase
{
    private readonly IPlanAddonService _planAddonService;
    private readonly IPlanService _planService;

    private ObservableCollection<PlanDto> _plans = new();
    private ObservableCollection<PlanAddonDto> _addons = new();
    private List<PlanAddonDto> _allAddons = new();
    private PlanDto? _selectedPlan;
    private PlanAddonDto? _selectedAddon;
    private bool _isBusy;
    private bool _isEditing;
    private string? _errorMessage;
    private string _searchText = string.Empty;
    private BillingType? _selectedBillingTypeFilter;

    // Form fields
    private string _formAddonName = string.Empty;
    private string? _formDescription;
    private decimal _formUnitPrice;
    private Currency _formCurrency;
    private BillingType _formBillingType;

    public ObservableCollection<PlanDto> Plans { get => this._plans; set => this.SetProperty(ref this._plans, value); }

    public ObservableCollection<PlanAddonDto> Addons { get => this._addons; set => this.SetProperty(ref this._addons, value); }

    public PlanDto? SelectedPlan
    {
        get => this._selectedPlan;
        set
        {
            this.SetProperty(ref this._selectedPlan, value);
            if (value is not null)
            {
                _ = this.LoadAddonsAsync(value.Id);
            }
        }
    }

    public PlanAddonDto? SelectedAddon
    {
        get => this._selectedAddon;
        set
        {
            this.SetProperty(ref this._selectedAddon, value);
            if (value is not null)
            {
                this.PopulateForm(value);
            }
        }
    }

    public bool IsBusy { get => this._isBusy; set => this.SetProperty(ref this._isBusy, value); }

    public bool IsEditing { get => this._isEditing; set => this.SetProperty(ref this._isEditing, value); }

    public string? ErrorMessage { get => this._errorMessage; set => this.SetProperty(ref this._errorMessage, value); }

    public string SearchText
    {
        get => this._searchText;
        set
        {
            this.SetProperty(ref this._searchText, value);
            this.FilterAddons();
        }
    }

    public BillingType? SelectedBillingTypeFilter
    {
        get => this._selectedBillingTypeFilter;
        set
        {
            this.SetProperty(ref this._selectedBillingTypeFilter, value);
            this.FilterAddons();
        }
    }

    public IEnumerable<BillingType> BillingTypes => Enum.GetValues<BillingType>();

    public string FormAddonName { get => this._formAddonName; set => this.SetProperty(ref this._formAddonName, value); }

    public string? FormDescription { get => this._formDescription; set => this.SetProperty(ref this._formDescription, value); }

    public decimal FormUnitPrice { get => this._formUnitPrice; set => this.SetProperty(ref this._formUnitPrice, value); }

    public Currency FormCurrency { get => this._formCurrency; set => this.SetProperty(ref this._formCurrency, value); }

    public BillingType FormBillingType { get => this._formBillingType; set => this.SetProperty(ref this._formBillingType, value); }

    public IEnumerable<Currency> Currencies => Enum.GetValues<Currency>();

    public IEnumerable<BillingType> BillingTypes => Enum.GetValues<BillingType>();

    public AsyncRelayCommand LoadPlansCommand { get; }

    public AsyncRelayCommand SaveCommand { get; }

    public AsyncRelayCommand DeleteCommand { get; }

    public RelayCommand NewAddonCommand { get; }

    public RelayCommand CancelEditCommand { get; }

    public PlanAddonsViewModel(IPlanAddonService planAddonService, IPlanService planService)
    {
        this._planAddonService = planAddonService;
        this._planService = planService;

        this.LoadPlansCommand = new AsyncRelayCommand(this.LoadPlansAsync);
        this.SaveCommand = new AsyncRelayCommand(this.SaveAsync, _ => !this.IsBusy && this.SelectedPlan is not null);
        this.DeleteCommand = new AsyncRelayCommand(this.DeleteAsync, _ => this.SelectedAddon is not null);
        this.NewAddonCommand = new RelayCommand(this.StartNew, () => this.SelectedPlan is not null);
        this.CancelEditCommand = new RelayCommand(this.CancelEdit);
    }

    private async Task LoadPlansAsync(object? _)
    {
        this.IsBusy = true;
        var result = await this._planService.GetAllAsync();
        if (result.Success)
        {
            this.Plans = new ObservableCollection<PlanDto>(result.Data!);
        }

        this.IsBusy = false;
    }

    private async Task LoadAddonsAsync(int planId)
    {
        this.IsBusy = true;
        var result = await this._planAddonService.GetByPlanIdAsync(planId);
        if (result.Success)
        {
            this._allAddons = result.Data!.ToList();
            this.Addons = new ObservableCollection<PlanAddonDto>(this._allAddons);
        }

        this.IsBusy = false;
    }

    private void FilterAddons()
    {
        var filtered = this._allAddons.AsEnumerable();

        // Apply text search
        if (!string.IsNullOrWhiteSpace(this.SearchText))
        {
            filtered = filtered.Where(a =>
                a.AddonName.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
                (a.Description?.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        // Apply billing type filter
        if (this.SelectedBillingTypeFilter.HasValue)
        {
            filtered = filtered.Where(a => a.BillingType == this.SelectedBillingTypeFilter.Value);
        }

        this.Addons = new ObservableCollection<PlanAddonDto>(filtered);
    }

    private async Task SaveAsync(object? _)
    {
        if (this.SelectedPlan is null)
        {
            return;
        }

        this.IsBusy = true;
        this.ErrorMessage = null;

        if (this.SelectedAddon is null)
        {
            var result = await this._planAddonService.CreateAsync(new CreatePlanAddonRequest
            {
                PlanId = this.SelectedPlan.Id,
                AddonName = this.FormAddonName,
                Description = this.FormDescription,
                UnitPrice = this.FormUnitPrice,
                Currency = this.FormCurrency,
                BillingType = this.FormBillingType
            });
            if (!result.Success)
            {
                this.ErrorMessage = result.Error;
            }
        }
        else
        {
            var result = await this._planAddonService.UpdateAsync(this.SelectedAddon.Id, new UpdatePlanAddonRequest
            {
                AddonName = this.FormAddonName,
                Description = this.FormDescription,
                UnitPrice = this.FormUnitPrice,
                Currency = this.FormCurrency,
                BillingType = this.FormBillingType
            });
            if (!result.Success)
            {
                this.ErrorMessage = result.Error;
            }
        }

        this.IsBusy = false;
        if (this.ErrorMessage is null)
        {
            this.IsEditing = false;
            await this.LoadAddonsAsync(this.SelectedPlan.Id);
        }
    }

    private async Task DeleteAsync(object? _)
    {
        if (this.SelectedAddon is null || this.SelectedPlan is null)
        {
            return;
        }

        var result = await this._planAddonService.DeleteAsync(this.SelectedAddon.Id);
        if (result.Success)
        {
            await this.LoadAddonsAsync(this.SelectedPlan.Id);
        }
        else
        {
            this.ErrorMessage = result.Error;
        }
    }

    private void StartNew()
    {
        this.SelectedAddon = null;
        this.ClearForm();
        this.IsEditing = true;
    }

    private void CancelEdit()
    {
        this.IsEditing = false;
        this.ClearForm();
    }

    private void PopulateForm(PlanAddonDto addon)
    {
        this.FormAddonName = addon.AddonName;
        this.FormDescription = addon.Description;
        this.FormUnitPrice = addon.UnitPrice;
        this.FormCurrency = addon.Currency;
        this.FormBillingType = addon.BillingType;
        this.IsEditing = true;
    }

    private void ClearForm()
    {
        this.FormAddonName = string.Empty;
        this.FormDescription = null;
        this.FormUnitPrice = 0;
        this.FormCurrency = Currency.USD;
        this.FormBillingType = BillingType.Recurring;
    }
}