namespace SaaS_PR.Core;

/// <summary>
/// Manages navigation between ViewModels with history support.
/// Uses a factory delegate to resolve ViewModels, keeping IServiceProvider
/// out of application code and avoiding the service locator anti-pattern.
/// </summary>
public class NavigationService : ViewModelBase
{
    private readonly Func<Type, ViewModelBase> _viewModelFactory;
    private ViewModelBase? _currentView;
    private readonly List<ViewModelBase> _history = new();
    private int _historyIndex = -1;

    public ViewModelBase? CurrentView
    {
        get => this._currentView;
        private set => this.SetProperty(ref this._currentView, value);
    }

    public bool CanGoBack => this._historyIndex > 0;

    public bool CanGoForward => this._historyIndex < this._history.Count - 1;

    public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
    {
        this._viewModelFactory = viewModelFactory;
    }

    public void NavigateTo<TViewModel>()
        where TViewModel : ViewModelBase
    {
        var viewModel = this._viewModelFactory(typeof(TViewModel));

        // If we navigated back and then navigate to a new view, truncate forward history
        if (this._historyIndex < this._history.Count - 1)
        {
            this._history.RemoveRange(this._historyIndex + 1, this._history.Count - this._historyIndex - 1);
        }

        // Add to history (avoid duplicates at consecutive positions)
        if (this._history.Count == 0 || this._history[^1].GetType() != typeof(TViewModel))
        {
            this._history.Add(viewModel);
            this._historyIndex++;
        }
        else
        {
            // Replace the current history entry with the new instance
            this._history[this._historyIndex] = viewModel;
        }

        this.CurrentView = viewModel;
        this.OnPropertyChanged(nameof(this.CanGoBack));
        this.OnPropertyChanged(nameof(this.CanGoForward));
    }

    public void GoBack()
    {
        if (!this.CanGoBack)
        {
            return;
        }

        this._historyIndex--;
        this.CurrentView = this._history[this._historyIndex];
        this.OnPropertyChanged(nameof(this.CanGoBack));
        this.OnPropertyChanged(nameof(this.CanGoForward));
    }

    public void GoForward()
    {
        if (!this.CanGoForward)
        {
            return;
        }

        this._historyIndex++;
        this.CurrentView = this._history[this._historyIndex];
        this.OnPropertyChanged(nameof(this.CanGoBack));
        this.OnPropertyChanged(nameof(this.CanGoForward));
    }
}