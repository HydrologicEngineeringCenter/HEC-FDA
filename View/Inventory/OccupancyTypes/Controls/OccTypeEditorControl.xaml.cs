using FdaViewModel.Editors;
using FdaViewModel.Inventory.OccupancyTypes;
using FunctionsView.ViewModel;
using HEC.Plotting.SciChart2D.Charts;
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
using HEC.Plotting.SciChart2D.ViewModel;

namespace View.Inventory.OccupancyTypes.Controls
{
    /// <summary>
    /// Interaction logic for OccTypeEditorControl.xaml
    /// </summary>
    public partial class OccTypeEditorControl : UserControl
    {

        public event EventHandler ListViewNeedsUpdating;
        private Chart2D _StructureChart;
        private Chart2D _ContentChart;
        private Chart2D _VehicleChart;
        private Chart2D _OtherChart;

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
            OccupancyTypeEditable vm = (OccupancyTypeEditable)this.DataContext;
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
            //FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            //if (vm == null) 
            //{ 
            //    return; 
            //}
            //StructureValueUncertainty.ReturnDistribution();
            //vm.SelectedOccType.StructureValueUncertainty = 
            //vm.SelectedOccType.StructureValueUncertainty = StructureValueUncertainty.ReturnDistribution();
        }

        private void ContentValueUncertainty_LostFocus(object sender, RoutedEventArgs e)
        {
            //FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            //if (vm == null) { return; }
            //vm.SelectedOccType.ContentValueUncertainty = ContentValueUncertainty.ReturnDistribution();
        }

        private void VehicleValueUncertainty_LostFocus(object sender, RoutedEventArgs e)
        {
            //FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            //if (vm == null) { return; }
            //vm.SelectedOccType.VehicleValueUncertainty = VehicleValueUncertainty.ReturnDistribution();
        }

        private void OtherValueUncertainty_LostFocus(object sender, RoutedEventArgs e)
        {
            //FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            //if (vm == null) { return; }
            //vm.SelectedOccType.OtherValueUncertainty = OtherValueUncertainty.ReturnDistribution();
        }

        private void OccTypeDescriptionBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            //if (vm == null || vm.SelectedOccType == null) { return; }
            //string desc = OccTypeDescriptionBox.Text;
            //if(desc == null)
            //{
            //    desc = "";
            //}
            //vm.SelectedOccType.Description = OccTypeDescriptionBox.Text;
        }

        private void FoundationHeightUncertainty_LostFocus(object sender, RoutedEventArgs e)
        {
            //FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM vm = (FdaViewModel.Inventory.OccupancyTypes.OccupancyTypesEditorVM)this.DataContext;
            //if(vm == null) { return; }
            //vm.SelectedOccType.FoundationHeightUncertainty = FoundationHeightUncertainty.ReturnDistribution();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: for some reason we cant use sci chart in the xaml. I also can't figure 
            //out how to set a binding to the selected occtypes editor vm's. So i will
            //create the asset editors for the chart at the OccTypeEditorVM level. Then 
            //when the selection changes i update those 4 editors.

            //OccupancyTypesEditorVM vm = (OccupancyTypesEditorVM)this.DataContext;
            //CoordinatesFunctionEditorVM editorVM = vm.StructureEditorVM; //vm.SelectedOccType.StructureEditorVM;
            //Chart = new Chart2D(editorVM.CoordinatesChartViewModel);


            //Binding myBinding = new Binding("SelectedOccType.StructureEditorVM");
            //myBinding.Source = this.DataContext;
            //chart.SetBinding(Chart2D.DataContextProperty, myBinding);

            //Binding myBinding = new Binding();
            //myBinding.Source = this.DataContext;
            //myBinding.Path = new PropertyPath("SelectedOccType.StructureEditorVM");
            //myBinding.Mode = BindingMode.OneWay;
            //myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //BindingOperations.SetBinding(chart, Chart2D.DataContextProperty, myBinding);

            //StructureTabGrid.Children.Add(Chart);
            //Grid.SetRow(Chart, 2);
            //Grid.SetColumn(Chart, 2);

            AddChart();

        }

        //public void AddChart()
        //{
        //    OccupancyTypesEditorVM vm = (OccupancyTypesEditorVM)this.DataContext;
        //    if(vm.SelectedOccType == null)
        //    {
        //        return;
        //    }

        //    StructureTabGrid.Children.Remove(_StructureChart);
        //    ContentTabGrid.Children.Remove(_ContentChart);
        //    VehicleTabGrid.Children.Remove(_VehicleChart);
        //    OtherTabGrid.Children.Remove(_OtherChart);


        //    //set the chart view models
        //    _StructureChart = new Chart2D( vm.SelectedOccType.StructureEditorVM.CoordinatesChartViewModel);
        //    _ContentChart = new Chart2D(vm.SelectedOccType.ContentEditorVM.CoordinatesChartViewModel);
        //    _VehicleChart = new Chart2D(vm.SelectedOccType.VehicleEditorVM.CoordinatesChartViewModel);
        //    _OtherChart = new Chart2D(vm.SelectedOccType.OtherEditorVM.CoordinatesChartViewModel);

        //    //add the new charts to the UI
        //    StructureTabGrid.Children.Add(_StructureChart);
        //    Grid.SetRow(_StructureChart, 2);
        //    Grid.SetColumn(_StructureChart, 2);

        //    ContentTabGrid.Children.Add(_ContentChart);
        //    Grid.SetRow(_ContentChart, 2);
        //    Grid.SetColumn(_ContentChart, 2);

        //    VehicleTabGrid.Children.Add(_VehicleChart);
        //    Grid.SetRow(_VehicleChart, 2);
        //    Grid.SetColumn(_VehicleChart, 2);

        //    OtherTabGrid.Children.Add(_OtherChart);
        //    Grid.SetRow(_OtherChart, 2);
        //    Grid.SetColumn(_OtherChart, 2);
        //}

        public void AddChart()
        {
            OccupancyTypeEditable vm = (OccupancyTypeEditable)this.DataContext;
            if (vm == null)
            {
                return;
            }

            StructureTabGrid.Children.Remove(_StructureChart);
            ContentTabGrid.Children.Remove(_ContentChart);
            VehicleTabGrid.Children.Remove(_VehicleChart);
            OtherTabGrid.Children.Remove(_OtherChart);


            //set the chart view models
            
            vm.StructureEditorVM.CoordinatesChartViewModel = new SciChart2DChartViewModel(vm.StructureEditorVM.CoordinatesChartViewModel);
            Chart2D structChart = new Chart2D(vm.StructureEditorVM.CoordinatesChartViewModel);
            
            vm.ContentEditorVM.CoordinatesChartViewModel = new SciChart2DChartViewModel(vm.ContentEditorVM.CoordinatesChartViewModel);
            Chart2D contentChart = new Chart2D(vm.ContentEditorVM.CoordinatesChartViewModel);
            
            vm.VehicleEditorVM.CoordinatesChartViewModel = new SciChart2DChartViewModel(vm.VehicleEditorVM.CoordinatesChartViewModel);
            Chart2D vehicleChart = new Chart2D(vm.VehicleEditorVM.CoordinatesChartViewModel);
            
            vm.OtherEditorVM.CoordinatesChartViewModel = new SciChart2DChartViewModel(vm.OtherEditorVM.CoordinatesChartViewModel);
            Chart2D otherChart = new Chart2D(vm.OtherEditorVM.CoordinatesChartViewModel);

            _StructureChart = structChart;
            _ContentChart = contentChart;
            _VehicleChart = vehicleChart;
            _OtherChart = otherChart;

            //add the new charts to the UI
            StructureTabGrid.Children.Add(_StructureChart);
            Grid.SetRow(_StructureChart, 2);
            Grid.SetColumn(_StructureChart, 2);

            ContentTabGrid.Children.Add(_ContentChart);
            Grid.SetRow(_ContentChart, 2);
            Grid.SetColumn(_ContentChart, 2);

            VehicleTabGrid.Children.Add(_VehicleChart);
            Grid.SetRow(_VehicleChart, 2);
            Grid.SetColumn(_VehicleChart, 2);

            OtherTabGrid.Children.Add(_OtherChart);
            Grid.SetRow(_OtherChart, 2);
            Grid.SetColumn(_OtherChart, 2);
        }


    }
}
