using System.Windows.Controls;
using SaaS_PR.ViewModels.Admin;

namespace SaaS_PR.Views.Admin;

public partial class UsersView : UserControl
{
    public UsersView()
    {
        this.InitializeComponent();
        this.Loaded += (_, _) => (this.DataContext as UsersViewModel)?.LoadCommand.Execute(null);
    }
}