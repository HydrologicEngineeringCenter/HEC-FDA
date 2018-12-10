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
    /// Interaction logic for AddStageDamageToCondition.xaml
    /// </summary>
    public partial class AddStageDamageToCondition : UserControl
    {
        public AddStageDamageToCondition()
        {
            InitializeComponent();
        }

        private void btn_NewWaterSurfaceElevation_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddStageDamageToConditionVM vm = (FdaViewModel.Conditions.AddStageDamageToConditionVM)this.DataContext;
            vm.LaunchNewWaterSurfaceElevation();
        }

        private void btn_StructureInventory_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddStageDamageToConditionVM vm = (FdaViewModel.Conditions.AddStageDamageToConditionVM)this.DataContext;
            vm.LaunchNewStructureInventory();
        }

        private void btn_TerrainFile_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddStageDamageToConditionVM vm = (FdaViewModel.Conditions.AddStageDamageToConditionVM)this.DataContext;
            vm.LaunchNewTerrainFile();
        }

        private void btn_OccupancyType_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddStageDamageToConditionVM vm = (FdaViewModel.Conditions.AddStageDamageToConditionVM)this.DataContext;
            vm.LaunchNewOccupancyType();
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddStageDamageToConditionVM vm = (FdaViewModel.Conditions.AddStageDamageToConditionVM)this.DataContext;
            if (vm.IsPoppedOut == true)
            {
                var window = Window.GetWindow(this);
                //vm.WasCanceled = true;
                //window.Close();
                if (window is ViewWindow)
                {

                    FdaViewModel.Utilities.WindowVM winVM = (FdaViewModel.Utilities.WindowVM)window.DataContext;
                    if (winVM.StudyVM != null) //then it is a tab not a seperate window
                    {
                        if (winVM.StudyVM.SelectedDynamicTabIndex != -1)
                        {
                            winVM.StudyVM.RemoveTabAtIndex(winVM.StudyVM.SelectedDynamicTabIndex);
                        }
                        else
                        {
                            window.Close();
                        }
                    }
                    else
                    {
                        window.Close();
                    }

                }
                else
                {
                    window.Close();
                }
            }
            vm.OKClicked();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddStageDamageToConditionVM vm = (FdaViewModel.Conditions.AddStageDamageToConditionVM)this.DataContext;
            if (vm.IsPoppedOut == true)
            {
                var window = Window.GetWindow(this);
                //vm.WasCanceled = true;
                //window.Close();
                if (window is ViewWindow)
                {

                    FdaViewModel.Utilities.WindowVM winVM = (FdaViewModel.Utilities.WindowVM)window.DataContext;
                    if (winVM.StudyVM != null) //then it is a tab not a seperate window
                    {
                        if (winVM.StudyVM.SelectedDynamicTabIndex != -1)
                        {
                            winVM.StudyVM.RemoveTabAtIndex(winVM.StudyVM.SelectedDynamicTabIndex);
                        }
                        else
                        {
                            window.Close();
                        }
                    }
                    else
                    {
                        window.Close();
                    }

                }
                else
                {
                    window.Close();
                }
            }
            vm.CancelClicked();
        }

        private void btn_NewStageDamage_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddStageDamageToConditionVM vm = (FdaViewModel.Conditions.AddStageDamageToConditionVM)this.DataContext;
            vm.LaunchNewStageDamage(sender, e);
        }

        private void btn_PopOutCurve_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddStageDamageToConditionVM vm = (FdaViewModel.Conditions.AddStageDamageToConditionVM)this.DataContext;
            vm.PopTheImporterOut();
        }
    }
}
