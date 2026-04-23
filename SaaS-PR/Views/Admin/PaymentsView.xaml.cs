using System.Windows.Controls;
using SaaS_Domain.Enums;
using SaaS_PR.ViewModels.Admin;

namespace SaaS_PR.Views.Admin;

public partial class PaymentsView : UserControl
{
    public PaymentsView()
    {
        this.InitializeComponent();
        this.Loaded += (_, _) => (this.DataContext as PaymentsViewModel)?.LoadUsersCommand.Execute(null);
    }

    // Status change inside DataGrid row requires code-behind since
    // ComboBox.SelectionChanged passes the new value as CommandParameter
    private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox combo)
        {
            return;
        }

        if (combo.SelectedItem is not PaymentStatus status)
        {
            return;
        }

        if (this.DataContext is not PaymentsViewModel vm)
        {
            return;
        }

        vm.UpdatePaymentStatusCommand.Execute(status);
    }
}