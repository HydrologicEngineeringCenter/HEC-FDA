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
using View.ImpactAreaScenario;

namespace View.Plots
{
    /// <summary>
    /// Interaction logic for IASHorizontalFailureFunction.xaml
    /// </summary>
    public partial class IASHorizontalFailureFunction : UserControl
    {
        public IASHorizontalFailureFunction()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ContentControl parentControl = Plots.IndividualLinkedPlotControl.FindParent<ContentControl>(this);
            if (parentControl != null && parentControl.GetType() == typeof(HorizontalDoubleLineModulatorWrapper))
            {
                parentControl = IndividualLinkedPlotControl.FindParent<ContentControl>(parentControl);
            }

            if (parentControl != null && parentControl.GetType() == typeof(IndividualLinkedPlotControl))
            {
                ViewModel.Plots.IndividualLinkedPlotControlVM vm = (ViewModel.Plots.IndividualLinkedPlotControlVM)parentControl.DataContext;
                vm.UpdateHorizontalFailureFunction += UpdateText;
   
            }
        }

        private void UpdateText(Object sender, EventArgs e)
        {
            ContentControl parentControl = Plots.IndividualLinkedPlotControl.FindParent<ContentControl>(this);
            if (parentControl != null && parentControl.GetType() == typeof(HorizontalDoubleLineModulatorWrapper))
            {
                parentControl = IndividualLinkedPlotControl.FindParent<ContentControl>(parentControl);
            }

            if (parentControl != null && parentControl.GetType() == typeof(IndividualLinkedPlotControl))
            {
                IndividualLinkedPlotControl indLinkedPlotControl = (IndividualLinkedPlotControl)parentControl;
                ViewModel.Plots.IndividualLinkedPlotControlVM vm = (ViewModel.Plots.IndividualLinkedPlotControlVM)parentControl.DataContext;
                double xval = vm.CurrentX;
                double yval = vm.CurrentY;

                //double xAxisMin = vm.GetChartViewModel().SharedXAxisMin; //indLinkedPlotControl.SharedYAxisMin;
                //double xAxisMax = vm.GetChartViewModel().SharedXAxisMax; //indLinkedPlotControl.SharedYAxisMax;

                //if (xAxisMax == 0 && xAxisMin == 0)
                //{
                //    return;
                //}

                string xUnits = vm.IndividualPlotWrapperVM.PlotVM.BaseFunction.XSeries.Units.ToString();

                lbl_FailureText.Content = "Chance of Failure: " + yval.ToString("0.0") + "% at " + xval.ToString("0.0") + " " + xUnits;

                //lbl_percentFailure.Content = yval.ToString("0.0");
                //lbl_stage.Content = xval.ToString("0.0") + xUnits;

                //DisplayLines(xAxisMin, xAxisMax, vm.MinX, vm.MaxX, vm.MinY, vm.MaxY, vm.CurrentX, vm.CurrentY);
            }
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            ContentControl parentControl = Plots.IndividualLinkedPlotControl.FindParent<ContentControl>(this);
 
            //if (parentControl != null && parentControl.GetType() == typeof(IASHorizontalFailureFunction)) //this occurs in the IAS editor
            {
                parentControl = IndividualLinkedPlotControl.FindParent<ContentControl>(parentControl);
                if (parentControl != null && parentControl.GetType() == typeof(IASPlotEditor))
                {
                    ((IASPlotEditor)parentControl).CancelLateralStructure(this, new EventArgs());
                    //ViewModel.Plots.IndividualLinkedPlotControlVM vm = (ViewModel.Plots.IndividualLinkedPlotControlVM)parentControl.DataContext;
                    //vm.CurrentVM = (ViewModel.BaseViewModel)vm.IndividualPlotWrapperVM;
                    //((IndividualLinkedPlotControl)parentControl).CancelTheLateralStructure();
                }
            }
        }
    }
}
