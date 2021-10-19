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
using ViewModel.Utilities;

namespace View.ImpactAreaScenario.Editor
{
    /// <summary>
    /// Interaction logic for IASEditor.xaml
    /// </summary>
    public partial class IASEditor : UserControl
    {
       

        public IASEditor()
        {
            InitializeComponent();
        }

        private void addThresholdBtn_Click(object sender, RoutedEventArgs e)
        {
            IASEditorVM vm = (IASEditorVM)this.DataContext;
            vm.AddThresholds();
        }

        //private void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    IASEditorVM vm = (IASEditorVM)this.DataContext;

        //    ////flow frequency
        //    //vm.FrequencyRelationshipControl.ChartVM = new SciChart2DChartViewModel(vm.FrequencyRelationshipControl.ChartVM);
        //    //_flowFreqChart = new Chart2D(vm.FrequencyRelationshipControl.ChartVM);

        //    ////rating
        //    //vm.RatingRelationshipControl.ChartVM = new SciChart2DChartViewModel(vm.RatingRelationshipControl.ChartVM);
        //    //_ratingChart = new Chart2D(vm.RatingRelationshipControl.ChartVM);

        //    ////stage damage
        //    //vm.StageDamageControl.ChartVM = new SciChart2DChartViewModel(vm.StageDamageControl.ChartVM);
        //    //_stageDamageChart = new Chart2D(vm.StageDamageControl.ChartVM);

        //    ////damage freq
        //    //vm.DamageFrequencyControl.ChartVM = new SciChart2DChartViewModel(vm.DamageFrequencyControl.ChartVM);
        //    //_damageFreqChart = new Chart2D(vm.DamageFrequencyControl.ChartVM);

        //}

       

        //private void plotBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    SpecificIASEditorVM vm = (SpecificIASEditorVM)this.DataContext;
        //    //if (vm.IsValid())
        //    {

        //        if (!_plotsHaveBeenAdded)
        //        {
        //            //flow frequency                
        //            PlotsGrid.Children.Add(_flowFreqChart);
        //            Grid.SetRow(_flowFreqChart, 0);
        //            Grid.SetColumn(_flowFreqChart, 1);

        //            //rating
        //            PlotsGrid.Children.Add(_ratingChart);
        //            Grid.SetRow(_ratingChart, 0);
        //            Grid.SetColumn(_ratingChart, 0);

        //            //stage damage
        //            PlotsGrid.Children.Add(_stageDamageChart);
        //            Grid.SetRow(_stageDamageChart, 1);
        //            Grid.SetColumn(_stageDamageChart, 0);

        //            //damage freq
        //            PlotsGrid.Children.Add(_damageFreqChart);
        //            Grid.SetRow(_damageFreqChart, 1);
        //            Grid.SetColumn(_damageFreqChart, 1);

        //            _plotsHaveBeenAdded = true;
        //        }


        //        //todo: link the charts everytime or only once?
        //        LinkTheCharts();
        //        //vm.Plot();
        //    }
        //}

        

        


    }
}
