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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            data_grid.Columns[2].CellStyle = this.FindResource("RedCellStyle") as Style;

        }

       
    }
}
