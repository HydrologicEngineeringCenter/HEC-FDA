using System.Windows;
using System.Windows.Controls;

namespace HEC.FDA.View.StageTransforms
{
    /// <summary>
    /// Interaction logic for RatingCurveEditor.xaml
    /// </summary>
    public partial class RatingCurveEditor : UserControl
    {
        public RatingCurveEditor()  
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //todo: leaving this here until we get the new table and plot control

            //CurveEditorVM vm = DataContext as CurveEditorVM;
            //if ( vm != null)
            //{
            //    CoordinatesFunctionEditorVM editorVM = vm.EditorVM;

            //    SciChart2DChartViewModel sciChart2DChartViewModel = new SciChart2DChartViewModel(editorVM.CoordinatesChartViewModel);
            //    Chart2D chart = new Chart2D(sciChart2DChartViewModel);
            //    editorVM.CoordinatesChartViewModel = sciChart2DChartViewModel;

            //    editorGrid.Children.Add(chart);
            //    Grid.SetColumn(chart, 2);
            //}
        }
    }
}
