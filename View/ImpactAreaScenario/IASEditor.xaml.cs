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
    /// Interaction logic for ImpactAreaScenario.xaml
    /// </summary>
    public partial class IASEditor : UserControl
    {
        public IASEditor()
        {
            InitializeComponent();
            
        }

        private void ShowInflowOutflow_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.ShowSelectedInflowOutflowCurve(sender, e);
        }
        private void ShowExteriorInterior_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.ShowSelectedExteriorInteriorCurve(sender, e);
        }
        private void ShowStageDamage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.ShowSelectedDamageCurve(sender, e);
        }
        private void ShowLateralStructure_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.ShowSelectedLateralStructure(sender, e);
        }
        private void ShowFailureFunction_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.ShowSelectedFailureFunctionCurve(sender, e);
        }
        private void ShowRatingCurve_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.ShowSelectedRatingCurve(sender, e);
        }

        private void ShowAnalyticalFlowFreq_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.ShowSelectedFrequencyCurve(sender, e);
        }




        private void NewAnalyticalFlowFreq_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.NewFrequencyCurve(sender, e);
        }

        private void NewInflowOutflowCurve_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.NewInflowOutflowCurve(sender, e);
        }

        private void NewExteriorInteriorCurve_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.NewExtIntStageCurve(sender, e);
        }
        private void NewLateralStructure_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.NewLeveeFeature(sender, e);
        }

        private void NewFailureFunction_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.NewFailureFunctionCurve(sender, e);
        }
        private void NewRatingCurve_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.NewRatingCurve(sender, e);
        }

        private void NewStageDamage_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.NewDamageCurve(sender, e);
        }

        private void NewStructureInventory_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.NewStructureInventory(sender, e);
        }

        private void ShowStructureInventory_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ImpactAreaScenario.IASEditorVM vm = (ViewModel.ImpactAreaScenario.IASEditorVM)DataContext;
            vm.ShowStructureInventory(sender, e);
        }
    }
}
