using System.Windows.Controls;
using SaaS_PR.ViewModels.User;

namespace SaaS_PR.Views.User;

public partial class MySubscriptionView : UserControl
{
    public MySubscriptionView()
    {
        this.InitializeComponent();
        this.Loaded += (_, _) => (this.DataContext as MySubscriptionViewModel)?.LoadCommand.Execute(null);
    }
}