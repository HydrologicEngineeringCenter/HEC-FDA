using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.View.Inventory.OccupancyTypes.Controls;
using HEC.FDA.ViewModel.Tabs;

namespace HEC.FDA.View.Inventory.OccupancyTypes
{
    /// <summary>
    /// Interaction logic for OccupancyTypesEditor.xaml
    /// </summary>
    public partial class OccupancyTypesEditor : UserControl
    {

        private bool _isFirstSettingOfOcctypeGroup = true;

        public OccupancyTypesEditor()
        {
            InitializeComponent();

            OccTypeEditorControl.ListViewNeedsUpdating += new EventHandler(UpdateTheListView);

            
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // I wanted the editor to open up with a group and occtype selected. This gets the first group and the first occtype.
            OccupancyTypesEditorVM vm = (OccupancyTypesEditorVM)this.DataContext;
            vm.CloseEditor += Vm_CloseEditor;
            UpdateTheListView(sender, e);
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

        /// <summary>
        /// This gets called if there are no occtype groups left and the user says that they want to close the editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vm_CloseEditor(object sender, EventArgs e)
        {
            TabController.Instance.CloseTabOrWindow(this);
        }

        public void UpdateTheListView(object sender, EventArgs e)
        {
            //load the list view
            OccupancyTypesEditorVM vm = (OccupancyTypesEditorVM)this.DataContext;
            if (vm.SelectedOccTypeGroup == null) { return; }

            ObservableCollection<IOccupancyTypeEditable> collectionOfOccTypes = new ObservableCollection<IOccupancyTypeEditable>();
            foreach (IOccupancyTypeEditable ot in vm.SelectedOccTypeGroup.Occtypes)
            {
                collectionOfOccTypes.Add(ot);
            }
            ListCollectionView lcv = new ListCollectionView(collectionOfOccTypes);

            lcv.GroupDescriptions.Add(new PropertyGroupDescription("DamageCategory"));        

            lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("DamageCategory", System.ComponentModel.ListSortDirection.Ascending));
            lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));

            OccTypeListView.ItemsSource = lcv;
        }

        private bool handleSelection = true;

        private void cmb_Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OccupancyTypesEditorVM vm = (OccupancyTypesEditorVM)this.DataContext;
            
            //i don't want to message that the occtype group is switching if this is we are setting it for the first time.
            //if (!_isFirstSettingOfOcctypeGroup && handleSelection)
            //{
            //    if (e.RemovedItems.Count > 0)
            //    {
            //        IOccupancyTypeGroupEditable prevGroup = (IOccupancyTypeGroupEditable)e.RemovedItems[0];
            //        //if the prev group has occtype changes then message before switching
            //        if (prevGroup.ModifiedOcctypes.Count>0)
            //        {
            //            //todo: maybe list the occtypes with changes?
            //            MessageBoxResult d;
            //            d = MessageBox.Show("Occupancy type group has unsaved changes. By switching, you will not lose these changes. Do you wish to continue?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //            if (d == MessageBoxResult.No)
            //            {
            //                //when i reset the combobox back to what it originally does, it was infanitely coming back into here because it is another selection changed event.
            //                //I had to add this handleSelection boolean to handle it.
            //                handleSelection = false;
            //                cmb_Group.SelectedItem = e.RemovedItems[0];

            //                return;
            //            }
            //        }
            //    }
            //}
            handleSelection = true;
            //load the list view
            if (vm.SelectedOccTypeGroup == null) { return; }

            ObservableCollection<IOccupancyTypeEditable> collectionOfOccTypes = new ObservableCollection<IOccupancyTypeEditable>();
            foreach (IOccupancyTypeEditable ot in vm.SelectedOccTypeGroup.Occtypes)
            {
                collectionOfOccTypes.Add(ot);
            }
            ListCollectionView lcv = new ListCollectionView(collectionOfOccTypes);

            lcv.GroupDescriptions.Add(new PropertyGroupDescription("DamageCategory"));

            lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("DamageCategory", System.ComponentModel.ListSortDirection.Ascending));
            lcv.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));

            OccTypeListView.ItemsSource = lcv;
            if (OccTypeListView.Items.Count == 0)
            {
                return;
            }
            OccTypeListView.SelectedItem = OccTypeListView.Items[0];
            _isFirstSettingOfOcctypeGroup = false;
            UpdateTheListView(sender, e);
        }

        private void ClearAllControls()
        {

            OccTypeEditorControl.OccTypeNameBox.Text = "";
            OccTypeEditorControl.OccTypeNameBox.IsEnabled = false;

            //OccTypeEditorControl.OccTypeDescriptionBox.Text = "";
            //OccTypeEditorControl.OccTypeDescriptionBox.IsEnabled = false;

            OccTypeEditorControl.DamageCategoryComboBox.SelectedIndex = -1;
            OccTypeEditorControl.DamageCategoryComboBox.IsEnabled = false;

            //OccTypeEditorControl.FoundationHeightUncertainty.IsEnabled = false;

            //todo: cody commented out on 2/20/2020
            //OccTypeEditorControl.tableWithPlot_Structures.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None) ;
            //OccTypeEditorControl.tableWithPlot_Content.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            //OccTypeEditorControl.tableWithPlot_vehicle.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            //OccTypeEditorControl.tableWithPlot_other.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

            //OccTypeEditorControl.txt_YearBox.IsEnabled = false;
            //OccTypeEditorControl.txt_Module.IsEnabled = false;
        }
        private void EnableAllControls()
        {

            //OccTypeEditorControl.OccTypeNameBox.Text = "";
            OccTypeEditorControl.OccTypeNameBox.IsEnabled = true;

            //OccTypeEditorControl.OccTypeDescriptionBox.Text = "";
            //OccTypeEditorControl.OccTypeDescriptionBox.IsEnabled = true;

            //OccTypeEditorControl.DamageCategoryComboBox.SelectedIndex = -1;
            OccTypeEditorControl.DamageCategoryComboBox.IsEnabled = true;

            //OccTypeEditorControl.FoundationHeightUncertainty.IsEnabled = true;

            //OccTypeEditorControl.tableWithPlot_Structures.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            //OccTypeEditorControl.tableWithPlot_Content.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            //OccTypeEditorControl.tableWithPlot_vehicle.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);
            //OccTypeEditorControl.tableWithPlot_other.Curve = new Statistics.UncertainCurveIncreasing(Statistics.UncertainCurveDataCollection.DistributionsEnum.None);

           // OccTypeEditorControl.txt_YearBox.IsEnabled = true;
            //OccTypeEditorControl.txt_Module.IsEnabled = true;
        }
    

        private void OccTypeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //I really don't like this but i was really struggling with how to update the chart
            //when the user switches occtypes.

            //OccTypeEditorControl.AddChart();

            //vm.
            //OccTypeEditorControl.Chart

            ////assign the continuous distributions for the previously selected "percentofmeanuncertaintyWarning" control
            //if (e.RemovedItems.Count > 0)
            //{
            //    IOccupancyType prev = (IOccupancyType)e.RemovedItems[0];
            //    HEC.FDA.ViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (HEC.FDA.ViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
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

            


            //ListView lv = (ListView)sender;
            //if(lv.Items.Count == 0) { return; }
            //if(lv.SelectedItem == null) { lv.SelectedItem = lv.Items[0]; }
            //IOccupancyTypeEditable ot = lv.SelectedItem as IOccupancyTypeEditable;

            //OccTypeEditorControl.OccTypeDescriptionBox.Text = ot.Description;

            //todo: cody commented out on 2/20/2020
            ////set the cont dist values for the selected occtype
            //if (ot.StructureValueUncertainty != null)
            //{
            //    OccTypeEditorControl.StructureValueUncertainty.LoadOccTypeData(ot.StructureValueUncertainty);
            //}
            //if (ot.ContentValueUncertainty != null)
            //{
            //    OccTypeEditorControl.ContentValueUncertainty.LoadOccTypeData(ot.ContentValueUncertainty);
            //}
            //if (ot.VehicleValueUncertainty != null)
            //{
            //    OccTypeEditorControl.VehicleValueUncertainty.LoadOccTypeData(ot.VehicleValueUncertainty);
            //}
            //if (ot.OtherValueUncertainty != null)
            //{
            //    OccTypeEditorControl.OtherValueUncertainty.LoadOccTypeData(ot.OtherValueUncertainty);
            //}

            ////load the foundation height uncertainty
            //if(ot.FoundationHeightUncertainty != null)
            //{
            //    OccTypeEditorControl.FoundationHeightUncertainty.LoadOccTypeData(ot.FoundationHeightUncertainty);
            //}






        }

        private void CreateNewOccTypeButton_Click(object sender, RoutedEventArgs e)
        {
            //CreateNewDamCatVM vm = new CreateNewDamCatVM(new List<string>());
            //CreateNewDamCat nameWindow = new CreateNewDamCat(vm);
            //nameWindow.ShowDialog();
            //if(!vm.WasCanceled) 
            //{
            //    vm.Name = "";
            //}
            OccupancyTypesEditorVM vm = (OccupancyTypesEditorVM)this.DataContext;
            vm.LaunchNewOccTypeWindow();
            UpdateTheListView(sender, e);
            EnableAllControls();
        }

        private void CopyExistingButton_Click(object sender, RoutedEventArgs e)
        {
            OccupancyTypesEditorVM vm = (OccupancyTypesEditorVM)this.DataContext;
            vm.LaunchCopyOccTypeWindow();
            UpdateTheListView(sender, e);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            OccupancyTypesEditorVM vm = (OccupancyTypesEditorVM)this.DataContext;
            vm.DeleteOccType();
            UpdateTheListView(sender, e);
            if(vm.SelectedOccType == null)
            {
                ClearAllControls();
            }
        }

        private void CreateNewOccTypeGroupButton_Click(object sender, RoutedEventArgs e)
        {
            OccupancyTypesEditorVM vm = (OccupancyTypesEditorVM)this.DataContext;
            vm.LaunchImportNewOccTypeGroup();
        }

        private void RenameOccTypeGroupButton_Click(object sender, RoutedEventArgs e)
        {
            OccupancyTypesEditorVM vm = (OccupancyTypesEditorVM)this.DataContext;
            vm.LaunchRenameOcctypeGroup();
        }

        private void DeleteOccTypeGroupButton_Click(object sender, RoutedEventArgs e)
        {
            OccupancyTypesEditorVM vm = (OccupancyTypesEditorVM)this.DataContext;
            vm.DeleteOccTypeGroup();
        }

        //private void btn_SaveAll_Click(object sender, RoutedEventArgs e)
        //{
        //    OccupancyTypesEditorVM vm = (OccupancyTypesEditorVM)this.DataContext;
        //    vm.SaveAll();
        //}
    }
}
