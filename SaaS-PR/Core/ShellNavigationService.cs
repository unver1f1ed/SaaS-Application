namespace SaaS_PR.Core;

/// <summary>
/// Handles navigation between feature ViewModels inside the shell content area.
/// Kept separate from RootNavigationService to avoid circular rendering
/// where the shell would render itself as its own content.
/// </summary>
public class ShellNavigationService : NavigationService
{
    public ShellNavigationService(Func<Type, ViewModelBase> viewModelFactory)
        : base(viewModelFactory)
    {
    }
}