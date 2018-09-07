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

namespace Fda.Plots
{
    /// <summary>
    /// Interaction logic for ConditionsIndividualPlotWrapper.xaml
    /// </summary>
    public partial class ConditionsIndividualPlotWrapper : UserControl
    {
        public ConditionsIndividualPlotWrapper()
        {
            InitializeComponent();
        }

        private void btn_DeleteCurve_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM vm = (FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM)this.DataContext;
            //clear the curve on the VM side
            vm.PlotVM.Curve = null; // the curve change callback will update all the linkages

            //null the SelectedCurve on the control
            ContentControl parentControl = Plots.IndividualLinkedPlotControl.FindParent<ContentControl>(this);
            if (parentControl != null && parentControl.GetType() == typeof(IndividualLinkedPlotControl))
            {
                ((IndividualLinkedPlotControl)parentControl).LinkedPlot = null;
               // ((IndividualLinkedPlotControl)parentControl).UpdateThePlots();
            }
            //make the button reappear
            vm.ShowTheImportButton(this,new EventArgs());

            LinkedPlot.OxyPlot1.Model.Series.Clear();
            LinkedPlot.OxyPlot1.Model.InvalidatePlot(true);
        }

        private void btn_ChangeCurve_Click(object sender, RoutedEventArgs e)
        {
            //the curve needs to not be touched in case they don't choose a new one.
            FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM vm = (FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM)this.DataContext;
           
            ////null the SelectedCurve on the control
            //ContentControl parentControl = Plots.IndividualLinkedPlotControl.FindParent<ContentControl>(this);
            //if (parentControl != null && parentControl.GetType() == typeof(IndividualLinkedPlotControl))
            //{
            //    ((IndividualLinkedPlotControl)parentControl).SelectedCurve = null;
            //    ((IndividualLinkedPlotControl)parentControl).UpdateThePlots();
            //}

            //make the button reappear
            vm.ShowTheImporterForm(this, new EventArgs());

            ////raise an event so that the parent control can handle it
            //if (ChangeThisCurve != null)
            //{
            //    this.ChangeThisCurve(this, new EventArgs());
            //}
        }



    }
}
