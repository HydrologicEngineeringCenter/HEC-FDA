using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
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

namespace HEC.FDA.View.Inventory.OccupancyTypes.Controls
{
    /// <summary>
    /// Interaction logic for CreateNewDamCat.xaml
    /// </summary>
    public partial class CreateNewDamCat : UserControl
    {
        public CreateNewDamCat( )
        {
            InitializeComponent();          
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txt_NameTextBox.Focus();
        }
    }
}
