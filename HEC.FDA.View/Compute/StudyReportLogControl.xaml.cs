using System.Windows;
using System.Windows.Controls;
using HEC.FDA.ViewModel.Compute;
using HEC.FDA.ViewModel.Tabs;

namespace HEC.FDA.View.Compute;

public partial class StudyReportLogControl : UserControl
{
    public StudyReportLogControl() => InitializeComponent();
    private void Cancel_Click(object sender, RoutedEventArgs e) => (DataContext as StudyReportLogVM)?.Cancel();
    private void Close_Click(object sender, RoutedEventArgs e) => TabController.Instance.CloseTabOrWindow(this);
}
