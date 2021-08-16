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
using System.Windows.Shapes;
using ViewModel.Output;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace View.Output
{
    /// <summary>
    /// Interaction logic for DeleteMeWindow.xaml
    /// </summary>
    public partial class LinkedPlots : UserControl
    {
        private PlotModel _OxyPlotModel;
        private PlotModel _OxyPlotModel2;
        private PlotModel _OxyPlotModel3;
        private PlotModel _OxyPlotModel4;
        private bool _MouseDown = false;
        private int _PlotEntered;
        private double _ACE;
        private double _Flow;
        private double _Stage;
        private double _Damage;
        private double _Area;

        private bool _DisplayLowerACEAreaPlotInPlotOne; // either in 1 or 4
        private bool _DisplayLowerFlowAreaPlotInPlotOne; //either in 1 or 2
        private bool _DisplayLowerStageAreaPlotInPlotTwo; //either in 2 or 3
        private bool _DisplayLowerDamageAreaPlotInPlotThree; //either in 3 or 4

        private bool _DisplayHigherACEAreaPlotInPlotOne; // either in 1 or 4
        private bool _DisplayHigherFlowAreaPlotInPlotOne; //either in 1 or 2
        private bool _DisplayHigherStageAreaPlotInPlotTwo; //either in 2 or 3
        private bool _DisplayHigherDamageAreaPlotInPlotThree; //either in 3 or 4

        private double _ACEMin = double.MaxValue;
        private double _ACEHigherMin = double.MinValue;
        private double _ACEMax = double.MinValue;
        private double _ACELowerMax = double.MaxValue;

        private double _FlowMin = double.MaxValue;
        private double _FlowHigherMin = double.MinValue;
        private double _FlowMax = double.MinValue;
        private double _FlowLowerMax = double.MaxValue;

        private double _StageMin = double.MaxValue;
        private double _StageHigherMin = double.MinValue;
        private double _StageMax = double.MinValue;
        private double _StageLowerMax = double.MaxValue;

        private double _DamageMin = double.MaxValue;
        private double _DamageHigherMin = double.MinValue;
        private double _DamageMax = double.MinValue;
        private double _DamageLowerMax = double.MaxValue;

        private bool _HideTracker = false;

       // private FdaModel.ComputationPoint.Outputs.Result _Result;

        //private List<FdaModel.Functions.BaseFunction> _InputFunctions;

        // Related to specific iteration
        private int _CurrentIteration = 0;

        #region Properties
        //public FdaModel.ComputationPoint.Outputs.Result Result
        //{
        //    get { return _Result; }
        //    set { _Result = value; }
        //}
        //public List<FdaModel.Functions.BaseFunction> InputFunctions
        //{
        //    get { return _InputFunctions; }
        //    set { _InputFunctions = value; }
        //}
        public OxyPlot.PlotModel OxyPlotModel
        {
            get { return _OxyPlotModel; }
            set { _OxyPlotModel = value; }
        }
        public OxyPlot.PlotModel OxyPlotModel2
        {
            get { return _OxyPlotModel2; }
            set { _OxyPlotModel2 = value; }
        }
        public OxyPlot.PlotModel OxyPlotModel3
        {
            get { return _OxyPlotModel3; }
            set { _OxyPlotModel3 = value; }
        }
        public OxyPlot.PlotModel OxyPlotModel4
        {
            get { return _OxyPlotModel4; }
            set { _OxyPlotModel4 = value; }
        }
        #endregion
        public LinkedPlots()
        {
            InitializeComponent();
        }
    //    public LinkedPlots(FdaModel.ComputationPoint.Outputs.Result result)
    //    {
    //        Result = result;
    //        InputFunctions = Result.Realizations[_CurrentIteration].Functions;

    //        _OxyPlotModel = new PlotModel();
    //        _OxyPlotModel2 = new PlotModel();
    //        _OxyPlotModel3 = new PlotModel();
    //        _OxyPlotModel4 = new OxyPlot.PlotModel();


    //        setAxesValues();

    //        setUpPlot1();
    //        setUpPlot2();
    //        setUpPlot3();
    //        setUpPlot4();

    //        InitializeComponent();

    //        //These turn the scrolling off
    //        OxyPlot1.ActualController.UnbindAll();
    //        OxyPlot2.ActualController.UnbindAll();
    //        OxyPlot3.ActualController.UnbindAll();
    //        OxyPlot4.ActualController.UnbindAll();

    //        lbl_AEP.Content = String.Format("{0:.###}", Math.Round(1 - Result.AEP.GetMean, 4));
    //        lbl_EAD.Content = String.Format("{0:0,0}", Math.Round(Result.EAD.GetMean, 2));

    //        txt_Iteration.Text = "0";
    //        lbl_TotalIterations.Content = "of " + (Result.Realizations.Count - 1).ToString() + " iterations.";
    //        lbl_IterationValues.Content = "AEP: " + Math.Round(1 - Result.Realizations[0].AnnualExceedanceProbability, 3).ToString() + Environment.NewLine + "EAD: " + Math.Round(Result.Realizations[0].ExpectedAnnualDamage, 0).ToString();
          


    //    }

    //    public void PlotIteration(int iteration)
    //    {
    //        //reasign the input functions
    //        InputFunctions = Result.Realizations[iteration].Functions;

    //        //clear all axes
    //        OxyPlotModel.Axes.Clear();
    //        OxyPlotModel2.Axes.Clear();
    //        OxyPlotModel3.Axes.Clear();
    //        OxyPlotModel4.Axes.Clear();
            

    //        //clear all series
    //        OxyPlotModel.Series.Clear();
    //        OxyPlotModel4.Series.Clear();
    //        OxyPlotModel2.Series.Clear();
    //        OxyPlotModel3.Series.Clear();

    //        //set the new axes values
    //        clearAxisValues();
    //        setAxesValues();

    //        //create the new series
    //        setUpPlot1();
    //        setUpPlot2();
    //        setUpPlot3();
    //        setUpPlot4();

    //        //redraw all plots
    //        OxyPlotModel.InvalidatePlot(true);
    //        OxyPlotModel2.InvalidatePlot(true);
    //        OxyPlotModel3.InvalidatePlot(true);
    //        OxyPlotModel4.InvalidatePlot(true);

    //        lbl_IterationValues.Content = "AEP: " + Math.Round(1 - Result.Realizations[_CurrentIteration].AnnualExceedanceProbability, 3).ToString() + Environment.NewLine + "EAD: " + Math.Round(Result.Realizations[_CurrentIteration].ExpectedAnnualDamage, 0).ToString();


    //    }

    //    private void setAxesValues()
    //    {
    //        foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //        {
    //            if (func.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency || func.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
    //            {
    //                FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction iof;
    //                if (func.GetType().BaseType == typeof(FdaModel.Functions.FrequencyFunctions.FrequencyFunction))
    //                {
    //                    iof = ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)func).GetOrdinatesFunction();
    //                }
    //                else
    //                {
    //                    iof = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func).GetOrdinatesFunction();
    //                }
    //                // aep min
    //                if (iof.Function.get_X(0) < _ACEMin)
    //                {
    //                    _ACEMin = iof.Function.get_X(0);
    //                    _DisplayLowerACEAreaPlotInPlotOne = false;
    //                }
    //                // aep higher min
    //                if (iof.Function.get_X(0) > _ACEHigherMin)
    //                {
    //                    _ACEHigherMin = iof.Function.get_X(0);
    //                    _DisplayLowerACEAreaPlotInPlotOne = true;
    //                }
    //                // aep max
    //                if (iof.Function.get_X(iof.Function.Count - 1) > _ACEMax)
    //                {
    //                    _ACEMax = iof.Function.get_X(iof.Function.Count - 1);
    //                    _DisplayHigherACEAreaPlotInPlotOne = false;
    //                }
    //                // aep lower max
    //                if (iof.Function.get_X(iof.Function.Count - 1) < _ACELowerMax)
    //                {
    //                    _ACELowerMax = iof.Function.get_X(iof.Function.Count - 1);
    //                    _DisplayHigherACEAreaPlotInPlotOne = true;

    //                }
    //                // flow min
    //                if (iof.Function.get_Y(0) < _FlowMin)
    //                {
    //                    _FlowMin = iof.Function.get_Y(0);
    //                    _DisplayLowerFlowAreaPlotInPlotOne = false;
    //                }
    //                // flow higher min
    //                if (iof.Function.get_Y(0) > _FlowHigherMin)
    //                {
    //                    _FlowHigherMin = iof.Function.get_Y(0);
    //                    _DisplayLowerFlowAreaPlotInPlotOne = true;
    //                }
    //                // flow max
    //                if (iof.Function.get_Y(iof.Function.Count - 1) > _FlowMax)
    //                {
    //                    _FlowMax = iof.Function.get_Y(iof.Function.Count - 1);
    //                    _DisplayHigherFlowAreaPlotInPlotOne = false;
    //                }
    //                // flow lower max
    //                if (iof.Function.get_Y(iof.Function.Count - 1) < _FlowLowerMax)
    //                {
    //                    _FlowLowerMax = iof.Function.get_Y(iof.Function.Count - 1);
    //                    _DisplayHigherFlowAreaPlotInPlotOne = true;
    //                }

    //            }
    //            else if (func.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
    //            {
    //                // for the rating curve the x and y axes need to be swapped
    //                FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction iof = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func);
    //                //stage min
    //                if (iof.Function.get_Y(0) < _StageMin)
    //                {
    //                    _StageMin = iof.Function.get_Y(0);
    //                    _DisplayLowerStageAreaPlotInPlotTwo = false;
    //                }
    //                //stage higher min
    //                if (iof.Function.get_Y(0) > _StageHigherMin)
    //                {
    //                    _StageHigherMin = iof.Function.get_Y(0);
    //                    _DisplayLowerStageAreaPlotInPlotTwo = true;
    //                }
    //                //stage max
    //                if (iof.Function.get_Y(iof.Function.Count - 1) > _StageMax)
    //                {
    //                    _StageMax = iof.Function.get_Y(iof.Function.Count - 1);
    //                    _DisplayHigherStageAreaPlotInPlotTwo = false;
    //                }
    //                //stage lower max
    //                if (iof.Function.get_Y(iof.Function.Count - 1) < _StageLowerMax)
    //                {
    //                    _StageLowerMax = iof.Function.get_Y(iof.Function.Count - 1);
    //                    _DisplayHigherStageAreaPlotInPlotTwo = true;
    //                }
    //                //Flow min
    //                if (iof.Function.get_X(0) < _FlowMin)
    //                {
    //                    _FlowMin = iof.Function.get_X(0);
    //                    _DisplayLowerFlowAreaPlotInPlotOne = true;
    //                }
    //                //flow higher min
    //                if (iof.Function.get_X(0) > _FlowHigherMin)
    //                {
    //                    _FlowHigherMin = iof.Function.get_X(0);
    //                    _DisplayLowerFlowAreaPlotInPlotOne = false;
    //                }
    //                //flow max
    //                if (iof.Function.get_X(iof.Function.Count - 1) > _FlowMax)
    //                {
    //                    _FlowMax = iof.Function.get_X(iof.Function.Count - 1);
    //                    _DisplayHigherFlowAreaPlotInPlotOne = true;
    //                }
    //                //flow lower max
    //                if (iof.Function.get_X(iof.Function.Count - 1) < _FlowLowerMax)
    //                {
    //                    _FlowLowerMax = iof.Function.get_X(iof.Function.Count - 1);
    //                    _DisplayHigherFlowAreaPlotInPlotOne = false;
    //                }

    //            }
    //            else if (func.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
    //            {
    //                FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction iof = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func);
    //                // stage min
    //                if (iof.Function.get_X(0) < _StageMin)
    //                {
    //                    _StageMin = iof.Function.get_X(0);
    //                    _DisplayLowerStageAreaPlotInPlotTwo = true;
    //                }
    //                // stage higher min
    //                if (iof.Function.get_X(0) > _StageHigherMin)
    //                {
    //                    _StageHigherMin = iof.Function.get_X(0);
    //                    _DisplayLowerStageAreaPlotInPlotTwo = false;
    //                }
    //                // stage max
    //                if (iof.Function.get_X(iof.Function.Count - 1) > _StageMax)
    //                {
    //                    _StageMax = iof.Function.get_X(iof.Function.Count - 1);
    //                    _DisplayHigherStageAreaPlotInPlotTwo = true;
    //                }
    //                //stage lower max
    //                if (iof.Function.get_X(iof.Function.Count - 1) < _StageLowerMax)
    //                {
    //                    _StageLowerMax = iof.Function.get_X(iof.Function.Count - 1);
    //                    _DisplayHigherStageAreaPlotInPlotTwo = false;
    //                }
    //                // damage min
    //                if (iof.Function.get_Y(0) < _DamageMin)
    //                {
    //                    _DamageMin = iof.Function.get_Y(0);
                        
    //                    _DisplayLowerDamageAreaPlotInPlotThree = false;
    //                }
    //                //damage higher min
    //                if (iof.Function.get_Y(0) > _DamageHigherMin)
    //                {
    //                    _DamageHigherMin = iof.Function.get_Y(0);
    //                    _DisplayLowerDamageAreaPlotInPlotThree = true;
    //                }
    //                // damage max
    //                if (iof.Function.get_Y(iof.Function.Count - 1) > _DamageMax)
    //                {
    //                    _DamageMax = iof.Function.get_Y(iof.Function.Count - 1);
    //                    _DisplayHigherDamageAreaPlotInPlotThree = false;
    //                }
    //                //damage lower max
    //                if (iof.Function.get_Y(iof.Function.Count - 1) < _DamageLowerMax)
    //                {
    //                    _DamageLowerMax = iof.Function.get_Y(iof.Function.Count - 1);
    //                    _DisplayHigherDamageAreaPlotInPlotThree = true;
    //                }
    //            }
    //            else if (func.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
    //            {
    //                FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction iof = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func);
    //                // aep min
    //                if (iof.Function.get_X(0) < _ACEMin)
    //                {
    //                    _ACEMin = iof.Function.get_X(0);
    //                    _DisplayLowerACEAreaPlotInPlotOne = true;
    //                }
    //                //aep higher min
    //                if (iof.Function.get_X(0) > _ACEHigherMin)
    //                {
    //                    _ACEHigherMin = iof.Function.get_X(0);
    //                    _DisplayLowerACEAreaPlotInPlotOne = false;
    //                }
    //                //aep max
    //                if (iof.Function.get_X(iof.Function.Count - 1) > _ACEMax)
    //                {
    //                    _ACEMax = iof.Function.get_X(iof.Function.Count - 1);
    //                    _DisplayHigherACEAreaPlotInPlotOne = true;
    //                }
    //                //aep lower max
    //                if (iof.Function.get_X(iof.Function.Count - 1) < _ACELowerMax)
    //                {
    //                    _ACELowerMax = iof.Function.get_X(iof.Function.Count - 1);
    //                    _DisplayHigherACEAreaPlotInPlotOne = false;
    //                }
    //                // damage min
    //                if (iof.Function.get_Y(0) < _DamageMin)
    //                {
    //                    _DamageMin = iof.Function.get_Y(0);
    //                    _DisplayLowerDamageAreaPlotInPlotThree = true;
    //                }
    //                // damage higher min
    //                if (iof.Function.get_Y(0) > _DamageHigherMin)
    //                {
    //                    _DamageHigherMin = iof.Function.get_Y(0);
    //                    _DisplayLowerDamageAreaPlotInPlotThree = false;
    //                }
    //                //damage max
    //                if (iof.Function.get_Y(iof.Function.Count - 1) > _DamageMax)
    //                {
    //                    _DamageMax = iof.Function.get_Y(iof.Function.Count - 1);
    //                    _DisplayHigherDamageAreaPlotInPlotThree = true;
    //                }
    //                //damage lower max
    //                if (iof.Function.get_Y(iof.Function.Count - 1) < _DamageLowerMax)
    //                {
    //                    _DamageLowerMax = iof.Function.get_Y(iof.Function.Count - 1);
    //                    _DisplayHigherDamageAreaPlotInPlotThree = false;
    //                }
    //            }
    //        }

    //    }

    //    #region Set_Up_Plots
    //    private void setUpPlot1()
    //    {
    //        //_OxyPlotModel = new OxyPlot.PlotModel();
    //        _OxyPlotModel.MouseDown += _OxyPlotModel_MouseDown;
    //        _OxyPlotModel.MouseMove += _OxyPlotModel_MouseMove;
    //        //_OxyPlotModel.MouseUp += _OxyPlotModel_MouseUp;
    //        _OxyPlotModel.MouseEnter += _OxyPlotModel_MouseEnter;

    //        _OxyPlotModel.Title = "Flow Frequency Relationship";

    //        //LinearAxis XAxis = new LinearAxis();
    //        LinearAxis XAxis = new LinearAxis();

    //        XAxis.Position = AxisPosition.Bottom;
    //        XAxis.Title = "Annual Chance Exceedance";
    //        //XAxis.Minimum = 0;
    //        //XAxis.Maximum = 1;
    //        XAxis.StartPosition = 1;
    //        XAxis.EndPosition = .001;
    //       // XAxis.AbsoluteMaximum = 1;

    //        XAxis.AbsoluteMinimum = .001;
    //        XAxis.Minimum = _ACEMin;
    //        XAxis.Maximum = _ACEMax;
    //        //XAxis.Maximum = .5;
    //        //XAxis.AxisTitleDistance = 20;

    //        //XAxis.MajorGridlineStyle = LineStyle.Solid;
    //        //XAxis.MinorGridlineStyle = LineStyle.Dash;
    //        _OxyPlotModel.Axes.Add(XAxis);

    //        LogarithmicAxis YAxis = new LogarithmicAxis();
    //        YAxis.Position = AxisPosition.Left;

    //        YAxis.Title = "Flow (cfs)";
    //        //YAxis.MajorGridlineStyle = LineStyle.Solid;
    //        //YAxis.MinorGridlineStyle = LineStyle.Dash;
    //        YAxis.Minimum = _FlowMin;
    //        YAxis.Maximum = _FlowMax;
    //        _OxyPlotModel.Axes.Add(YAxis);

    //        _OxyPlotModel.LegendBackground = OxyColors.White;
    //        _OxyPlotModel.LegendBorder = OxyColors.DarkGray;
    //        _OxyPlotModel.LegendPosition = LegendPosition.TopRight;

    //        _OxyPlotModel.PlotMargins = new OxyThickness(60, 10, 10, 40);


    //        _OxyPlotModel.Series.Add(getSeries1Data());
            

    //        if(_DisplayHigherACEAreaPlotInPlotOne == true)
    //        {
    //            _OxyPlotModel.Series.Add(getAEPAreaMaxPlot1Data());

    //        }
    //        else
    //        {
    //            _OxyPlotModel.Series.Add(getUpperVerticalRedLineSeries(1));
    //        }

    //        if (_DisplayLowerACEAreaPlotInPlotOne == true)
    //        {
    //            _OxyPlotModel.Series.Add(getAEPAreaMinPlot1Data());

    //        }
    //        else
    //        {
    //            _OxyPlotModel.Series.Add(getLowerVerticalRedLineSeries(1));

    //        }

    //        if (_DisplayHigherFlowAreaPlotInPlotOne == true)
    //        {
    //            _OxyPlotModel.Series.Add(getFlowAreaMaxPlot1Data());

    //        }
    //        else
    //        {
    //            _OxyPlotModel.Series.Add(getUpperHorizontalRedLineSeries(1));
    //        }


    //        if (_DisplayLowerFlowAreaPlotInPlotOne == true)
    //        {
    //            _OxyPlotModel.Series.Add(getFlowAreaMinPlot1Data());

    //        }
    //        else
    //        {
    //            _OxyPlotModel.Series.Add(getLowerHorizontalRedLineSeries(1));
    //        }


    //        _OxyPlotModel.InvalidatePlot(true);
    //    }



    //    private void setUpPlot2()
    //    {
    //        //_OxyPlotModel2 = new OxyPlot.PlotModel();
    //        _OxyPlotModel2.MouseDown += _OxyPlotModel2_MouseDown;
    //        _OxyPlotModel2.MouseMove += _OxyPlotModel2_MouseMove;
    //        _OxyPlotModel2.MouseUp += _OxyPlotModel2_MouseUp;
    //        _OxyPlotModel2.MouseEnter += _OxyPlotModel2_MouseEnter;

    //        _OxyPlotModel2.Title = "Stage Flow Relationship";

    //        LinearAxis XAxis = new LinearAxis();

    //        XAxis.Position = AxisPosition.Bottom;
    //        XAxis.Title = "Stage (ft)";
    //        //XAxis.MajorGridlineStyle = LineStyle.Solid;
    //        //XAxis.MinorGridlineStyle = LineStyle.Dash;
    //        XAxis.Minimum = _StageMin;
    //        XAxis.Maximum = _StageMax;
    //        _OxyPlotModel2.Axes.Add(XAxis);

    //        LogarithmicAxis YAxis = new LogarithmicAxis();
    //        YAxis.Position = AxisPosition.Left;

    //        YAxis.Title = "Flow (cfs)";
    //        //YAxis.MajorGridlineStyle = LineStyle.Solid;
    //        //YAxis.MinorGridlineStyle = LineStyle.Dash;
    //        YAxis.Minimum = _FlowMin;
    //        YAxis.Maximum = _FlowMax;
    //        _OxyPlotModel2.Axes.Add(YAxis);

    //        _OxyPlotModel2.LegendBackground = OxyColors.White;
    //        _OxyPlotModel2.LegendBorder = OxyColors.DarkGray;
    //        _OxyPlotModel2.LegendPosition = LegendPosition.TopRight;

    //        _OxyPlotModel2.PlotMargins = new OxyThickness(60, 10, 10, 40);

    //        _OxyPlotModel2.Series.Add(getSeries2Data());

    //        if (_DisplayHigherStageAreaPlotInPlotTwo == true)
    //        {
    //            _OxyPlotModel2.Series.Add(getStageAreaMaxPlot2Data());

    //        }
    //        else
    //        {
    //            _OxyPlotModel2.Series.Add(getUpperVerticalRedLineSeries(2));
    //        }

    //        if (_DisplayLowerStageAreaPlotInPlotTwo == true)
    //        {
    //            _OxyPlotModel2.Series.Add(getStageAreaMinPlot2Data());

    //        }
    //        else
    //        {
    //            _OxyPlotModel2.Series.Add(getLowerVerticalRedLineSeries(2));
    //        }

    //        if (_DisplayHigherFlowAreaPlotInPlotOne == false)
    //        {
    //             _OxyPlotModel2.Series.Add(getFlowAreaMaxPlot2Data());

    //        }
    //        else
    //        {
    //            _OxyPlotModel2.Series.Add(getUpperHorizontalRedLineSeries(2));
    //        }

    //        if(_DisplayLowerFlowAreaPlotInPlotOne == false)
    //        {
    //            _OxyPlotModel2.Series.Add(getFlowAreaMinPlot2Data());

    //        }
    //        else
    //        {
    //            _OxyPlotModel2.Series.Add(getLowerHorizontalRedLineSeries(2));
    //        }



    //        _OxyPlotModel2.InvalidatePlot(true);
    //    }



    //    private void setUpPlot3()
    //    {
    //        //_OxyPlotModel3 = new OxyPlot.PlotModel();
    //        _OxyPlotModel3.MouseDown += _OxyPlotModel3_MouseDown;
    //        _OxyPlotModel3.MouseMove += _OxyPlotModel3_MouseMove;
    //        _OxyPlotModel3.MouseUp += _OxyPlotModel3_MouseUp;
    //        _OxyPlotModel3.MouseEnter += _OxyPlotModel3_MouseEnter;

    //        _OxyPlotModel3.Title = "Stage Damage Relationship";

    //        LinearAxis XAxis = new LinearAxis();

    //        XAxis.Position = AxisPosition.Bottom;
    //        XAxis.Title = "Stage (ft)";
    //        //XAxis.MajorGridlineStyle = LineStyle.Solid;
    //        //XAxis.MinorGridlineStyle = LineStyle.Dash;
    //        XAxis.Minimum = _StageMin;
    //        XAxis.Maximum = _StageMax;
    //        _OxyPlotModel3.Axes.Add(XAxis);

    //        LinearAxis YAxis = new LinearAxis();
    //        YAxis.Position = AxisPosition.Left;

    //        YAxis.Title = "Damage ($)";
    //        //YAxis.MajorGridlineStyle = LineStyle.Solid;
    //        //YAxis.MinorGridlineStyle = LineStyle.Dash;
    //        YAxis.Minimum = _DamageMin;
    //        YAxis.Maximum = _DamageMax;
    //        _OxyPlotModel3.Axes.Add(YAxis);

    //        _OxyPlotModel3.LegendBackground = OxyColors.White;
    //        _OxyPlotModel3.LegendBorder = OxyColors.DarkGray;
    //        _OxyPlotModel3.LegendPosition = LegendPosition.TopRight;

    //        _OxyPlotModel3.PlotMargins = new OxyThickness(60, 10, 10, 40);


    //        _OxyPlotModel3.Series.Add(getSeries3Data());

    //        if(_DisplayLowerStageAreaPlotInPlotTwo == false)
    //        {
    //            _OxyPlotModel3.Series.Add(getStageAreaMinPlot3Data());

    //        }
    //        else
    //        {
    //            _OxyPlotModel3.Series.Add(getLowerVerticalRedLineSeries(3));
    //        }

    //        if(_DisplayHigherStageAreaPlotInPlotTwo == false)
    //        {
    //            _OxyPlotModel3.Series.Add(getStageAreaMaxPlot3Data());

    //        }
    //        else
    //        {
    //            _OxyPlotModel3.Series.Add(getUpperVerticalRedLineSeries(3));
    //        }

    //        if(_DisplayHigherDamageAreaPlotInPlotThree == true)
    //        {
    //            _OxyPlotModel3.Series.Add(getDamageAreaMaxPlot3Data());

    //        }
    //        else
    //        {
    //            _OxyPlotModel3.Series.Add(getUpperHorizontalRedLineSeries(3));
    //        }

    //        if (_DisplayLowerDamageAreaPlotInPlotThree == true)
    //        {
    //            _OxyPlotModel3.Series.Add(getDamageAreaMinPlot3Data());

    //        }
    //        else
    //        {
    //            _OxyPlotModel3.Series.Add(getLowerHorizontalRedLineSeries(3));
    //        }



    //        _OxyPlotModel3.InvalidatePlot(true);
    //    }



    //    private void setUpPlot4()
    //    {
    //        //_OxyPlotModel4 = new OxyPlot.PlotModel();
    //        _OxyPlotModel4.MouseDown += _OxyPlotModel4_MouseDown;
    //        _OxyPlotModel4.MouseMove += _OxyPlotModel4_MouseMove;
    //        _OxyPlotModel4.MouseUp += _OxyPlotModel4_MouseUp;
    //        _OxyPlotModel4.MouseEnter += _OxyPlotModel4_MouseEnter;

    //        _OxyPlotModel4.Title = "Damage Frequency Relationship";

    //        LinearAxis XAxis = new LinearAxis();

    //        XAxis.Position = AxisPosition.Bottom;
    //        XAxis.Title = "Annual Chance Exceedance";
    //        //XAxis.MajorGridlineStyle = LineStyle.Solid;
    //        //XAxis.MinorGridlineStyle = LineStyle.Dash;
    //        XAxis.StartPosition = 1;
    //        XAxis.EndPosition = .001;
    //        //XAxis.AbsoluteMaximum = .5;
    //        //XAxis.AbsoluteMinimum = .05;
    //        XAxis.Minimum = _ACEMin;
    //        XAxis.Maximum = _ACEMax;
    //        _OxyPlotModel4.Axes.Add(XAxis);

    //        LinearAxis YAxis = new LinearAxis();
    //        YAxis.Position = AxisPosition.Left;

    //        YAxis.Title = "Damage ($)";
    //        //YAxis.MajorGridlineStyle = LineStyle.Solid;
    //        //YAxis.MinorGridlineStyle = LineStyle.Dash;
    //        YAxis.Minimum = _DamageMin;
    //        YAxis.Maximum = _DamageMax;
    //        _OxyPlotModel4.Axes.Add(YAxis);

    //        _OxyPlotModel4.LegendBackground = OxyColors.White;
    //        _OxyPlotModel4.LegendBorder = OxyColors.DarkGray;
    //        _OxyPlotModel4.LegendPosition = LegendPosition.TopLeft;
            


    //        _OxyPlotModel4.PlotMargins = new OxyThickness(60, 10, 10, 40);

    //        _OxyPlotModel4.Series.Add(getEADSeriesData()); // this is adding the area series

    //        _OxyPlotModel4.Series.Add(getSeries4Data());

    //        if (_DisplayHigherACEAreaPlotInPlotOne == false)
    //        {
    //            _OxyPlotModel4.Series.Add(getAEPAreaMaxPlot4Data());

    //        }
    //        else
    //        {
    //           // _OxyPlotModel4.Series.Add(getUpperVerticalRedLineSeries(4));
    //        }

    //        if (_DisplayLowerACEAreaPlotInPlotOne == false)
    //        {
    //            _OxyPlotModel4.Series.Add(getAEPAreaMinPlot4Data());

    //        }
    //        else
    //        {
    //           // _OxyPlotModel4.Series.Add(getLowerVerticalRedLineSeries(4));
    //        }

    //        if (_DisplayHigherDamageAreaPlotInPlotThree == false)
    //        {
    //        _OxyPlotModel4.Series.Add(getDamageAreaMaxPlot4Data());

    //        }
    //        else
    //        {
    //            //_OxyPlotModel4.Series.Add(getUpperHorizontalRedLineSeries(4));
    //        }

    //        if (_DisplayLowerDamageAreaPlotInPlotThree == false)
    //        {
    //        _OxyPlotModel4.Series.Add(getDamageAreaMinPlot4Data());

    //        }
    //        else
    //        {
    //           // _OxyPlotModel4.Series.Add(getLowerHorizontalRedLineSeries(4));
    //        }



    //        _OxyPlotModel4.InvalidatePlot(true);
    //    }



    //    #endregion

    //    #region Get_Data

    //    #region Plot 1 Data
    //    private Series getSeries1Data()
    //    {
    //        LineSeries mySeries = new LineSeries();

    //        foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //        {

    //            if (func.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency || func.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
    //            {
    //                FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction iof;
    //                if (func.GetType().BaseType == typeof(FdaModel.Functions.FrequencyFunctions.FrequencyFunction))
    //                {
    //                    iof = ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)func).GetOrdinatesFunction();
    //                }
    //                else
    //                {
    //                    iof = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func).GetOrdinatesFunction();
    //                }
    //                for (int i = 0; i < iof.Function.Count; i++)
    //                {
    //                    mySeries.Points.Add(new DataPoint(1 - iof.Function.get_X(i), iof.Function.get_Y(i)));
                        
    //                }
    //            }
    //        }
    //        mySeries.Color =  OxyColor.FromRgb(0,0,0);

    //        return mySeries;

    //    }
      
    //    private Series getFlowAreaMaxPlot1Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(_ACEMin, _FlowMax));
    //        mySeries.Points.Add(new DataPoint(_ACEMax, _FlowMax));
    //        mySeries.Points2.Add(new DataPoint(_ACEMin, _FlowLowerMax));
    //        mySeries.Points2.Add(new DataPoint(_ACEMax, _FlowLowerMax));

    //        mySeries.Color = OxyColor.FromRgb(220,79,44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }
    //    private Series getFlowAreaMinPlot1Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(_ACEMin, _FlowMin));
    //        mySeries.Points.Add(new DataPoint(_ACEMax, _FlowMin));
    //        mySeries.Points2.Add(new DataPoint(_ACEMin, _FlowHigherMin));
    //        mySeries.Points2.Add(new DataPoint(_ACEMax, _FlowHigherMin));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }

    //    private Series getAEPAreaMaxPlot1Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(1 - _ACELowerMax, _FlowMin));
    //        mySeries.Points.Add(new DataPoint(1 - _ACEMax, _FlowMin));
    //        mySeries.Points2.Add(new DataPoint(1 - _ACELowerMax, _FlowMax));
    //        mySeries.Points2.Add(new DataPoint(1 - _ACEMax, _FlowMax));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;
    //        return mySeries;
    //    }
    //    private Series getAEPAreaMinPlot1Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(1 - _ACEMin, _FlowMin));
    //        mySeries.Points.Add(new DataPoint(1 - _ACEHigherMin, _FlowMin));
    //        mySeries.Points2.Add(new DataPoint(1 - _ACEMin, _FlowMax));
    //        mySeries.Points2.Add(new DataPoint(1 - _ACEHigherMin, _FlowMax));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;
    //        return mySeries;
    //    }

        

    //    #endregion

    //    #region Plot 2 Data
    //    private Series getSeries2Data()
    //    {
    //        LineSeries mySeries = new LineSeries();

    //        foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //        {
    //            if (func.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
    //            {
    //                FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction iof = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func);
    //                for (int i = 0; i < iof.Function.Count; i++)
    //                {
    //                    mySeries.Points.Add(new DataPoint(iof.Function.get_Y(i), iof.Function.get_X(i)));
    //                }
    //            }
    //        }
    //        mySeries.Color = OxyColor.FromRgb(0, 0, 0);
    //        return mySeries;
    //    }

        

    //    private Series getFlowAreaMaxPlot2Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(_StageMin, _FlowMax));
    //        mySeries.Points.Add(new DataPoint(_StageMax, _FlowMax));
    //        mySeries.Points2.Add(new DataPoint(_StageMin, _FlowLowerMax));
    //        mySeries.Points2.Add(new DataPoint(_StageMax, _FlowLowerMax));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }
    //    private Series getFlowAreaMinPlot2Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(_StageMin, _FlowMin));
    //        mySeries.Points.Add(new DataPoint(_StageMax, _FlowMin));
    //        mySeries.Points2.Add(new DataPoint(_StageMin, _FlowHigherMin));
    //        mySeries.Points2.Add(new DataPoint(_StageMax, _FlowHigherMin));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }
    //    private Series getStageAreaMaxPlot2Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(_StageLowerMax, _FlowMin));
    //        mySeries.Points.Add(new DataPoint(_StageMax, _FlowMin));
    //        mySeries.Points2.Add(new DataPoint(_StageLowerMax, _FlowMax));
    //        mySeries.Points2.Add(new DataPoint(_StageMax, _FlowMax));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }
    //    private Series getStageAreaMinPlot2Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(_StageMin, _FlowMin));
    //        mySeries.Points.Add(new DataPoint(_StageHigherMin, _FlowMin));
    //        mySeries.Points2.Add(new DataPoint(_StageMin, _FlowMax));
    //        mySeries.Points2.Add(new DataPoint(_StageHigherMin, _FlowMax));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }

    //    #endregion

    //    #region Plot 3 Data
    //    private Series getSeries3Data()
    //    {
    //        LineSeries mySeries = new LineSeries();

    //        foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //        {

    //            if (func.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
    //            {
    //                FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction iof = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func);
    //                for (int i = 0; i < iof.Function.Count; i++)
    //                {
    //                    mySeries.Points.Add(new DataPoint(iof.Function.get_X(i), iof.Function.get_Y(i)));
    //                }
    //            }
    //        }
    //        mySeries.Color = OxyColor.FromRgb(0, 0, 0);
    //        return mySeries;

    //    }
    //    private Series getStageAreaMaxPlot3Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(_StageLowerMax, _DamageMin));
    //        mySeries.Points.Add(new DataPoint(_StageMax, _DamageMin));
    //        mySeries.Points2.Add(new DataPoint(_StageLowerMax, _DamageMax));
    //        mySeries.Points2.Add(new DataPoint(_StageMax, _DamageMax));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }
    //    private Series getStageAreaMinPlot3Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(_StageMin, _DamageMin));
    //        mySeries.Points.Add(new DataPoint(_StageHigherMin, _DamageMin));
    //        mySeries.Points2.Add(new DataPoint(_StageMin, _DamageMax));
    //        mySeries.Points2.Add(new DataPoint(_StageHigherMin, _DamageMax));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }

    //    private Series getDamageAreaMaxPlot3Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(_StageMin, _DamageMax));
    //        mySeries.Points.Add(new DataPoint(_StageMax, _DamageMax));
    //        mySeries.Points2.Add(new DataPoint(_StageMin, _DamageLowerMax));
    //        mySeries.Points2.Add(new DataPoint(_StageMax, _DamageLowerMax));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }
    //    private Series getDamageAreaMinPlot3Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(_StageMin, _DamageMin));
    //        mySeries.Points.Add(new DataPoint(_StageMax, _DamageMin));
    //        mySeries.Points2.Add(new DataPoint(_StageMin, _DamageHigherMin));
    //        mySeries.Points2.Add(new DataPoint(_StageMax, _DamageHigherMin));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }
    //    #endregion

    //    #region Plot 4 Data
    //    private Series getSeries4Data()
    //    {
    //        LineSeries mySeries = new LineSeries();
            

    //        foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //        {

    //            if (func.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
    //            {
    //                FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction iof = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func);
    //                for (int i = 0; i < iof.Function.Count; i++)
    //                {
    //                    mySeries.Points.Add(new DataPoint(1-iof.Function.get_X(i), iof.Function.get_Y(i)));
    //                }
    //            }
    //        }
    //        mySeries.Color = OxyColor.FromRgb(0, 0, 0);
    //        return mySeries;

    //    }

    //    private Series getEADSeriesData()
    //    {
    //        AreaSeries mySeries = new AreaSeries();


    //        foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //        {

    //            if (func.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
    //            {
    //                FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction iof = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func);
    //                for (int i = 0; i < iof.Function.Count; i++)
    //                {
    //                    mySeries.Points.Add(new DataPoint(1 - iof.Function.get_X(i), iof.Function.get_Y(i)));
    //                    mySeries.Points2.Add(new DataPoint(1 - iof.Function.get_X(i), .01)); // for some reason it didn't like 0 so i am putting .01

    //                    //mySeries. ConstantY2
                            
    //                }

    //                //double areaUnderCurve = 0;
    //                //for (int i = 0; i < iof.Xs.Count() - 1; i++)
    //                //{
    //                //    areaUnderCurve += ((iof.Xs[i + 1] - iof.Xs(i)) * iof.Ys[i + 1] + (iof.Xs[i + 1] - iof.Xs(i)) * iof.Ys(i)) / 2;
    //                //}
    //                //_Area = areaUnderCurve;

    //            }
    //        }


    //        mySeries.Color = OxyColor.FromRgb(192,192,192);//  System.Drawing.Color.Red;
            
    //        return mySeries;

    //    }
    //    private Series getAEPAreaMaxPlot4Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(1 - _ACELowerMax, _DamageMin));
    //        mySeries.Points.Add(new DataPoint(1 - _ACEMax, _DamageMin));
    //        mySeries.Points2.Add(new DataPoint(1 - _ACELowerMax, _DamageMax));
    //        mySeries.Points2.Add(new DataPoint(1 - _ACEMax, _DamageMax));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }
    //    private Series getAEPAreaMinPlot4Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(1 - _ACEMin, _DamageMin));
    //        mySeries.Points.Add(new DataPoint(1 - _ACEHigherMin, _DamageMin));
    //        mySeries.Points2.Add(new DataPoint(1 - _ACEMin, _DamageMax));
    //        mySeries.Points2.Add(new DataPoint(1 - _ACEHigherMin, _DamageMax));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }

    //    private Series getDamageAreaMaxPlot4Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(1 - _ACEMin, _DamageMax));
    //        mySeries.Points.Add(new DataPoint(1 - _ACEMax, _DamageMax));
    //        mySeries.Points2.Add(new DataPoint(1 - _ACEMin, _DamageLowerMax));
    //        mySeries.Points2.Add(new DataPoint(1 - _ACEMax, _DamageLowerMax));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }
    //    private Series getDamageAreaMinPlot4Data()
    //    {
    //        AreaSeries mySeries = new AreaSeries();
    //        mySeries.Points.Add(new DataPoint(1 - _ACEMin, _DamageMin));
    //        mySeries.Points.Add(new DataPoint(1 - _ACEMax, _DamageMin));
    //        mySeries.Points2.Add(new DataPoint(1 - _ACEMin, _DamageHigherMin));
    //        mySeries.Points2.Add(new DataPoint(1 - _ACEMax, _DamageHigherMin));

    //        mySeries.Color = OxyColor.FromRgb(220, 79, 44);//  System.Drawing.Color.Red;

    //        return mySeries;
    //    }
    //    #endregion





    //    #endregion


    //    #region linking_the_plots


    //    #region Plot 1 Code
    //    //********* Plot 1 code ***************


    //    private void _OxyPlotModel_MouseEnter(object sender, OxyMouseEventArgs e)
    //    {
           

    //        //_PlotEntered = 1;

    //        //Series mySeries = OxyPlotModel.GetSeriesFromPoint(e.Position);
    //        //if (mySeries != null)
    //        //{
    //        //    TrackerHitResult result = mySeries.GetNearestPoint(e.Position, true);
    //        //    if (result != null && !result.DataPoint.Equals(null))
    //        //    {
    //        //        DataPoint dp = result.DataPoint;
    //        //        // at this point i have the (x,y) value on the line series that was entered.
    //        //        _ACE = dp.X;
    //        //        _Flow = dp.Y;
    //        //        ShowTrackerInPlot2(dp.Y);
    //        //    }
    //        //}
    //    }
    //    private void _OxyPlotModel_MouseDown(object sender, OxyMouseDownEventArgs e)
    //    {
    //        ////txt_AEP.Text = "";
    //        ////txt_Damage.Text = "";
    //        ////txt_Flow.Text = "";
    //        ////txt_Stage.Text = "";

    //        //_MouseDown = true;
    //        ////_ClickedPlot = 1;

    //        //Series mySeries = OxyPlotModel.GetSeriesFromPoint(e.Position);
    //        //if (mySeries != null)
    //        //{
    //        //    TrackerHitResult result = mySeries.GetNearestPoint(e.Position, true);
    //        //    if (result != null)
    //        //    {
    //        //        OxyPlot1.ShowTracker(result);
                    
    //        //    }
    //        //    //if (result != null && !result.DataPoint.Equals(null))
    //        //    //{
    //        //    //    DataPoint dp = result.DataPoint;
    //        //    //    // at this point i have the (x,y) value on the line series that was clicked.
    //        //    //    _AEP = dp.X;
    //        //    //    _Flow = dp.Y;
    //        //    //    ShowTrackerInPlot2(dp.Y);
    //        //    //}
    //        //}


    //        //_MouseDown = true;


    //    }

    //    private void _OxyPlotModel_MouseMove(object sender, OxyMouseEventArgs e)
    //    {
    //        _PlotEntered = 1;
    //        if (_MouseDown == false && _HideTracker == false)
    //        {
    //            Series mySeries = OxyPlotModel.GetSeriesFromPoint(e.Position);
    //            if (mySeries != null)
    //            {
    //                TrackerHitResult result = mySeries.GetNearestPoint(e.Position, true);
    //                if (result != null && !result.DataPoint.Equals(null))
    //                {

    //                    DataPoint dp = result.DataPoint;

    //                    result.Text = "ACE: " + Math.Round(dp.X, 3).ToString() + Environment.NewLine + "Flow: " + Math.Round(dp.Y, 3).ToString();
    //                    OxyPlot1.ShowTracker(result);

    //                    _ACE = dp.X;
    //                    _Flow = dp.Y;
    //                    // at this point i have the (x,y) value on the line series that was clicked.
    //                    ShowTrackerInPlot2(dp.Y); // this will also call plot 3 and plot 4
    //                }
    //            }
    //        }
    //    }


    //    //private void _OxyPlotModel_MouseUp(object sender, OxyMouseEventArgs e)
    //    //{
    //    //    //_MouseDown = false;
    //    //    //OxyPlot2.HideTracker();
    //    //    //OxyPlot3.HideTracker();
    //    //    //OxyPlot4.HideTracker();

    //    //    //txt_AEP.Text = _AEP.ToString();
    //    //    //txt_Damage.Text = _Damage.ToString();
    //    //    //txt_Flow.Text = _Flow.ToString();
    //    //    //txt_Stage.Text = _Stage.ToString();

    //    //}

    //    //***********  end plot 1 code ********************
    //    #endregion

    //    #region Plot 2 Code


    //    private void _OxyPlotModel2_MouseEnter(object sender, OxyMouseEventArgs e)
    //    {
            

    //        ////_MouseDown = true;
    //        //_PlotEntered = 2;

    //        //Series mySeries = OxyPlotModel2.GetSeriesFromPoint(e.Position);
    //        //if (mySeries != null)
    //        //{
    //        //    TrackerHitResult result = mySeries.GetNearestPoint(e.Position, true);
    //        //    if (result != null && !result.DataPoint.Equals(null))
    //        //    {
    //        //        DataPoint dp = result.DataPoint;
    //        //        _Stage = dp.X;
    //        //        _Flow = dp.Y;
    //        //        // at this point i have the (x,y) value on the line series that was clicked.
    //        //        ShowTrackerInPlot3(dp.X);
    //        //    }
    //        //}
    //    }
    //    private void _OxyPlotModel2_MouseDown(object sender, OxyMouseDownEventArgs e)
    //    {
            

    //        _MouseDown = true;
    //        _PlotEntered = 2;

    //        Series mySeries = OxyPlotModel2.GetSeriesFromPoint(e.Position);
    //        if (mySeries != null)
    //        {
    //            TrackerHitResult result = mySeries.GetNearestPoint(e.Position, true);
    //            if (result != null && !result.DataPoint.Equals(null))
    //            {
    //                DataPoint dp = result.DataPoint;
    //                _Stage = dp.X;
    //                _Flow = dp.Y;
    //                // at this point i have the (x,y) value on the line series that was clicked.
    //                ShowTrackerInPlot3(dp.X);
    //            }
    //        }
    //    }

    //    private void _OxyPlotModel2_MouseUp(object sender, OxyMouseEventArgs e)
    //    {
    //        _MouseDown = false;
    //        OxyPlot1.HideTracker();
    //        OxyPlot3.HideTracker();
    //        OxyPlot4.HideTracker();

            
    //    }

    //    private void _OxyPlotModel2_MouseMove(object sender, OxyMouseEventArgs e)
    //    {
    //        _PlotEntered = 2;
    //        if (_MouseDown == false && _HideTracker == false)
    //        {
    //            Series mySeries = OxyPlotModel2.GetSeriesFromPoint(e.Position);
    //            if (mySeries != null)
    //            {
    //                TrackerHitResult result = mySeries.GetNearestPoint(e.Position, true);
    //                if (result != null && !result.DataPoint.Equals(null))
    //                {
    //                    DataPoint dp = result.DataPoint;
    //                    result.Text = "Stage: " + Math.Round(dp.X, 3).ToString() + Environment.NewLine + "Flow: " + Math.Round(dp.Y, 3).ToString();

    //                    OxyPlot2.ShowTracker(result);
    //                    _Stage = dp.X;
    //                    _Flow = dp.Y;
    //                    // at this point i have the (x,y) value on the line series that was clicked.
    //                    ShowTrackerInPlot3(dp.X);
    //                }
    //            }
    //        }
    //    }


    //    #endregion

    //    #region Plot 3 Code
    //    private void _OxyPlotModel3_MouseUp(object sender, OxyMouseEventArgs e)
    //    {
    //        _MouseDown = false;
    //        OxyPlot1.HideTracker();
    //        OxyPlot2.HideTracker();
    //        OxyPlot4.HideTracker();

           
    //    }

    //    private void _OxyPlotModel3_MouseMove(object sender, OxyMouseEventArgs e)
    //    {
    //        _PlotEntered = 3;
    //        if (_MouseDown == false && _HideTracker == false)
    //        {
    //            Series mySeries = OxyPlotModel3.GetSeriesFromPoint(e.Position);
    //            if (mySeries != null)
    //            {
    //                TrackerHitResult result = mySeries.GetNearestPoint(e.Position, true);
    //                if (result != null && !result.DataPoint.Equals(null))
    //                {
    //                    DataPoint dp = result.DataPoint;
    //                    result.Text = "Stage: " + Math.Round(dp.X, 3).ToString() + Environment.NewLine + "Damage: " + Math.Round(dp.Y, 3).ToString();

    //                    OxyPlot3.ShowTracker(result);
    //                    _Stage = dp.X;
    //                    _Damage = dp.Y;
    //                    // at this point i have the (x,y) value on the line series that was clicked.
    //                    ShowTrackerInPlot4(dp.Y);
    //                }
    //            }
    //        }
    //    }
    //    private void _OxyPlotModel3_MouseEnter(object sender, OxyMouseEventArgs e)
    //    {
            

    //        ////_MouseDown = true;
    //        //_PlotEntered = 3;
    //        //Series mySeries = OxyPlotModel3.GetSeriesFromPoint(e.Position);
    //        //if (mySeries != null)
    //        //{
    //        //    TrackerHitResult result = mySeries.GetNearestPoint(e.Position, true);
    //        //    if (result != null && !result.DataPoint.Equals(null))
    //        //    {
    //        //        DataPoint dp = result.DataPoint;
    //        //        _Stage = dp.X;
    //        //        _Damage = dp.Y;
    //        //        // at this point i have the (x,y) value on the line series that was clicked.
    //        //        ShowTrackerInPlot4(dp.Y);
    //        //    }
    //        //}
    //    }
    //    private void _OxyPlotModel3_MouseDown(object sender, OxyMouseDownEventArgs e)
    //    {
            

    //        _MouseDown = true;
    //        _PlotEntered = 3;
    //        Series mySeries = OxyPlotModel3.GetSeriesFromPoint(e.Position);
    //        if (mySeries != null)
    //        {
    //            TrackerHitResult result = mySeries.GetNearestPoint(e.Position, true);
    //            if (result != null && !result.DataPoint.Equals(null))
    //            {
    //                DataPoint dp = result.DataPoint;
    //                _Stage = dp.X;
    //                _Damage = dp.Y;
    //                // at this point i have the (x,y) value on the line series that was clicked.
    //                ShowTrackerInPlot4(dp.Y);
    //            }
    //        }
    //    }
    //    #endregion

    //    #region Plot 4 Code

    //    private void _OxyPlotModel4_MouseUp(object sender, OxyMouseEventArgs e)
    //    {
    //        _MouseDown = false;
    //        OxyPlot1.HideTracker();
    //        OxyPlot2.HideTracker();
    //        OxyPlot3.HideTracker();

            
    //    }

    //    private void _OxyPlotModel4_MouseMove(object sender, OxyMouseEventArgs e)
    //    {
    //        _PlotEntered = 4;
    //        if (_MouseDown == false && _HideTracker == false)
    //        {
    //            Series mySeries = OxyPlotModel4.GetSeriesFromPoint(e.Position);
    //            if (mySeries != null)
    //            {
    //                TrackerHitResult result = mySeries.GetNearestPoint(e.Position, true);
    //                if (result != null && !result.DataPoint.Equals(null))
    //                {
    //                    DataPoint dp = result.DataPoint;
    //                    result.Text = "ACE: " + Math.Round(dp.X, 3).ToString() + Environment.NewLine + "Damage: " + Math.Round(dp.Y, 3).ToString();

    //                    OxyPlot4.ShowTracker(result);
    //                    _ACE = dp.X;
    //                    _Damage = dp.Y;
    //                    // at this point i have the (x,y) value on the line series that was clicked.
    //                    ShowTrackerInPlot1(dp.X);
    //                }
    //            }
    //        }
    //    }

    //    private void _OxyPlotModel4_MouseEnter(object sender, OxyMouseEventArgs e)
    //    {
            

    //        ////_MouseDown = true;
    //        //_PlotEntered = 4;
    //        //Series mySeries = OxyPlotModel4.GetSeriesFromPoint(e.Position);
    //        //if (mySeries != null)
    //        //{
    //        //    TrackerHitResult result = mySeries.GetNearestPoint(e.Position, true);
    //        //    if (result != null && !result.DataPoint.Equals(null))
    //        //    {
    //        //        DataPoint dp = result.DataPoint;
    //        //        _ACE = dp.X;
    //        //        _Damage = dp.Y;
    //        //        // at this point i have the (x,y) value on the line series that was clicked.
    //        //        ShowTrackerInPlot1(dp.X);
    //        //    }
    //        //}
    //    }

    //    private void _OxyPlotModel4_MouseDown(object sender, OxyMouseDownEventArgs e)
    //    {
           

    //        _MouseDown = true;
    //        _PlotEntered = 4;
    //        Series mySeries = OxyPlotModel4.GetSeriesFromPoint(e.Position);
    //        if (mySeries != null)
    //        {
    //            TrackerHitResult result = mySeries.GetNearestPoint(e.Position, true);
    //            if (result != null && !result.DataPoint.Equals(null))
    //            {
    //                DataPoint dp = result.DataPoint;
    //                _ACE = dp.X;
    //                _Damage = dp.Y;
    //                // at this point i have the (x,y) value on the line series that was clicked.
    //                ShowTrackerInPlot1(dp.X);
    //            }
    //        }
    //    }

    //    #endregion

    //    #region Shared Between Plots

    //    private Series getUpperVerticalRedLineSeries(int plotNumber)
    //    {
    //        LineSeries upperRedLineSeries = new LineSeries();
    //        switch (plotNumber)
    //        {
    //            case 1:
    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {
    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency || func.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
    //                    {
    //                        upperRedLineSeries = AddUpperVerticalRedLineSeries(func, _ACELowerMax,OxyPlotModel);
    //                    }
    //                }
    //                break;
    //            case 2:
    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {

    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
    //                    {
    //                        upperRedLineSeries = AddUpperVerticalRedLineSeries(func, _StageLowerMax,OxyPlotModel2);
    //                    }
    //                }
    //                break;

    //            case 3:
    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {

    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
    //                    {
    //                        upperRedLineSeries = AddUpperVerticalRedLineSeries(func, _StageLowerMax, OxyPlotModel3);
    //                    }
    //                }
    //                break;

    //            case 4:
    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {

    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
    //                    {
    //                        upperRedLineSeries = AddUpperVerticalRedLineSeries(func, _ACELowerMax, OxyPlotModel4);

    //                    }
    //                }
    //                break;
    //        }
            
    //        upperRedLineSeries.Color = OxyColor.FromRgb(255, 0, 0);
    //        upperRedLineSeries.StrokeThickness = 3;
    //        return upperRedLineSeries;

    //    }
    //    private Series getLowerVerticalRedLineSeries(int plotNumber)
    //    {
    //        LineSeries lowerRedLineSeries = new LineSeries();

    //        switch (plotNumber)
    //        {
    //            case 1:
    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {
    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency || func.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
    //                    {
    //                        lowerRedLineSeries = AddLowerVerticalRedLineSeries(func, _ACEHigherMin,OxyPlotModel);
    //                    }
    //                }
    //                break;
    //            case 2:

    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {
    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
    //                    {
    //                        lowerRedLineSeries = AddLowerVerticalRedLineSeries(func, _StageHigherMin,OxyPlotModel2);

    //                    }
    //                }
    //                break;

    //            case 3:

    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {

    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
    //                    {
    //                        lowerRedLineSeries = AddLowerVerticalRedLineSeries(func, _StageHigherMin,OxyPlotModel3);

    //                    }
    //                }
    //                break;

    //            case 4:
    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {
    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
    //                    {
    //                        lowerRedLineSeries = AddLowerVerticalRedLineSeries(func, _ACEHigherMin,OxyPlotModel4);
    //                    }
    //                }
    //                break;
               

    //        }
  
    //        lowerRedLineSeries.Color = OxyColor.FromRgb(255, 0, 0);
    //        lowerRedLineSeries.StrokeThickness = 3;
    //        return lowerRedLineSeries;

    //    }

    //    private Series getUpperHorizontalRedLineSeries(int plotNumber)
    //    {
    //        LineSeries upperRedLineSeries = new LineSeries();
    //        switch (plotNumber)
    //        {
    //            case 1:
    //                FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction of;

    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {
    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency || func.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
    //                    {
    //                        upperRedLineSeries = AddUpperHorizontalRedLineSeries(func, _FlowLowerMax,OxyPlotModel);
    //                    }
    //                }
    //                break;

    //            case 2:
    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {
    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
    //                    {       
    //                       upperRedLineSeries = AddUpperHorizontalRedLineSeries(func, _FlowLowerMax,OxyPlotModel2);
    //                    }
    //                }
    //                break;

    //            case 3:
    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {
    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
    //                    {
    //                        upperRedLineSeries = AddUpperHorizontalRedLineSeries(func, _DamageLowerMax,OxyPlotModel3);
    //                    }
    //                }
    //                break;

    //            case 4:
    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {
    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
    //                    {
    //                        upperRedLineSeries = AddUpperHorizontalRedLineSeries(func, _DamageLowerMax,OxyPlotModel4);
    //                    }
    //                }
    //                break;
    //        }

    //        upperRedLineSeries.Color = OxyColor.FromRgb(255, 0, 0);
    //        upperRedLineSeries.StrokeThickness = 3;
    //        return upperRedLineSeries;

    //    }

    //    private Series getLowerHorizontalRedLineSeries(int plotNumber)
    //    {
    //        LineSeries lowerRedLineSeries = new LineSeries();

    //        switch (plotNumber)
    //        {
    //            case 1:
    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {
    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency || func.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
    //                    {
    //                        lowerRedLineSeries = AddLowerHorizontalRedLineSeries(func, _DamageHigherMin,OxyPlotModel);
    //                    }
    //                }
    //                break;
    //            case 2:

    //                FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction of2;


    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {

    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
    //                    {
    //                        lowerRedLineSeries = AddLowerHorizontalRedLineSeries(func, _FlowHigherMin,OxyPlotModel2);
    //                    }
    //                }
    //                break;

    //            case 3:
    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {
    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.InteriorStageDamage)
    //                    {                          
    //                        lowerRedLineSeries = AddLowerHorizontalRedLineSeries(func, _DamageHigherMin,OxyPlotModel3);                      
    //                    }
    //                }
    //                break;

    //            case 4:
    //                foreach (FdaModel.Functions.BaseFunction func in InputFunctions)
    //                {

    //                    if (func.FunctionType == FdaModel.Functions.FunctionTypes.DamageFrequency)
    //                    {
    //                        lowerRedLineSeries = AddLowerHorizontalRedLineSeries(func, _DamageHigherMin,OxyPlotModel4);
    //                    }
    //                }
    //                break;
    //        }
    //        return lowerRedLineSeries;

    //    }


    //    private LineSeries AddUpperVerticalRedLineSeries(FdaModel.Functions.BaseFunction func, double startPoint, OxyPlot.PlotModel plotModel)
    //    {

    //        LineSeries upperRedLineSeries = new LineSeries();
    //        FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction of;

    //        if (func.GetType().BaseType == typeof(FdaModel.Functions.FrequencyFunctions.FrequencyFunction))
    //        {
    //            of = ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)func).GetOrdinatesFunction();
    //        }
    //        else
    //        {
    //            of = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func).GetOrdinatesFunction();

    //        }

    //        if (func.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
    //        {
    //            //swap the xs and ys
    //            double[] newXs = new double[of.Function.Count];
    //            double[] newYs = new double[of.Function.Count];
    //            for (int i = 0; i < of.Function.Count; i++)
    //            {
    //                newXs[i] = of.Function.get_Y(i);
    //                newYs[i] = of.Function.get_X(i);
    //            }
    //            of = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(newXs, newYs, FdaModel.Functions.FunctionTypes.Rating);

    //        }


    //        int k = 0;

    //        while (of.Function.get_X(k) < startPoint)
    //        {
    //            k++;
    //        }

            

    //        if (func.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency || func.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
    //        {
    //            upperRedLineSeries.Points.Add(new DataPoint(1- startPoint, GetPairedValue(1-startPoint, true, plotModel,true)));

    //            for (int i = k; i < of.Function.Count; i++)
    //            {
    //                upperRedLineSeries.Points.Add(new DataPoint(1 - of.Function.get_X(i), of.Function.get_Y(i)));
    //            }
    //        }
    //        else
    //        {
    //            upperRedLineSeries.Points.Add(new DataPoint(startPoint, GetPairedValue(startPoint, true, plotModel)));

    //            for (int i = k; i < of.Function.Count; i++)
    //            {
    //                upperRedLineSeries.Points.Add(new DataPoint(of.Function.get_X(i), of.Function.get_Y(i)));
    //            }
    //        }
            
           
            


    //        upperRedLineSeries.Color = OxyColor.FromRgb(255, 0, 0);
    //        upperRedLineSeries.StrokeThickness = 3;
    //        return upperRedLineSeries;
    //    }
    //    private LineSeries AddLowerVerticalRedLineSeries(FdaModel.Functions.BaseFunction func, double endPoint, OxyPlot.PlotModel plotModel)
    //    {
    //        LineSeries lowerRedLineSeries = new LineSeries();
    //        FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction of;

    //        if (func.GetType().BaseType == typeof(FdaModel.Functions.FrequencyFunctions.FrequencyFunction))
    //        {
    //            of = ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)func).GetOrdinatesFunction();
    //        }
    //        else
    //        {
    //            of = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func).GetOrdinatesFunction();
    //        }


    //        if (func.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
    //        {
    //            //swap the xs and ys
    //            double[] newXs = new double[of.Function.Count];
    //            double[] newYs = new double[of.Function.Count];
    //            for (int i = 0; i < of.Function.Count; i++)
    //            {
    //                newXs[i] = of.Function.get_Y(i);
    //                newYs[i] = of.Function.get_X(i);
    //            }
    //            of = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(newXs, newYs, FdaModel.Functions.FunctionTypes.Rating);

    //        }

    //        int k = 0;
    //        // evaluate the vertical points
    //        while (of.Function.get_X(k) < endPoint)
    //        {
    //            k++;
    //        }
    //        if (func.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency || func.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
    //        {
    //            for (int i = 0; i < k; i++)
    //            {
    //                lowerRedLineSeries.Points.Add(new DataPoint(1 - of.Function.get_X(i), of.Function.get_Y(i)));
    //            }
    //            //add the last point that is right on the endPoint
    //            lowerRedLineSeries.Points.Add(new DataPoint(1 - endPoint, GetPairedValue(1-endPoint, true, plotModel, true)));
    //        }
    //        else
    //        {
    //            for (int i = 0; i < k; i++)
    //            {
    //                lowerRedLineSeries.Points.Add(new DataPoint(of.Function.get_X(i), of.Function.get_Y(i)));
                    
    //            }
    //            //add the last point that is right on the endPoint
    //            lowerRedLineSeries.Points.Add(new DataPoint(endPoint, GetPairedValue(endPoint, true, plotModel, false)));
    //        }           

    //        lowerRedLineSeries.Color = OxyColor.FromRgb(255, 0, 0);
    //        lowerRedLineSeries.StrokeThickness = 3;
    //        return lowerRedLineSeries;
    //    }

    //    private LineSeries AddUpperHorizontalRedLineSeries(FdaModel.Functions.BaseFunction func, double startPoint, OxyPlot.PlotModel plotModel)
    //    {
    //        LineSeries upperRedLineSeries = new LineSeries();
    //        FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction of;

    //        if (func.GetType().BaseType == typeof(FdaModel.Functions.FrequencyFunctions.FrequencyFunction))
    //        {
    //            of = ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)func).GetOrdinatesFunction();
    //        }
    //        else
    //        {
    //            of = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func).GetOrdinatesFunction();
    //        }


    //        if (func.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
    //        {
    //            //swap the xs and ys
    //            double[] newXs = new double[of.Function.Count];
    //            double[] newYs = new double[of.Function.Count];
    //            for (int i = 0; i < of.Function.Count; i++)
    //            {
    //                newXs[i] = of.Function.get_Y(i);
    //                newYs[i] = of.Function.get_X(i);
    //            }
    //            of = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(newXs, newYs, FdaModel.Functions.FunctionTypes.Rating);

    //        }

    //        int k = 0;

    //        while (of.Function.get_Y(k) < startPoint)
    //        {
    //            k++;
    //        }

            
    //        if (func.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency || func.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
    //        {
    //            upperRedLineSeries.Points.Add(new DataPoint(GetPairedValue(startPoint,false, plotModel,true),startPoint));

    //            for (int i = k; i < of.Function.Count; i++)
    //            {
    //                upperRedLineSeries.Points.Add(new DataPoint(1 - of.Function.get_X(i), of.Function.get_Y(i)));
    //            }
    //        }
    //        else
    //        {
    //            upperRedLineSeries.Points.Add(new DataPoint(GetPairedValue(startPoint, false, plotModel),startPoint));

    //            for (int i = k; i < of.Function.Count; i++)
    //            {
    //                upperRedLineSeries.Points.Add(new DataPoint(of.Function.get_X(i), of.Function.get_Y(i)));
    //            }
    //        }
            


    //        upperRedLineSeries.Color = OxyColor.FromRgb(255, 0, 0);
    //        upperRedLineSeries.StrokeThickness = 3;
    //        return upperRedLineSeries;
    //    }

    //    private LineSeries AddLowerHorizontalRedLineSeries(FdaModel.Functions.BaseFunction func, double endPoint,OxyPlot.PlotModel plotModel)
    //    {
    //        LineSeries lowerRedLineSeries = new LineSeries();
    //        FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction of;

    //        if (func.GetType().BaseType == typeof(FdaModel.Functions.FrequencyFunctions.FrequencyFunction))
    //        {
    //            of = ((FdaModel.Functions.FrequencyFunctions.FrequencyFunction)func).GetOrdinatesFunction();
    //        }
    //        else
    //        {
    //            of = ((FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction)func).GetOrdinatesFunction();
    //        }


    //        if (func.FunctionType == FdaModel.Functions.FunctionTypes.Rating)
    //        {
    //            //swap the xs and ys
    //            double[] newXs = new double[of.Function.Count];
    //            double[] newYs = new double[of.Function.Count];
    //            for (int i = 0; i < of.Function.Count; i++)
    //            {
    //                newXs[i] = of.Function.get_Y(i);
    //                newYs[i] = of.Function.get_X(i);
    //            }
    //            of = new FdaModel.Functions.OrdinatesFunctions.OrdinatesFunction(newXs, newYs, FdaModel.Functions.FunctionTypes.Rating);

    //        }

    //        int k = 0;
    //        while (of.Function.get_Y(k) < endPoint)
    //        {
    //            k++;
    //        }
    //        if (func.FunctionType == FdaModel.Functions.FunctionTypes.InflowFrequency || func.FunctionType == FdaModel.Functions.FunctionTypes.OutflowFrequency)
    //        {
    //            for (int i = 0; i < k; i++)
    //            {
    //                lowerRedLineSeries.Points.Add(new DataPoint(1 - of.Function.get_X(i), of.Function.get_Y(i)));
    //            }
    //            //add the last point that is right on the endPoint
    //            lowerRedLineSeries.Points.Add(new DataPoint( GetPairedValue(endPoint,false, plotModel, true),endPoint));
    //        }
    //        else
    //        {
    //            for (int i = 0; i < k; i++)
    //            {
    //                lowerRedLineSeries.Points.Add(new DataPoint(of.Function.get_X(i), of.Function.get_Y(i)));
    //            }
    //            //add the last point that is right on the endPoint
    //            lowerRedLineSeries.Points.Add(new DataPoint(GetPairedValue(endPoint, false, plotModel, false), endPoint));
    //        }

            
    //        lowerRedLineSeries.Color = OxyColor.FromRgb(255, 0, 0);
    //        lowerRedLineSeries.StrokeThickness = 3;
    //        return lowerRedLineSeries;
    //    }

    //    private void ShowTrackerInPlot1(double knownValue) //plot 2 is Stage vs Flow
    //    {
    //        if (_PlotEntered == 1) { return; }
    //        double yValue = GetPairedValue(knownValue, true, OxyPlotModel, true);
    //        if (yValue == -1) { return; }
    //        DataPoint newPoint = new DataPoint(knownValue, yValue);
    //        _ACE = newPoint.X;
    //        _Flow = newPoint.Y;
    //        var position = OxyPlotModel.Axes[0].Transform(newPoint.X, newPoint.Y, OxyPlotModel.Axes[1]);
    //        ScreenPoint sp = new ScreenPoint(position.X, position.Y);
    //        TrackerHitResult thr = new TrackerHitResult();// _OxyPlotModel.Series[0], newPoint, sp);
    //        thr.Series = _OxyPlotModel.Series[0];
    //        thr.DataPoint = newPoint;
    //        thr.Position = sp;
    //        thr.Text = "ACE: " + Math.Round(knownValue, 3).ToString() + Environment.NewLine + "Flow: " + Math.Round(yValue, 3).ToString();
    //        OxyPlot1.ShowTracker(thr);

    //        ShowTrackerInPlot2(yValue);
    //    }
    //    private void ShowTrackerInPlot2(double knownValue) //plot 2 is Stage vs Flow
    //    {
    //        if (_PlotEntered == 2) { return; }

    //        double xValue = GetPairedValue(knownValue, false, OxyPlotModel2);
    //        if (xValue == -1) { return; }
    //        DataPoint newPoint = new DataPoint(xValue, knownValue);
    //        _Stage = newPoint.X;
    //        _Flow = newPoint.Y;
    //        var position = OxyPlotModel2.Axes[0].Transform(newPoint.X, newPoint.Y, OxyPlotModel2.Axes[1]);
    //        ScreenPoint sp = new ScreenPoint(position.X, position.Y);
    //        TrackerHitResult thr = new TrackerHitResult();// _OxyPlotModel2.Series[0], newPoint, sp);
    //        thr.Series = _OxyPlotModel.Series[0];
    //        thr.DataPoint = newPoint;
    //        thr.Position = sp;
    //        thr.Text = "Stage: " + Math.Round(xValue, 3).ToString() + Environment.NewLine + "Flow: " + Math.Round(knownValue, 3).ToString();
    //        OxyPlot2.ShowTracker(thr);

    //        ShowTrackerInPlot3(xValue);
    //    }
    //    private void ShowTrackerInPlot3(double knownValue) // plot 3 is Stage vs Damage; the knownValue is the stage (x value)
    //    {
    //        if (_PlotEntered == 3) { return; }

    //        double yValue = GetPairedValue(knownValue, true, OxyPlotModel3);
    //        if (yValue == -1) { return; }
    //        DataPoint newPoint = new DataPoint(knownValue, yValue);
    //        _Stage = newPoint.X;
    //        _Damage = newPoint.Y;
    //        var position = OxyPlotModel3.Axes[0].Transform(newPoint.X, newPoint.Y, OxyPlotModel3.Axes[1]);
    //        ScreenPoint sp = new ScreenPoint(position.X, position.Y);
    //        TrackerHitResult thr = new TrackerHitResult();// _OxyPlotModel3.Series[0], newPoint, sp);
    //        thr.Series = _OxyPlotModel.Series[0];
    //        thr.DataPoint = newPoint;
    //        thr.Position = sp;
    //        thr.Text = "Stage: " + Math.Round(knownValue, 3).ToString() + Environment.NewLine + "Damage: " + Math.Round(yValue, 3).ToString();
    //        OxyPlot3.ShowTracker(thr);

    //        ShowTrackerInPlot4(yValue);
    //    }
    //    private void ShowTrackerInPlot4(double knownValue) // plot 4 is AEP vs Damage; the knownValue is the Damage (y value)
    //    {
    //        if (_PlotEntered == 4) { return; }

    //        double xValue = GetPairedValue(knownValue, false, OxyPlotModel4, true);
    //        if (xValue == -1) { return; }
    //        DataPoint newPoint = new DataPoint(xValue, knownValue);
    //        _ACE = newPoint.X;
    //        _Damage = newPoint.Y;
    //        var position = OxyPlotModel4.Axes[0].Transform(newPoint.X, newPoint.Y, OxyPlotModel4.Axes[1]);
    //        ScreenPoint sp = new ScreenPoint(position.X, position.Y);
    //        TrackerHitResult thr = new TrackerHitResult();// _OxyPlotModel4.Series[0], newPoint, sp);
    //        thr.Series = _OxyPlotModel.Series[0];
    //        thr.DataPoint = newPoint;
    //        thr.Position = sp;
    //        thr.Text = "ACE: " + Math.Round(xValue, 3).ToString() + Environment.NewLine + "Damage: " + Math.Round(knownValue, 3).ToString();
    //        OxyPlot4.ShowTracker(thr);

    //        ShowTrackerInPlot1(xValue);
    //    }
    //    /// <summary>
    //    /// This function returns the x-value or the y-value for a particular point in the specified plotModel
    //    /// </summary>
    //    /// <param name="knownValue"> Either the x-value or the y-value. Whichever is known.</param>
    //    /// <param name="lookingForY">Boolean asking if you are looking for the y-value or the x-value.</param>
    //    /// <param name="PM">The plot model. This way I know which plot model to pull the point from.</param>
    //    /// <param name="axisReversed">This if for the probability axes which are reversed. </param>
    //    /// <returns></returns>
    //    private double GetPairedValue(double knownValue, bool lookingForY, PlotModel PM, bool axisReversed = false)
    //    {
    //        double pairedValue = -1;

    //        List<DataPoint> seriesPoints = ((LineSeries)PM.Series[0]).Points.ToList<DataPoint>();
    //        if (lookingForY == true)
    //        {
    //            if (axisReversed == false)
    //            {
    //                for (int i = 0; i < seriesPoints.Count(); i++)
    //                {
    //                    if (knownValue < seriesPoints[0].X) { return -1; }
    //                    if (knownValue < seriesPoints[i].X) //what about equal to
    //                    {
    //                        double slope = (seriesPoints[i].Y - seriesPoints[i - 1].Y) / (seriesPoints[i].X - seriesPoints[i - 1].X);
    //                        double yIntercept = -1 * ((slope * seriesPoints[i].X) - seriesPoints[i].Y);
    //                        pairedValue = slope * knownValue + yIntercept; // y = mx + b
    //                        return pairedValue;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                for (int i = 0; i < seriesPoints.Count(); i++)
    //                {
    //                    if (knownValue > seriesPoints[0].X) { return -1; }

    //                    if (knownValue > seriesPoints[i].X) //what about equal to
    //                    {
    //                        double slope = (seriesPoints[i].Y - seriesPoints[i - 1].Y) / (seriesPoints[i].X - seriesPoints[i - 1].X);
    //                        double yIntercept = -1 * ((slope * seriesPoints[i].X) - seriesPoints[i].Y);
    //                        pairedValue = slope * knownValue + yIntercept; // y = mx + b
    //                        return pairedValue;
    //                    }
    //                }
    //            }

    //        }
    //        else if (lookingForY == false)
    //        {
    //            for (int i = 0; i < seriesPoints.Count(); i++)// need to handle the end cases!!!!
    //            {
    //                if (knownValue < seriesPoints[0].Y) { return -1; }
    //                if (knownValue < seriesPoints[i].Y) //what about equal to
    //                {
    //                    double slope = (seriesPoints[i].Y - seriesPoints[i - 1].Y) / (seriesPoints[i].X - seriesPoints[i - 1].X);
    //                    if (slope == 0) { throw new Exception(); }
    //                    double yIntercept = -1 * ((slope * seriesPoints[i].X) - seriesPoints[i].Y);
    //                    pairedValue = (knownValue - yIntercept) / slope; //x = (y - b)/m
    //                    return pairedValue;
    //                }
    //            }
    //        }

    //        return pairedValue;
    //    }

        private void btn_ShowTracker_Click(object sender, RoutedEventArgs e)
        {
    //        _HideTracker = false;

    //        btn_ShowTracker.IsEnabled = false;
    //        btn_HideTracker.IsEnabled = true;
        }

        private void btn_HideTracker_Click(object sender, RoutedEventArgs e)
        {
    //        _HideTracker = true;

    //        OxyPlot1.HideTracker();
    //        OxyPlot2.HideTracker();
    //        OxyPlot3.HideTracker();
    //        OxyPlot4.HideTracker();

    //        btn_ShowTracker.IsEnabled = true;
    //        btn_HideTracker.IsEnabled = false;

        }

        private void btn_EadHistogram_Click(object sender, RoutedEventArgs e)
        {
            LinkedPlotsVM vm = (LinkedPlotsVM)this.DataContext;
            vm.DisplayEADHistogram();
            //HistogramViewer hv = new HistogramViewer(Result, true);
            //hv.Owner = this;
            //hv.Show();
        }

        private void btn_AepHistogram_Click(object sender, RoutedEventArgs e)
        {
            LinkedPlotsVM vm = (LinkedPlotsVM)this.DataContext;
            vm.DisplayAEPHistogram();
            //        HistogramViewer hv = new HistogramViewer(Result, false);
            //        hv.Owner = this;
            //        hv.Show();

        }

        private void btn_Plot_Click(object sender, RoutedEventArgs e)
        {
    //        //is it an int?
    //        int newInt;
    //        if (Int32.TryParse(txt_Iteration.Text, out newInt))
    //        {
    //            newInt = Int32.Parse(txt_Iteration.Text);
    //            if (newInt < Result.Realizations.Count)
    //            {
    //                _CurrentIteration = newInt;
    //                //LinkedPlots newPlot = new LinkedPlots(Result);
    //                PlotIteration(_CurrentIteration);
    //            }
    //        }


        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
    //        if(_CurrentIteration < Result.Realizations.Count - 1)
    //        {
    //            _CurrentIteration += 1;
    //            PlotIteration(_CurrentIteration);
    //            txt_Iteration.Text = _CurrentIteration.ToString();

    //        }
    //        if (_CurrentIteration > 0)
    //        {
    //            btn_Prev.IsEnabled = true;
    //        }
    //        if(_CurrentIteration == Result.Realizations.Count-1)
    //        {
    //            btn_Next.IsEnabled = false;
    //        }

        }

        private void btn_Prev_Click(object sender, RoutedEventArgs e)
        {
    //        if (_CurrentIteration >= 1)
    //        {
    //            _CurrentIteration -= 1;
    //            PlotIteration(_CurrentIteration);
    //            txt_Iteration.Text = _CurrentIteration.ToString();
    //        }
    //        if(_CurrentIteration == 0)
    //        {
    //            btn_Prev.IsEnabled = false;
    //        }
    //        if(_CurrentIteration < Result.Realizations.Count-1)
    //        {
    //            btn_Next.IsEnabled = true;
    //        }

        }

    //    private void clearAxisValues()
    //    {
    //        _ACEMin = double.MaxValue;
    //    _ACEHigherMin = double.MinValue;
    //    _ACEMax = double.MinValue;
    //     _ACELowerMax = double.MaxValue;

    //     _FlowMin = double.MaxValue;
    //     _FlowHigherMin = double.MinValue;
    //     _FlowMax = double.MinValue;
    //     _FlowLowerMax = double.MaxValue;

    //     _StageMin = double.MaxValue;
    //     _StageHigherMin = double.MinValue;
    //    _StageMax = double.MinValue;
    //     _StageLowerMax = double.MaxValue;

    //     _DamageMin = double.MaxValue;
    //     _DamageHigherMin = double.MinValue;
    //    _DamageMax = double.MinValue;
    //     _DamageLowerMax = double.MaxValue;
    //}
        private void btn_PlotUncertainty_Click(object sender, RoutedEventArgs e)
        {

    //        DamageWithUncertainty dwu = new DamageWithUncertainty(Result);
    //        dwu.Owner = this;
    //        dwu.Show();
        }
    //    #endregion




        //private void _OxyPlotModel_TrackerChanged(object sender, TrackerEventArgs e)
        //{
        //    //TrackerEventArgs tea = new TrackerEventArgs();

        //    if (e.HitResult != null)
        //    {
        //        AEP = e.HitResult.DataPoint.X;
        //        flow = e.HitResult.DataPoint.Y;
        //        //MessageBox.Show(xs.ToString());
        //        OxyMouseDownEventArgs es = new OxyMouseDownEventArgs();

        //        _OxyPlotModel2_MouseDown(sender, es);
        //    }

        //}

      //  #endregion
        //The x axis has to be added to the plotmodel before the Y axis!
        //private void ShowTrackerInPlot(int plotNumber, double knownValue, bool lookingForY)
        //{
        //    PlotModel PM;
        //    IPlotView PV;
        //    switch (plotNumber)
        //    {
        //        case 1:
        //            PM = OxyPlotModel;
        //            PV = OxyPlot1;
        //            break;
        //        case 2:
        //            PM = OxyPlotModel2;
        //            PV = OxyPlot2;
        //            break;
        //        case 3:
        //            PM = OxyPlotModel3;
        //            PV = OxyPlot3;
        //            break;
        //        case 4:
        //            PM = OxyPlotModel4;
        //            PV = OxyPlot4;
        //            break;
        //        default:
        //            return;
        //    }

        //    if (lookingForY == false)
        //    {
        //        double xValue = GetPairedValue(knownValue, false, PM);
        //        DataPoint newPoint = new DataPoint(xValue, knownValue);

        //        var position = PM.Axes[0].Transform(newPoint.X, newPoint.Y, PM.Axes[1]);
        //        ScreenPoint sp = new ScreenPoint(position.X, position.Y);

        //        TrackerHitResult thr = new TrackerHitResult(PM.Series[0], newPoint, sp);

        //        switch (plotNumber)
        //        {
        //            case 1:
        //                thr.Text = "Stage: " + xValue.ToString() + Environment.NewLine + "Flow: " + knownValue.ToString();
        //                break;
        //            case 2:
        //                thr.Text = "Stage: " + xValue.ToString() + Environment.NewLine + "Flow: " + knownValue.ToString();

        //                break;
        //            case 3:
        //                thr.Text = "Stage: " + xValue.ToString() + Environment.NewLine + "Flow: " + knownValue.ToString();

        //                break;
        //            case 4:
        //                thr.Text = "Stage: " + xValue.ToString() + Environment.NewLine + "Flow: " + knownValue.ToString();

        //                break;
        //            default:
        //                return;
        //        }
        //    }
        //        thr.Text = "Stage: " + xValue.ToString() + Environment.NewLine + "Flow: " + knownValue.ToString();

        //    PV.ShowTracker(thr);
        //        if(plotNumber <4)
        //        {
        //            ShowTrackerInPlot(plotNumber +1,)
        //        }
        //}
    }
}
