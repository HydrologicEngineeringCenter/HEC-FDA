using System.Windows;
using System.Windows.Controls;
using HEC.FDA.ViewModel.Tabs;

namespace HEC.FDA.View.Compute;
/// <summary>
/// Interaction logic for StageDamageComputeLogControl.xaml
/// </summary>
public partial class StageDamageComputeLogControl : UserControl
{
    public StageDamageComputeLogControl()
    {
        InitializeComponent();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        TabController.Instance.CloseTabOrWindow(this);
    }
}
