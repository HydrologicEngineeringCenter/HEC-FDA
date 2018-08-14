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

            doubleLineModulator.PopOutThePlot += new EventHandler(PopPlot1Out);
            DoubleLineHorizontal.PopOutThePlot += new EventHandler(PopPlot5Out);
            //plot0.SetYAxisToLogarithmic = true;


            //set up the linking. I might need to do some magic to handle if some of the plots aren't there

            
            plot0.SetAsStartNode();
            // plot0.SetNextPlotLinkage(plot1, "y", "x");
             plot0.SetNextPlotLinkage(doubleLineModulator, "y", "x");

            //plot0.SetPreviousPlotLinkage(plot8);


            //plot1.SetNextPlotLinkage(plot3, "y", "y");
            //plot1.SetPreviousPlotLinkage(plot0);

            doubleLineModulator.SetNextPlotLinkage(plot3, "y", "y");
            doubleLineModulator.SetPreviousPlotLinkage(plot0);

            plot3.SetNextPlotLinkage(DoubleLineHorizontal, "x", "x");
            plot3.SetPreviousPlotLinkage(doubleLineModulator);



            DoubleLineHorizontal.SetNextPlotLinkage(plot7, "y", "x");
            DoubleLineHorizontal.SetPreviousPlotLinkage(plot3);

     

            plot7.SetNextPlotLinkage(plot8, "y", "y");
            plot7.SetPreviousPlotLinkage(DoubleLineHorizontal);

          

           // plot8.SetNextPlotLinkage(plot0, "x", "x");
            plot8.SetPreviousPlotLinkage(plot7);
            //plot8.SetNextPlotLinkage(plot0, "x", "x");
            plot8.SetAsEndNode();


          


        }

       
       
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            

            //minimize plot 1 off the start
            //plot1.Visibility = Visibility.Collapsed;
            //mainGrid.RowDefinitions[0].Height = new GridLength(0);
            //mainGrid.RowDefinitions[1].Height = new GridLength(0);

            //sets the min and max values for the shared axes of the two shared plots
            plot0.SetSharedXAxisWithPlot(plot8);
            plot0.SetSharedYAxisWithPlot(plot3);
            plot3.SetSharedXAxisWithPlot(plot7);
            plot7.SetSharedYAxisWithPlot(plot8);
            //plot8.SetSharedXAxisWithPlot(plot0); // make sure you never double set. O was already setting with 8

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


                //restructure the linking. Plot0 now goes to plot 3
                plot0.SetNextPlotLinkage(plot3, "y", "y");
                //set previous plot
                plot3.SetPreviousPlotLinkage(plot0);
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
                plot3.SetNextPlotLinkage(plot7, "x", "x");
                plot7.SetPreviousPlotLinkage(plot3);
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


            cmb_PlotNames.Items.Add(plot0.Title);
            if (Plot1DoesntExist == false)
            {
                cmb_PlotNames.Items.Add(plot1.Title);
            }
            cmb_PlotNames.Items.Add(plot3.Title);
            if (Plot5DoesntExist == false)
            {
                cmb_PlotNames.Items.Add(plot5.Title);
            }
            cmb_PlotNames.Items.Add(plot7.Title);
            cmb_PlotNames.Items.Add(plot8.Title);


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
            plot0.SetNextPlotLinkage(plot1, "y", "x");

            plot1.SetNextPlotLinkage(plot3, "y", "y");
            plot1.SetPreviousPlotLinkage(plot0);

            plot3.SetPreviousPlotLinkage(plot1);
            // expand the main grid row 0
            mainGrid.RowDefinitions[1].Height = new GridLength(.4, GridUnitType.Star);



        }

        private void PopPlot5Out(object sender, EventArgs e)
        {
            Plot5PoppedOut = true;
            //collapse the row the modulator is in
            FourPlotGrid.RowDefinitions[2].Height = new GridLength(0);

            //break the linkage to the modulator and reset it to plot 1
            plot3.SetNextPlotLinkage(plot5, "x", "x");

            plot5.SetNextPlotLinkage(plot7, "y", "x");
            plot5.SetPreviousPlotLinkage(plot3);

            plot7.SetPreviousPlotLinkage(plot5);
            // expand the main grid column 0
            mainGrid.ColumnDefinitions[0].Width = new GridLength(.45, GridUnitType.Star);

            //move the toolbar over
            Grid.SetColumn(tool_toolbar, 0);


        }

        private void btn_CollapsePlot1_Click(object sender, RoutedEventArgs e)
        {
            Plot1PoppedOut = false;
            if (Plot1DoesntExist == true) { return; }

            //collapse the column the modulator is in
            FourPlotGrid.ColumnDefinitions[2].Width = new GridLength(45);

            //break the linkage to the modulator and reset it to plot 1
            plot0.SetNextPlotLinkage(doubleLineModulator, "y", "x");

            doubleLineModulator.SetNextPlotLinkage(plot3, "y", "y");
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
            plot3.SetNextPlotLinkage(DoubleLineHorizontal, "x", "x");

            DoubleLineHorizontal.SetNextPlotLinkage(plot7, "y", "x");
            DoubleLineHorizontal.SetPreviousPlotLinkage(plot3);

            plot7.SetPreviousPlotLinkage(DoubleLineHorizontal);
            // expand the main grid column 0
            mainGrid.ColumnDefinitions[0].Width = new GridLength(0);
            //move the toolbar over
            Grid.SetColumn(tool_toolbar, 1);
        }

        private void btn_PopPlotsOut_Click(object sender, RoutedEventArgs e)
        {
            if (Plot1PoppedOut == false || Plot5PoppedOut == false)
            {
                PopPlot1Out(sender, e);
                PopPlot5Out(sender, e);
                //change the image to pop in and change the tooltip
                img_PopPlotsOut.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/PopDown.png"));
                btn_PopPlotsOut.ToolTip = "Pop Plots In";
            }
            else
            {
                btn_CollapsePlot1_Click(sender, e);
                btn_CollapsePlot5_Click(sender, e);
                //change the image to pop out and change the tooltip
                img_PopPlotsOut.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/PopUp.png"));
                btn_PopPlotsOut.ToolTip = "Pop Plots Out";

            }
        }

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
                if (Plot1PoppedOut == false)
                {
                    FourPlotGrid.ColumnDefinitions[2].Width = new GridLength(45);
                }
                if (Plot5PoppedOut == false)
                {
                    FourPlotGrid.RowDefinitions[2].Height = new GridLength(45);
                }
                HideTrackers = false;
                //change the tracker button image and change the tooltip
                img_HideTrackers.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/HideTrackers.png"));
                btn_HideTrackers.ToolTip = "Hide Trackers";

            }
        }


        #region Plot Specific Point
        private void btn_PlotSpecificPoint_Click(object sender, RoutedEventArgs e)
        {
            //if both the x and the y have values in them then just go with the x value
            //if neither of them have anything then don't do anything
            //message if value is not a double or is out of range

            if(cmb_PlotNames.SelectedIndex == -1) { return; }

            IndividualLinkedPlot theSelectedPlot = new IndividualLinkedPlot() ;
            double xValue = 0;
            double yValue = 0;

            if (cmb_PlotNames.SelectedItem.ToString() == plot0.Title)
            {
                theSelectedPlot = plot0;
                GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, true);

            }
            else if (cmb_PlotNames.SelectedItem.ToString() == plot1.Title)
            {
                //plot1 is only plugged into the loop when it is popped out, otherwise it is the doublelineModulator that is plugged in
                if (Plot1PoppedOut == true)
                {
                    theSelectedPlot = plot1;
                    GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, false);

                }
                else
                {
                    //since plot1 is not popped out, the xvalue (inflow) is the same as plot0's yvalue. just plot that.
                    if(txt_XValue.Text != null && txt_XValue.Text != "")
                    {
                        theSelectedPlot = plot0;
                        txt_YValue.Text = txt_XValue.Text;
                        txt_XValue.Text = "";
                        GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, true);
                        //above code works fine, but it is displaying the wrong x value for plot1(it is displaying plot0 x value)
                        txt_XValue.Text = ((IndividualLinkedPlot)plot1).GetPairedValue(yValue, false, plot1.OxyPlot1.Model, false).ToString();

                    }
                    // if it is the y value that is sought after, that is the same as the y from plot 3
                    else if (txt_YValue.Text != null && txt_YValue.Text != "")
                    {
                        theSelectedPlot = plot3;
                        GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, true);
                    }
                }
              
                
            }
            else if (cmb_PlotNames.SelectedItem.ToString() == plot3.Title)
            {
                theSelectedPlot = plot3;
                GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, false);
            }
            else if (cmb_PlotNames.SelectedItem.ToString() == plot5.Title)
            {
                //plot5 is only plugged into the loop when it is popped out, otherwise it is the horizontaldoublelineModulator that is plugged in
                if (Plot1PoppedOut == true)
                {
                    theSelectedPlot = plot5;
                    GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, false);
                }
            }
            else if (cmb_PlotNames.SelectedItem.ToString() == plot7.Title)
            {
                theSelectedPlot = plot7;
                GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, false);
            }
            else if (cmb_PlotNames.SelectedItem.ToString() == plot8.Title)
            {
                theSelectedPlot = plot8;
                GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref xValue, ref yValue, theSelectedPlot, true);
            }


            ScreenPoint position = theSelectedPlot.OxyPlot1.Model.Axes[0].Transform(xValue, yValue, theSelectedPlot.OxyPlot1.Model.Axes[1]);

            //if the trackers are already frozen, unfreeze them
           if( theSelectedPlot.FreezeNextTracker == true)
            {
                theSelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
            }
            theSelectedPlot.DisplayTheTrackers(position);
            //now that the trackers are displayed, freeze them
            if (theSelectedPlot.FreezeNextTracker == false)
            {
                theSelectedPlot.Model_MouseDown(new object(), new OxyMouseDownEventArgs());
            }

         



        }


        private void GetAndValidateXAndYValuesForFreezingSpecificPlotPoint(ref double xValue,ref double yValue,IndividualLinkedPlot theSelectedPlot,bool isAxisReversed)
        {
            if (txt_XValue.Text == null || txt_XValue.Text == "")
            {
                if (txt_YValue.Text == null || txt_YValue.Text == "")
                {
                    //do nothing, there are no values
                }
                else
                {
                    //x has nothing, and y has some number
                    yValue = Convert.ToDouble(txt_YValue.Text);
                    xValue = ((IndividualLinkedPlot)theSelectedPlot).GetPairedValue(yValue, false, theSelectedPlot.OxyPlot1.Model, isAxisReversed);
                    if (yValue > theSelectedPlot.OxyPlot1.Model.Axes[1].Maximum || yValue < theSelectedPlot.OxyPlot1.Model.Axes[1].Minimum)
                    {
                        //FdaViewModel.Utilities.CustomMessageBoxVM vm = new FdaViewModel.Utilities.CustomMessageBoxVM(FdaViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "Y Value is out of range");
                        MessageBox.Show("Y Value is out of range", "Out of Range");
                        return;
                    }
                    txt_XValue.Text = Math.Round(xValue, 3).ToString();
                }
            }
            else
            {
                //x has a number and we don't care what y is
                xValue = Convert.ToDouble(txt_XValue.Text);
                yValue = ((IndividualLinkedPlot)theSelectedPlot).GetPairedValue(xValue, true, theSelectedPlot.OxyPlot1.Model, isAxisReversed);
                if (xValue > theSelectedPlot.OxyPlot1.Model.Axes[0].Maximum || xValue < theSelectedPlot.OxyPlot1.Model.Axes[0].Minimum)
                {
                    //FdaViewModel.Utilities.CustomMessageBoxVM vm = new FdaViewModel.Utilities.CustomMessageBoxVM(FdaViewModel.Utilities.CustomMessageBoxVM.ButtonsEnum.OK, "X Value is out of range");
                    MessageBox.Show("X Value is out of range", "Out of Range");
                    return;
                }
                txt_YValue.Text = Math.Round(yValue, 3).ToString();
            }

        }


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
                plot0.RemoveAreaPlots();
                plot3.RemoveAreaPlots();
                plot7.RemoveAreaPlots();
                plot8.RemoveAreaPlots();
                AreaPlotsHaveBeenRemoved = true;
                //change the image and change the tooltip
                img_HideAreaPlots.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/ShowAreaPlots.png"));
                btn_ToggleAreaPlots.ToolTip = "Show Area Plots";
            }
            else
            {
                plot0.AddAreaPlots();
                plot3.AddAreaPlots();
                plot7.AddAreaPlots();
                plot8.AddAreaPlots();
                AreaPlotsHaveBeenRemoved = false;
                //change the image and change the tooltip
                img_HideAreaPlots.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(new Uri(@"pack://application:,,,/Fda;component/Resources/HideAreaPlots.png"));
                btn_ToggleAreaPlots.ToolTip = "Hide Area Plots";
            }
        }

        #endregion
    }
}
