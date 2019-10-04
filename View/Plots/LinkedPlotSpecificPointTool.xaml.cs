using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Fda.Plots
{
    /// <summary>
    /// Interaction logic for LinkedPlotSpecificPointTool.xaml
    /// </summary>
    public partial class LinkedPlotSpecificPointTool : UserControl
    {
        public event EventHandler PlotSpecifiedPoint;
        public event EventHandler OutsideTheRangeAttempted;

        private double _SpecifiedXValue;
        private double _SpecifiedYValue;

        public static readonly DependencyProperty AvailablePlotsFromVMProperty = DependencyProperty.Register("AvailablePlotsFromVM", typeof(ObservableCollection<FdaViewModel.Plots.IndividualLinkedPlotVM>), typeof(LinkedPlotSpecificPointTool), new FrameworkPropertyMetadata(new ObservableCollection<FdaViewModel.Plots.IndividualLinkedPlotVM>(), new PropertyChangedCallback(AvailablePlotsFromVMCallBack)));
        //public static readonly DependencyProperty AvailablePlotsFromViewProperty = DependencyProperty.Register("AvailablePlotsFromView", typeof(ObservableCollection<ILinkedPlot>), typeof(LinkedPlotSpecificPointTool), new FrameworkPropertyMetadata(new ObservableCollection<ILinkedPlot>(), new PropertyChangedCallback(AvailablePlotsFromViewCallBack)));


        public ObservableCollection<FdaViewModel.Plots.IndividualLinkedPlotVM> AvailablePlotsFromVM
        {
            get { return (ObservableCollection<FdaViewModel.Plots.IndividualLinkedPlotVM>)GetValue(AvailablePlotsFromVMProperty); }
            set { SetValue(AvailablePlotsFromVMProperty, value); }
        }
        public ObservableCollection<ILinkedPlot> AvailablePlotsFromView
        {
            //get { return (ObservableCollection<ILinkedPlot>)GetValue(AvailablePlotsFromViewProperty); }
            //set { SetValue(AvailablePlotsFromViewProperty, value); }
            get;set;
        }
        public IndividualLinkedPlot SelectedPlot { get; set; }
        //public double SpecifiedXValue { get; set; }
        //public double SpecifiedYValue { get; set; }
        public LinkedPlotSpecificPointTool()
        {
            InitializeComponent();
        }


        private static void AvailablePlotsFromVMCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinkedPlotSpecificPointTool owner = d as LinkedPlotSpecificPointTool;
            owner.AvailablePlotsFromVM = (ObservableCollection<FdaViewModel.Plots.IndividualLinkedPlotVM>)e.NewValue;
          
        }
        //private static void AvailablePlotsFromViewCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    LinkedPlotSpecificPointTool owner = d as LinkedPlotSpecificPointTool;
        //    owner.AvailablePlotsFromView = (ObservableCollection<ILinkedPlot>)e.NewValue;

        //}


        private void txt_XValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cmb_PlotNames.SelectedItem == null) { return; }
            FdaViewModel.Plots.IndividualLinkedPlotVM selectedPlot = (FdaViewModel.Plots.IndividualLinkedPlotVM)cmb_PlotNames.SelectedItem;
            //i need to find the corresponding individualLinkedPlot
            foreach (ILinkedPlot plot in AvailablePlotsFromView)
            {
                if (plot.BaseFunction.FunctionType == selectedPlot.BaseFunction.FunctionType)
                {
                    SelectedPlot = (IndividualLinkedPlot)plot;
                    break;
                }
            }


            if (SelectedPlot == null) { return; }
            // if (cmb_PlotNames.SelectedIndex == -1) { return; }
            //_SelectedPlot = (IndividualLinkedPlot)cmb_PlotNames.SelectedItem;
            // _SelectedPlot.TrackerIsOutsideTheCurveRange = false;
            SelectedPlot.TurnOutsideOfRangeOff();
           // FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM vm = (FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM)SelectedPlot.DataContext;
            //vm.PlotIsInsideRange(this, new EventArgs());

            if (SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency ||
                    SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency ||
                    SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
            {
                GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(true, true);
            }
            else
            {
                GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(false, true);

            }
            ScreenPoint position = SelectedPlot.OxyPlot1.Model.Axes[0].Transform(_SpecifiedXValue, _SpecifiedYValue, SelectedPlot.OxyPlot1.Model.Axes[1]);

            //if the trackers are already frozen, unfreeze them
            if (SelectedPlot.FreezeNextTracker == true)
            {
                SelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
            }
            SelectedPlot.DisplayTheTrackers(position);
            //now that the trackers are displayed, freeze them
            if (SelectedPlot.FreezeNextTracker == false)
            {
                SelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
            }

        }

        private void txt_YValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cmb_PlotNames.SelectedItem == null) { return; }

           // if (cmb_PlotNames.SelectedIndex == -1) { return; }
           // _SelectedPlot = (IndividualLinkedPlot)cmb_PlotNames.SelectedItem;
            FdaViewModel.Plots.IndividualLinkedPlotVM control = (FdaViewModel.Plots.IndividualLinkedPlotVM)cmb_PlotNames.SelectedItem;
           // i need to find the corresponding individualLinkedPlot
            foreach (ILinkedPlot plot in AvailablePlotsFromView)
            {
                if (plot.BaseFunction.FunctionType == control.BaseFunction.FunctionType)
                {
                    SelectedPlot = (IndividualLinkedPlot)plot;
                    break;
                }
            }

            if (SelectedPlot == null) { return; }
            SelectedPlot.TurnOutsideOfRangeOff();
            //FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM vm = (FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM)SelectedPlot.DataContext;
            //vm.PlotIsInsideRange(this, new EventArgs());

            if (SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency ||
                    SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency ||
                    SelectedPlot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
            {
                GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(true, false);
            }
            else
            {
                GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(false, false);

            }
            ScreenPoint position = SelectedPlot.OxyPlot1.Model.Axes[0].Transform(_SpecifiedXValue, _SpecifiedYValue, SelectedPlot.OxyPlot1.Model.Axes[1]);

            //if the trackers are already frozen, unfreeze them
            if (SelectedPlot.FreezeNextTracker == true)
            {
                SelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
            }
            SelectedPlot.DisplayTheTrackers(position);
            //now that the trackers are displayed, freeze them
            if (SelectedPlot.FreezeNextTracker == false)
            {
                SelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
            }
        }




        private void GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(bool isAxisReversed, bool plotBasedOnXValue)
        {
            //try
            //{
            //if (_SelectedPlotControl)
            //if(_SelectedPlotControl.Content.GetType() != typeof(ConditionsIndividualPlotWrapper)) { return; }

            // ConditionsIndividualPlotWrapper theSelectedPlotWrapper = ((ConditionsIndividualPlotWrapper)_SelectedPlotControl.Content);
            //IndividualLinkedPlot theSelectedPlot = theSelectedPlotWrapper.LinkedPlot;

            //if (txt_XValue.Text == null || txt_XValue.Text == "")
            if (plotBasedOnXValue == false)
            {
                //if (txt_YValue.Text == null || txt_YValue.Text == "")
                {
                    //do nothing, there are no values
                }
                // else
                {
                    //x has nothing, and y has some number
                    if (Double.TryParse(txt_YValue.Text, out _SpecifiedYValue) == false) { return; }
                    //_SpecifiedYValue = Convert.ToDouble(txt_YValue.Text);
                    _SpecifiedXValue = SelectedPlot.GetPairedValue(_SpecifiedYValue, false, SelectedPlot.OxyPlot1.Model, isAxisReversed);
                    if (_SpecifiedYValue > SelectedPlot.OxyPlot1.Model.Axes[1].Maximum || _SpecifiedYValue < SelectedPlot.OxyPlot1.Model.Axes[1].Minimum)
                    {
                        //FdaViewModel.Utilities.CustomMessageBoxVM vm = new FdaViewModel.Utilities.CustomMessageBoxVM(FdaViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "Y Value is out of range");
                        MessageBox.Show("Y Value is out of range", "Out of Range");
                        txt_YValue.Text = "";
                        return;
                    }
                    txt_XValue.Text = Math.Round(_SpecifiedXValue, 3).ToString();
                }
            }
            else
            {
                //x has a number and we don't care what y is

                if (Double.TryParse(txt_XValue.Text, out _SpecifiedXValue) == false)
                {
                    //MessageBox.Show("X Value is not a valid number", "Not A Number");
                    txt_XValue.Text = "";
                    return;
                }
                //_SpecifiedXValue = Convert.ToDouble(txt_XValue.Text);

                //I need a way to know which plot this is so that i can put the right arguments into this method
                _SpecifiedYValue = ((IndividualLinkedPlot)SelectedPlot).GetPairedValue(_SpecifiedXValue, true, SelectedPlot.OxyPlot1.Model, isAxisReversed);
                if (_SpecifiedXValue > SelectedPlot.OxyPlot1.Model.Axes[0].Maximum || _SpecifiedXValue < SelectedPlot.OxyPlot1.Model.Axes[0].Minimum)
                {
                    //FdaViewModel.Utilities.CustomMessageBoxVM vm = new FdaViewModel.Utilities.CustomMessageBoxVM(FdaViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "X Value is out of range");
                    MessageBox.Show("X Value is out of range", "Out of Range");
                    txt_XValue.Text = "";
                    return;
                }
                txt_YValue.Text = Math.Round(_SpecifiedYValue, 3).ToString();
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cmb_PlotNames.ItemsSource = AvailablePlotsFromVM;
        }
    }
}
