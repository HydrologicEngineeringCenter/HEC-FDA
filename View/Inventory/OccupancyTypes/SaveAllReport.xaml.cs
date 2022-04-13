using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.Inventory.OccupancyTypes
{
    /// <summary>
    /// Interaction logic for SaveAllReport.xaml
    /// </summary>
    public partial class SaveAllReport : UserControl
    {
        public SaveAllReport()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(DataContext is SaveAllReportVM vm)
            {
                vm.SaveOcctypes();
            }
            var myWindow = Window.GetWindow(this);
            myWindow.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var myWindow = Window.GetWindow(this);
            myWindow.Close();
        }
    }
}
