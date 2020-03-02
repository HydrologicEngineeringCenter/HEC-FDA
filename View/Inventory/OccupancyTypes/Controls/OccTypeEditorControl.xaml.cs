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

namespace View.Inventory.OccupancyTypes.Controls
{
    /// <summary>
    /// Interaction logic for OccTypeEditorControl.xaml
    /// </summary>
    public partial class OccTypeEditorControl : UserControl
    {

        public event EventHandler ListViewNeedsUpdating;
        public OccTypeEditorControl()
        {
            InitializeComponent();
        }

       

      

        private void DamageCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //redraw the list view so that the occtype that changed dam cats will be in the correct group
            if(this.ListViewNeedsUpdating != null)
            {
                this.ListViewNeedsUpdating(this, new EventArgs());
            }
        }

        private void CreateNewDamCat_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            if (vm == null) { return; }
            vm.LaunchNewDamCatWindow();
            if (this.ListViewNeedsUpdating != null)
            {
                this.ListViewNeedsUpdating(this, new EventArgs());
            }
        }
        private void OccTypeNameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            //if (vm == null) { return; }
            //if (vm.SelectedOccType == null) { return; }
            //vm.UpdateKeyInTabsDictionary(vm.SelectedOccType.Name, OccTypeNameBox.Text);
            //vm.SelectedOccType.Name = OccTypeNameBox.Text;
            //if (this.ListViewNeedsUpdating != null)
            //{
            //    this.ListViewNeedsUpdating(this, new EventArgs());
            //}
        }


        //private void EditStructureDamageButton_Click(object sender, RoutedEventArgs e)
        //{
        //    FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
        //    if (vm == null) { return; }
        //    vm.LaunchDepthDamageEditor();
        //}

        private void StructureValueUncertainty_LostFocus(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            if (vm == null) 
            { 
                return; 
            }
            //StructureValueUncertainty.ReturnDistribution();
            //vm.SelectedOccType.StructureValueUncertainty = 
            //vm.SelectedOccType.StructureValueUncertainty = StructureValueUncertainty.ReturnDistribution();
        }

        private void ContentValueUncertainty_LostFocus(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            if (vm == null) { return; }
            //vm.SelectedOccType.ContentValueUncertainty = ContentValueUncertainty.ReturnDistribution();
        }

        private void VehicleValueUncertainty_LostFocus(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            if (vm == null) { return; }
            //vm.SelectedOccType.VehicleValueUncertainty = VehicleValueUncertainty.ReturnDistribution();
        }

        private void OtherValueUncertainty_LostFocus(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            if (vm == null) { return; }
            //vm.SelectedOccType.OtherValueUncertainty = OtherValueUncertainty.ReturnDistribution();
        }

        private void OccTypeDescriptionBox_LostFocus(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            if (vm == null || vm.SelectedOccType == null) { return; }
            string desc = OccTypeDescriptionBox.Text;
            if(desc == null)
            {
                desc = "";
            }
            vm.SelectedOccType.Description = OccTypeDescriptionBox.Text;
        }

        private void FoundationHeightUncertainty_LostFocus(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            if(vm == null) { return; }
            //vm.SelectedOccType.FoundationHeightUncertainty = FoundationHeightUncertainty.ReturnDistribution();
        }
    }
}
