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

namespace View.StageTransforms
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
            CurveEditorVM vm = (CurveEditorVM)this.DataContext;
            CoordinatesFunctionEditorVM editorVM = vm.EditorVM;
            Chart2D chart = new Chart2D(new SciChart2DChartViewModel(editorVM.CoordinatesChartViewModel));
            //Binding myBinding = new Binding("EditorVM.CoordinatesChartViewModel");
            //myBinding.Source = this.DataContext;
            //chart.SetBinding(Chart2D.DataContextProperty, myBinding);

            editorGrid.Children.Add(chart);
            Grid.SetColumn(chart, 2);
        }
    }
}
