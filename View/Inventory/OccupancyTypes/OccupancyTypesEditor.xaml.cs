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

        public OccupancyTypesEditor()
        {
            InitializeComponent();
            OccTypeEditorControl.ListViewNeedsUpdating += new EventHandler(UpdateTheListView);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // I wanted the editor to open up with a group and occtype selected. This gets the first group and the first occtype.
            if(DataContext is OccupancyTypesEditorVM vm)
            {
                vm.CloseEditor += Vm_CloseEditor;
                UpdateTheListView(sender, e);
            }
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
            if (DataContext is OccupancyTypesEditorVM vm)
            {
                if (vm.SelectedOccTypeGroup == null) 
                { 
                    return; 
                }

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
        }    

        private void cmb_Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is OccupancyTypesEditorVM vm)
            {
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
                UpdateTheListView(sender, e);
            }
        }

        private void ClearAllControls()
        {
            OccTypeEditorControl.OccTypeNameBox.Text = "";
            OccTypeEditorControl.OccTypeNameBox.IsEnabled = false;
            OccTypeEditorControl.DamageCategoryComboBox.SelectedIndex = -1;
            OccTypeEditorControl.DamageCategoryComboBox.IsEnabled = false;
        }
        private void EnableAllControls()
        {
            OccTypeEditorControl.OccTypeNameBox.IsEnabled = true;
            OccTypeEditorControl.DamageCategoryComboBox.IsEnabled = true;
        }

        private void CreateNewOccTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OccupancyTypesEditorVM vm)
            {
                vm.LaunchNewOccTypeWindow();
                UpdateTheListView(sender, e);
                EnableAllControls();
            }
        }

        private void CopyExistingButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OccupancyTypesEditorVM vm)
            {
                vm.LaunchCopyOccTypeWindow();
                UpdateTheListView(sender, e);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OccupancyTypesEditorVM vm)
            {
                vm.DeleteOccType();
                UpdateTheListView(sender, e);
                if (vm.SelectedOccType == null)
                {
                    ClearAllControls();
                }
            }
        }

        private void CreateNewOccTypeGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OccupancyTypesEditorVM vm)
            {
                vm.LaunchImportNewOccTypeGroup();
            }
        }

        private void RenameOccTypeGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OccupancyTypesEditorVM vm)
            {
                vm.LaunchRenameOcctypeGroup();
            }
        }

        private void DeleteOccTypeGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is OccupancyTypesEditorVM vm)
            {
                vm.DeleteOccTypeGroup();
            }
        }
    }
}
