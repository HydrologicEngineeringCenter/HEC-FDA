using ViewModel.Tabs;
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

namespace View.ImpactAreaScenario
{
    /// <summary>
    /// Interaction logic for AddInflowOutflowToCondition.xaml
    /// </summary>
    public partial class AddInflowOutflowToIAS : UserControl
    {
        public AddInflowOutflowToIAS()
        {
            InitializeComponent();
        }

        private void btn_NewInflowOutflow_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.AddInflowOutflowToIASVM vm = (ViewModel.ImpactAreaScenario.AddInflowOutflowToIASVM)this.DataContext;
            vm.NewInflowOutflowCurve(sender, e);
            
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.AddInflowOutflowToIASVM vm = (ViewModel.ImpactAreaScenario.AddInflowOutflowToIASVM)this.DataContext;

            if (vm.IsPoppedOut == true)
            {
                TabController.Instance.CloseTabOrWindow(this);
            }
            vm.OKClicked();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.AddInflowOutflowToIASVM vm = (ViewModel.ImpactAreaScenario.AddInflowOutflowToIASVM)this.DataContext;

            if (vm.IsPoppedOut == true)
            {
                TabController.Instance.CloseTabOrWindow(this);
            }
            vm.CancelClicked();
        }

        private void btn_PopOutImporter_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.AddInflowOutflowToIASVM vm = (ViewModel.ImpactAreaScenario.AddInflowOutflowToIASVM)this.DataContext;
            vm.PopTheImporterOut();
        }
    }
}
