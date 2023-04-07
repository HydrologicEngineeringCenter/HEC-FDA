using OxyPlot;
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

namespace HEC.FDA.View.TableWithPlot
{
    /// <summary>
    /// Interaction logic for PlotWithToolbar.xaml
    /// </summary>
    public partial class PlotWithToolbar : UserControl
    {
        public PlotWithToolbar()
        {
            InitializeComponent();
        }

        private void SelectButton_Checked(object sender, RoutedEventArgs e)
        {
            var withBlock = oxyPlot.ActualController;
            withBlock.UnbindAll();
            withBlock.BindMouseDown(OxyMouseButton.Middle, PlotCommands.PanAt);
            withBlock.BindMouseDown(OxyMouseButton.Left, PlotCommands.SnapTrack);       
            withBlock.BindMouseWheel(PlotCommands.ZoomWheel);
            withBlock.BindKeyDown(OxyKey.Escape, PlotCommands.Reset);
        }

        private void PanButton_Checked(object sender, RoutedEventArgs e)
        {
            var withBlock = oxyPlot.ActualController;
            withBlock.UnbindAll();
            withBlock.BindMouseDown(OxyMouseButton.Middle, PlotCommands.PanAt);
            withBlock.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);       
            withBlock.BindMouseWheel(PlotCommands.ZoomWheel);
            withBlock.BindKeyDown(OxyKey.Escape, PlotCommands.Reset);
        }

        private void ZoomButton_Checked(object sender, RoutedEventArgs e)
        {
            var withBlock = oxyPlot.ActualController;
            withBlock.UnbindAll();
            withBlock.BindMouseDown(OxyMouseButton.Middle, PlotCommands.PanAt);
            withBlock.BindMouseDown(OxyMouseButton.Left, PlotCommands.ZoomRectangle);
            withBlock.BindMouseWheel(PlotCommands.ZoomWheel);
            withBlock.BindKeyDown(OxyKey.Escape, PlotCommands.Reset);
        }

        private void ZoomAllButton_Checked(object sender, RoutedEventArgs e)
        {
            oxyPlot.ResetAllAxes();
            oxyPlot.InvalidatePlot(false);
            Focus();
        }
    }
}
