using System.Windows.Controls;
using SaaS_PR.ViewModels.Admin;

namespace SaaS_PR.Views.Admin;

public partial class PlansView : UserControl
{
    public PlansView()
    {
        this.InitializeComponent();
        this.Loaded += (_, _) => (this.DataContext as PlansViewModel)?.LoadCommand.Execute(null);
    }
}