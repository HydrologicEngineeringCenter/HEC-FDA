using FdaViewModel.Tabs;
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

namespace View.Conditions
{
    /// <summary>
    /// Interaction logic for AddFlowFrequencyToCondition.xaml
    /// </summary>
    public partial class AddFlowFrequencyToCondition : UserControl
    {

        public AddFlowFrequencyToCondition()
        {
            InitializeComponent();
            cmb_FrequencyType.SelectedIndex = 0;
            
        }

        //private void rad_FlowFreq_Checked(object sender, RoutedEventArgs e)
        //{
        //    if(rad_FlowFreq.IsChecked == true)
        //    {
        //        //Uncollapse row 1
        //        //grid_main.RowDefinitions[1].Height = new GridLength(80);
        //        //grid_FlowFreq.Visibility = Visibility.Visible;
        //    }
        //}

        //private void rad_StageFreq_Checked(object sender, RoutedEventArgs e)
        //{
        //    //collapse row 1
        //    //grid_main.RowDefinitions[1].Height = new GridLength(0);
        //    //grid_FlowFreq.Visibility = Visibility.Hidden;

        //}

        private void rad_InflowFreq_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void rad_OutflowFreq_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btn_NewInflow_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddFlowFrequencyToConditionVM vm = (FdaViewModel.Conditions.AddFlowFrequencyToConditionVM)this.DataContext;
            vm.LauchNewFlowFrequency(sender, e);
        }

        private void btn_NewOutflowFreq_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_NewStageFreq_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
           
            FdaViewModel.Conditions.AddFlowFrequencyToConditionVM vm = (FdaViewModel.Conditions.AddFlowFrequencyToConditionVM)this.DataContext;
            
            if (vm.IsPoppedOut == true)
            {
                TabController.Instance.CloseTabOrWindow(this);

            }
            vm.OKClicked();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddFlowFrequencyToConditionVM vm = (FdaViewModel.Conditions.AddFlowFrequencyToConditionVM)this.DataContext;

            if (vm.IsPoppedOut == true)
            {
                TabController.Instance.CloseTabOrWindow(this);
            }
            vm.CancelClicked();
           
        }


        private void btn_PopOutImporter_Click(object sender, RoutedEventArgs e)
        {
            FdaViewModel.Conditions.AddFlowFrequencyToConditionVM vm = (FdaViewModel.Conditions.AddFlowFrequencyToConditionVM)this.DataContext;
            vm.PopTheImporterOut();
        }

        private void cmb_FrequencyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmb_FrequencyType.SelectedIndex == 1)
            {
                grid_FlowFreq.Visibility = Visibility.Collapsed;
                stk_StageFreq.Visibility = Visibility.Visible;
            }
            else if(cmb_FrequencyType.SelectedIndex == 0)
            {
                grid_FlowFreq.Visibility = Visibility.Visible;
                stk_StageFreq.Visibility = Visibility.Collapsed;
            }
        }

   
    }
}
