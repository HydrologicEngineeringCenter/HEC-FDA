using HEC.FDA.ViewModel.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Utilities
{
    /// <summary>
    /// Interaction logic for TabHeaderControl.xaml
    /// </summary>
    public partial class TabHeaderControl : UserControl
    {
        public TabHeaderControl()
        {
            InitializeComponent();
        }

        private void btn_PopOut_Click(object sender, RoutedEventArgs e)
        {
            DynamicTabVM vm = (DynamicTabVM)this.DataContext;
            vm.PopTabIntoWindow();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            DynamicTabVM vm = (DynamicTabVM)this.DataContext;
            vm.RemoveTab();
        }

    }
}
