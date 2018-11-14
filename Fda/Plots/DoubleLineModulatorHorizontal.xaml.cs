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
    /// Interaction logic for DoubleLineModulatorHorizontal.xaml
    /// </summary>
    public partial class DoubleLineModulatorHorizontal : UserControl,ILinkedPlot
    {
        public event EventHandler PopOutThePlot;

        public static readonly DependencyProperty BaseFunctionProperty = DependencyProperty.Register("BaseFunction", typeof(FdaModel.Functions.BaseFunction), typeof(DoubleLineModulatorHorizontal), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(BaseFunctionChangedCallBack)));

        public static readonly DependencyProperty CurveProperty = DependencyProperty.Register("Curve", typeof(Statistics.CurveIncreasing), typeof(DoubleLineModulatorHorizontal), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(CurveChangedCallBack)));

        private bool _FreezeTracker;
        private SharedAxisEnum _NextPlotSharedAxisEnum = SharedAxisEnum.unknown;
        private SharedAxisEnum _PreviousPlotSharedAxisEnum = SharedAxisEnum.unknown;
        private double _Height;
        private double _Width;
        private bool _FlipFreqAxis = false;
        private ILinkedPlot _NextPlot;
        private ILinkedPlot _PreviousPlot;

        #region Properties
        public bool TrackerIsOutsideTheCurveRange { get; set; }

        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public double MinX { get; set; }
        public double MinY { get; set; }
        public FdaModel.Functions.BaseFunction BaseFunction
        {
            get { return (FdaModel.Functions.BaseFunction)GetValue(BaseFunctionProperty); }
            set { SetValue(BaseFunctionProperty, value); }
        }

        private static void BaseFunctionChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DoubleLineModulatorHorizontal owner = d as DoubleLineModulatorHorizontal;
            owner.BaseFunction = e.NewValue as FdaModel.Functions.BaseFunction;

        }
        public bool IsStartNode { get; set; }
        public bool IsEndNode { get; set; }
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
        public double AbsoluteMaxStage { get; set; }
        public double AbsoluteMinStage { get; set; }

        public double TotalRange { get; set; }
        public double MinExteriorStage { get; set; }
        public double MaxExteriorStage { get; set; }
        public double MinInteriorStage { get; set; }
        public double MaxInteriorStage { get; set; }
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
        public DoubleLineModulatorHorizontal()
        {
            InitializeComponent();
        }

    
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(Curve == null || Curve.Count==0) { return; }
            MinExteriorStage = Curve.get_X(0);
            MaxExteriorStage = Curve.get_X(Curve.Count - 1);

            MinInteriorStage = Curve.get_Y(0);
            MaxInteriorStage = Curve.get_Y(Curve.Count - 1);


            if (MinExteriorStage < MinInteriorStage)
            {
                AbsoluteMinStage = MinExteriorStage;
            }
            else
            {
                AbsoluteMinStage = MinInteriorStage;
            }
            if (MaxExteriorStage > MaxInteriorStage)
            {
                AbsoluteMaxStage = MaxExteriorStage;
            }
            else
            {
                AbsoluteMaxStage = MaxInteriorStage;
            }
            TotalRange = AbsoluteMaxStage - AbsoluteMinStage;

            //find parent and add this plot to its selectedPlot property.
            ContentControl parentControl = Plots.IndividualLinkedPlotControl.FindParent<ContentControl>(this);
            if (parentControl != null && parentControl.GetType() == typeof(Plots.HorizontalDoubleLineModulatorWrapper))
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
            DoubleLineModulatorHorizontal owner = d as DoubleLineModulatorHorizontal;
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
        public void SetAsEndNode()
        {
            IsEndNode = true;
        }

        public void SetNextPlotLinkage(ILinkedPlot plot, string thisAxis, string linkedAxis)
        {
            ////if (ThisIsEndNode == true) { return; }
            //_NextPlot = plot;
            //this.SetNextPlotSharedAxis(thisAxis, linkedAxis);


            if (IsEndNode == true) { return; }
            thisAxis = "";
            linkedAxis = "";

            if (plot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
            {
                thisAxis = "y";
                linkedAxis = "x";
            }

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
        public void DisplayLines(double exteriorStage, double interiorStage)
        {
            //double xAxisMinimum = ((IndividualLinkedPlot)PreviousPlot).OxyPlot1.Model.Axes[0].Minimum;
            //double xAxisMaximum = ((IndividualLinkedPlot)PreviousPlot).OxyPlot1.Model.Axes[0].Maximum;
            ILinkedPlot sharedPlot;
            if (PreviousPlot == null && NextPlot == null)
            {
                return;
            }
            else if (PreviousPlot == null)
            {
                sharedPlot = NextPlot;
            }
            else
            {
                sharedPlot = PreviousPlot;
            }
            double xAxisMinimum = ((IndividualLinkedPlot)sharedPlot).MinX;
            double xAxisMaximum = ((IndividualLinkedPlot)sharedPlot).MaxX;
            if (exteriorStage >= xAxisMinimum && exteriorStage <= xAxisMaximum)
            {
                myCanvas.Children.Clear();

                double canvasWidth = myCanvas.ActualWidth;
                double totalRectangleWidth = RectBottom.ActualWidth;


                double offsetFromLeft = 70;// (canvasWidth - totalRectangleWidth) * .7;/// 2;
                int circleDiameter = 7;
                int distanceFromTopToTopLine = 15;
                int distanceFromTopToBottomLine = 30;




                double totalLineRange = xAxisMaximum - xAxisMinimum;



                double percentFromLeft = (exteriorStage- xAxisMinimum) / totalLineRange;
                double topScreenPointXValue = totalRectangleWidth * percentFromLeft;


                ////////   top point  /////////////////////////


                Ellipse topCircle = new Ellipse();
                topCircle.Stroke = System.Windows.Media.Brushes.Green;
                topCircle.Fill = Brushes.Green;
                topCircle.Width = circleDiameter;
                topCircle.Height = circleDiameter;

                int circleRadius = (int)topCircle.Width / 2;

                Canvas.SetTop(topCircle, distanceFromTopToTopLine - circleRadius); // 50 - half the width of the circle
                Canvas.SetLeft(topCircle, topScreenPointXValue + offsetFromLeft - circleRadius);



                //percentFromLeft = (interiorStage - MinInteriorStage) / (AbsoluteMaxStage - AbsoluteMinStage);




                ////////   bottom point  /////////////////////////

                percentFromLeft = (interiorStage - xAxisMinimum) / totalLineRange;//(AbsoluteMaxStage - AbsoluteMinStage);
                double bottomScreenPointXValue = totalRectangleWidth * percentFromLeft;


                Ellipse bottomCircle = new Ellipse();
                bottomCircle.Stroke = System.Windows.Media.Brushes.Green;
                bottomCircle.Fill = Brushes.Green;
                bottomCircle.Width = circleDiameter;
                bottomCircle.Height = circleDiameter;

                Canvas.SetTop(bottomCircle, distanceFromTopToBottomLine - circleRadius); // 50 - half the width of the circle
                Canvas.SetLeft(bottomCircle, bottomScreenPointXValue + offsetFromLeft - circleRadius);


                ///////////  line between the two points   /////////

                Line betweenLine = new Line();
                betweenLine.Y1 = distanceFromTopToTopLine;
                betweenLine.Y2 = distanceFromTopToBottomLine;
                betweenLine.X2 = bottomScreenPointXValue + offsetFromLeft;
                betweenLine.X1 = topScreenPointXValue + offsetFromLeft;
                betweenLine.Stroke = System.Windows.Media.Brushes.Black;

                betweenLine.Visibility = Visibility.Visible;
                betweenLine.StrokeThickness = 1;


                ///////////////   Top line ////////////////////
                Line topLine = new Line();
                topLine.Y1 = distanceFromTopToBottomLine;
                topLine.Y2 = distanceFromTopToBottomLine + 15;
                topLine.X1 = bottomScreenPointXValue + offsetFromLeft;
                topLine.X2 = bottomScreenPointXValue + offsetFromLeft;
                topLine.Stroke = System.Windows.Media.Brushes.Black;

                topLine.Visibility = Visibility.Visible;
                topLine.StrokeThickness = 1;
                ///////////////   bottom line ////////////////////

                Line bottomLine = new Line();
                bottomLine.Y1 = 0;
                bottomLine.Y2 = distanceFromTopToTopLine;
                bottomLine.X1 = topScreenPointXValue + offsetFromLeft;
                bottomLine.X2 = topScreenPointXValue + offsetFromLeft;
                bottomLine.Stroke = System.Windows.Media.Brushes.Black;

                bottomLine.Visibility = Visibility.Visible;
                bottomLine.StrokeThickness = 1;


                myCanvas.Children.Add(bottomLine);


                myCanvas.Children.Add(topLine);


                myCanvas.Children.Add(betweenLine);

                myCanvas.Children.Add(topCircle);
                myCanvas.Children.Add(bottomCircle);


               


            }
           


        }

        public void DisplayNextTracker(double x, double y)
        {

            if (PreviousPlot.TrackerIsOutsideTheCurveRange == true)
            {

                this.TrackerIsOutsideTheCurveRange = true;
                myCanvas.Children.Clear();
                _NextPlot.DisplayNextTracker(0, 0);//values don't matter here
                return;
            }
            else
            {
                this.TrackerIsOutsideTheCurveRange = false;

            }

            //display my own stuff
            DisplayLines(x, y);

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
            DisplayLines(x, y);
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
            ////raise an event that gets handled at the parent
            //if (this.PopOutThePlot != null)
            //{
            //    this.PopOutThePlot(this, new EventArgs());
            //}
            //clear the drawing of the circles and lines. They are showing up over the top of the other plots for some reason
            myCanvas.Children.Clear();
           
            

            ContentControl parentControl = Plots.IndividualLinkedPlotControl.FindParent<ContentControl>(this);
            if (parentControl != null && parentControl.GetType() == typeof(Plots.LinkedPlots))
            {
                ((Plots.LinkedPlots)parentControl).PopPlot5Out(this, new EventArgs());
            }
            else if (parentControl != null && parentControl.GetType() == typeof(Plots.HorizontalDoubleLineModulatorWrapper)) //this occurs in the conditions editor
            {
                parentControl = IndividualLinkedPlotControl.FindParent<ContentControl>(parentControl);
                if (parentControl != null && parentControl.GetType() == typeof(IndividualLinkedPlotControl))
                {
                    FdaViewModel.Plots.IndividualLinkedPlotControlVM vm = (FdaViewModel.Plots.IndividualLinkedPlotControlVM)parentControl.DataContext;
                    vm.CurrentVM = (FdaViewModel.BaseViewModel)vm.IndividualPlotWrapperVM;
                    ((IndividualLinkedPlotControl)parentControl).PopThePlotIntoPlot5();
                }
            }


        }

    }
}
