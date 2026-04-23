using System.Windows.Controls;
using SaaS_PR.ViewModels.User;

namespace SaaS_PR.Views.User;

public partial class MyPaymentsView : UserControl
{
    public MyPaymentsView()
    {
        this.InitializeComponent();
        this.Loaded += (_, _) => (this.DataContext as MyPaymentsViewModel)?.LoadCommand.Execute(null);
    }
}