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
using System.Collections.ObjectModel;

namespace Fda.Inventory
{
    /// <summary>
    /// Interaction logic for DefineSIAttributes.xaml
    /// </summary>
    public partial class DefineSIAttributes : UserControl
    {
        


        public DefineSIAttributes()
        {
            InitializeComponent();
            //FdaViewModel.Inventory.DefineSIAttributesVM vm = (FdaViewModel.Inventory.DefineSIAttributesVM)this.DataContext;
            //cmb_Path.CmbSelectionMade += Cmb_Path_CmbSelectionMade;

        }

        //private void Cmb_Path_CmbSelectionMade(string path)
        //{



        //    FdaViewModel.Inventory.DefineSIAttributesVM vm = (FdaViewModel.Inventory.DefineSIAttributesVM)this.DataContext;
            

        //    if (!System.IO.File.Exists(System.IO.Path.ChangeExtension(path, "dbf")))
        //    {
        //        vm.ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("This path has no associated *.dbf file.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Report));
        //        //NextButton.IsEnabled = false;
        //        return;
        //    }
        //    //NextButton.IsEnabled = true;
        //    //NextButton.Content = "Next→";
        //    //PreviousButton.Visibility = Visibility.Hidden;
        //    vm.loadUniqueNames(path);



        //}

        private void rad_FirstFloorElevation_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.DefineSIAttributesVM vm = (FdaViewModel.Inventory.DefineSIAttributesVM)this.DataContext;
            if(vm.FirstFloorElevationIsChecked == true)
            {
                AttributeDefinitionGrid.RowDefinitions[2].Height = new GridLength(0);
                AttributeDefinitionGrid.RowDefinitions[3].Height = new GridLength(0);
                AttributeDefinitionGrid.RowDefinitions[4].Height = new GridLength(28);

            }
            

        }

        private void rad_GroundElevationAndFoundationHeight_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.DefineSIAttributesVM vm = (FdaViewModel.Inventory.DefineSIAttributesVM)this.DataContext;
            if (vm.GroundElevationIsChecked == true)
            {
                AttributeDefinitionGrid.RowDefinitions[4].Height = new GridLength(0);
                AttributeDefinitionGrid.RowDefinitions[2].Height = new GridLength(28);
                AttributeDefinitionGrid.RowDefinitions[3].Height = new GridLength(28);
            }
           
        }
    }
}
