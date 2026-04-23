namespace SaaS_PR.Core;

/// <summary>
/// Manages navigation between ViewModels.
/// Uses a factory delegate to resolve ViewModels, keeping IServiceProvider
/// out of application code and avoiding the service locator anti-pattern.
/// </summary>
public class NavigationService : ViewModelBase
{
    private readonly Func<Type, ViewModelBase> _viewModelFactory;
    private ViewModelBase? _currentView;

    public ViewModelBase? CurrentView
    {
        get => this._currentView;
        private set => this.SetProperty(ref this._currentView, value);
    }

    public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
    {
        this._viewModelFactory = viewModelFactory;
    }

    public void NavigateTo<TViewModel>()
        where TViewModel : ViewModelBase
    {
        this.CurrentView = this._viewModelFactory(typeof(TViewModel));
    }
}