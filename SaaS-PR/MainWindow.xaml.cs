using SaaS_PR.Core;

namespace SaaS_PR;

public partial class MainWindow
{
    public ViewLocator ViewLocator { get; }

    public MainWindow(RootNavigationService rootNav, ViewLocator viewLocator)
    {
        this.InitializeComponent();
        this.DataContext = rootNav;
        this.ViewLocator = viewLocator;
    }
}