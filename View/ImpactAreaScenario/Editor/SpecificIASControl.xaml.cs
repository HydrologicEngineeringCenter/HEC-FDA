using HEC.Plotting.Core;
using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.Controller;
using HEC.Plotting.SciChart2D.ViewModel;
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
using ViewModel.ImpactAreaScenario.Editor;

namespace View.ImpactAreaScenario.Editor
{
    /// <summary>
    /// Interaction logic for SpecificIASControl.xaml
    /// </summary>
    public partial class SpecificIASControl : UserControl
    {

        private bool _plotsHaveBeenAdded;

        public SpecificIASControl()
        {
            InitializeComponent();
        }

        private void plotBtn_Click(object sender, RoutedEventArgs e)
        {
            SpecificIASEditorVM vm = (SpecificIASEditorVM)this.DataContext;
            vm.Plot();
        }

        private void addThresholdBtn_Click(object sender, RoutedEventArgs e)
        {
            SpecificIASEditorVM vm = (SpecificIASEditorVM)this.DataContext;
            vm.AddThresholds();
        }
    }
}
