using FdaViewModel.Editors;
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

namespace View.GeoTech
{
    /// <summary>
    /// Interaction logic for FailureFunctionEditor.xaml
    /// </summary>
    public partial class FailureFunctionEditor : UserControl
    {
        public FailureFunctionEditor()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CurveEditorVM vm = (CurveEditorVM)this.DataContext;
            CoordinatesFunctionEditorVM editorVM = vm.EditorVM;
            var model = new SciChart2DChartViewModel(editorVM.CoordinatesChartViewModel);
            //editorVM.CoordinatesChartViewModel = model;
            Chart2D chart = new Chart2D(model);
            PlotGrid.Children.Add(chart);
            Grid.SetColumn(chart, 2);
        }
    }
}
