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
using Statistics;

namespace Fda.Plots
{
    /// <summary>
    /// Interaction logic for DoubleLineModulator.xaml
    /// </summary>
    public partial class DoubleLineModulator : UserControl,ILinkedPlot
    {
        public event EventHandler PopOutThePlot;

        public static readonly DependencyProperty BaseFunctionProperty = DependencyProperty.Register("BaseFunction", typeof(FdaModel.Functions.BaseFunction), typeof(DoubleLineModulator), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(BaseFunctionChangedCallBack)));

        public static readonly DependencyProperty CurveProperty = DependencyProperty.Register("Curve", typeof(Statistics.CurveIncreasing), typeof(DoubleLineModulator), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(CurveChangedCallBack)));
        public static readonly DependencyProperty TrackerVisibleProperty = DependencyProperty.Register("TrackerVisible", typeof(bool), typeof(DoubleLineModulator), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(TrackerVisibleCallBack)));



        private bool _FreezeTracker;
        private SharedAxisEnum _NextPlotSharedAxisEnum = SharedAxisEnum.unknown;
        private SharedAxisEnum _PreviousPlotSharedAxisEnum = SharedAxisEnum.unknown;
        private double _Height;
        private double _Width;
        private bool _FlipFreqAxis = false;
        private ILinkedPlot _NextPlot;
        private ILinkedPlot _PreviousPlot;

        public bool TrackerIsOutsideTheCurveRange { get; set; }

        #region Properties
        public string SelectedElementName { get; set; } = "cody test";

        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public double MinX { get; set; }
        public double MinY { get; set; }
        public bool IsStartNode { get; set; }
        public bool IsEndNode { get; set; }
        public bool TrackerVisible
        {
            get { return (bool)GetValue(TrackerVisibleProperty); }
            set { SetValue(TrackerVisibleProperty, value); }
        }
        public FdaModel.Functions.BaseFunction BaseFunction
        {
            get { return (FdaModel.Functions.BaseFunction)GetValue(BaseFunctionProperty); }
            set { SetValue(BaseFunctionProperty, value); }
            
        }

        private static void BaseFunctionChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleLineModulator owner = d as DoubleLineModulator;
            owner.BaseFunction = e.NewValue as FdaModel.Functions.BaseFunction;

        }
        public bool FreezeNextTracker
        {
            get { return _FreezeTracker; }
            set
            {
                
                _FreezeTracker = value;
                if (value == true)
                {
                    if (IsEndNode == false)
                    {
                        NextPlot.FreezeNextTracker = true;

                    }
                }
                else
                {
                    if (IsEndNode == false)
                    {
                        NextPlot.FreezeNextTracker = false;

                    }
                }

            }
        }
        public bool FreezePreviousTracker
        {
            get { return _FreezeTracker; }
            set
            {
                _FreezeTracker = value;
                if (value == true)
                {
                    PreviousPlot.FreezePreviousTracker = true;
                  
                }
                else
                {
                    PreviousPlot.FreezePreviousTracker = false;                
                }

            }
        }
        public SharedAxisEnum NextPlotSharedAxisEnum
        {
            get { return _NextPlotSharedAxisEnum; }
            set { _NextPlotSharedAxisEnum = value; }
        }
        public ILinkedPlot NextPlot
        {
            get { return _NextPlot; }

        }
        public ILinkedPlot PreviousPlot
        {
            get { return _PreviousPlot; }

        }
        //public CurveIncreasing Curve { get; set; }
        public Statistics.CurveIncreasing Curve
        {
            get { return (Statistics.CurveIncreasing)GetValue(CurveProperty); }
            set { SetValue(CurveProperty, value); }
        }
        public double AbsoluteMaxFlow { get; set; }
        public double AbsoluteMinFlow { get; set; }

        public double TotalRange { get; set; }
        public double MinInflow { get; set; }
        public double MaxInflow { get; set; }
        public double MinOutflow { get; set; }
        public double MaxOutflow { get; set; }
        public double Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        public double Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        

        public bool FlipFrequencyAxis { get { return false; } set { _FlipFreqAxis = false; } }

        

        #endregion


        public DoubleLineModulator()
        {

            InitializeComponent();

            //Width = RectRight.ActualWidth;
            //Height = RectRight.ActualHeight;

            //MinValue = 100;
            //MaxValue = 2000;

            //DisplayTracker(150);


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(Curve == null) { return; }
            if(Curve.Count == 0) { return; }
            MinInflow = Curve.get_X(0);
            MaxInflow = Curve.get_X(Curve.Count - 1);

            MinOutflow = Curve.get_Y(0);
            MaxOutflow = Curve.get_Y(Curve.Count - 1);

            
            if(MinInflow<MinOutflow)
            {
                AbsoluteMinFlow = Math.Log10( MinInflow);
            }
            else
            {
                AbsoluteMinFlow = Math.Log10( MinOutflow);
            }
            if(MaxInflow > MaxOutflow)
            {
                AbsoluteMaxFlow = Math.Log10(MaxInflow);
            }
            else
            {
                AbsoluteMaxFlow = Math.Log10(MaxOutflow);
            }
            TotalRange = AbsoluteMaxFlow - AbsoluteMinFlow;


            //find parent and add this plot to its selectedPlot property.
            ContentControl parentControl = Plots.IndividualLinkedPlotControl.FindParent<ContentControl>(this);
            if (parentControl != null && parentControl.GetType() == typeof(Conditions.ConditionsDoubleLineModulatorWrapper))
            {
                parentControl = IndividualLinkedPlotControl.FindParent<ContentControl>(parentControl);
            }

            if (parentControl != null && parentControl.GetType() == typeof(IndividualLinkedPlotControl))
            {
                FdaViewModel.Plots.IndividualLinkedPlotControlVM vm = (FdaViewModel.Plots.IndividualLinkedPlotControlVM)parentControl.DataContext;

                this.BaseFunction = vm.IndividualPlotWrapperVM.PlotVM.BaseFunction;
                ((Plots.IndividualLinkedPlotControl)parentControl).LinkedPlot = this;
                ((IndividualLinkedPlotControl)parentControl).UpdateThePlots();
            }

        }

        private static void CurveChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleLineModulator owner = d as DoubleLineModulator;
            owner.Curve = e.NewValue as Statistics.CurveIncreasing;

            

            //LineSeries series1 = new LineSeries();

            //for (int i = 0; i < curve.Count; i++)
            //{
            //    series1.Points.Add(new DataPoint(curve.get_X(i), curve.get_Y(i)));

            //}

            //owner.OxyPlot1.Model.Series.Add(series1);

            //SetMinMaxValues(owner);

        }

        public void SetAsStartNode()
        {
            //i am just fulfilling the interface contract. This should never be a start node right?
        }
        public void SetNextPlotLinkage(ILinkedPlot plot, string thisAxis = "", string linkedAxis = "")
        {
            ////if (ThisIsEndNode == true) { return; }
            //_NextPlot = plot;
            //this.SetNextPlotSharedAxis(thisAxis, linkedAxis);


            if (IsEndNode == true) { return; }
            thisAxis = "";
            linkedAxis = "";

            if (plot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
            {
                thisAxis = "y";
                linkedAxis = "y";
            }
            //((IndividualLinkedPlot)plot).SetSharedYAxisWithPlot
            this.SetNextPlotSharedAxis(thisAxis, linkedAxis);
            _NextPlot = plot;


        }
        public void SetPreviousPlotLinkage(ILinkedPlot plot)
        {
            //if (ThisIsStartNode == true) { return; }
            _PreviousPlot = plot;
            _PreviousPlotSharedAxisEnum = PreviousPlot.NextPlotSharedAxisEnum;
        }
        private void SetNextPlotSharedAxis(string thisAxis, string linkedAxis)
        {
            string thisAxisUpper = thisAxis.ToUpper();
            string linkedAxisUpper = linkedAxis.ToUpper();
            if (thisAxisUpper == "X" && linkedAxisUpper == "X")  //enum 1
            {
                _NextPlotSharedAxisEnum = SharedAxisEnum.XX;
            }
            else if (thisAxisUpper == "X" && linkedAxisUpper == "Y") //enum 2
            {
                _NextPlotSharedAxisEnum = SharedAxisEnum.XY;
            }
            else if (thisAxisUpper == "Y" && linkedAxisUpper == "X")  // enum 3
            {
                _NextPlotSharedAxisEnum = SharedAxisEnum.YX;
            }
            else if (thisAxisUpper == "Y" && linkedAxisUpper == "Y") // enum 4
            {
                _NextPlotSharedAxisEnum = SharedAxisEnum.YY;
            }
        }
        /// <summary>
        /// This displays the horizontal lines on the vertical lines
        /// </summary>
        /// <param name="knownValue"></param>
        public void DisplayLines(double inflow, double outflow)
        {
            //if(inflow >= ((IndividualLinkedPlot)PreviousPlot).OxyPlot1.Model.Axes[1].Minimum && inflow <= ((IndividualLinkedPlot)PreviousPlot).OxyPlot1.Model.Axes[1].Maximum)
            ILinkedPlot sharedPlot ;
            if (PreviousPlot == null && NextPlot == null)
            {
                return;
            }
            else if(PreviousPlot == null)
            {
                sharedPlot = NextPlot;
            }
            else
            {
                sharedPlot = PreviousPlot;
            }


            //if (inflow >= ((IndividualLinkedPlot)sharedPlot).MinY && inflow <= ((IndividualLinkedPlot)sharedPlot).MaxY)
            double myMinX = Curve.XValues.First();
            double myMaxX = Curve.XValues.Last();
            double myMinY = Curve.YValues.First();
            double myMaxY = Curve.YValues.Last();

                if (inflow >= myMinX && inflow <= myMaxX && outflow >= myMinY && outflow <= myMaxY)

                {
                TurnOutsideOfRangeOff();
                myCanvas.Children.Clear();

                double canvasHeight = myCanvas.ActualHeight;
                double totalRectangleHeight = RectRight.ActualHeight;

                //double newOffset = (canvasHeight-rectHi)/ 2;

                double offsetFromTop = (canvasHeight - totalRectangleHeight) / 2;
                int circleDiameter = 7;
                int distanceFromLeftToLeftLine = 15;
                int distanceFromLeftToRightLine = 30;



                double logMax = Math.Log10(((IndividualLinkedPlot)sharedPlot).MaxY); //OxyPlot1.Model.Axes[1].Maximum);
                double logMin = Math.Log10(((IndividualLinkedPlot)sharedPlot).MinY); //OxyPlot1.Model.Axes[1].Minimum);
                double logTotalRange = logMax - logMin;



                ////////////////  right point ////////////////

                double distanceFromTop = Math.Log10(inflow) - logMin;
                double percentFromTop =1-( distanceFromTop / logTotalRange);// (AbsoluteMaxFlow - AbsoluteMinFlow);
                double rightScreenPointYValue = totalRectangleHeight * percentFromTop;

                Ellipse rightCircle = new Ellipse();
                rightCircle.Stroke = System.Windows.Media.Brushes.Green;
                rightCircle.Fill = Brushes.Green;
                rightCircle.Width = circleDiameter;
                rightCircle.Height = circleDiameter;

                int circleRadius = (int)rightCircle.Width / 2;
                //rightCircle.HorizontalAlignment = HorizontalAlignment.Left;

                Canvas.SetLeft(rightCircle, distanceFromLeftToRightLine - circleRadius); // 50 - half the width of the circle
                Canvas.SetTop(rightCircle, rightScreenPointYValue+offsetFromTop-circleRadius);



                ////////   left point  /////////////////////////


                //distanceFromTop = AbsoluteMaxFlow - Math.Log10(outflow);
                //percentFromTop = distanceFromTop / logTotalRange;// (AbsoluteMaxFlow - AbsoluteMinFlow);            
                 distanceFromTop = Math.Log10(outflow) - logMin;
                 percentFromTop =1 - ( distanceFromTop / logTotalRange);// (AbsoluteMaxFlow - AbsoluteMinFlow);


                //get the screen point for this value
                totalRectangleHeight = RectRight.ActualHeight;
                 double leftScreenPointYValue = totalRectangleHeight * percentFromTop;

               
                Ellipse leftCircle = new Ellipse();
                leftCircle.Stroke = System.Windows.Media.Brushes.Green;
                leftCircle.Fill = Brushes.Green;
                leftCircle.Width = circleDiameter;
                leftCircle.Height = circleDiameter;

                //circleRadius = (int)rightCircle.Width / 2;
                //rightCircle.HorizontalAlignment = HorizontalAlignment.Left;

                Canvas.SetLeft(leftCircle, distanceFromLeftToLeftLine - circleRadius); // 50 - half the width of the circle
                Canvas.SetTop(leftCircle, leftScreenPointYValue + offsetFromTop - circleRadius);


                ///////////  line between the two points   /////////

                Line betweenLine = new Line();
                betweenLine.X1 = distanceFromLeftToLeftLine;
                betweenLine.X2 = distanceFromLeftToRightLine;
                betweenLine.Y1 = leftScreenPointYValue + offsetFromTop;
                betweenLine.Y2 = rightScreenPointYValue + offsetFromTop;
                betweenLine.Stroke = System.Windows.Media.Brushes.Black;

                betweenLine.Visibility = Visibility.Visible;
                betweenLine.StrokeThickness = 1;


                //Line line = new Line();
                //Thickness thickness = new Thickness(101, -11, 362, 250);
                //line.Margin = thickness;
                //line.Visibility = System.Windows.Visibility.Visible;
                //line.StrokeThickness = 4;
                //line.Stroke = System.Windows.Media.Brushes.Pink;
                //line.X1 = 10;
                //line.X2 = 40;
                //line.Y1 = 50;
                //line.Y2 = 70;    

                ///////////////   right line ////////////////////
                Line rightLine = new Line();
                rightLine.X1 = distanceFromLeftToRightLine;
                rightLine.X2 = distanceFromLeftToRightLine+15;
                rightLine.Y1 = rightScreenPointYValue + offsetFromTop;
                rightLine.Y2 = rightScreenPointYValue + offsetFromTop;
                rightLine.Stroke = System.Windows.Media.Brushes.Black;

                rightLine.Visibility = Visibility.Visible;
                rightLine.StrokeThickness = 1;

                Line leftLine = new Line();
                leftLine.X1 = 0;
                leftLine.X2 = distanceFromLeftToLeftLine;
                leftLine.Y1 = leftScreenPointYValue + offsetFromTop;
                leftLine.Y2 = leftScreenPointYValue + offsetFromTop;
                leftLine.Stroke = System.Windows.Media.Brushes.Black;

                leftLine.Visibility = Visibility.Visible;
                leftLine.StrokeThickness = 1;


                myCanvas.Children.Add(leftLine);


                myCanvas.Children.Add(rightLine);


                myCanvas.Children.Add(betweenLine);

                myCanvas.Children.Add(rightCircle);
                myCanvas.Children.Add(leftCircle);

                TrackerIsOutsideTheCurveRange = false;
            }
            else
            {
                //we are out of range
                TurnOutsideOfRangeOn();
            }


        }

        public void SetAsEndNode()
        {
            IsEndNode = true;
        }
        public void DisplayNextTracker(double x, double y)
        {
            //if prev plot is outside the range then there is nothing for me to track
            if ( PreviousPlot.TrackerIsOutsideTheCurveRange)
            {
                TurnOutsideOfRangeOn();
                NextPlot.DisplayNextTracker(0, 0);
                return;
            }

            //display my own stuff

            DisplayLines(x, y);

            if (IsEndNode == true) { return; }

            //if my tracker would be out of range...
            if(TrackerIsOutsideTheCurveRange)
            {
                NextPlot.DisplayNextTracker(0, 0);
                return;
            }

            if (NextPlot == null || NextPlot.Curve == null) { return; }

            //get the x and y values for the next plot
            double otherValue;



            switch (_NextPlotSharedAxisEnum)
            {
                case SharedAxisEnum.XX:
                    {
                        //otherValue = GetPairedValue(x, true, NextPlot.OxyPlot1.Model, NextPlot.FlipFrequencyAxis);
                        if (NextPlot.FlipFrequencyAxis == true)
                        {
                            otherValue = NextPlot.Curve.GetYfromX(1 - x);

                        }
                        else
                        {
                            otherValue = NextPlot.Curve.GetYfromX(x);

                        }
                        NextPlot.DisplayNextTracker(x, otherValue);

                        break;
                    }
                case SharedAxisEnum.XY:
                    {
                        //otherValue = GetPairedValue(x, false, NextPlot.OxyPlot1.Model, NextPlot.FlipFrequencyAxis);

                        otherValue = NextPlot.Curve.GetXfromY(x);
                        if (NextPlot.FlipFrequencyAxis == true)
                        {
                            NextPlot.DisplayNextTracker(1 - x, otherValue);

                        }
                        else
                        {
                            NextPlot.DisplayNextTracker(x, otherValue);
                        }
                        break;
                    }
                case SharedAxisEnum.YX:
                    {
                        //otherValue = GetPairedValue(y, true, NextPlot.OxyPlot1.Model, NextPlot.FlipFrequencyAxis);
                        if (NextPlot.FlipFrequencyAxis == true)
                        {
                            otherValue = NextPlot.Curve.GetYfromX(y);

                        }
                        else
                        {
                            otherValue = NextPlot.Curve.GetYfromX(y);

                        }
                        NextPlot.DisplayNextTracker(y, otherValue);
                        break;
                    }
                case SharedAxisEnum.YY:
                    {
                        otherValue = NextPlot.Curve.GetXfromY(y);
                        //otherValue = GetPairedValue(y, false, NextPlot.OxyPlot1.Model, NextPlot.FlipFrequencyAxis);
                        if (NextPlot.FlipFrequencyAxis == true)
                        {
                            NextPlot.DisplayNextTracker(1 - otherValue, y);

                        }
                        else
                        {
                            NextPlot.DisplayNextTracker(otherValue, y);
                        }
                        break;
                    }
            }
        }

        public void DisplayPreviousTracker(double x, double y)
        {

            if (NextPlot.TrackerIsOutsideTheCurveRange)
            {
                TurnOutsideOfRangeOn();
                PreviousPlot.DisplayPreviousTracker(0, 0);
                return;
            }

            DisplayLines(x, y);

            if (TrackerIsOutsideTheCurveRange)
            {
                PreviousPlot.DisplayPreviousTracker(0, 0);
                return;
            }

            if (PreviousPlot == null || PreviousPlot.Curve == null) { return; }
            //get the x and y values for the previous plot
            double otherValue;
            switch (_PreviousPlotSharedAxisEnum)
            {
                case SharedAxisEnum.XX:
                    {
                        //otherValue = GetPairedValue(x, true, PreviousPlot.OxyPlot1.Model, PreviousPlot.FlipFrequencyAxis);
                        if (PreviousPlot.FlipFrequencyAxis == true)
                        {
                            otherValue = PreviousPlot.Curve.GetYfromX(1 - x);

                        }
                        else
                        {
                            otherValue = PreviousPlot.Curve.GetYfromX(x);

                        }
                        PreviousPlot.DisplayPreviousTracker(x, otherValue);

                        break;
                    }
                case SharedAxisEnum.XY:
                    {
                        //otherValue = GetPairedValue(dp.Y, true, PreviousPlot.OxyPlot1.Model, PreviousPlot.FlipFrequencyAxis);
                        if (PreviousPlot.FlipFrequencyAxis == true)
                        {
                            otherValue = PreviousPlot.Curve.GetYfromX(1 - y);

                        }
                        else
                        {
                            otherValue = PreviousPlot.Curve.GetYfromX(y);
                        }
                        PreviousPlot.DisplayPreviousTracker(x, otherValue);

                        break;
                    }
                case SharedAxisEnum.YX:
                    {
                        otherValue = PreviousPlot.Curve.GetXfromY(x);
                        //otherValue = GetPairedValue(dp.X, false, PreviousPlot.OxyPlot1.Model, PreviousPlot.FlipFrequencyAxis);
                        if (PreviousPlot.FlipFrequencyAxis == true)
                        {
                            PreviousPlot.DisplayPreviousTracker(1 - otherValue, x);

                        }
                        else
                        {
                            PreviousPlot.DisplayPreviousTracker(otherValue, x);

                        }

                        break;
                    }
                case SharedAxisEnum.YY:
                    {
                        otherValue = PreviousPlot.Curve.GetXfromY(y);
                        //otherValue = GetPairedValue(y, false, PreviousPlot.OxyPlot1.Model, PreviousPlot.FlipFrequencyAxis);
                        if (PreviousPlot.FlipFrequencyAxis == true)
                        {
                            PreviousPlot.DisplayPreviousTracker(1 - otherValue, y);

                        }
                        else
                        {
                            PreviousPlot.DisplayPreviousTracker(otherValue, y);

                        }

                        break;
                    }
            }
        }

        private void btn_PopPlotOut_Click(object sender, RoutedEventArgs e)
        {
            //clear the drawing of the circles and lines. They are showing up over the top of the other plots for some reason
            myCanvas.Children.Clear();
            //raise an event that gets handled at the parent
            //if (this.PopOutThePlot != null)
            //{
            //    this.PopOutThePlot(this, new EventArgs());
            //}

            ContentControl parentControl = Plots.IndividualLinkedPlotControl.FindParent<ContentControl>(this);
            if (parentControl != null && parentControl.GetType() == typeof(Conditions.ConditionsDoubleLineModulatorWrapper))
            {
                parentControl = IndividualLinkedPlotControl.FindParent<ContentControl>(parentControl);
            }

            if (parentControl != null && parentControl.GetType() == typeof(IndividualLinkedPlotControl))
            {
                FdaViewModel.Plots.IndividualLinkedPlotControlVM vm = (FdaViewModel.Plots.IndividualLinkedPlotControlVM)parentControl.DataContext;
                vm.CurrentVM = (FdaViewModel.BaseViewModel)vm.IndividualPlotWrapperVM;
                ((IndividualLinkedPlotControl)parentControl).PopThePlotIntoPlot1();
            }


        }


        public void TurnOutsideOfRangeOn()
        {
            myCanvas.Children.Clear();
            TrackerIsOutsideTheCurveRange = true;


        }
        public void TurnOutsideOfRangeOff()
        {
            
            //txt_OutsideOfRange.Visibility = Visibility.Hidden;

            this.TrackerIsOutsideTheCurveRange = false;
            //ShowTracker();

        }


        private static void TrackerVisibleCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleLineModulator owner = d as DoubleLineModulator;
            bool trackerVisible = Convert.ToBoolean(e.NewValue);
            if (trackerVisible == true)
            {
               // owner.ShowTracker();
            }
            else
            {
                owner.myCanvas.Children.Clear();

                //owner.HideTracker();
                //if there is no tracker then turn the outside of range label off
                // owner.TurnOutsideOfRangeOff();
            }
        }

    }
}
