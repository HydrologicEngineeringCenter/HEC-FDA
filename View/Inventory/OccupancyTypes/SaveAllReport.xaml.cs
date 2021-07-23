using ViewModel.Inventory.OccupancyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace View.Inventory.OccupancyTypes
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
