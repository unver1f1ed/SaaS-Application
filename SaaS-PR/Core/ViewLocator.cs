using System.Windows;
using System.Windows.Controls;
using SaaS_PR.ViewModels.Admin;
using SaaS_PR.ViewModels.User;

namespace SaaS_PR.Core;

/// <summary>
/// Resolves the correct View for a given ViewModel automatically.
/// Follows the convention: SaaS_PR.ViewModels.X.FooViewModel → SaaS_PR.Views.X.FooView
/// Overrides allow multiple ViewModels to share a single View.
/// </summary>
public class ViewLocator : DataTemplateSelector
{
    /// <summary>
    /// Explicit ViewModel → View type overrides that bypass the naming convention.
    /// Add entries here when multiple ViewModels share the same View.
    /// </summary>
    private static readonly Dictionary<Type, string> _overrides = new ()
    {
        { typeof(AdminProfileViewModel), "SaaS_PR.Views.Shared.ProfileView" },
        { typeof(UserProfileViewModel),  "SaaS_PR.Views.Shared.ProfileView" },
    };

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (item is null)
        {
            return null;
        }

        var viewModelType = item.GetType();

        var viewTypeName = _overrides.TryGetValue(viewModelType, out var overrideName)
            ? overrideName
            : viewModelType.FullName!
                .Replace(".ViewModels.", ".Views.")
                .Replace("ViewModel", "View");

        var viewType = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.FullName == viewTypeName);

        if (viewType is null)
        {
            return null;
        }

        return new DataTemplate
        {
            DataType = viewModelType,
            VisualTree = new FrameworkElementFactory(viewType)
        };
    }
}