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

namespace Fda.Inventory.OccupancyTypes
{
    /// <summary>
    /// Interaction logic for OccupancyTypesEditor.xaml
    /// </summary>
    public partial class OccupancyTypesEditor : UserControl
    {
        public OccupancyTypesEditor()
        {
            InitializeComponent();

            OccTypeEditorControl.ListViewNeedsUpdating += new EventHandler(UpdateTheListView);

            
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // I wanted the editor to open up with a group and occtype selected. This gets the first group and the first occtype.
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
           // if (vm.OccTypeGroups.Count > 0)
           // {
                //if (vm.OccTypeGroups.Count > 0)
                //{
                //    vm.SelectedOccTypeGroup = vm.OccTypeGroups[0];

                //    if (vm.SelectedOccTypeGroup.ListOfOccupancyTypes.Count > 0)
                //    {
                //        vm.SelectedOccType = vm.SelectedOccTypeGroup.ListOfOccupancyTypes[0];

                //        cmb_Group.SelectedItem = vm.SelectedOccTypeGroup;
                //        OccTypeListView.SelectedItem = vm.SelectedOccType;
                //    }

                //}
                
            //}
        }

        public void UpdateTheListView(object sender, EventArgs e)
        {
            //load the list view
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            if (vm.SelectedOccTypeGroup == null) { return; }

            ObservableCollection<Consequences_Assist.ComputableObjects.OccupancyType> collectionOfOccTypes = new ObservableCollection<Consequences_Assist.ComputableObjects.OccupancyType>();
            foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in vm.SelectedOccTypeGroup.ListOfOccupancyTypes)
            {
                collectionOfOccTypes.Add(ot);
            }
            ListCollectionView lcv = new ListCollectionView(collectionOfOccTypes);

            lcv.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Consequences_Assist.ComputableObjects.OccupancyType.DamageCategoryName)));

            // var groupedOcctypes = collectionOfOccTypes.GroupBy(ot => ot.DamageCategory.Name);
            //foreach(var group in groupedOcctypes)
            //{
            //    string name = group.Key;
            //}

            lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(Consequences_Assist.ComputableObjects.OccupancyType.DamageCategoryName), System.ComponentModel.ListSortDirection.Ascending));
            lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(Consequences_Assist.ComputableObjects.OccupancyType.Name), System.ComponentModel.ListSortDirection.Ascending));

            OccTypeListView.ItemsSource = lcv;
        }

        private void cmb_Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            //load the list view
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            if(vm.SelectedOccTypeGroup == null) { return; }

            ObservableCollection<Consequences_Assist.ComputableObjects.OccupancyType> collectionOfOccTypes = new ObservableCollection<Consequences_Assist.ComputableObjects.OccupancyType>();
            foreach (Consequences_Assist.ComputableObjects.OccupancyType ot in vm.SelectedOccTypeGroup.ListOfOccupancyTypes)
            {
                collectionOfOccTypes.Add(ot);
            }
            ListCollectionView lcv = new ListCollectionView(collectionOfOccTypes);

            lcv.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Consequences_Assist.ComputableObjects.OccupancyType.DamageCategoryName)));

           // var groupedOcctypes = collectionOfOccTypes.GroupBy(ot => ot.DamageCategory.Name);
            //foreach(var group in groupedOcctypes)
            //{
            //    string name = group.Key;
            //}
            
            lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(Consequences_Assist.ComputableObjects.OccupancyType.DamageCategoryName), System.ComponentModel.ListSortDirection.Ascending));
            lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(Consequences_Assist.ComputableObjects.OccupancyType.Name), System.ComponentModel.ListSortDirection.Ascending));
             
            OccTypeListView.ItemsSource = lcv;
            if (OccTypeListView.Items.Count == 0)
            {
                return;
            }
            OccTypeListView.SelectedItem = OccTypeListView.Items[0];
            vm.LoadTheIsTabsCheckedDictionary();

        }

        private void ClearAllControls()
        {

            OccTypeEditorControl.OccTypeNameBox.Text = "";
            OccTypeEditorControl.OccTypeNameBox.IsEnabled = false;

            OccTypeEditorControl.OccTypeDescriptionBox.Text = "";
            OccTypeEditorControl.OccTypeDescriptionBox.IsEnabled = false;

            OccTypeEditorControl.DamageCategoryComboBox.SelectedIndex = -1;
            OccTypeEditorControl.DamageCategoryComboBox.IsEnabled = false;

            OccTypeEditorControl.FoundationHeightUncertainty.IsEnabled = false;

            OccTypeEditorControl.tableWithPlot_Structures.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None) ;
            OccTypeEditorControl.tableWithPlot_Content.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            OccTypeEditorControl.tableWithPlot_vehicle.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            OccTypeEditorControl.tableWithPlot_other.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

            OccTypeEditorControl.txt_YearBox.IsEnabled = false;
            OccTypeEditorControl.txt_Module.IsEnabled = false;
        }
        private void EnableAllControls()
        {

            //OccTypeEditorControl.OccTypeNameBox.Text = "";
            OccTypeEditorControl.OccTypeNameBox.IsEnabled = true;

            //OccTypeEditorControl.OccTypeDescriptionBox.Text = "";
            OccTypeEditorControl.OccTypeDescriptionBox.IsEnabled = true;

            //OccTypeEditorControl.DamageCategoryComboBox.SelectedIndex = -1;
            OccTypeEditorControl.DamageCategoryComboBox.IsEnabled = true;

            OccTypeEditorControl.FoundationHeightUncertainty.IsEnabled = true;

            //OccTypeEditorControl.tableWithPlot_Structures.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            //OccTypeEditorControl.tableWithPlot_Content.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            //OccTypeEditorControl.tableWithPlot_vehicle.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            //OccTypeEditorControl.tableWithPlot_other.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

            OccTypeEditorControl.txt_YearBox.IsEnabled = true;
            OccTypeEditorControl.txt_Module.IsEnabled = true;
        }
    

        private void OccTypeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            ////assign the continuous distributions for the previously selected "percentofmeanuncertaintyWarning" control
            //if (e.RemovedItems.Count > 0)
            //{
            //    Consequences_Assist.ComputableObjects.OccupancyType prev = (Consequences_Assist.ComputableObjects.OccupancyType)e.RemovedItems[0];
            //    FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            //    //structure
            //    prev.StructureValueUncertainty = OccTypeEditorControl.StructureValueUncertainty.ReturnDistribution();
            //    //content
            //    prev.ContentValueUncertainty = OccTypeEditorControl.ContentValueUncertainty.ReturnDistribution();
            //    //vehicle
            //    prev.VehicleValueUncertainty = OccTypeEditorControl.VehicleValueUncertainty.ReturnDistribution();
            //    //other
            //    prev.OtherValueUncertainty = OccTypeEditorControl.OtherValueUncertainty.ReturnDistribution();

            //    //assign the description
            //    prev.Description = vm.Description;

            //    //assign the foundation height uncertainty
            //    prev.FoundationHeightUncertainty = OccTypeEditorControl.FoundationHeightUncertainty.ReturnDistribution();

            //}

            


            ListView lv = (ListView)sender;
            if(lv.Items.Count == 0) { return; }
            if(lv.SelectedItem == null) { lv.SelectedItem = lv.Items[0]; }
            Consequences_Assist.ComputableObjects.OccupancyType ot = lv.SelectedItem as Consequences_Assist.ComputableObjects.OccupancyType;

            OccTypeEditorControl.OccTypeDescriptionBox.Text = ot.Description;

            //set the cont dist values for the selected occtype
            if (ot.StructureValueUncertainty != null)
            {
                OccTypeEditorControl.StructureValueUncertainty.LoadOccTypeData(ot.StructureValueUncertainty);
            }
            if (ot.ContentValueUncertainty != null)
            {
                OccTypeEditorControl.ContentValueUncertainty.LoadOccTypeData(ot.ContentValueUncertainty);
            }
            if (ot.VehicleValueUncertainty != null)
            {
                OccTypeEditorControl.VehicleValueUncertainty.LoadOccTypeData(ot.VehicleValueUncertainty);
            }
            if (ot.OtherValueUncertainty != null)
            {
                OccTypeEditorControl.OtherValueUncertainty.LoadOccTypeData(ot.OtherValueUncertainty);
            }

            //load the foundation height uncertainty
            if(ot.FoundationHeightUncertainty != null)
            {
                OccTypeEditorControl.FoundationHeightUncertainty.LoadOccTypeData(ot.FoundationHeightUncertainty);
            }

            ////if the structure, content, vehicle, or other, depth damage curve value is "", then set the combo box to -1;
            //if (ot.StructureDepthDamageName == "" || ot.StructureDepthDamageName == null)
            //{
            //    OccTypeEditorControl.StructureDamageComboBox.SelectedIndex = -1;
            //}
            //if (ot.ContentDepthDamageName == "" || ot.ContentDepthDamageName == null)
            //{
            //    OccTypeEditorControl.ContentDamageComboBox.SelectedIndex = -1;
            //}
            //if (ot.VehicleDepthDamageName == "" || ot.VehicleDepthDamageName == null)
            //{
            //    OccTypeEditorControl.VehicleDamageComboBox.SelectedIndex = -1;
            //}
            //if (ot.OtherDepthDamageName == "" || ot.OtherDepthDamageName == null)
            //{
            //    OccTypeEditorControl.OtherDamageComboBox.SelectedIndex = -1;
            //}




        }

        private void CreateNewOccTypeButton_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            vm.LaunchNewOccTypeWindow();
            UpdateTheListView(sender,e);
            EnableAllControls();
        }

        private void CopyExistingButton_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            vm.LaunchCopyOccTypeWindow();
            UpdateTheListView(sender, e);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            vm.DeleteOccType();
            UpdateTheListView(sender, e);
            if(vm.SelectedOccType == null)
            {
                ClearAllControls();
            }
        }

        private void CreateNewOccTypeGroupButton_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            vm.LaunchImportNewOccTypeGroup();
        }

        private void RenameOccTypeGroupButton_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            vm.LaunchRenameOcctypeGroup();
        }

        private void DeleteOccTypeGroupButton_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            vm.DeleteOccTypeGroup();
        }
    }
}
