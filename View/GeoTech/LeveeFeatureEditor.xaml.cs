using System.Windows;
using System.Windows.Controls;

namespace View.GeoTech
{
    /// <summary>
    /// Interaction logic for LeveeFeatureEditor.xaml
    /// </summary>
    public partial class LeveeFeatureEditor : UserControl
    {
        public LeveeFeatureEditor()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //CurveEditorVM vm = DataContext as CurveEditorVM;
            //if ( vm != null)
            //{
            //    CoordinatesFunctionEditorVM editorVM = vm.EditorVM;
            //    editorVM.CoordinatesChartViewModel = new SciChart2DChartViewModel(editorVM.CoordinatesChartViewModel);
            //    Chart2D chart = new Chart2D(editorVM.CoordinatesChartViewModel);
                
            //    PlotGrid.Children.Add(chart);
            //    Grid.SetColumn(chart, 2);
            //}
        }

        private void rad_default_Checked(object sender, RoutedEventArgs e)
        {
            //it comes in here when initializing
            if(PlotGrid == null)
            {
                return;
            }
            PlotGrid.Visibility = Visibility.Hidden;
        }

        private void rad_userDefined_Checked(object sender, RoutedEventArgs e)
        {
            PlotGrid.Visibility = Visibility.Visible;

        }
    }
}
