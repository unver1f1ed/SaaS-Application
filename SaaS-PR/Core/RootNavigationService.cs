namespace SaaS_PR.Core;

/// <summary>
/// Handles top-level navigation: Login → AdminShell or UserShell.
/// Bound to MainWindow's ContentControl only.
/// </summary>
public class RootNavigationService : NavigationService
{
    public RootNavigationService(Func<Type, ViewModelBase> viewModelFactory)
        : base(viewModelFactory)
    {
    }
}