using System.Windows.Controls;
using SaaS_PR.ViewModels.Admin;

namespace SaaS_PR.Views.Admin;

public partial class SubscriptionsView : UserControl
{
    public SubscriptionsView()
    {
        this.InitializeComponent();
        this.Loaded += (_, _) => (this.DataContext as SubscriptionsViewModel)?.LoadUsersCommand.Execute(null);
    }
}