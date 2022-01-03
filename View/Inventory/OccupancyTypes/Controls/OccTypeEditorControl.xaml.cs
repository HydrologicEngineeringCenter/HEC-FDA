using HEC.Plotting.SciChart2D.Charts;
using System;
using System.Windows;
using System.Windows.Controls;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.View.Inventory.OccupancyTypes.Controls
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
            if(ListViewNeedsUpdating != null)
            {
                ListViewNeedsUpdating(this, new EventArgs());
            }
        }

        private void CreateNewDamCat_Click(object sender, RoutedEventArgs e)
        {
            OccupancyTypeEditable vm = (OccupancyTypeEditable)DataContext;
            if (vm == null) { return; }
            vm.LaunchNewDamCatWindow();
            if (ListViewNeedsUpdating != null)
            {
                ListViewNeedsUpdating(this, new EventArgs());
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: for some reason we cant use sci chart in the xaml. I also can't figure 
            //out how to set a binding to the selected occtypes editor vm's. So i will
            //create the asset editors for the chart at the OccTypeEditorVM level. Then 
            //when the selection changes i update those 4 editors.

            //AddChart();
        }

        private void UpdateChart(ref Chart2D chart, CoordinatesFunctionEditorVM editorVM)
        {
            SciChart2DChartViewModel sciChart2DChartViewModel = new SciChart2DChartViewModel(editorVM.CoordinatesChartViewModel);
            chart = new Chart2D(sciChart2DChartViewModel);
            editorVM.CoordinatesChartViewModel = sciChart2DChartViewModel;
        }

        private void AddChart()
        {
            if (DataContext is OccupancyTypeEditable vm)
            {
                //structure
                UpdateChart(ref _StructureChart, vm.StructureEditorVM);
                StructureTabGrid.Children.Add(_StructureChart);
                Grid.SetRow(_StructureChart, 2);
                Grid.SetColumn(_StructureChart, 2);
                //content
                UpdateChart(ref _ContentChart, vm.ContentEditorVM);
                ContentTabGrid.Children.Add(_ContentChart);
                Grid.SetRow(_ContentChart, 2);
                Grid.SetColumn(_ContentChart, 2);
                //vehicle
                UpdateChart(ref _VehicleChart, vm.VehicleEditorVM);
                VehicleTabGrid.Children.Add(_VehicleChart);
                Grid.SetRow(_VehicleChart, 2);
                Grid.SetColumn(_VehicleChart, 2);
                //other
                UpdateChart(ref _OtherChart, vm.OtherEditorVM);
                OtherTabGrid.Children.Add(_OtherChart);
                Grid.SetRow(_OtherChart, 2);
                Grid.SetColumn(_OtherChart, 2);
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AddChart();
        }
    }
}
