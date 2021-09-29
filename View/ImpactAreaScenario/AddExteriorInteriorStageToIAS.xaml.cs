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
using ViewModel.ImpactAreaScenario;

namespace View.ImpactAreaScenario
{
    /// <summary>
    /// Interaction logic for AddExteriorInteriorStageToCondition.xaml
    /// </summary>
    public partial class AddExteriorInteriorStageToIAS : UserControl
    {
        public AddExteriorInteriorStageToIAS()
        {
            InitializeComponent();
        }

      

        private void btn_NewExteriorInteriorStage_Click(object sender, RoutedEventArgs e)
        {
            AddExteriorInteriorStageToIASVM vm = (AddExteriorInteriorStageToIASVM)this.DataContext;
            vm.LaunchNewExteriorInteriorStage(sender, e);
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {

            AddExteriorInteriorStageToIASVM vm = (AddExteriorInteriorStageToIASVM)this.DataContext;

            if (vm.IsPoppedOut == true)
            {
                TabController.Instance.CloseTabOrWindow(this);
            }
            vm.OKClicked();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            AddExteriorInteriorStageToIASVM vm = (AddExteriorInteriorStageToIASVM)this.DataContext;

            if (vm.IsPoppedOut == true)
            {
                TabController.Instance.CloseTabOrWindow(this);
            }
            vm.CancelClicked();

        }


        private void btn_PopOutImporter_Click(object sender, RoutedEventArgs e)
        {
            AddExteriorInteriorStageToIASVM vm = (AddExteriorInteriorStageToIASVM)this.DataContext;
            vm.PopTheImporterOut();
        }


    }
}
