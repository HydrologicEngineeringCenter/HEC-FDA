using System.Windows;
using System.Windows.Controls;

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
