using FunctionsView.ViewModel;
using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.ViewModel;
using System.Windows;
using System.Windows.Controls;
using ViewModel.Editors;

namespace View.StageTransforms
{
    /// <summary>
    /// Interaction logic for RatingCurveEditor.xaml
    /// </summary>
    public partial class RatingCurveEditor : UserControl
    {
        private Chart2D _chart;
        public RatingCurveEditor()  
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CurveEditorVM vm = DataContext as CurveEditorVM;
            if (_chart == null && vm != null)
            {
                CoordinatesFunctionEditorVM editorVM = vm.EditorVM;
                _chart = new Chart2D(editorVM.CoordinatesChartViewModel);
                editorGrid.Children.Add(_chart);
                Grid.SetColumn(_chart, 2);
            }
        }
    }
}
