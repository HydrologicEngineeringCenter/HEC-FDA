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

namespace View.Plots
{
    /// <summary>
    /// Interaction logic for DoubleLineModulatorCoverButton.xaml
    /// </summary>
    public partial class DoubleLineModulatorCoverButton : UserControl
    {
        public DoubleLineModulatorCoverButton()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Plots.DoubleLineModulatorCoverButtonVM vm = (ViewModel.Plots.DoubleLineModulatorCoverButtonVM)this.DataContext;
            //vm.ButtonClicked();//this will change the current VM to be the importer

            //pop the plot out
            //find parent and pop this plot out.
            ContentControl parentControl = Plots.IndividualLinkedPlotControl.FindParent<ContentControl>(this);
            if (parentControl != null && parentControl.GetType() == typeof(ConditionsIndividualPlotWrapper))
            {
                parentControl = IndividualLinkedPlotControl.FindParent<ContentControl>(parentControl);
            }

            if (parentControl != null && parentControl.GetType() == typeof(IndividualLinkedPlotControl))
            {
                //ViewModel.Plots.IndividualLinkedPlotControlVM vm = (ViewModel.Plots.IndividualLinkedPlotControlVM)parentControl.DataContext;

                //this.BaseFunction = vm.IndividualPlotWrapperVM.PlotVM.BaseFunction;
                //((Plots.IndividualLinkedPlotControl)parentControl).SelectedCurve = this;
                ((IndividualLinkedPlotControl)parentControl).PopTheImporterIntoPlot1();
            }

        }

    }
}
