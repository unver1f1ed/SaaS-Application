using System.Windows.Controls;
using SaaS_PR.ViewModels.Admin;

namespace SaaS_PR.Views.Admin;

public partial class PlanAddonsView : UserControl
{
    public PlanAddonsView()
    {
        this.InitializeComponent();
        this.Loaded += (_, _) => (this.DataContext as PlanAddonsViewModel)?.LoadPlansCommand.Execute(null);
    }
}