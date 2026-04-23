using System.Windows.Controls;
using SaaS_PR.ViewModels.User;

namespace SaaS_PR.Views.User;

public partial class UserDashboardView : UserControl
{
    public UserDashboardView()
    {
        this.InitializeComponent();
        this.Loaded += (_, _) => (this.DataContext as UserDashboardViewModel)?.LoadCommand.Execute(null);
    }
}