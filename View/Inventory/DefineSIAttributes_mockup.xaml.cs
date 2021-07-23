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

namespace View.Inventory
{
    /// <summary>
    /// Interaction logic for DefineSIAttributes_mockup.xaml
    /// </summary>
    public partial class DefineSIAttributes_mockup : UserControl
    {
        public DefineSIAttributes_mockup()
        {
            InitializeComponent();
        }

        private void rad_FirstFloorElevation_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Inventory.DefineSIAttributesVM vm = (ViewModel.Inventory.DefineSIAttributesVM)this.DataContext;
            if (vm.FirstFloorElevationIsChecked == true)
            {
                AttributeDefinitionGrid.RowDefinitions[2].Height = new GridLength(0);
                AttributeDefinitionGrid.RowDefinitions[3].Height = new GridLength(0);
                AttributeDefinitionGrid.RowDefinitions[4].Height = new GridLength(28);

            }


        }

        private void rad_GroundElevationAndFoundationHeight_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Inventory.DefineSIAttributesVM vm = (ViewModel.Inventory.DefineSIAttributesVM)this.DataContext;
            if (vm.GroundElevationIsChecked == true)
            {
                AttributeDefinitionGrid.RowDefinitions[4].Height = new GridLength(0);
                AttributeDefinitionGrid.RowDefinitions[2].Height = new GridLength(28);
                AttributeDefinitionGrid.RowDefinitions[3].Height = new GridLength(28);
            }

        }
    }
}
