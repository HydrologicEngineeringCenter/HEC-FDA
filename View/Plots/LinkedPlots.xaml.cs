using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Fda.Output;
using System.Collections.ObjectModel;

namespace Fda.Plots
{
    /// <summary>
    /// Interaction logic for LinkedPlots.xaml
    /// </summary>
    public partial class LinkedPlots : UserControl
    {
        public bool HideTrackers { get; set; }
        public bool Plot1PoppedOut { get; set; }
        public bool Plot1DoesntExist { get; set; }
        public bool Plot5PoppedOut { get; set; }
        public bool Plot5DoesntExist { get; set; }
        public bool AreaPlotsHaveBeenRemoved { get; set; }

        public LinkedPlots()
        {
            InitializeComponent();

        }

        private void SetTheLinkages()
        {
            ViewModel.Plots.LinkedPlotsVM vm = (ViewModel.Plots.LinkedPlotsVM)this.DataContext;

            doubleLineModulator.BaseFunction = plot1.BaseFunction;
            DoubleLineHorizontal.BaseFunction = plot5.BaseFunction;

            doubleLineModulator.PopOutThePlot += new EventHandler(PopPlot1Out);
            DoubleLineHorizontal.PopOutThePlot += new EventHandler(PopPlot5Out);

            //I am assuming that we will always have a valid plot 0 here?
            plot0.SetAsStartNode();
            if (vm.Plot1VM != null && vm.Plot1VM.BaseFunction != null)
            {
                plot0.SetNextPlotLinkage(doubleLineModulator);
                doubleLineModulator.SetNextPlotLinkage(plot3);
                doubleLineModulator.SetPreviousPlotLinkage(plot0);
            }
            else
            {
                plot0.SetNextPlotLinkage(plot3);
            }
            //plot0.SetPreviousPlotLinkage(plot8);


            //plot1.SetNextPlotLinkage(plot3, "y", "y");
            //plot1.SetPreviousPlotLinkage(plot0);

            if (vm.Plot5VM.BaseFunction != null)
            {
                plot3.SetNextPlotLinkage(DoubleLineHorizontal);
                if (vm.Plot1VM.BaseFunction != null)
                {
                    plot3.SetPreviousPlotLinkage(doubleLineModulator);
                }
                else
                {
                    plot3.SetPreviousPlotLinkage(plot0);
                }

                DoubleLineHorizontal.SetNextPlotLinkage(plot7);
                DoubleLineHorizontal.SetPreviousPlotLinkage(plot3);

                plot7.SetPreviousPlotLinkage(DoubleLineHorizontal);

            }
            else
            {

                plot3.SetNextPlotLinkage(plot7);
                if (vm.Plot1VM.BaseFunction != null)
                {
                    plot3.SetPreviousPlotLinkage(doubleLineModulator);
                }
                else
                {
                    plot3.SetPreviousPlotLinkage(plot0);
                }


                plot7.SetPreviousPlotLinkage(plot3);
            }


            plot7.SetNextPlotLinkage(plot8);



            // plot8.SetNextPlotLinkage(plot0, "x", "x");
            plot8.SetPreviousPlotLinkage(plot7);
            //plot8.SetNextPlotLinkage(plot0, "x", "x");
            plot8.SetAsEndNode();



            //UpdatePlotVisibility();
            //List<ILinkedPlot> includedPlots = new List<ILinkedPlot>() { plot0, plot3, plot7, plot8 };
            //if(Plot1DoesntExist == false)
            //{
            //    includedPlots.Add(plot1);
            //}
            //if(Plot5DoesntExist == false)
            //{
            //    includedPlots.Add(plot5);
            //}

            //Conditions.ConditionsPlotEditor.UpdateTheLinkages(includedPlots);
            //SetTheSharedAxes();

        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            SetTheLinkages();
            //minimize plot 1 off the start
            //plot1.Visibility = Visibility.Collapsed;
            //mainGrid.RowDefinitions[0].Height = new GridLength(0);
            //mainGrid.RowDefinitions[1].Height = new GridLength(0);

            //sets the min and max values for the shared axes of the two shared plots
            SetTheSharedAxes();
            //plot8.SetSharedXAxisWithPlot(plot0); // make sure you never double set. O was already setting with 8


            UpdatePlotVisibility();

            

            plot8.PlotAreaUnderTheCurve();

            

            ObservableCollection<ILinkedPlot> availablePlots = new ObservableCollection<ILinkedPlot>();           

            availablePlots.Add(plot0);
            if (Plot1DoesntExist == false)
            {
                availablePlots.Add(plot1);
            }
            availablePlots.Add(plot3);
            if (Plot5DoesntExist == false)
            {
                availablePlots.Add(plot5);
            }
            availablePlots.Add(plot7);
            availablePlots.Add(plot8);
            
            PlotSpecificPoint.AvailablePlotsFromView = availablePlots;

            //this is all kinds of bad i think. The whole system of getting the names into the vm and into here
            //needs work i think. but for now...
            //some of the plots can be created when running the compute. At least plot 5 can (ext int stage)
            ViewModel.Plots.LinkedPlotsVM vm = (ViewModel.Plots.LinkedPlotsVM)this.DataContext;
            List<string> selectedElementNames = vm.SelectedElementNames;
            if(availablePlots.Count == selectedElementNames.Count)
            {
                for(int i = 0;i<selectedElementNames.Count;i++)
                {
                    ((IndividualLinkedPlot)availablePlots[i]).OxyPlot1.Model.Subtitle = selectedElementNames[i];
                }
            }

            FdaModel.ComputationPoint.PerformanceThreshold pt = new FdaModel.ComputationPoint.PerformanceThreshold(vm.ThresholdType, vm.ThresholdValue);
            plot7.PlotThreshold(pt);
            plot7.OxyPlot1.InvalidatePlot(true);
          
            lbl_Aep.Content = vm.AEP.ToString("N3");
            lbl_Ead.Content = vm.EAD.ToString("N3");
        }

        private void SetTheSharedAxes()
        {
            plot0.SetSharedYAxisWithPlot(plot3);
            plot3.SetSharedXAxisWithPlot(plot7);
            plot7.SetSharedYAxisWithPlot(plot8);
            plot8.SetSharedXAxisWithPlot(plot0);
        }

        private void UpdatePlotVisibility()
        {
            if (plot0.Curve.Count == 0)
            {
                plot0.Visibility = Visibility.Collapsed;
            }
            if (plot1.Curve.Count == 0)
            {
                plot1.Visibility = Visibility.Collapsed;
                Plot1DoesntExist = true;
                // if there is no plot here, collapse the grid rows 0 and 1
                //mainGrid.RowDefinitions[0].Height = new GridLength(0);
                //mainGrid.RowDefinitions[1].Height = new GridLength(0);


                ////restructure the linking. Plot0 now goes to plot 3
                //plot0.SetNextPlotLinkage(plot3, "y", "y");
                ////set previous plot
                //plot3.SetPreviousPlotLinkage(plot0);
                //turn off the doublelinemodulator
                FourPlotGrid.ColumnDefinitions[2].Width = new GridLength(0);

            }
            if (plot3.Curve.Count == 0)
            {
                plot3.Visibility = Visibility.Collapsed;
            }
            if (plot5.Curve.Count == 0)
            {
                plot5.Visibility = Visibility.Collapsed;
                Plot5DoesntExist = true;
                //reset the linking. plot3 now goes to plot7
                //plot3.SetNextPlotLinkage(plot7, "x", "x");
                //plot7.SetPreviousPlotLinkage(plot3);
                FourPlotGrid.RowDefinitions[2].Height = new GridLength(0);
            }
            if (plot7.Curve.Count == 0)
            {
                plot7.Visibility = Visibility.Collapsed;
            }
            if (plot8.Curve.Count == 0)
            {
                plot8.Visibility = Visibility.Collapsed;
            }
        }

        #region toolbar stuff

        #region Pop Plots In and Out
        private void PopPlot1Out(object sender, EventArgs e)
        {
            Plot1PoppedOut = true;
            if (Plot1DoesntExist == true) { return; }

            //collapse the column the modulator is in
            FourPlotGrid.ColumnDefinitions[2].Width = new GridLength(0);

            //break the linkage to the modulator and reset it to plot 1
            plot0.SetNextPlotLinkage(plot1);

            plot1.SetNextPlotLinkage(plot3);
            plot1.SetPreviousPlotLinkage(plot0);

            plot3.SetPreviousPlotLinkage(plot1);
            // expand the main grid row 0
            mainGrid.RowDefinitions[1].Height = new GridLength(.4, GridUnitType.Star);



        }

        public void PopPlot5Out(object sender, EventArgs e)
        {
            Plot5PoppedOut = true;
            //collapse the row the modulator is in
            FourPlotGrid.RowDefinitions[2].Height = new GridLength(0);

            //break the linkage to the modulator and reset it to plot 1
            plot3.SetNextPlotLinkage(plot5);

            plot5.SetNextPlotLinkage(plot7);
            plot5.SetPreviousPlotLinkage(plot3);

            plot7.SetPreviousPlotLinkage(plot5);
            // expand the main grid column 0
            mainGrid.ColumnDefinitions[0].Width = new GridLength(.45, GridUnitType.Star);

            //move the toolbar over
            Grid.SetColumn(grid_TopRow, 0);


        }

        private void btn_CollapsePlot1_Click(object sender, RoutedEventArgs e)
        {
            Plot1PoppedOut = false;
            if (Plot1DoesntExist == true) { return; }

            //collapse the column the modulator is in
            FourPlotGrid.ColumnDefinitions[2].Width = new GridLength(45);

            //break the linkage to the modulator and reset it to plot 1
            plot0.SetNextPlotLinkage(doubleLineModulator);

            doubleLineModulator.SetNextPlotLinkage(plot3);
            doubleLineModulator.SetPreviousPlotLinkage(plot0);

            plot3.SetPreviousPlotLinkage(doubleLineModulator);
            // expand the main grid row 0
            mainGrid.RowDefinitions[1].Height = new GridLength(0);
        }

        private void btn_CollapsePlot5_Click(object sender, RoutedEventArgs e)
        {
            Plot5PoppedOut = false;
            //collapse the row the modulator is in
            FourPlotGrid.RowDefinitions[2].Height = new GridLength(45);

            //break the linkage to the modulator and reset it to plot 1
            plot3.SetNextPlotLinkage(DoubleLineHorizontal);

            DoubleLineHorizontal.SetNextPlotLinkage(plot7);
            DoubleLineHorizontal.SetPreviousPlotLinkage(plot3);

            plot7.SetPreviousPlotLinkage(DoubleLineHorizontal);
            // expand the main grid column 0
            mainGrid.ColumnDefinitions[0].Width = new GridLength(0);
            //move the toolbar over
            Grid.SetColumn(grid_TopRow, 1);
        }

        //private void btn_PopPlotsOut_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Plot1PoppedOut == false || Plot5PoppedOut == false)
        //    {
        //        PopPlot1Out(sender, e);
        //        PopPlot5Out(sender, e);
        //        //change the image to pop in and change the tooltip
        //        img_PopPlotsOut.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/PopDown.png"));
        //        btn_PopPlotsOut.ToolTip = "Pop Plots In";
        //    }
        //    else
        //    {
        //        btn_CollapsePlot1_Click(sender, e);
        //        btn_CollapsePlot5_Click(sender, e);
        //        //change the image to pop out and change the tooltip
        //        img_PopPlotsOut.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/PopUp.png"));
        //        btn_PopPlotsOut.ToolTip = "Pop Plots Out";

        //    }
        //}

        #endregion

        private void btn_HideTrackers_Click(object sender, RoutedEventArgs e)
        {
            if (HideTrackers == false)
            {
                plot0.HideTracker();
                plot1.HideTracker();
                plot3.HideTracker();
                plot5.HideTracker();
                plot7.HideTracker();
                plot8.HideTracker();

                plot0.TurnOutsideOfRangeOff();
                plot1.TurnOutsideOfRangeOff();
                plot3.TurnOutsideOfRangeOff();
                plot5.TurnOutsideOfRangeOff();
                plot7.TurnOutsideOfRangeOff();
                plot8.TurnOutsideOfRangeOff();



                FourPlotGrid.ColumnDefinitions[2].Width = new GridLength(0);
                FourPlotGrid.RowDefinitions[2].Height = new GridLength(0);
                HideTrackers = true;

                //change the tracker button image and change the tooltip
                img_HideTrackers.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/ShowTrackers.png"));
                btn_HideTrackers.ToolTip = "Show Trackers";

            }
            else
            {
                plot0.ShowTracker();
                plot1.ShowTracker();
                plot3.ShowTracker();
                plot5.ShowTracker();
                plot7.ShowTracker();
                plot8.ShowTracker();
                if (Plot1DoesntExist == false) //if plot 1 exists
                {
                    if (Plot1PoppedOut == false)
                    {
                        FourPlotGrid.ColumnDefinitions[2].Width = new GridLength(45);
                    }
                }
                if (Plot5DoesntExist == false) //if plot 5 exists
                {
                    if (Plot5PoppedOut == false)
                    {
                        FourPlotGrid.RowDefinitions[2].Height = new GridLength(45);
                    }
                }
                HideTrackers = false;
                //change the tracker button image and change the tooltip
                img_HideTrackers.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/HideTrackers.png"));
                btn_HideTrackers.ToolTip = "Hide Trackers";

            }
        }


        #region Plot Specific Point
        //private void btn_PlotSpecificPoint_Click(object sender, RoutedEventArgs e)
        //{
        //    //if both the x and the y have values in them then just go with the x value
        //    //if neither of them have anything then don't do anything
        //    //message if value is not a double or is out of range

        //    if(cmb_PlotNames.SelectedIndex == -1) { return; }

        //    IndividualLinkedPlot theSelectedPlot = new IndividualLinkedPlot() ;
        //    double xValue = 0;
        //    double yValue = 0;

        //    if (cmb_PlotNames.SelectedItem.ToString() == plot0.Title)
        //    {
        //        theSelectedPlot = plot0;
        //        GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, true);

        //    }
        //    else if (cmb_PlotNames.SelectedItem.ToString() == plot1.Title)
        //    {
        //        //plot1 is only plugged into the loop when it is popped out, otherwise it is the doublelineModulator that is plugged in
        //        if (Plot1PoppedOut == true)
        //        {
        //            theSelectedPlot = plot1;
        //            GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, false);

        //        }
        //        else
        //        {
        //            //since plot1 is not popped out, the xvalue (inflow) is the same as plot0's yvalue. just plot that.
        //            if(txt_XValue.Text != null && txt_XValue.Text != "")
        //            {
        //                theSelectedPlot = plot0;
        //                txt_YValue.Text = txt_XValue.Text;
        //                txt_XValue.Text = "";
        //                GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, true);
        //                //above code works fine, but it is displaying the wrong x value for plot1(it is displaying plot0 x value)
        //                txt_XValue.Text = ((IndividualLinkedPlot)plot1).GetPairedValue(yValue, false, plot1.OxyPlot1.Model, false).ToString();

        //            }
        //            // if it is the y value that is sought after, that is the same as the y from plot 3
        //            else if (txt_YValue.Text != null && txt_YValue.Text != "")
        //            {
        //                theSelectedPlot = plot3;
        //                GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, true);
        //            }
        //        }
              
                
        //    }
        //    else if (cmb_PlotNames.SelectedItem.ToString() == plot3.Title)
        //    {
        //        theSelectedPlot = plot3;
        //        GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, false);
        //    }
        //    else if (cmb_PlotNames.SelectedItem.ToString() == plot5.Title)
        //    {
        //        //plot5 is only plugged into the loop when it is popped out, otherwise it is the horizontaldoublelineModulator that is plugged in
        //        if (Plot1PoppedOut == true)
        //        {
        //            theSelectedPlot = plot5;
        //            GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, false);
        //        }
        //    }
        //    else if (cmb_PlotNames.SelectedItem.ToString() == plot7.Title)
        //    {
        //        theSelectedPlot = plot7;
        //        GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, false);
        //    }
        //    else if (cmb_PlotNames.SelectedItem.ToString() == plot8.Title)
        //    {
        //        theSelectedPlot = plot8;
        //        GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, true);
        //    }


        //    ScreenPoint position = theSelectedPlot.OxyPlot1.Model.Axes[0].Transform(xValue, yValue, theSelectedPlot.OxyPlot1.Model.Axes[1]);

        //    //if the trackers are already frozen, unfreeze them
        //   if( theSelectedPlot.FreezeNextTracker == true)
        //    {
        //        theSelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
        //    }
        //    theSelectedPlot.DisplayTheTrackers(position);
        //    //now that the trackers are displayed, freeze them
        //    if (theSelectedPlot.FreezeNextTracker == false)
        //    {
        //        theSelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
        //    }

         



        //}


        //private void GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref double xValue,ref double yValue,IndividualLinkedPlot theSelectedPlot,bool isAxisReversed)
        //{
        //    if (txt_XValue.Text == null || txt_XValue.Text == "")
        //    {
        //        if (txt_YValue.Text == null || txt_YValue.Text == "")
        //        {
        //            //do nothing, there are no values
        //        }
        //        else
        //        {
        //            //x has nothing, and y has some number
        //            yValue = Convert.ToDouble(txt_YValue.Text);
        //            xValue = ((IndividualLinkedPlot)theSelectedPlot).GetPairedValue(yValue, false, theSelectedPlot.OxyPlot1.Model, isAxisReversed);
        //            if (yValue > theSelectedPlot.OxyPlot1.Model.Axes[1].Maximum || yValue < theSelectedPlot.OxyPlot1.Model.Axes[1].Minimum)
        //            {
        //                //ViewModel.Utilities.CustomMessageBoxVM vm = new ViewModel.Utilities.CustomMessageBoxVM(ViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "Y Value is out of range");
        //                MessageBox.Show("Y Value is out of range", "Out of Range");
        //                return;
        //            }
        //            txt_XValue.Text = Math.Round(xValue, 3).ToString();
        //        }
        //    }
        //    else
        //    {
        //        //x has a number and we don't care what y is
        //        xValue = Convert.ToDouble(txt_XValue.Text);
        //        yValue = ((IndividualLinkedPlot)theSelectedPlot).GetPairedValue(xValue, true, theSelectedPlot.OxyPlot1.Model, isAxisReversed);
        //        if (xValue > theSelectedPlot.OxyPlot1.Model.Axes[0].Maximum || xValue < theSelectedPlot.OxyPlot1.Model.Axes[0].Minimum)
        //        {
        //            //ViewModel.Utilities.CustomMessageBoxVM vm = new ViewModel.Utilities.CustomMessageBoxVM(ViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "X Value is out of range");
        //            MessageBox.Show("X Value is out of range", "Out of Range");
        //            return;
        //        }
        //        txt_YValue.Text = Math.Round(yValue, 3).ToString();
        //    }

        //}


        private void txt_XValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            double doubleValue = 0;
            if (double.TryParse(box.Text, out doubleValue) == false && box.Text != "-" && box.Text != ".")
            {
                if (box.Text.Length == 1) { box.Text = ""; }
            }


        }

        private void txt_XValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tBox = (TextBox)sender;
            if (!char.IsDigit(Convert.ToChar(e.Text))) { e.Handled = true; }//numeric only
            if (Convert.ToChar(e.Text) == (char)8) { e.Handled = false; }//allow backspace
            if (e.Text == " ") { e.Handled = true; }//don't allow spaces
            if (e.Text == "-" && tBox.SelectionStart == 0 && tBox.Text.IndexOf("-") == -1) { e.Handled = false; }//allow negative
            if (e.Text == "." && tBox.Text.IndexOf(".") == -1) { e.Handled = false; }//allow decimal


        }

        #endregion








        private void btn_ToggleAreaPlots_Click(object sender, RoutedEventArgs e)
        {
            if (AreaPlotsHaveBeenRemoved == false)
            {
                RemoveAreaPlotsFromButtonClick();
                //change the image and change the tooltip
                img_HideAreaPlots.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/ShowAreaPlots.png"));
                btn_ToggleAreaPlots.ToolTip = "Show Area Plots";
            }
            else
            {
                AddAreaPlots();
                //change the image and change the tooltip
                img_HideAreaPlots.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/HideAreaPlots.png"));
                btn_ToggleAreaPlots.ToolTip = "Hide Area Plots";
            }
        }

        private void RemoveAreaPlotsFromButtonClick()
        {
            plot0.RemoveAreaPlotsFromButtonClick();
            plot3.RemoveAreaPlotsFromButtonClick();
            plot7.RemoveAreaPlotsFromButtonClick();
            plot8.RemoveAreaPlotsFromButtonClick();
            AreaPlotsHaveBeenRemoved = true;
        }
        private void RemoveAreaPlots()
        {
            plot0.RemoveAreaPlots();
            plot3.RemoveAreaPlots();
            plot7.RemoveAreaPlots();
            plot8.RemoveAreaPlots();
            //AreaPlotsHaveBeenRemoved = true;
        }

        private void AddAreaPlots()
        {
            plot0.AddAreaPlots();
            plot3.AddAreaPlots();
            plot7.AddAreaPlots();
            plot8.AddAreaPlots();
            AreaPlotsHaveBeenRemoved = false;
        }

        #endregion

        private void btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Plots.LinkedPlotsVM vm = (ViewModel.Plots.LinkedPlotsVM)this.DataContext;
            //the current iteration is actually 1 less than what the vm returns because the value is bound
            //to the vm and i want it to start with 1
            int currentIteration = vm.IterationNumber-1;
            int prevIteration = currentIteration - 1;
            if (currentIteration >= 1)
            {
                vm.IterationNumber -= 1;
                PlotIteration(prevIteration);
            }
            if (prevIteration == 0)
            {
                btn_Prev.IsEnabled = false;
            }
            if (prevIteration < vm.Result.Realizations.Count - 1)
            {
                btn_Next.IsEnabled = true;
            }
        }


        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Plots.LinkedPlotsVM vm = (ViewModel.Plots.LinkedPlotsVM)this.DataContext;
            //the current iteration is actually 1 less than what the vm returns because the value is bound
            //to the vm and i want it to start with 1
            int currentIteration = vm.IterationNumber - 1;
            
            int nextIteration = currentIteration + 1;

            if (currentIteration < vm.Result.Realizations.Count - 1)
            {
                vm.IterationNumber += 1;
                PlotIteration(nextIteration);

            }
            if (nextIteration > 0)
            {
                btn_Prev.IsEnabled = true;
            }
            if (nextIteration == vm.Result.Realizations.Count - 1)
            {
                btn_Next.IsEnabled = false;
            }
        }

        private void ClearAreaSeriesLists()
        {
            plot0.ListOfRemovedAreaSeries.Clear();
            plot1.ListOfRemovedAreaSeries.Clear();
            plot3.ListOfRemovedAreaSeries.Clear();
            plot5.ListOfRemovedAreaSeries.Clear();
            plot7.ListOfRemovedAreaSeries.Clear();
            plot8.ListOfRemovedAreaSeries.Clear();
        }

        public void PlotIteration(int iteration)
        {
            if(AreaPlotsHaveBeenRemoved)
            {
                //if the user has toggled the area series off, then each plot stores its list of
                //removed area series. When the user toggles them on, it adds the removed area series
                //to the plot. If the user iterates to a new plot, however, inbetween the button clicks
                //then it will add the wrong area series to the plots
                ClearAreaSeriesLists();
            }

            //reasign the input functions
            ViewModel.Plots.LinkedPlotsVM vm = (ViewModel.Plots.LinkedPlotsVM)this.DataContext;
            //InputFunctions = vm.Result.Realizations[iteration].Functions;

            

            //clear all axes
            //plot0.OxyPlot1.Model.Axes.Clear();
            //plot1.OxyPlot1.Model.Axes.Clear();
            //plot3.OxyPlot1.Model.Axes.Clear();
            //plot5.OxyPlot1.Model.Axes.Clear();
            //plot7.OxyPlot1.Model.Axes.Clear();
            //plot8.OxyPlot1.Model.Axes.Clear();

            ////OxyPlotModel.Axes.Clear();
            ////OxyPlotModel2.Axes.Clear();
            ////OxyPlotModel3.Axes.Clear();
            ////OxyPlotModel4.Axes.Clear();


            ////clear all series
            //plot0.OxyPlot1.Model.Series.Clear();
            //plot1.OxyPlot1.Model.Series.Clear();
            //plot3.OxyPlot1.Model.Series.Clear();
            //plot5.OxyPlot1.Model.Series.Clear();
            //plot7.OxyPlot1.Model.Series.Clear();
            //plot8.OxyPlot1.Model.Series.Clear();

            //OxyPlotModel.Series.Clear();
            //OxyPlotModel4.Series.Clear();
            //OxyPlotModel2.Series.Clear();
            //OxyPlotModel3.Series.Clear();
            RemoveAreaPlots();
            vm.UpdateCurvesToIteration(iteration);
            //set the new axes values


            //ResetMinMaxValues();
            //IndividualLinkedPlot.FlipFreqAxis(plot8);// plot8.FlipFrequencyAxis = true;
            //IndividualLinkedPlot.FlipFreqAxis(plot0);

            //set the shared axes is what actually creates the area series
            SetTheSharedAxes();
            SetTheLinkages();
            UpdatePlotVisibility();


            //SetAxesValues();

            //create the new series
            //setUpPlot1();
            //setUpPlot2();
            //setUpPlot3();
            //setUpPlot4();

            //redraw all plots
            //plot0.OxyPlot1.Model.InvalidatePlot(true);
            //plot1.OxyPlot1.Model.InvalidatePlot(true);
            //plot3.OxyPlot1.Model.InvalidatePlot(true);
            //plot5.OxyPlot1.Model.InvalidatePlot(true);
            //plot7.OxyPlot1.Model.InvalidatePlot(true);
            //plot8.OxyPlot1.Model.InvalidatePlot(true);

            plot8.PlotAreaUnderTheCurve();
            FdaModel.ComputationPoint.PerformanceThreshold pt = new FdaModel.ComputationPoint.PerformanceThreshold(vm.ThresholdType, vm.ThresholdValue);
            plot7.PlotThreshold(pt);

            if (AreaPlotsHaveBeenRemoved)
            {
                //the area plots automatically get drawn if "areaplotvisibility" is true
                //so here we need to remove them
                RemoveAreaPlotsFromButtonClick();
            }

                //AddAreaPlots();
                plot0.OxyPlot1.InvalidatePlot(true);
            plot1.OxyPlot1.InvalidatePlot(true);
            plot3.OxyPlot1.InvalidatePlot(true);
            plot5.OxyPlot1.InvalidatePlot(true);
            plot7.OxyPlot1.InvalidatePlot(true);
            plot8.OxyPlot1.InvalidatePlot(true);

            //OxyPlotModel.InvalidatePlot(true);
            //OxyPlotModel2.InvalidatePlot(true);
            //OxyPlotModel3.InvalidatePlot(true);
            //OxyPlotModel4.InvalidatePlot(true);

            //lbl_IterationValues.Content = "AEP: " + Math.Round(1 - Result.Realizations[_CurrentIteration].AnnualExceedanceProbability, 3).ToString() + Environment.NewLine + "EAD: " + Math.Round(Result.Realizations[_CurrentIteration].ExpectedAnnualDamage, 0).ToString();


        }

        private void ResetMinMaxValues()
        {
            IndividualLinkedPlot.SetMinMaxValues(plot0);
            IndividualLinkedPlot.SetMinMaxValues(plot1);
            IndividualLinkedPlot.SetMinMaxValues(plot3);
            IndividualLinkedPlot.SetMinMaxValues(plot5);
            IndividualLinkedPlot.SetMinMaxValues(plot7);
            IndividualLinkedPlot.SetMinMaxValues(plot8);
        }

        private void btn_ViewMeanAEP_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Plots.LinkedPlotsVM vm = (ViewModel.Plots.LinkedPlotsVM)this.DataContext;
            HistogramViewer hv = new HistogramViewer(vm.Result, false);
            //hv.Owner = this;
            hv.Show();
        }

        private void btn_ViewMeanEAD_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Plots.LinkedPlotsVM vm = (ViewModel.Plots.LinkedPlotsVM)this.DataContext;
            HistogramViewer hv = new HistogramViewer(vm.Result, true);
            //hv.Owner = this;
            hv.Show();
        }

        //private void btn_DisplayIteration_Click(object sender, RoutedEventArgs e)
        //{
        //    ViewModel.Plots.LinkedPlotsVM vm = (ViewModel.Plots.LinkedPlotsVM)this.DataContext;
        //    int iteration;
        //    bool parseWorked = int.TryParse( txt_IterationNumber.Text, out iteration);
        //    if (parseWorked && iteration > 0 && iteration <= vm.TotalRealizations)
        //    {
        //        PlotIteration(iteration-1);
        //        //update the next and prev button visibility
        //        btn_Prev.IsEnabled = (iteration > 1);
        //        btn_Next.IsEnabled = (iteration < vm.TotalRealizations);

        //    }
        //}

        private void txt_IterationNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.Plots.LinkedPlotsVM vm = (ViewModel.Plots.LinkedPlotsVM)this.DataContext;
            int iteration;
            bool parseWorked = int.TryParse(txt_IterationNumber.Text, out iteration);
            if (parseWorked && iteration > 0 && iteration <= vm.TotalRealizations)
            {
                PlotIteration(iteration - 1);
                //update the next and prev button visibility
                btn_Prev.IsEnabled = (iteration > 1);
                btn_Next.IsEnabled = (iteration < vm.TotalRealizations);

            }
        }
    }
}
