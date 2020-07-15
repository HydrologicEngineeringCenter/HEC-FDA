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
using System.ComponentModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using FdaViewModel.Plots;
using Model;
using Model.Inputs.Functions.ImpactAreaFunctions;

namespace View.Plots
{
    /// <summary>
    /// Interaction logic for FailureFunctionPlot.xaml
    /// </summary>
    public partial class IndividualLinkedPlot : UserControl//, INotifyPropertyChanged, ILinkedPlot
    {
     //   public static readonly DependencyProperty BaseFunctionProperty = DependencyProperty.Register("BaseFunction", typeof(Model.IFdaFunction), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(BaseFunctionChangedCallBack)));

     //   //public static readonly DependencyProperty CurveProperty = DependencyProperty.Register("Curve", typeof(Statistics.CurveIncreasing), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(CurveChangedCallBack)));
     //   //public static readonly DependencyProperty NonStandardDeviationCurveProperty = DependencyProperty.Register("NonStandardDeviationCurve", typeof(Statistics.CurveIncreasing), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(null));

     //   public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(TitleChangedCallBack)));

     //   public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register("SubTitle", typeof(string), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(SubTitleChangedCallBack)));

     //   public static readonly DependencyProperty XAxisLabelProperty = DependencyProperty.Register("XAxisLabel", typeof(string), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(XAxisLabelChangedCallBack)));
     //   public static readonly DependencyProperty YAxisLabelProperty = DependencyProperty.Register("YAxisLabel", typeof(string), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(YAxisLabelChangedCallBack)));
     //   public static readonly DependencyProperty SetYAxisToLogarithmicProperty = DependencyProperty.Register("SetYAxisToLogarithmic", typeof(bool), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(SetYAxisToLogarithmicCallBack)));
     //   public static readonly DependencyProperty FlipFrequencyAxisProperty = DependencyProperty.Register("FlipFrequencyAxis", typeof(bool), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(FlipFrequencyAxisCallBack)));
     //   ////public static readonly DependencyProperty MaxXProperty = DependencyProperty.Register("MaxX", typeof(double), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(MaxXCallBack)));
     //   public static readonly DependencyProperty TrackerVisibleProperty = DependencyProperty.Register("TrackerVisible", typeof(bool), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(TrackerVisibleCallBack)));
     //   public static readonly DependencyProperty AreaPlotVisibleProperty = DependencyProperty.Register("AreaPlotVisible", typeof(bool), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(AreaPlotVisibleCallBack)));
     //   public static readonly DependencyProperty ThresholdProperty = DependencyProperty.Register("Threshold", typeof(IMetric), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PlotThresholdCallBack)));
     //   public static readonly DependencyProperty XAxisAsStandardDeviationProperty = DependencyProperty.Register("XAxisAsStandardDeviation", typeof(bool), typeof(IndividualLinkedPlot), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(XAxisAsStandardDeviationCallBack)));


     //   public event PropertyChangedEventHandler PropertyChanged;
     //   public event EventHandler ChangeThisCurve;
     //   //private OxyPlot.Wpf.PlotView _linkedPlot;//this is the plot that "this" will link with
     //   private bool _FreezeTracker;
     //   private bool _HideTracker;


     //   private OxyColor AREA_UNDER_CURVE_COLOR = OxyColor.FromArgb(50, 128, 255, 128);
     //   private OxyColor AREA_PLOT_COLOR = OxyColor.FromArgb(100, 100, 100, 100);
     //   private OxyColor THRESHOLD_LINE_COLOR = OxyColor.FromArgb(100, 0, 0, 255);

     //   private SharedAxisEnum _NextPlotSharedAxisEnum = SharedAxisEnum.unknown;
     //   private SharedAxisEnum _PreviousPlotSharedAxisEnum = SharedAxisEnum.unknown;

     //   private ILinkedPlot _NextPlot;
     //   private ILinkedPlot _PreviousPlot;
     //   private ILinkedPlot _PlotThatSharesAnAreaPlot;

     //   private string _SelectedName;


     //   #region Properties
     //   public bool ThresholdLineIsShowing { get; set; }
     //   public LineSeries VerticalThresholdLine { get; set; }
     //   public LineSeries HorizontalThresholdLine { get; set; }

     //   public string SelectedElementName { get; set; } = "cody test";

     //   public bool TrackerIsOutsideTheCurveRange { get; set; }

     //   //public string TestName
     //   //{
     //   //    get
     //   //    {
     //   //        return "Cody";
     //   //    }
     //   //    set { _SelectedName = value; }
     //   //}
     //   public bool HasYAreaPlots { get; set; }
     //   public bool HasXAreaPlots { get; set; }
     //   //public bool OutOfRange { get; set; }

     //   public List<AreaSeries> ListOfRemovedAreaSeries { get; set; }
     //   public bool FreezeNextTracker
     //   {
     //       get { return _FreezeTracker; }
     //       set
     //       {
     //           _FreezeTracker = value;
     //           if (value == true)
     //           {
     //               if(IsEndNode == false)
     //               {
     //                   NextPlot.FreezeNextTracker = true;

     //               }
     //           }
     //           else
     //           {
     //               if (IsEndNode == false)
     //               {
     //                   NextPlot.FreezeNextTracker = false;

     //               }
     //           }

     //       }
     //   }
     //   public bool FreezePreviousTracker
     //   {
     //       get { return _FreezeTracker; }
     //       set
     //       {
     //           _FreezeTracker = value;
     //           if (value == true)
     //           {
     //               if (IsStartNode == false)
     //               {
     //                   PreviousPlot.FreezePreviousTracker = true;

     //               }
     //           }
     //           else
     //           {
     //               if (IsStartNode == false)
     //               {
     //                   PreviousPlot.FreezePreviousTracker = false;

     //               }
     //           }

     //       }
     //   }

     //   //public bool SharedAxisIsX { get; set; }
     //   public bool IsStartNode { get; set; }
     //   public bool IsEndNode { get; set; }
     //   public double MinX { get; set; }
     //   private double HigherMinX { get; set; }
     //   private double LowerMaxX { get; set; }
     //   public double MaxX { get; set; }
 
     //   public double MinY { get; set; }
     //   private double HigherMinY { get; set; }
     //   private double LowerMaxY { get; set; }
     //   public double MaxY { get; set; }
     //   public bool SetYAxisToLogarithmic { get; set; }
     //   public bool FlipFrequencyAxis {
     //       get;
     //       set; }
     //   public SharedAxisEnum NextPlotSharedAxisEnum
     //   {
     //       get { return _NextPlotSharedAxisEnum; }
     //       set { _NextPlotSharedAxisEnum = value; }
     //   }
     //   public ILinkedPlot NextPlot
     //   {
     //       get { return _NextPlot; }

     //   }
     //   public ILinkedPlot PreviousPlot
     //   {
     //       get { return _PreviousPlot; }

     //   }
        
     //   //**********  dependency properties area ***************
     //   //public double MaxX
     //   //{
     //   //    get { return (double)GetValue(MaxXProperty); }
     //   //    set { SetValue(MaxXProperty, value); }

     //   //}
     //   public string XAxisLabel
     //   {
     //       get { return (string)GetValue(XAxisLabelProperty); }
     //       set { SetValue(XAxisLabelProperty, value); }
     //   }
     //   public string YAxisLabel
     //   {
     //       get { return (string)GetValue(YAxisLabelProperty); }
     //       set { SetValue(YAxisLabelProperty, value); }
     //   }
     //   public Model.IFdaFunction BaseFunction
     //   {
     //       get { return (Model.IFdaFunction)GetValue(BaseFunctionProperty); }
     //       set { SetValue(BaseFunctionProperty, value); }
     //   }
     //   //public Statistics.CurveIncreasing Curve
     //   //{
     //   //    get { return (Statistics.CurveIncreasing)GetValue(CurveProperty); }
     //   //    set { SetValue(CurveProperty, value); }
     //   //}
     //   //public Statistics.CurveIncreasing NonStandardDeviationCurve
     //   //{
     //   //    get { return (Statistics.CurveIncreasing)GetValue(NonStandardDeviationCurveProperty); }
     //   //    set { SetValue(NonStandardDeviationCurveProperty, value); }
     //   //}
        
     //   public string Title
     //   {
     //       get { return (string)GetValue(TitleProperty); }
     //       set { SetValue(TitleProperty, value); }
     //   }
     //   public string SubTitle
     //   {
     //       get { return (string)GetValue(SubTitleProperty); }
     //       set { SetValue(SubTitleProperty, value);  }
     //   }

        

     //   public bool TrackerVisible
     //   {
     //       get { return (bool)GetValue(TrackerVisibleProperty); }
     //       set { SetValue(TrackerVisibleProperty, value); }
     //   }
     //   public bool AreaPlotVisible
     //   {
     //       get { return (bool)GetValue(AreaPlotVisibleProperty); }
     //       set { SetValue(AreaPlotVisibleProperty, value); }
     //   }
     //   public bool XAxisAsStandardDeviation
     //   {
     //       get { return (bool)GetValue(XAxisAsStandardDeviationProperty); }
     //       set { SetValue(XAxisAsStandardDeviationProperty, value); }
     //   }
     //   public bool Threshold
     //   {
     //       get { return (bool)GetValue(ThresholdProperty); }
     //       set { SetValue(ThresholdProperty, value); }
     //   }
     //   #endregion

     //   #region CallBacks
     //   //private static void LinkedPlotChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   //{
     //   //    FailureFunctionPlot owner = d as FailureFunctionPlot;
     //   //    OxyPlot.Wpf.PlotView myLinkedPlot = e.NewValue as OxyPlot.Wpf.PlotView;

     //   //    owner.LinkedPlot = myLinkedPlot;
     //   //}


     //   private static void FlipFrequencyAxisCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   {
     //       IndividualLinkedPlot owner = d as IndividualLinkedPlot;
     //       bool flipFreqAxis = Convert.ToBoolean(e.NewValue);
     //       if (flipFreqAxis == true)
     //       {
     //           //FlipFreqAxis(owner);
     //           owner.FlipFrequencyAxis = true;
     //           FlipFreqAxis(owner);
                
     //       }
     //   }
     //   //private static void MaxXCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   //{
     //   //    IndividualLinkedPlot owner = d as IndividualLinkedPlot;
     //   //    double maxX = Convert.ToDouble(e.NewValue);

     //   //        owner.MaxX = maxX;

     //   //}
        
     //   private static void PlotThresholdCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   {
     //       IndividualLinkedPlot owner = d as IndividualLinkedPlot;
     //       if (e.NewValue != null)
     //       {
     //           if (e.NewValue.GetType() == typeof(IMetric))
     //           {
     //               IMetric pt = (IMetric)e.NewValue;

     //               //DataPoint dp = owner.GetPairedValue(pt.ExceedanceTarget)
     //               owner.PlotThreshold(pt);

     //               //}
     //               //else
     //               //{
     //               //    owner.RemoveThresholdPlot();
     //               //}
     //           }
     //       }
     //       else
     //       {
     //           owner.RemoveThresholdPlot();
     //       }
           
     //   }
        
     //   private static void XAxisAsStandardDeviationCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   {
     //       //i don't need anything in here. The property gets set to true or false in xaml binding
     //   }
     //   private static void AreaPlotVisibleCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   {
     //       IndividualLinkedPlot owner = d as IndividualLinkedPlot;
     //       bool trackerVisible = Convert.ToBoolean(e.NewValue);
     //       if (trackerVisible == true)
     //       {
     //           owner.AddAreaPlots();
                
     //       }
     //       else
     //       {
     //           owner.RemoveAreaPlotsFromButtonClick();
     //       }
     //   }
     //   private static void TrackerVisibleCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   {
     //       IndividualLinkedPlot owner = d as IndividualLinkedPlot;
     //       bool trackerVisible = Convert.ToBoolean(e.NewValue);
     //       if (trackerVisible == true)
     //       {
     //           owner.ShowTracker();
     //       }
     //       else
     //       {
     //           owner.HideTracker();
     //           //if there is no tracker then turn the outside of range label off
     //          owner.TurnOutsideOfRangeOff();
     //       }
     //   }

     //   private static void SetYAxisToLogarithmicCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   {
     //       IndividualLinkedPlot owner = d as IndividualLinkedPlot;
     //       bool setYToLog = Convert.ToBoolean(e.NewValue);
     //       if (setYToLog == true)
     //       {
     //           SetYAxisToLog(owner);
     //           owner.SetYAxisToLogarithmic = true;
     //       }
     //   }
     //   private static void YAxisLabelChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   {
     //       IndividualLinkedPlot owner = d as IndividualLinkedPlot;
     //       string yAxisLabel = e.NewValue as string;

     //       owner.OxyPlot1.Model.Axes[1].Title = yAxisLabel;
     //   }
     //   private static void TitleChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   {
     //       IndividualLinkedPlot owner = d as IndividualLinkedPlot;
     //       string title = e.NewValue as string;

     //       owner.OxyPlot1.Model.Title = title;
     //   }
     //   private static void SubTitleChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   {
     //       IndividualLinkedPlot owner = d as IndividualLinkedPlot;
     //      // owner.TestName = e.NewValue as string;

     //       //owner.OxyPlot1.Model.Title += " - " + subTitle;

     //   }
     //   private static void XAxisLabelChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   {
     //       IndividualLinkedPlot owner = d as IndividualLinkedPlot;
     //       string xAxisLabel = e.NewValue as string;

     //       owner.OxyPlot1.Model.Axes[0].Title = xAxisLabel;
     //   }

     //   private static void BaseFunctionChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   {
     //       IndividualLinkedPlot owner = d as IndividualLinkedPlot;
     //       owner.BaseFunction = e.NewValue as FdaModel.Functions.BaseFunction;
            
     //   }
     //   private static void CurveChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
     //   {
     //       IndividualLinkedPlot owner = d as IndividualLinkedPlot;
     //       Statistics.CurveIncreasing curve = e.NewValue as Statistics.CurveIncreasing;

     //       ContentControl parentControl = Plots.IndividualLinkedPlotControl.FindParent<ContentControl>(owner);
     //       if (curve == null)

     //       {
     //           //find parent 
     //           if (parentControl != null && parentControl.GetType() == typeof(ConditionsIndividualPlotWrapper))
     //           {
     //               parentControl = IndividualLinkedPlotControl.FindParent<ContentControl>(parentControl);
     //           }

     //           if (parentControl != null && parentControl.GetType() == typeof(IndividualLinkedPlotControl))
     //           {
     //               FdaViewModel.Plots.IndividualLinkedPlotControlVM vm = (FdaViewModel.Plots.IndividualLinkedPlotControlVM)parentControl.DataContext;

     //               owner.BaseFunction = vm.IndividualPlotWrapperVM.PlotVM.BaseFunction;
     //               ((Plots.IndividualLinkedPlotControl)parentControl).LinkedPlot = null;
     //               ((IndividualLinkedPlotControl)parentControl).UpdateThePlots();
     //           }

     //           return;
     //       }

     //       ////if the y axis is log scale then no values of "0" will be displayed because there is no log(0). I change them to be close to zero.
     //       //if (owner.SetYAxisToLogarithmic == true)
     //       //{
     //       //    //foreach(double d in Curve.YValues)
     //       //    for (int i = 0; i < curve.YValues.Count; i++)
     //       //    {
     //       //        if (curve.get_Y(i) == 0)
     //       //        {
     //       //            double xValue = curve.get_X(i);
     //       //            curve.RemoveAt(i);
     //       //            curve.Insert(i, xValue, .00000000000001);
     //       //        }
     //       //    }
     //       //}
     //       owner.OxyPlot1.Model.Series.Clear();
     //       LineSeries series1 = new LineSeries();
     //       if (owner.XAxisAsStandardDeviation == true)//owner.Name != null && owner.Name.Equals("plot8"))
     //       {
     //           //Statistics.CurveIncreasing newCurve = owner.ConvertXValuesToStandardDeviation(curve);
     //           //for (int i = 0; i < newCurve.Count; i++)
     //           //{
     //           //    series1.Points.Add(new DataPoint(curve.get_X(i), curve.get_Y(i)));

     //           //}
     //           owner.OxyPlot1.Model.Axes[0].LabelFormatter = owner.ProbabilityAxisFormatter;
     //       }
     //       //else
     //       {
     //           for (int i = 0; i < curve.Count; i++)
     //           {
     //               //if the y axis is log scale then no values of "0" will be displayed because there is no log(0). I change them to be close to zero.
     //               if (owner.SetYAxisToLogarithmic == true && curve.get_Y(i) == 0)
     //               {
     //                   //if the value is 0 then don't plot that point
     //                   //series1.Points.Add(new DataPoint(curve.get_X(i), .000001));

     //               }
     //               else
     //               {
     //                   series1.Points.Add(new DataPoint(curve.get_X(i), curve.get_Y(i)));

     //               }

     //           }
     //       }

     //       owner.OxyPlot1.Model.Series.Add(series1);

     //       SetMinMaxValues(owner);

     //       if (curve.Count != 0 && owner.FlipFrequencyAxis == true)
     //       {
     //          FlipFreqAxis(owner);
     //       }

     //   }

     //   private  string ProbabilityAxisFormatter(double d)
     //   {
     //       double yValue = GetPairedValue(d, true, OxyPlot1.Model);
     //       //then i just need to get the paired value off of the original curve
     //       //cant use "vm" because it is different between the two. need a dep prop for it i guess.
     //       if(NonStandardDeviationCurve != null)
     //       {
                
     //           double prob = GetPairedValue(yValue, false, NonStandardDeviationCurve);
     //           if(prob != -1)
     //           {
     //               return (1- prob).ToString("N3");
     //           }
     //           else
     //           {
     //               return "";
     //           }
     //       }
     //       else
     //       {
     //           return "";
     //       }


     //       //if (d<0)
     //       //{
     //       //    return ".5";
     //       //}
     //       //else if (d <= .5 && d > 0)
     //       //{
     //       //    return ".69";
     //       //}
     //       //else if (d <= 1 && d > .5)
     //       //{
     //       //    return ".84";
     //       //}
     //       //else if (d <=1.5  && d > 1)
     //       //{
     //       //    return ".93";
     //       //}
     //       //else if (d <=2 && d > 1.5)
     //       //{
     //       //    return ".97";
     //       //}
     //       //else if(d<=2.5 && d>2)
     //       //{
     //       //    return ".99";
     //       //}
     //       //else if (d <= 3 && d > 2.5)
     //       //{
     //       //    return ".999";
     //       //}
     //       //else
     //       //{
     //       //    return "test";
     //       //}
     //   }
        

       


     //   #endregion

     //   #region Constructors
     //   public IndividualLinkedPlot()
     //   {
     //       InitializeComponent();


     //       //This turns off the scrolling
     //       OxyPlot1.ActualController.UnbindAll();

     //       OxyPlot1.Model = new PlotModel();
     //       OxyPlot1.Model.Title = "Failure Function Plot";
     //       LinearAxis xAxis = new LinearAxis();
     //       xAxis.Position = AxisPosition.Bottom;
     //       xAxis.Title = "Exterior Stage";
     //       LinearAxis yAxis = new LinearAxis();
     //       yAxis.Position = AxisPosition.Left;
     //       yAxis.Title = "Probability";
     //       OxyPlot1.Model.Axes.Add(xAxis);
     //       OxyPlot1.Model.Axes.Add(yAxis);

     //       OxyPlot1.Model.PlotMargins = new OxyThickness(60, 10, 10, 40);

     //       OxyPlot1.Model.MouseMove += Model_MouseMove;
     //       OxyPlot1.Model.MouseDown += Model_MouseDown;

     //       ListOfRemovedAreaSeries = new List<AreaSeries>();

     //   }

        




     //   #endregion

        

     //   private void UserControl_Loaded(object sender, RoutedEventArgs e)
     //   {
     //       //if the y axis is log scale then we can't have a zero. I change them from zero to close to zero. I don't change the actual 
     //       // curve that we are storing, just the curve that is being displayed. It should have no effect on any computations.
     //       if (SetYAxisToLogarithmic == true)
     //       {
     //           foreach (Series s in OxyPlot1.Model.Series)
     //           {
     //               if (s.GetType() == typeof(LineSeries))
     //               {
     //                   for (int i = 0; i < ((LineSeries)s).Points.Count; i++)
     //                   {
     //                       //if the y axis is log scale then no values of "0" will be displayed because there is no log(0). I change them to be close to zero.
     //                       if ( ((LineSeries)s).Points[i].Y == 0)
     //                       {
     //                           ((LineSeries)s).Points[i] = new DataPoint(((LineSeries)s).Points[i].X, .001);

     //                       }
     //                   }
     //               }
     //           }
     //       }
           
     //       //PlotLowerXAreaPlot();
     //       //PlotHigherXAreaPlot();
     //       //PlotLowerYAreaPlot();
     //       //PlotHigherYAreaPlot();
     //       //if(ThisIsEndNode == true)//this is my way of knowing if it is plot 8
     //       //{
     //       //    PlotAreaUnderTheCurve();
     //       //}

     //       //find parent and add this plot to its selectedPlot property.
     //       ContentControl parentControl = Plots.IndividualLinkedPlotControl.FindParent<ContentControl>(this);
     //       if (parentControl != null && parentControl.GetType() == typeof(ConditionsIndividualPlotWrapper))
     //       {
     //           parentControl = IndividualLinkedPlotControl.FindParent<ContentControl>(parentControl);
     //       }

     //       if (parentControl != null && parentControl.GetType() == typeof(IndividualLinkedPlotControl))
     //       {
     //           FdaViewModel.Plots.IndividualLinkedPlotControlVM vm = (FdaViewModel.Plots.IndividualLinkedPlotControlVM)parentControl.DataContext;

     //           this.BaseFunction = vm.IndividualPlotWrapperVM.PlotVM.BaseFunction;
     //           ((IndividualLinkedPlotControl)parentControl).LinkedPlot = this;
     //           ((IndividualLinkedPlotControl)parentControl).UpdateThePlots();
     //       }




     //   }

     //   public void TurnOutsideOfRangeOn()
     //   {
     //       txt_OutsideOfRange.Visibility = Visibility.Visible;
     //       this.TrackerIsOutsideTheCurveRange = true;
     //       TurnTrackerOff();
            
     //   }
     //   public void TurnOutsideOfRangeOff()
     //   {
     //       txt_OutsideOfRange.Visibility = Visibility.Hidden;

     //       this.TrackerIsOutsideTheCurveRange = false;
     //       //ShowTracker();

     //   }

     //   //private void SetCurve(Statistics.CurveIncreasing curve)
     //   //{
     //   //    OxyPlot1.Model.Series.Clear();
     //   //    LineSeries series1 = new LineSeries();

     //   //    for (int i = 0; i < curve.Count; i++)
     //   //    {
     //   //        //if the y axis is log scale then no values of "0" will be displayed because there is no log(0). I change them to be close to zero.
     //   //        if (SetYAxisToLogarithmic == true && curve.get_Y(i) == 0)
     //   //        {
     //   //            series1.Points.Add(new DataPoint(curve.get_X(i), .000001));

     //   //        }
     //   //        else
     //   //        {
     //   //            series1.Points.Add(new DataPoint(curve.get_X(i), curve.get_Y(i)));

     //   //        }

     //   //    }

     //   //    OxyPlot1.Model.Series.Add(series1);

     //   //    SetMinMaxValues(this);

     //   //    if (curve.Count != 0 && FlipFrequencyAxis == true)
     //   //    {
     //   //        FlipFreqAxis(this);
     //   //    }

     //   //}

     //   #region Hide/show tracker
     //   public void HideTracker()
     //   {
     //       TurnTrackerOff();
     //       _HideTracker = true;
     //   }
     //   private void TurnTrackerOff()
     //   {
     //       if (OxyPlot1.Model.Series.Count == 0) { return; }
     //       double largeNegativeNumber = -100000000; // just a big number that should be off every plot
     //       DataPoint dp = new DataPoint(largeNegativeNumber, largeNegativeNumber);
     //       ScreenPoint position = OxyPlot1.Model.Axes[0].Transform(largeNegativeNumber, largeNegativeNumber, OxyPlot1.Model.Axes[1]);
     //       //ScreenPoint sp = new ScreenPoint(position.X, position.Y);
     //       TrackerHitResult thr = new TrackerHitResult(); // _OxyPlotModel.Series[0], newPoint, sp);
     //       thr.Series = OxyPlot1.Model.Series[0];
     //       thr.DataPoint = dp;
     //       thr.Position = position;
     //       //thr.Text = XAxisLabel + ": " + Math.Round(dp.X, 3).ToString() + Environment.NewLine + YAxisLabel + ": " + Math.Round(dp.Y, 3).ToString();
     //       OxyPlot1.ShowTracker(thr);
     //   }
     //   public void ShowTracker()
     //   {
     //       _HideTracker = false;

     //   }
     //   #endregion
     //   #region Produce Area Plots

     //   private LineSeries GetCurrentLineSeries()
     //   {
     //       if (OxyPlot1.Model.Series.Count > 0)
     //       {
     //           foreach (Series s in OxyPlot1.Model.Series)
     //           {
     //               if (s.GetType() == typeof(LineSeries))
     //               {
     //                   return (LineSeries)s;
     //               }
     //           }
     //       }        
     //       return null;      
     //   }
     //   public void PlotAreaUnderTheCurve()
     //   {
     //       AreaSeries AreaUnderTheCurveSeries = new AreaSeries();
     //       LineSeries thisLineSeries = GetCurrentLineSeries();
     //       if (thisLineSeries != null)
     //       {

     //           //double startValue = 1 - thisLineSeries.Points[0].X;
     //           //double endValue = 1 - thisLineSeries.Points[thisLineSeries.Points.Count - 1].X;
     //           if(MinY == 0)
     //           {
     //               MinY = .001;
     //           }
     //           foreach (DataPoint dp in thisLineSeries.Points)
     //           {
     //               AreaUnderTheCurveSeries.Points.Add(dp);
     //               AreaUnderTheCurveSeries.Points2.Add(new DataPoint(dp.X, MinY));//can't do zero y because log axis will lose its mind

     //           }


     //           //while (startValue < endValue)//&& startValue > MinX)
     //           //{
     //           //    DataPoint dp = new DataPoint(startValue, GetPairedValue(startValue, true, this.OxyPlot1.Model, true));
     //           //    if (dp.Y != -1)

     //           //        AreaUnderTheCurveSeries.Points.Add(dp);
     //           //    AreaUnderTheCurveSeries.Points2.Add(new DataPoint(dp.X, MinY));//can't do zero y because log axis will lose its mind

     //           //    startValue += incrementValue;
     //           //}

     //           //AreaUnderTheCurveSeries.Points.Add(new DataPoint(MinX, .01));
     //           //AreaUnderTheCurveSeries.Points.Add(new DataPoint(HigherMinX, .01));
     //           //AreaUnderTheCurveSeries.Points2.Add(new DataPoint(MinX, OxyPlot1.Model.Axes[1].Maximum));
     //           //AreaUnderTheCurveSeries.Points2.Add(new DataPoint(HigherMinX, OxyPlot1.Model.Axes[1].Maximum));
     //           AreaUnderTheCurveSeries.Color = AREA_UNDER_CURVE_COLOR;
     //           //lowerAreaSeries.Fill = OxyColors.LightPink;
     //           AreaUnderTheCurveSeries.Fill = AREA_UNDER_CURVE_COLOR;

     //           this.OxyPlot1.Model.Series.Add(AreaUnderTheCurveSeries);
     //           this.OxyPlot1.InvalidatePlot(true);
     //       }
     //   }


     //   public void PlotLowerXAreaPlot()
     //   {
     //       if (FlipFrequencyAxis == false)
     //       {
     //           AreaSeries lowerAreaSeries = new AreaSeries();
     //           lowerAreaSeries.Points.Add(new DataPoint(MinX, OxyPlot1.Model.Axes[1].ActualMinimum));// .001));
     //           lowerAreaSeries.Points.Add(new DataPoint(HigherMinX, OxyPlot1.Model.Axes[1].ActualMinimum));// .001));
     //           //lowerAreaSeries.Points2.Add(new DataPoint(MinX, OxyPlot1.Model.Axes[1].Maximum));
     //           //lowerAreaSeries.Points2.Add(new DataPoint(HigherMinX, OxyPlot1.Model.Axes[1].Maximum));
     //           lowerAreaSeries.Points2.Add(new DataPoint(MinX, MaxY));
     //           lowerAreaSeries.Points2.Add(new DataPoint(HigherMinX, MaxY));
     //           lowerAreaSeries.Color = AREA_PLOT_COLOR;
     //           //lowerAreaSeries.Fill = OxyColors.LightPink;
     //           lowerAreaSeries.Fill = AREA_PLOT_COLOR;

     //           this.OxyPlot1.Model.Series.Add(lowerAreaSeries);
     //           this.OxyPlot1.InvalidatePlot(true);
     //       }
     //       else
     //       {
     //           AreaSeries lowerAreaSeries = new AreaSeries();
     //           lowerAreaSeries.Points.Add(new DataPoint(1 - MinX, OxyPlot1.Model.Axes[1].ActualMinimum));// .001));
     //           lowerAreaSeries.Points.Add(new DataPoint(1 - HigherMinX, OxyPlot1.Model.Axes[1].ActualMinimum));// .001));
     //           //lowerAreaSeries.Points2.Add(new DataPoint(1 - MinX, OxyPlot1.Model.Axes[1].Maximum));
     //           //lowerAreaSeries.Points2.Add(new DataPoint(1 - HigherMinX, OxyPlot1.Model.Axes[1].Maximum));
     //           lowerAreaSeries.Points2.Add(new DataPoint(1 - MinX, MaxY));
     //           lowerAreaSeries.Points2.Add(new DataPoint(1 - HigherMinX, MaxY));
     //           lowerAreaSeries.Color = AREA_PLOT_COLOR;
     //           //lowerAreaSeries.Fill = OxyColors.LightPink;
     //           lowerAreaSeries.Fill = AREA_PLOT_COLOR;

     //           this.OxyPlot1.Model.Series.Add(lowerAreaSeries);
     //           this.OxyPlot1.InvalidatePlot(true);
     //       }
            



     //   }

     //   public void PlotHigherXAreaPlot()
     //   {
     //       if (FlipFrequencyAxis == false)
     //       {
     //           AreaSeries lowerAreaSeries = new AreaSeries();
     //           lowerAreaSeries.Points.Add(new DataPoint(LowerMaxX, OxyPlot1.Model.Axes[1].ActualMinimum));//.001));
     //           lowerAreaSeries.Points.Add(new DataPoint(MaxX, OxyPlot1.Model.Axes[1].ActualMinimum));//.001));
     //           //lowerAreaSeries.Points2.Add(new DataPoint(LowerMaxX, OxyPlot1.Model.Axes[1].Maximum));
     //           //lowerAreaSeries.Points2.Add(new DataPoint(MaxX, OxyPlot1.Model.Axes[1].Maximum));
     //           lowerAreaSeries.Points2.Add(new DataPoint(LowerMaxX, MaxY));
     //           lowerAreaSeries.Points2.Add(new DataPoint(MaxX, MaxY));
     //           lowerAreaSeries.Color = AREA_PLOT_COLOR;
     //           //lowerAreaSeries.Fill = OxyColors.LightPink;
     //           lowerAreaSeries.Fill = AREA_PLOT_COLOR;

     //           this.OxyPlot1.Model.Series.Add(lowerAreaSeries);
     //           this.OxyPlot1.InvalidatePlot(true);
     //       }
     //       else
     //       {
     //           AreaSeries lowerAreaSeries = new AreaSeries();
     //           lowerAreaSeries.Points.Add(new DataPoint(1 - LowerMaxX, OxyPlot1.Model.Axes[1].ActualMinimum));//.001));
     //           lowerAreaSeries.Points.Add(new DataPoint(1 - MaxX, OxyPlot1.Model.Axes[1].ActualMinimum));//.001));
     //           //lowerAreaSeries.Points2.Add(new DataPoint(1 - LowerMaxX, OxyPlot1.Model.Axes[1].Maximum));
     //           //lowerAreaSeries.Points2.Add(new DataPoint(1 - MaxX, OxyPlot1.Model.Axes[1].Maximum));
     //           lowerAreaSeries.Points2.Add(new DataPoint(1 - LowerMaxX, MaxY));
     //           lowerAreaSeries.Points2.Add(new DataPoint(1 - MaxX, MaxY));
     //           lowerAreaSeries.Color = AREA_PLOT_COLOR;
     //           //lowerAreaSeries.Fill = OxyColors.LightPink;
     //           lowerAreaSeries.Fill = AREA_PLOT_COLOR;

     //           this.OxyPlot1.Model.Series.Add(lowerAreaSeries);
     //           this.OxyPlot1.InvalidatePlot(true);
     //       }
     //   }

     //   private void UpdateYAxisAreaPlots()
     //   {
     //       if(!(HigherMinY == 0 && LowerMaxY == 0))
     //       {
     //           RemoveAreaPlots();
     //           PlotLowerYAreaPlot();
     //           PlotHigherYAreaPlot();
     //       }
     //   }

     //   private void UpdateXAxisAreaPlots()
     //   {
     //       if (!(HigherMinX == 0 && LowerMaxX == 0))
     //       {
     //           RemoveAreaPlots();
     //           PlotLowerXAreaPlot();
     //           PlotHigherXAreaPlot();
     //       }
     //   }

     //   public void PlotLowerYAreaPlot()
     //   {
     //       AreaSeries lowerAreaSeries = new AreaSeries();
     //       lowerAreaSeries.Points.Add(new DataPoint(OxyPlot1.Model.Axes[0].ActualMinimum,MinY));//.001, MinY));
     //       lowerAreaSeries.Points.Add(new DataPoint(OxyPlot1.Model.Axes[0].ActualMinimum,HigherMinY));//.001, HigherMinY));
     //       //lowerAreaSeries.Points2.Add(new DataPoint(OxyPlot1.Model.Axes[0].Maximum, MinY));
     //       //lowerAreaSeries.Points2.Add(new DataPoint(OxyPlot1.Model.Axes[0].Maximum, HigherMinY));
     //       lowerAreaSeries.Points2.Add(new DataPoint(MaxX, MinY));
     //       lowerAreaSeries.Points2.Add(new DataPoint(MaxX, HigherMinY));
     //       lowerAreaSeries.Color = AREA_PLOT_COLOR;
     //       //lowerAreaSeries.Fill = OxyColors.LightPink;
     //       lowerAreaSeries.Fill = AREA_PLOT_COLOR;

     //       this.OxyPlot1.Model.Series.Add(lowerAreaSeries);
     //       this.OxyPlot1.InvalidatePlot(true);
     //   }

     //   public void PlotHigherYAreaPlot()
     //   {
     //       AreaSeries lowerAreaSeries = new AreaSeries();
     //       lowerAreaSeries.Points.Add(new DataPoint(OxyPlot1.Model.Axes[0].ActualMinimum,LowerMaxY));//.001, LowerMaxY));
     //       lowerAreaSeries.Points.Add(new DataPoint(OxyPlot1.Model.Axes[0].ActualMinimum, MaxY));//, MaxY));
     //       //lowerAreaSeries.Points2.Add(new DataPoint(OxyPlot1.Model.Axes[0].Maximum, LowerMaxY));
     //       //lowerAreaSeries.Points2.Add(new DataPoint(OxyPlot1.Model.Axes[0].Maximum, MaxY));
     //       lowerAreaSeries.Points2.Add(new DataPoint(MaxX, LowerMaxY));
     //       lowerAreaSeries.Points2.Add(new DataPoint(MaxX, MaxY));
     //       lowerAreaSeries.Color = AREA_PLOT_COLOR;
     //       //lowerAreaSeries.Fill = OxyColors.LightPink;
     //       lowerAreaSeries.Fill = AREA_PLOT_COLOR;

     //       this.OxyPlot1.Model.Series.Add(lowerAreaSeries);
     //       this.OxyPlot1.InvalidatePlot(true);
     //   }

     //   #endregion

     //   #region AxisSettings
     //   public static void FlipFreqAxis(IndividualLinkedPlot plot)
     //   {
     //       //if(plot.OxyPlot1.Model.Series.Count == 0) { return; }
     //       //LineSeries thisLineSeries = (LineSeries)plot.OxyPlot1.Model.Series[0];
     //       LineSeries thisLineSeries = new LineSeries();
     //       int k = 0;
     //       if (plot.OxyPlot1.Model.Series.Count > 0)
     //       {
     //           for(k=0; k<plot.OxyPlot1.Model.Series.Count;k++)// (Series s in plot.OxyPlot1.Model.Series)
     //           {
     //               if (plot.OxyPlot1.Model.Series[k].GetType() == typeof(LineSeries))
     //               {
     //                   thisLineSeries = (LineSeries)plot.OxyPlot1.Model.Series[k];
     //                   break;
     //               }
     //           }
     //       }
     //       else { return; }
     //       double[] xValues = new double[thisLineSeries.Points.Count];
     //       double[] yValues = new double[thisLineSeries.Points.Count];
     //       for(int i = 0;i<thisLineSeries.Points.Count;i++)
     //       {
     //           xValues[i] = 1 - thisLineSeries.Points[i].X;
     //           yValues[i] = thisLineSeries.Points[i].Y;
     //       }
     //       LineSeries newLineSeries = new LineSeries();
     //       for (int i = 0; i < xValues.Length; i++)
     //       {
     //           newLineSeries.Points.Add(new DataPoint(xValues[i], yValues[i]));

     //       }
     //       plot.OxyPlot1.Model.Series[k] = newLineSeries;
     //       plot.OxyPlot1.Model.Axes[0].StartPosition = 1;
     //       plot.OxyPlot1.Model.Axes[0].EndPosition = .001;

     //       //plot.MinX = xValues[xValues.Count()-1];
     //       //plot.MinY = yValues[yValues.Count()-1];
     //       //plot.MaxX = xValues[0];
     //       //plot.MaxY = yValues[0];

     //       plot.OxyPlot1.Model.Axes[0].Maximum = 1-plot.MinX; //xValues[0];
     //       plot.OxyPlot1.Model.Axes[0].Minimum = 1- plot.MaxX; //xValues[xValues.Count() - 1];
     //       //plot.OxyPlot1.Model.Axes[1].Maximum = yValues[0];
     //       //plot.OxyPlot1.Model.Axes[1].Minimum = yValues[yValues.Count() - 1];

     //       plot.OxyPlot1.InvalidatePlot(true);
            

     //   }
     //   private static void SetYAxisToLog(IndividualLinkedPlot plot)
     //   {
     //       PlotModel thisPlotModel = plot.OxyPlot1.Model;

     //       LogarithmicAxis yAxis = new LogarithmicAxis();
     //       yAxis.Position = AxisPosition.Left;
     //       yAxis.Title = plot.YAxisLabel; //this is because this method is creating a new axis which was clearing out the label
     //       thisPlotModel.Axes[1] = yAxis;

     //   }
     //   #endregion

     //   #region SetAxisValues
     //   /// <summary>
     //   /// This method gets the min and max x and y values to be used when setting up the axes for plotting.
     //   /// 
     //   /// </summary>
     //   public static void SetMinMaxValues(IndividualLinkedPlot plot)
     //   {
     //       LineSeries thisLineSeries = new LineSeries();
     //       if(plot.OxyPlot1.Model.Series.Count>0)
     //       {
     //           foreach(Series s in plot.OxyPlot1.Model.Series)
     //           {
     //               if(s.GetType() == typeof(LineSeries))
     //               {
     //                   thisLineSeries = (LineSeries)s;
     //               }
     //           }
     //       }
     //       else { return; }
     //       //thisLineSeries = (LineSeries)plot.OxyPlot1.Model.Series[0];
     //       if(thisLineSeries.Points.Count == 0) { return; }

     //       double minX = thisLineSeries.Points[0].X;
     //       double maxX = thisLineSeries.Points[thisLineSeries.Points.Count - 1].X;
     //       double minY = thisLineSeries.Points[0].Y;
     //       double maxY = thisLineSeries.Points[thisLineSeries.Points.Count - 1].Y;

     //       if (minX > maxX)
     //       {
     //           double tempX = minX;
     //           minX = maxX;
     //           maxX = tempX;
     //       }
     //       if (minY > maxY)
     //       {
     //           double tempY = minY;
     //           minY = maxY;
     //           maxY = tempY;
     //       }
     //       plot.MinX = minX;
     //       plot.MinY = minY;
     //       plot.MaxX = maxX;
     //       plot.MaxY = maxY;

     //       plot.OxyPlot1.Model.Axes[0].Minimum = minX;
     //       plot.OxyPlot1.Model.Axes[0].Maximum = maxX;
     //       plot.OxyPlot1.Model.Axes[1].Minimum = minY;
     //       plot.OxyPlot1.Model.Axes[1].Maximum = maxY;
     //   }

     //   private void ResetMinMaxValues(IndividualLinkedPlot otherPlot)
     //   {
     //       SetMinMaxValues(otherPlot);
     //       HigherMinX = 0;
     //       HigherMinY = 0;
     //       LowerMaxX = 0;
     //       LowerMaxY = 0;

     //       otherPlot.HigherMinX = 0;
     //       otherPlot.HigherMinY = 0;
     //       otherPlot.LowerMaxX = 0;
     //       otherPlot.LowerMaxY = 0;


     //   }

     //   /// <summary>
     //   /// This method will get the lowest x value between the two plots and will change their axes to start at that point.
     //   /// This method will get the highest x value between the two plots and will change their axes to end at that point and redraw the plot.
     //   /// </summary>
     //   /// <param name="plotToCompareWith"></param>
     //   public void SetSharedXAxisWithPlot(IndividualLinkedPlot plotToCompareWith)
     //   {
     //       //clear out the min and max values before resetting
     //       //ResetMinMaxValues(plotToCompareWith);
     //       HasXAreaPlots = true;
     //       plotToCompareWith.HasXAreaPlots = true;

     //       _PlotThatSharesAnAreaPlot = plotToCompareWith;

     //       //double minX;
     //       if (this.MinX < plotToCompareWith.MinX)
     //       {
     //           this.HigherMinX = plotToCompareWith.MinX;
     //           plotToCompareWith.MinX = this.MinX;
     //           plotToCompareWith.HigherMinX = this.HigherMinX;

     //       }
     //       else
     //       {
     //           plotToCompareWith.HigherMinX = this.MinX;
     //           this.MinX = plotToCompareWith.MinX;
     //           this.HigherMinX = plotToCompareWith.HigherMinX;
     //       }



     //       //double maxX;
     //       if (this.MaxX < plotToCompareWith.MaxX)
     //       {
     //           plotToCompareWith.LowerMaxX = this.MaxX;
     //           MaxX = plotToCompareWith.MaxX;
     //           this.LowerMaxX = plotToCompareWith.LowerMaxX;
     //       }
     //       else
     //       {
     //           this.LowerMaxX = plotToCompareWith.MaxX;
     //           plotToCompareWith.MaxX = this.MaxX;
     //           plotToCompareWith.LowerMaxX = this.LowerMaxX;
     //       }
       
     //           this.OxyPlot1.Model.Axes[0].Maximum = MaxX;
     //           this.OxyPlot1.Model.Axes[0].Minimum = MinX;

     //           plotToCompareWith.OxyPlot1.Model.Axes[0].Maximum = MaxX;
     //           plotToCompareWith.OxyPlot1.Model.Axes[0].Minimum = MinX;

            


     //       if (AreaPlotVisible)
     //       {

     //           //UpdateYAxisAreaPlots();
     //           RemoveAreaPlots();
     //       plotToCompareWith.RemoveAreaPlots();

     //       if (HasYAreaPlots)
     //       {
     //           PlotLowerYAreaPlot();
     //           PlotHigherYAreaPlot();
     //       }
     //       if (plotToCompareWith.HasYAreaPlots)
     //       {
     //           plotToCompareWith.PlotLowerYAreaPlot();
     //           plotToCompareWith.PlotHigherYAreaPlot();
     //       }
           

           
     //           PlotLowerXAreaPlot();
     //           PlotHigherXAreaPlot();
     //           plotToCompareWith.PlotLowerXAreaPlot();
     //           plotToCompareWith.PlotHigherXAreaPlot();
     //           //draw the area plot under the curve of plot8
     //           if (BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
     //           {
     //               PlotAreaUnderTheCurve();
     //           }
     //           else if (plotToCompareWith.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
     //           {
     //               plotToCompareWith.PlotAreaUnderTheCurve();
     //           }
     //       }

     //       this.OxyPlot1.InvalidatePlot(true);
     //       plotToCompareWith.OxyPlot1.InvalidatePlot(true);


     //   }


     //   public void SetSharedYAxisWithPlot(IndividualLinkedPlot plotToCompareWith)
     //   {
     //       //clear out the min and max values before resetting
     //       //ResetMinMaxValues(plotToCompareWith);
     //       HasYAreaPlots = true;
     //       plotToCompareWith.HasYAreaPlots = true;

     //       _PlotThatSharesAnAreaPlot = plotToCompareWith;


     //       if (this.MinY < plotToCompareWith.MinY)
     //       {
     //           HigherMinY = plotToCompareWith.MinY;
     //           plotToCompareWith.MinY = this.MinY;
     //           plotToCompareWith.HigherMinY = this.HigherMinY;
     //       }
     //       else
     //       {
     //           HigherMinY = MinY;
     //           MinY = plotToCompareWith.MinY;
     //           plotToCompareWith.HigherMinY = HigherMinY;

     //       }


     //       if (this.MaxY < plotToCompareWith.MaxY)
     //       {
     //           LowerMaxY = MaxY;
     //           MaxY = plotToCompareWith.MaxY;
     //           plotToCompareWith.LowerMaxY = LowerMaxY;

     //       }
     //       else
     //       {
     //           LowerMaxY = plotToCompareWith.MaxY;
     //           plotToCompareWith.MaxY = this.MaxY;
     //           plotToCompareWith.LowerMaxY = LowerMaxY;
     //       }


     //       this.OxyPlot1.Model.Axes[1].Maximum = MaxY;
     //       this.OxyPlot1.Model.Axes[1].Minimum = MinY;

     //       plotToCompareWith.OxyPlot1.Model.Axes[1].Maximum = MaxY;
     //       plotToCompareWith.OxyPlot1.Model.Axes[1].Minimum = MinY;

            

     //       if (AreaPlotVisible)
     //       {
     //           //UpdateXAxisAreaPlots();
     //           RemoveAreaPlots();
     //       plotToCompareWith.RemoveAreaPlots();

     //       if (HasXAreaPlots)
     //       {
     //           PlotLowerXAreaPlot();
     //           PlotHigherXAreaPlot();
     //       }
     //       if (plotToCompareWith.HasXAreaPlots)
     //       {
     //           plotToCompareWith.PlotLowerXAreaPlot();
     //           plotToCompareWith.PlotHigherXAreaPlot();
     //       }
            

          
     //           PlotLowerYAreaPlot();
     //           PlotHigherYAreaPlot();
     //           plotToCompareWith.PlotLowerYAreaPlot();
     //           plotToCompareWith.PlotHigherYAreaPlot();
     //           //draw the area plot under the curve of plot8
     //           if (BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
     //           {
     //               PlotAreaUnderTheCurve();
     //           }
     //           else if (plotToCompareWith.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
     //           {
     //               plotToCompareWith.PlotAreaUnderTheCurve();
     //           }
     //       }

     //       this.OxyPlot1.InvalidatePlot(true);
     //       plotToCompareWith.OxyPlot1.InvalidatePlot(true);
     //   }

     //   #endregion

     //   #region DisplayTrackers

     //   private void DisplayCurrentTracker(DataPoint dp)
     //   {
     //       ScreenPoint position = OxyPlot1.Model.Axes[0].Transform(dp.X, dp.Y, OxyPlot1.Model.Axes[1]);
     //       //ScreenPoint sp = new ScreenPoint(position.X, position.Y);
     //       TrackerHitResult thr = new TrackerHitResult(); // _OxyPlotModel.Series[0], newPoint, sp);
     //       thr.Series = OxyPlot1.Model.Series[0];
     //       thr.DataPoint = dp;
     //       thr.Position = position;
     //       if (XAxisAsStandardDeviation)
     //       {
     //           double newXValue = GetPairedValue(dp.Y, false, NonStandardDeviationCurve);
     //           thr.Text = XAxisLabel + ": " + Math.Round(1-newXValue, 3).ToString() + Environment.NewLine + YAxisLabel + ": " + Math.Round(dp.Y, 3).ToString();

     //       }
     //       else
     //       {
     //           thr.Text = XAxisLabel + ": " + Math.Round(dp.X, 3).ToString() + Environment.NewLine + YAxisLabel + ": " + Math.Round(dp.Y, 3).ToString();
     //       }
     //       OxyPlot1.ShowTracker(thr);
     //   }

     //   private bool IsPointInRange(double x, double y)
     //   {
     //       //only display to the top and bottom of curve?
     //       if (x > MaxX || y > MaxY || x < MinX || y < MinY)
     //       {
     //           return false;
     //       } //this is checking if the point will be somewhere in the entire plot window

     //       double maxCurveX = Curve.XValues.Max();
     //       double minCurveX = Curve.XValues.Min();
     //       double maxCurveY = Curve.YValues.Max();
     //       double minCurveY = Curve.YValues.Min();

     //       if (FlipFrequencyAxis)
     //       {
     //           //maxCurveX = 1 - minCurveX;
     //           //minCurveX = 1 - maxCurveX;
     //           x = 1 - x;
     //       }

     //       if (x > maxCurveX || y > maxCurveY || x < minCurveX || y< minCurveY)
     //       {
     //           return false;
     //       } 
     //       return true;
     //   }

     //   /// <summary>
     //   /// This method displays an oxyplot tracker in the next linked plot at position (x,y). Then the x and y value of the next plot after that is calculated, and "DisplayNextTracker" is called with the new (x,y) values.
     //   /// This will continue down the line until "isEndNode" is true
     //   /// </summary>
     //   /// <param name="x"></param>
     //   /// <param name="y"></param>
     //   public void DisplayNextTracker(double x, double y)
     //   {

     //       if (PreviousPlot.TrackerIsOutsideTheCurveRange == true)
     //       {
     //           TurnOutsideOfRangeOn();
     //           _NextPlot.DisplayNextTracker(0, 0);//values don't matter here
     //           return;
     //       }
     //       else
     //       {
     //           TurnOutsideOfRangeOff();
     //       }

     //       if (IsPointInRange(x, y) == false)
     //       {
     //           TurnOutsideOfRangeOn();
     //           _NextPlot.DisplayNextTracker(0, 0);//values don't matter here because tracker is out of range
     //           return;
     //       }
           
     //       ////only display to the top and bottom of curve?
     //       //if (x > MaxX || y > MaxY || x < MinX || y < MinY)

     //       //{
     //       //    //TrackerOutsideRangeFromDisplayNextTracker();
     //       //    TurnOutsideOfRangeOn();
     //       //    _NextPlot.DisplayNextTracker(0, 0);
     //       //    return; } //this is checking if the point will be somewhere in the entire plot window
     //       //if (x > Curve.XValues.Max() || y > Curve.YValues.Max()) { 
     //       //    //TrackerOutsideRangeFromDisplayNextTracker();
     //       //    TurnOutsideOfRangeOn();
     //       //    _NextPlot.DisplayNextTracker(0, 0);
     //       //    return; } //this checks if the point is out of the range of the curve

            
     //       //if (x < Curve.XValues.Min() || y < Curve.YValues.Min())
     //       //{ 
     //       //    //TrackerOutsideRangeFromDisplayNextTracker();
     //       //    TurnOutsideOfRangeOn();
     //       //    _NextPlot.DisplayNextTracker(0, 0);

     //       //    return;
     //       //}

     //       DataPoint dp = new DataPoint(x, y);

     //       DisplayCurrentTracker(dp);

     //       if (IsEndNode == true)
     //       {
     //           return;
     //       }

     //       //get the x and y values for the next plot
     //       try
     //       {
     //           DataPoint nextPlotDataPoint = GetNextTrackerDataPoint(_NextPlotSharedAxisEnum, dp);
     //           _NextPlot.DisplayNextTracker(nextPlotDataPoint.X, nextPlotDataPoint.Y);
     //       }
     //       catch (Exception e)
     //       {
     //           //it wasn't able to get the point from getPreviousTrackerDataPoint because it was out of the range of the curve. I don't think i care.
     //       }
     //   }

     //   //private void TrackerOutsideRangeFromDisplayNextTracker()
     //   //{
     //   //    this.TrackerIsOutsideTheCurveRange = true;
     //   //    FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM vm = (FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM)this.DataContext;
     //   //    vm.PlotIsOutsideRange(this, new EventArgs());

     //   //    TurnTrackerOff();
     //   //    _NextPlot.DisplayNextTracker(0, 0);//values don't matter here
     //   //}

     //   //private void TrackerOutsideRangeFromDisplayPreviousTracker()
     //   //{
     //   //    this.TrackerIsOutsideTheCurveRange = true;
     //   //    FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM vm = (FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM)this.DataContext;
     //   //    vm.PlotIsOutsideRange(this, new EventArgs());

     //   //    TurnTrackerOff();
     //   //    _PreviousPlot.DisplayPreviousTracker(0, 0);//values don't matter here
     //   //}

     //   private DataPoint GetNextTrackerDataPoint(SharedAxisEnum nextPlotSharedAxis, DataPoint currentDataPoint)
     //   {
     //       DataPoint dp = new DataPoint();
     //       switch (nextPlotSharedAxis)
     //       {
     //           case SharedAxisEnum.XX:
     //               {
     //                   dp = GetNextTrackerDataPointForSharedAxisXX(currentDataPoint.X);
     //                   break;
     //               }
     //           case SharedAxisEnum.XY:
     //               {
     //                   dp = GetNextTrackerDataPointForSharedAxisXY(currentDataPoint.X);
     //                   break;
     //               }
     //           case SharedAxisEnum.YX:
     //               {
     //                   dp = GetNextTrackerDataPointForSharedAxisYX(currentDataPoint.Y);
     //                   break;
     //               }
     //           case SharedAxisEnum.YY:
     //               {
     //                   dp = GetNextTrackerDataPointForSharedAxisYY(currentDataPoint.Y);
     //                   break;
     //               }
     //       }
     //       return dp;
     //   }

     //private DataPoint getPreviousTrackerDataPoint(SharedAxisEnum previousPlotSharedAxis, DataPoint currentDataPoint)
     //   {
     //       DataPoint nextPlotDataPoint = new DataPoint();
     //       switch (_PreviousPlotSharedAxisEnum)
     //       {
     //           case SharedAxisEnum.XX:
     //               {
     //                   nextPlotDataPoint = GetPreviousTrackerDataPointForSharedAxisXX(currentDataPoint.X);
     //                   break;
     //               }
     //           case SharedAxisEnum.XY:
     //               {
     //                   nextPlotDataPoint = GetPreviousTrackerDataPointForSharedAxisXY(currentDataPoint.X, currentDataPoint.Y);
     //                   break;
     //               }
     //           case SharedAxisEnum.YX:
     //               {
     //                   nextPlotDataPoint = GetPreviousTrackerDataPointForSharedAxisYX(currentDataPoint.X);
     //                   break;
     //               }
     //           case SharedAxisEnum.YY:
     //               {
     //                   nextPlotDataPoint = GetPreviousTrackerDataPointForSharedAxisYY(currentDataPoint.Y);
     //                   break;
     //               }
     //       }
     //       return nextPlotDataPoint;
     //   }

     //   public void DisplayPreviousTracker(double x, double y)
     //   {

     //       if (NextPlot.TrackerIsOutsideTheCurveRange == true)
     //       {
     //           TurnOutsideOfRangeOn();               
     //           _PreviousPlot.DisplayPreviousTracker(0, 0);//values don't matter here
     //           return;
     //       }
     //       else
     //       {
     //           TurnOutsideOfRangeOff();
     //       }


     //       ////only display to the top and bottom of the curve my cursor is on?
     //       //if (x > MaxX || y > MaxY || x < MinX || y < MinY) {
     //       //    //TrackerOutsideRangeFromDisplayPreviousTracker();
     //       //    TurnOutsideOfRangeOn();
     //       //    _PreviousPlot.DisplayPreviousTracker(0, 0);
     //       //    return; } //this is checking if the point will be somewhere in the entire plot window
     //       //if (x > Curve.XValues.Max() || y > Curve.YValues.Max()) {
     //       //    TurnOutsideOfRangeOn();
     //       //    _PreviousPlot.DisplayPreviousTracker(0, 0);
     //       //    return; } //this checks if the point is out of the range of the curve
     //       //if (x < Curve.XValues.Min() || y < Curve.YValues.Min()) { 
     //       //    TurnOutsideOfRangeOn();
     //       //    _PreviousPlot.DisplayPreviousTracker(0, 0);
     //       //    return; }
     //       if (IsPointInRange(x, y) == false)
     //       {
     //           TurnOutsideOfRangeOn();
     //           _PreviousPlot.DisplayPreviousTracker(0, 0);//values don't matter here because tracker is out of range
     //           return;
     //       }

     //       DataPoint dp = new DataPoint(x, y);

     //       DisplayCurrentTracker(dp);

     //       if (IsStartNode == true)
     //       {
     //           return;
     //       }

     //       //get the x and y values for the next plot
     //       try
     //       {
     //           DataPoint nextPlotTrackerPoint = getPreviousTrackerDataPoint(_PreviousPlotSharedAxisEnum, dp);
     //           _PreviousPlot.DisplayPreviousTracker(nextPlotTrackerPoint.X, nextPlotTrackerPoint.Y);
     //       }
     //       catch(Exception e)
     //       {
     //           //it wasn't able to get the point from getPreviousTrackerDataPoint because it was out of the range of the curve. I don't think i care.
     //       }
     //   }

     //   /// <summary>
     //   /// This is how i freeze the trackers when the user clicks in a plot. I basically set a boolean on all the plots that the tracker is frozen. Then in the "DisplayTheTrackers" method i check if the tracker is frozen on the first line.
     //   /// </summary>
     //   /// <param name="sender"></param>
     //   /// <param name="e"></param>
     //   public void Model_MouseDown(object sender, OxyMouseDownEventArgs e)
     //   {
     //       if (_FreezeTracker == false)
     //       {
     //           FreezeNextTracker = true;
     //           FreezePreviousTracker = true;
     //           //right here i could capture the point and then redraw the plots if the window resizes.
     //       }
     //       else
     //       {
     //           FreezeNextTracker = false;
     //           FreezePreviousTracker = false;
     //       }
     //   }

     //   private void Model_MouseMove(object sender, OxyMouseEventArgs e)
     //   {
     //       if (_HideTracker)
     //       {
     //           return;
     //       }
     //       if (_FreezeTracker == false)
     //       {

     //       TurnOutsideOfRangeOff();
     //       }
     //          // this.TrackerIsOutsideTheCurveRange = false;
     //       //    //this is hacky but the cond editor uses a wrapper around the individual linked plot and the results viewer doesn't
     //       //    if (this.DataContext.GetType() == typeof(FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM))
     //       //    {
     //       //        FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM vm = (FdaViewModel.Plots.ConditionsIndividualPlotWrapperVM)this.DataContext;
     //       //        vm.PlotIsInsideRange(this, new EventArgs());
     //       //    }
     //       //    else if (this.DataContext.GetType() == typeof(FdaViewModel.Plots.IndividualLinkedPlotVM))
     //       //    {
     //       //        FdaViewModel.Plots.IndividualLinkedPlotVM vm = (FdaViewModel.Plots.IndividualLinkedPlotVM)this.DataContext;
     //       //        //vm.DisplayOutOfRange = false;
     //       //    }
     //       //}


     //       DisplayTheTrackers(e.Position);

     //   }

     //   public void DisplayTheTrackers(ScreenPoint sp)
     //   { 
     //       //updatedValues = true;
     //       if (_FreezeTracker == false && _HideTracker == false)
     //       {
     //           Series mySeries = OxyPlot1.Model.GetSeriesFromPoint(sp);
     //           if (mySeries != null)
     //           {
     //               TrackerHitResult result = mySeries.GetNearestPoint(sp, true);
     //               if (result != null && !result.DataPoint.Equals(null))
     //               {
     //                   DataPoint dp = result.DataPoint;
     //                   if (XAxisAsStandardDeviation == true)
     //                   {
     //                       double newXValue = GetPairedValue(dp.Y, false, NonStandardDeviationCurve);
     //                       result.Text = XAxisLabel + ": " + Math.Round(1-newXValue, 3).ToString() + Environment.NewLine + YAxisLabel + ": " + Math.Round(dp.Y, 3).ToString();
     //                   }
     //                   else
     //                   {
     //                       result.Text = XAxisLabel + ": " + Math.Round(dp.X, 3).ToString() + Environment.NewLine + YAxisLabel + ": " + Math.Round(dp.Y, 3).ToString();
     //                   }
                        
     //                   OxyPlot1.ShowTracker(result);

     //                   ///////////////   DISPLAY NEXT PLOT TRACKER ///////////////////
     //                   if (IsEndNode == false)
     //                   {
     //                       try
     //                       {
     //                           DataPoint nextPlotDataPoint = GetNextTrackerDataPoint(_NextPlotSharedAxisEnum, dp);
     //                           _NextPlot.DisplayNextTracker(nextPlotDataPoint.X, nextPlotDataPoint.Y);
     //                       }
     //                       catch (Exception e)
     //                       {
     //                           //it wasn't able to get the point from getPreviousTrackerDataPoint because it was out of the range of the curve. I don't think i care.
     //                       }
     //                   }
     //                   if(IsStartNode == false)
     //                   {
     //                       ////////////////////////   DISPLAY PREVIOUS PLOT TRACKERS  //////////////////
     //                       try
     //                       {
     //                           DataPoint prevPlotDataPoint = getPreviousTrackerDataPoint(_PreviousPlotSharedAxisEnum, dp);
     //                           _PreviousPlot.DisplayPreviousTracker(prevPlotDataPoint.X, prevPlotDataPoint.Y);
     //                       }
     //                       catch (Exception e)
     //                       {
     //                           //it wasn't able to get the point from getPreviousTrackerDataPoint because it was out of the range of the curve. I don't think i care.
     //                       }
     //                   }
     //               }
     //           }
     //       }
     //   }

     //   //private bool CheckForOutsideTheRange(double value)
     //   //{
     //   //    double previousPlotHighestXValue = 0;
     //   //    foreach(Series s in ((IndividualLinkedPlot)PreviousPlot).OxyPlot1.Model.Series)
     //   //    {
     //   //        if(s.GetType() == typeof(LineSeries))
     //   //        {
     //   //            previousPlotHighestXValue = ((LineSeries)s).Points.Last().X;
     //   //        }
     //   //    }
     //   //    if(previousPlotHighestXValue<value) //turn this into a property on the ilinkedplot so that it will work with the double line modulators. that way i dont have to cast here
     //   //    {
     //   //        ((IndividualLinkedPlot)PreviousPlot).OutOfRange = true;
     //   //        return true;
     //   //    }
     //   //    return false;
     //   //}

     //   public void DisplayMaxOutOfRangeTracker()
     //   {
     //       DataPoint highestPoint = new DataPoint();
     //       foreach (Series s in OxyPlot1.Model.Series)
     //       {
     //           if (s.GetType() == typeof(LineSeries))
     //           {
     //               highestPoint = ((LineSeries)s).Points.Last();
     //           }

     //           //DataPoint dp = new DataPoint(x, y);
     //           ScreenPoint position = OxyPlot1.Model.Axes[0].Transform(highestPoint.X, highestPoint.Y, OxyPlot1.Model.Axes[1]);
     //           //ScreenPoint sp = new ScreenPoint(position.X, position.Y);
     //           TrackerHitResult thr = new TrackerHitResult(); // _OxyPlotModel.Series[0], newPoint, sp);
     //           thr.Series = s;
     //           thr.DataPoint = highestPoint;
     //           thr.Position = position;
     //   //        thr.PlotModel.DefaultColors = new List<OxyColor>
     //   //{
     //   //    OxyColors.Red,
     //   //    OxyColors.Green,
     //   //    OxyColors.Blue,
     //   //    OxyColor.FromRgb(0x20, 0x4A, 0x87)
     //   //};
     //           thr.Text ="OUT OF RANGE" + Environment.NewLine + XAxisLabel + ": " + Math.Round(highestPoint.X, 3).ToString() + Environment.NewLine + YAxisLabel + ": " + Math.Round(highestPoint.Y, 3).ToString();
     //           OxyPlot1.ShowTracker(thr);
     //       }
     //   }

     //   public void DisplayMinOutOfRangeTracker()
     //   {
     //       DataPoint lowestPoint = new DataPoint();
     //       foreach (Series s in OxyPlot1.Model.Series)
     //       {
     //           if (s.GetType() == typeof(LineSeries))
     //           {
     //               lowestPoint = ((LineSeries)s).Points.First();
     //           }
     //           ScreenPoint position = OxyPlot1.Model.Axes[0].Transform(lowestPoint.X, lowestPoint.Y, OxyPlot1.Model.Axes[1]);
     //           TrackerHitResult thr = new TrackerHitResult(); // _OxyPlotModel.Series[0], newPoint, sp);
     //           thr.Series = s;
     //           thr.DataPoint = lowestPoint;
     //           thr.Position = position;
     //           thr.Text = "OUT OF RANGE" + Environment.NewLine + XAxisLabel + ": " + Math.Round(lowestPoint.X, 3).ToString() + Environment.NewLine + YAxisLabel + ": " + Math.Round(lowestPoint.Y, 3).ToString();
     //           OxyPlot1.ShowTracker(thr);
     //       }
     //   }

     //   #endregion
     //   private void ShowTrackerInLinkedPlot(double sharedAxisValue)
     //   {

     //       //if (_PlotEntered == 1) { return; }
     //     //  double yValue = GetPairedValue(sharedAxisValue, true, LinkedPlot.Model, true);
     //       //if (yValue == -1) { return; }
     //       //DataPoint newPoint = new DataPoint(sharedAxisValue, yValue);
     //       //_ACE = newPoint.X;
     //       //_Flow = newPoint.Y;
     //       //var position = OxyPlotModel.Axes[0].Transform(newPoint.X, newPoint.Y, OxyPlotModel.Axes[1]);
     //       //ScreenPoint sp = new ScreenPoint(position.X, position.Y);
     //       //TrackerHitResult thr = new TrackerHitResult();// _OxyPlotModel.Series[0], newPoint, sp);
     //       //thr.Series = _OxyPlotModel.Series[0];
     //       //thr.DataPoint = newPoint;
     //       //thr.Position = sp;
     //       //thr.Text = "ACE: " + Math.Round(sharedAxisValue, 3).ToString() + Environment.NewLine + "Flow: " + Math.Round(yValue, 3).ToString();
     //       //OxyPlot1.ShowTracker(thr);

     //       //ShowTrackerInPlot2(yValue);

     //   }

     //   #region SetLinkages

     //   private void SetNextPlotSharedAxis(string thisAxis, string linkedAxis)
     //   {
     //       string thisAxisUpper = thisAxis.ToUpper();
     //       string linkedAxisUpper = linkedAxis.ToUpper();
     //       if(thisAxisUpper == "X" && linkedAxisUpper == "X")  //enum 1
     //       {
     //           _NextPlotSharedAxisEnum = SharedAxisEnum.XX;
     //       }
     //       else if (thisAxisUpper == "X" && linkedAxisUpper == "Y") //enum 2
     //       {
     //           _NextPlotSharedAxisEnum = SharedAxisEnum.XY;
     //       }
     //       else if (thisAxisUpper == "Y" && linkedAxisUpper == "X")  // enum 3
     //       {
     //           _NextPlotSharedAxisEnum = SharedAxisEnum.YX;
     //       }
     //       else if (thisAxisUpper == "Y" && linkedAxisUpper == "Y") // enum 4
     //       {
     //           _NextPlotSharedAxisEnum = SharedAxisEnum.YY;
     //       }
     //   }

       

     //   /// <summary>
     //   /// 
     //   /// </summary>
     //   /// <param name="plot">This is the plot that this plot will link with.</param>
     //   /// <param name="thisAxis">This should be "x" or "y". It is not case sensitive.</param>
     //   /// <param name="linkedAxis">This should be "x" or "y". It is not case sensitive.</param>
     //   public void SetNextPlotLinkage(ILinkedPlot plot)
     //   {
     //       if (IsEndNode == true) { return; }
     //       string thisAxis = "";
     //       string linkedAxis = "";
     //       switch (BaseFunction.FunctionType)
     //       {
     //           case FdaModel.Functions.FunctionTypes.InflowFrequency:
     //           case FdaModel.Functions.FunctionTypes.OutflowFrequency:
     //               if (plot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InflowOutflow)
     //               {
     //                   thisAxis = "y";
     //                   linkedAxis = "x";
     //               }
     //               else if (plot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
     //               {
     //                   thisAxis = "y";
     //                   linkedAxis = "y";
     //               }
     //               break;
     //           case FdaModel.Functions.FunctionTypes.InflowOutflow:
     //               if (plot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
     //               {
     //                   thisAxis = "y";
     //                   linkedAxis = "y";
     //               }
     //               break;
     //           case FdaModel.Functions.FunctionTypes.Rating:
     //               if (plot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.ExteriorInteriorStage)
     //               {
     //                   thisAxis = "x";
     //                   linkedAxis = "x";
     //               }
     //               else if (plot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
     //               {
     //                   thisAxis = "x";
     //                   linkedAxis = "x";
     //               }
     //               break;
     //           case FdaModel.Functions.FunctionTypes.ExteriorInteriorStage:
     //               if (plot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
     //               {
     //                   thisAxis = "y";
     //                   linkedAxis = "x";
     //               }
     //               break;
     //           case FdaModel.Functions.FunctionTypes.InteriorStageDamage:
     //               if (plot.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
     //               {
     //                   thisAxis = "y";
     //                   linkedAxis = "y";
     //               }
     //               break;

     //       }
     //       this.SetNextPlotSharedAxis(thisAxis, linkedAxis);
     //       _NextPlot = plot;
     //   }
     //   public void SetPreviousPlotLinkage(ILinkedPlot plot)
     //   {
     //       if (IsStartNode == true) { return; }
     //       _PreviousPlot = plot;
     //       _PreviousPlotSharedAxisEnum = PreviousPlot.NextPlotSharedAxisEnum;
     //   }

     //   public void SetAsStartNode()
     //   {
     //       IsStartNode = true;
     //   }
     //   public void SetAsEndNode()
     //   {
     //       IsEndNode = true;
     //   }

     //   #endregion

     //   //public void createLineSeries(double[] xValues, double[] yValues)
     //   //{
     //   //    LineSeries series1 = new LineSeries();
     //   //    for (int i = 0; i < xValues.Length; i++)
     //   //    {
     //   //        series1.Points.Add(new DataPoint(xValues[i], yValues[i]));

     //   //    }
     //   //    OxyPlot1.Model.Series.Add(series1);
     //   //    OxyPlot1.Model.InvalidatePlot(true);
     //   //}
     //   //public void createLineSeries(FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction failureFunction)
     //   //{
     //   //    LineSeries series1 = new LineSeries();
     //   //    for (int i = 0; i < failureFunction.Function.Count; i++)
     //   //    {
     //   //        series1.Points.Add(new DataPoint(failureFunction.Function.get_X(i), failureFunction.Function.get_Y(i)));

     //   //    }
     //   //    OxyPlot1.Model.Series.Add(series1);
     //   //    OxyPlot1.Model.InvalidatePlot(true);
     //   //}

     //   //public void ChangePlotTitle(string newTitle)
     //   //{
     //   //    OxyPlot1.Model.Title = newTitle;
     //   //}




     //   public double GetPairedValue(double knownValue, bool lookingForY, Statistics.CurveIncreasing curve)
     //   {
     //       double pairedValue = -1;

     //       //List<DataPoint> seriesPoints = ((LineSeries)PM.Series[0]).Points.ToList<DataPoint>();
     //       //if (lookingForY == true)
     //       //{
                
     //       //        for (int i = 0; i < seriesPoints.Count(); i++)
     //       //        {
     //       //            if (knownValue < seriesPoints[0].X) { return -1; }
     //       //            if (knownValue < seriesPoints[i].X) //what about equal to
     //       //            {
     //       //                double slope = (seriesPoints[i].Y - seriesPoints[i - 1].Y) / (seriesPoints[i].X - seriesPoints[i - 1].X);
     //       //                double yIntercept = -1 * ((slope * seriesPoints[i].X) - seriesPoints[i].Y);
     //       //                pairedValue = slope * knownValue + yIntercept; // y = mx + b
     //       //                return pairedValue;
     //       //            }
     //       //        }
                
               

     //       //}
     //       if (lookingForY == false)
     //       {
     //           //the known value is a y value
     //           if (knownValue < curve.YValues[0]) { return -1; }
     //           for (int i = 0; i < curve.XValues.Count(); i++)// need to handle the end cases!!!!
     //           {
     //               if (knownValue < curve.YValues[i]) //what about equal to
     //               {
     //                   double slope = (curve.YValues[i] - curve.YValues[i - 1]) / (curve.XValues[i] - curve.XValues[i - 1]);
     //                   if (slope == 0) { throw new Exception(); }
     //                   double yIntercept = -1 * ((slope * curve.XValues[i]) - curve.YValues[i]);
     //                   pairedValue = (knownValue - yIntercept) / slope; //x = (y - b)/m
     //                   return pairedValue;
     //               }
     //           }
     //       }

     //       return pairedValue;
     //   }


     //   public double GetPairedValue(double knownValue, bool lookingForY, PlotModel PM, bool axisReversed = false)
     //   {
     //       double pairedValue = -1;

     //       List<DataPoint> seriesPoints = ((LineSeries)PM.Series[0]).Points.ToList<DataPoint>();
     //       if (lookingForY == true)
     //       {
     //           if (axisReversed == false)
     //           {
     //               for (int i = 0; i < seriesPoints.Count(); i++)
     //               {
     //                   if (knownValue < seriesPoints[0].X) { return -1; }
     //                   if (knownValue < seriesPoints[i].X) //what about equal to
     //                   {
     //                       double slope = (seriesPoints[i].Y - seriesPoints[i - 1].Y) / (seriesPoints[i].X - seriesPoints[i - 1].X);
     //                       double yIntercept = -1 * ((slope * seriesPoints[i].X) - seriesPoints[i].Y);
     //                       pairedValue = slope * knownValue + yIntercept; // y = mx + b
     //                       return pairedValue;
     //                   }
     //               }
     //           }
     //           else
     //           {
     //               for (int i = 0; i < seriesPoints.Count(); i++)
     //               {
     //                   if (knownValue > seriesPoints[0].X) { return -1; }

     //                   if (knownValue > seriesPoints[i].X) //what about equal to
     //                   {
     //                       double slope = (seriesPoints[i].Y - seriesPoints[i - 1].Y) / (seriesPoints[i].X - seriesPoints[i - 1].X);
     //                       double yIntercept = -1 * ((slope * seriesPoints[i].X) - seriesPoints[i].Y);
     //                       pairedValue = slope * knownValue + yIntercept; // y = mx + b
     //                       return pairedValue;
     //                   }
     //               }
     //           }

     //       }
     //       else if (lookingForY == false)
     //       {
     //           for (int i = 0; i < seriesPoints.Count(); i++)// need to handle the end cases!!!!
     //           {
     //               if (knownValue < seriesPoints[0].Y) { return -1; }
     //               if (knownValue < seriesPoints[i].Y) //what about equal to
     //               {
     //                   double slope = (seriesPoints[i].Y - seriesPoints[i - 1].Y) / (seriesPoints[i].X - seriesPoints[i - 1].X);
     //                   if (slope == 0) { throw new Exception(); }
     //                   double yIntercept = -1 * ((slope * seriesPoints[i].X) - seriesPoints[i].Y);
     //                   pairedValue = (knownValue - yIntercept) / slope; //x = (y - b)/m
     //                   return pairedValue;
     //               }
     //           }
     //       }

     //       return pairedValue;
     //   }

     //   /// <summary>
     //   /// Each removed area series gets added to a list so that if the button
     //   /// in the toolbar gets clicked to toggle between adding and removing
     //   /// the area plots, it will be easy to put them back in the plot
     //   /// </summary>
     //   public void RemoveAreaPlotsFromButtonClick()
     //   {
     //       ListOfRemovedAreaSeries = new List<AreaSeries>();
     //       for (int i = 0; i < OxyPlot1.Model.Series.Count; i++)
     //       {
     //           if (OxyPlot1.Model.Series[i].GetType() == typeof(AreaSeries))
     //           {
     //               ListOfRemovedAreaSeries.Add((AreaSeries)OxyPlot1.Model.Series[i]);
     //               OxyPlot1.Model.Series.RemoveAt(i);
     //               i--;
     //           }
     //       }
     //       this.OxyPlot1.InvalidatePlot(true);
     //   }
     //   public void RemoveAreaPlots()
     //   {
     //       for (int i = 0; i < OxyPlot1.Model.Series.Count; i++)
     //       {
     //           if (OxyPlot1.Model.Series[i].GetType() == typeof(AreaSeries))
     //           {
     //               OxyPlot1.Model.Series.RemoveAt(i);
     //               i--;
     //           }
     //       }
     //       this.OxyPlot1.InvalidatePlot(true);
     //   }
     //   public void AddAreaPlots()
     //   {
     //       foreach(AreaSeries a in ListOfRemovedAreaSeries)
     //       {
     //           OxyPlot1.Model.Series.Add(a);
     //       }
     //       this.OxyPlot1.InvalidatePlot(true);
     //   }




     //   #region DisplayTrackerWithSharedAxisMethods

     //   //private Boolean isValueWithinRangeOfPlot(double value, Boolean evalueateXAxis, ILinkedPlot plot)
     //   //{
     //   //    if(evalueateXAxis)
     //   //    {
     //   //        if(value<plot.MinX || value>plot.MaxX)
     //   //        {
     //   //            return false;
     //   //        }
     //   //    }
     //   //    else
     //   //    {
     //   //        if (value < plot.MinY || value > plot.MaxY)
     //   //        {
     //   //            return false;
     //   //        }
     //   //    }
     //   //    return true;
     //   //}

     //   private DataPoint GetPreviousTrackerDataPointForSharedAxisXX(double dataPointXValue)
     //   {
     //       DataPoint dp = new DataPoint();
       
     //       double otherValue;
     //       if (PreviousPlot.FlipFrequencyAxis == true)
     //       {
     //           dataPointXValue = 1 - dataPointXValue;
     //       }

           
     //           otherValue =  PreviousPlot.Curve.GetYfromX(dataPointXValue);
     //           dp = new DataPoint(dataPointXValue, otherValue);
    
     //       return dp;
     //   }

     //   private DataPoint GetPreviousTrackerDataPointForSharedAxisXY(double dataPointX, double dataPointY)
     //   {
     //       double otherValue;
     //       if (PreviousPlot.FlipFrequencyAxis == true)
     //       {
     //           otherValue = PreviousPlot.Curve.GetYfromX(1 - dataPointY);
     //       }
     //       else
     //       {
     //           otherValue = PreviousPlot.Curve.GetYfromX(dataPointY);
     //       }
     //       return new DataPoint(dataPointX, otherValue);
     //   }

     //   private DataPoint GetPreviousTrackerDataPointForSharedAxisYX(double dataPointX)
     //   {
     //       double otherValue = PreviousPlot.Curve.GetXfromY(dataPointX);
     //       if (PreviousPlot.FlipFrequencyAxis == true)
     //       {
     //           otherValue = 1 - otherValue;
     //       }
     //       return new DataPoint(otherValue, dataPointX);
     //   }
     //   private DataPoint GetPreviousTrackerDataPointForSharedAxisYY(double dataPointY)
     //   {
     //       double otherValue = PreviousPlot.Curve.GetXfromY(dataPointY);
     //       if (PreviousPlot.FlipFrequencyAxis == true)
     //       {
     //           otherValue = 1 - otherValue;
     //       }
            
     //       return new DataPoint(otherValue, dataPointY);
     //   }


     //   private DataPoint GetNextTrackerDataPointForSharedAxisXX(double x)
     //   {
     //       double otherValue;
     //       if (NextPlot.FlipFrequencyAxis == true)
     //       {
     //           otherValue = NextPlot.Curve.GetYfromX(1 - x);
     //       }
     //       else
     //       {
     //           otherValue = NextPlot.Curve.GetYfromX(x);
     //       }
     //       return new DataPoint(x, otherValue);
     //   }

     //   private DataPoint GetNextTrackerDataPointForSharedAxisXY(double x)
     //   {
     //       double otherValue = NextPlot.Curve.GetXfromY(x);
     //       if (NextPlot.FlipFrequencyAxis == true)
     //       {
     //           x = (1 - x);
     //       }
     //       return new DataPoint(x, otherValue);
     //   }

     //   private DataPoint GetNextTrackerDataPointForSharedAxisYX(double y)
     //   {
     //       double otherValue;
     //       if (NextPlot.FlipFrequencyAxis == true)
     //       {
     //           otherValue = NextPlot.Curve.GetYfromX(y);
     //       }
     //       else
     //       {
     //           otherValue = NextPlot.Curve.GetYfromX(y);
     //       }

     //       return new DataPoint(y, otherValue);

     //   }

     //   private DataPoint GetNextTrackerDataPointForSharedAxisYY(double y)
     //   {
     //       double otherValue = NextPlot.Curve.GetXfromY(y);
     //       if (NextPlot.FlipFrequencyAxis == true)
     //       {
     //           otherValue = (1 - otherValue);
     //       }
           
     //       return new DataPoint(otherValue, y);

     //   }

     //   public void PlotThresholdLines(FdaModel.ComputationPoint.PerformanceThreshold pt)
     //   {
     //       if (pt.ThresholdType == FdaModel.ComputationPoint.PerformanceThresholdTypes.InteriorStage)
     //       {
     //           if (this.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
     //           {
     //               //PlotThreshold(pt.ThresholdType,)
     //               // i think i would just need to add a plot next threshold and a plot prev threshold and then 
     //               //kick it off.
     //           }
     //       }
     //   }

     //   public void PlotThreshold(IMetric pt)
     //   {
     //       string testName = this.Name;
     //       //get the type and the value from pt and plot it
     //       if (ThresholdLineIsShowing)
     //       {
     //           RemoveThresholdPlot();
     //       }
     //       LineSeries verticalThresholdSeries = new LineSeries();
     //       LineSeries horizontalThresholdSeries = new LineSeries();

     //       verticalThresholdSeries.Color = THRESHOLD_LINE_COLOR;
     //       horizontalThresholdSeries.Color = THRESHOLD_LINE_COLOR;


     //       if (pt.Type == MetricEnum.InteriorStage)
     //       {
     //           //logic for plot7
     //           if (this.BaseFunction.Type == ImpactAreaFunctionEnum.InteriorStageDamage)
     //           {
     //               double yValueWhereLineCrosses = GetPairedValue(pt.ExceedanceTarget, true, OxyPlot1.Model);

     //               verticalThresholdSeries.Points.Add(new DataPoint(pt.ExceedanceTarget, MinY));
     //               verticalThresholdSeries.Points.Add(new DataPoint(pt.ExceedanceTarget, yValueWhereLineCrosses));

     //               horizontalThresholdSeries.Points.Add(new DataPoint(pt.ExceedanceTarget, yValueWhereLineCrosses));
     //               horizontalThresholdSeries.Points.Add(new DataPoint(MaxX, yValueWhereLineCrosses));

     //               if(NextPlot != null && NextPlot.Function != null)
     //               {
     //                   //i can get the associated y value here
     //                   pt.ExceedanceTarget = yValueWhereLineCrosses;
     //                   ((IndividualLinkedPlot)NextPlot).PlotThreshold(pt);
     //               }

     //           }
     //           //logic for plot8
     //           else if (this.BaseFunction.Type == ImpactAreaFunctionEnum.DamageFrequency)
     //           {
     //               //DataPoint dp = GetPreviousTrackerDataPointForSharedAxisYY(pt.ExceedanceTarget);
                   
     //               double xValueWhereLineCrosses = GetPairedValue(pt.ExceedanceTarget, false, OxyPlot1.Model);

     //               verticalThresholdSeries.Points.Add(new DataPoint(xValueWhereLineCrosses, MinY));
     //               verticalThresholdSeries.Points.Add(new DataPoint(xValueWhereLineCrosses, pt.ExceedanceTarget));

     //               if(XAxisAsStandardDeviation == true)
     //               {
     //                   horizontalThresholdSeries.Points.Add(new DataPoint(MinX, pt.ExceedanceTarget));
     //                   horizontalThresholdSeries.Points.Add(new DataPoint(xValueWhereLineCrosses, pt.ExceedanceTarget));
     //               }
     //               else
     //               {
     //                   horizontalThresholdSeries.Points.Add(new DataPoint(MaxX, pt.ExceedanceTarget));
     //                   horizontalThresholdSeries.Points.Add(new DataPoint(xValueWhereLineCrosses, pt.ExceedanceTarget));
     //               }

     //           }

     //       }
     //       else if (pt.ThresholdType == FdaModel.ComputationPoint.PerformanceThresholdTypes.Damage)
     //       {
     //           ////Plot the line horizontally
     //           //verticalThresholdSeries.Points.Add(new DataPoint(MinX, pt.ExceedanceTarget));
     //           //verticalThresholdSeries.Points.Add(new DataPoint(MaxX, pt.ExceedanceTarget));
     //           //
     //           //logic for plot7
     //           if (this.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
     //           {
     //               double xValueWhereLineCrosses = GetPairedValue(pt.ExceedanceTarget, false, OxyPlot1.Model);

     //               verticalThresholdSeries.Points.Add(new DataPoint(xValueWhereLineCrosses, MinY));
     //               verticalThresholdSeries.Points.Add(new DataPoint(xValueWhereLineCrosses, pt.ExceedanceTarget));

     //               horizontalThresholdSeries.Points.Add(new DataPoint(xValueWhereLineCrosses, pt.ExceedanceTarget));
     //               horizontalThresholdSeries.Points.Add(new DataPoint(MaxX, pt.ExceedanceTarget));

     //               if (NextPlot != null && NextPlot.Curve != null)
     //               {
     //                   //i can get the associated y value here
     //                   //pt.ExceedanceTarget = xValueWhereLineCrosses;
     //                   ((IndividualLinkedPlot)NextPlot).PlotThreshold(pt);
     //               }

     //           }
     //           //logic for plot8
     //           else if (this.BaseFunction.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
     //           {
     //               //DataPoint dp = GetPreviousTrackerDataPointForSharedAxisYY(pt.ExceedanceTarget);

     //               double xValueWhereLineCrosses = GetPairedValue(pt.ExceedanceTarget, false, OxyPlot1.Model);

     //               verticalThresholdSeries.Points.Add(new DataPoint(xValueWhereLineCrosses, MinY));
     //               verticalThresholdSeries.Points.Add(new DataPoint(xValueWhereLineCrosses, pt.ExceedanceTarget));

     //               horizontalThresholdSeries.Points.Add(new DataPoint(MaxX, pt.ExceedanceTarget));
     //               horizontalThresholdSeries.Points.Add(new DataPoint(xValueWhereLineCrosses, pt.ExceedanceTarget));
     //           }
     //       }




     //       OxyPlot1.Model.Series.Add(verticalThresholdSeries);

     //       OxyPlot1.Model.Series.Add(horizontalThresholdSeries);

     //       OxyPlot1.InvalidatePlot(true);

     //       ThresholdLineIsShowing = true;
     //       VerticalThresholdLine = verticalThresholdSeries;
     //       HorizontalThresholdLine = horizontalThresholdSeries;
     //   }

     //   public void RemoveThresholdPlot()
     //   {
     //       if (ThresholdLineIsShowing)
     //       {
     //           if (OxyPlot1.Model.Series.Contains(VerticalThresholdLine))
     //           {
     //               OxyPlot1.Model.Series.Remove(VerticalThresholdLine);
     //           }
     //           if(OxyPlot1.Model.Series.Contains(HorizontalThresholdLine))
     //           {
     //               OxyPlot1.Model.Series.Remove(HorizontalThresholdLine);
     //           }
     //               OxyPlot1.InvalidatePlot(true);
     //               ThresholdLineIsShowing = false;
     //       }

     //   }

       

     //   #endregion

    }
}
