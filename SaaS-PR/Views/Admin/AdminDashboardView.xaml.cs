using System.Windows.Controls;
using SaaS_PR.ViewModels.Admin;

namespace SaaS_PR.Views.Admin;

public partial class AdminDashboardView : UserControl
{
    public AdminDashboardView()
    {
        this.InitializeComponent();
        this.Loaded += (_, _) => (this.DataContext as AdminDashboardViewModel)?.LoadCommand.Execute(null);
    }
}