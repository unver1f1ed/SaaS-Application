using System.Windows.Controls;
using SaaS_PR.ViewModels.User;

namespace SaaS_PR.Views.User;

public partial class BrowsePlansView : UserControl
{
    public BrowsePlansView()
    {
        this.InitializeComponent();
        this.Loaded += (_, _) => (this.DataContext as BrowsePlansViewModel)?.LoadCommand.Execute(null);
    }
}