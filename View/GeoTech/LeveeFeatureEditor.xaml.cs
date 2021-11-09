using ViewModel.Editors;
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
    /// Interaction logic for LeveeFeatureEditor.xaml
    /// </summary>
    public partial class LeveeFeatureEditor : UserControl
    {
        private Chart2D _chart;

        public LeveeFeatureEditor()
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
                PlotGrid.Children.Add(_chart);
                Grid.SetColumn(_chart, 2);
            }
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
