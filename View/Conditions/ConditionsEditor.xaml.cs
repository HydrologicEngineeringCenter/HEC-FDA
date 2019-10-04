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

namespace Fda.Conditions
{
    /// <summary>
    /// Interaction logic for Conditions.xaml
    /// </summary>
    public partial class ConditionsEditor : UserControl
    {
        public ConditionsEditor()
        {
            InitializeComponent();
            
        }

        private void ShowInflowOutflow_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.ShowSelectedInflowOutflowCurve(sender, e);
        }
        private void ShowExteriorInterior_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.ShowSelectedExteriorInteriorCurve(sender, e);
        }
        private void ShowStageDamage_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.ShowSelectedDamageCurve(sender, e);
        }
        private void ShowLateralStructure_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.ShowSelectedLateralStructure(sender, e);
        }
        private void ShowFailureFunction_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.ShowSelectedFailureFunctionCurve(sender, e);
        }
        private void ShowRatingCurve_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.ShowSelectedRatingCurve(sender, e);
        }

        private void ShowAnalyticalFlowFreq_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.ShowSelectedFrequencyCurve(sender, e);
        }




        private void NewAnalyticalFlowFreq_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.NewFrequencyCurve(sender, e);
        }

        private void NewInflowOutflowCurve_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.NewInflowOutflowCurve(sender, e);
        }

        private void NewExteriorInteriorCurve_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.NewExtIntStageCurve(sender, e);
        }
        private void NewLateralStructure_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.NewLeveeFeature(sender, e);
        }

        private void NewFailureFunction_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.NewFailureFunctionCurve(sender, e);
        }
        private void NewRatingCurve_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.NewRatingCurve(sender, e);
        }

        private void NewStageDamage_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.NewDamageCurve(sender, e);
        }

        private void NewStructureInventory_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.NewStructureInventory(sender, e);
        }

        private void ShowStructureInventory_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.ConditionsEditorVM vm = (FdaViewModel.Conditions.ConditionsEditorVM)DataContext;
            vm.ShowStructureInventory(sender, e);
        }
    }
}
