using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.TableWithPlot.Data;
using HEC.FDA.ViewModel.Utilities;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using Statistics.Distributions;
using System.Xml.Linq;
using System.Linq;
using Importer;
using Statistics;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using System.Security.Permissions;
using System;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;

public partial class GraphicalVM : ObservableObject
{
    [ObservableProperty]
    int _equivalentRecordLength;

    [ObservableProperty]
    bool _useFlow;

    [ObservableProperty]
    string _xLabel = StringConstants.EXCEEDANCE_PROBABILITY;

    [ObservableProperty]
    string _yLabel = StringConstants.DISCHARGE;

    [ObservableProperty]
    string _name;

    [ObservableProperty]
    DeterministicDataProvider _inputDataProvider;

    [ObservableProperty]
    GraphicalDataProvider _outputDataProvider;

    [ObservableProperty]
    ViewResolvingPlotModel _calcdPlotModel;

    /// <summary>
    /// Creating a new function
    /// </summary>
    public GraphicalVM(string name, string xlabel, string ylabel): this()
    {
        Name = name;
        XLabel = xlabel;
        YLabel = ylabel;
        UseFlow = false;
    }

    /// <summary>
    /// Loading from disk
    /// </summary>
    public GraphicalVM(XElement vmEle) : this()
    {
        LoadFromXML(vmEle);
    }

    /// <summary>
    /// Potentially necessary for 1.4.x import. 
    /// </summary>
    public GraphicalVM(ProbabilityFunction pf) : this()
    {
        throw new NotImplementedException("This constructor is not implemented yet. Please use the other constructor.");
    }

    /// <summary>
    /// reflection 
    /// </summary>
    public GraphicalVM()
    {
        InputDataProvider = new(true, true);
        OutputDataProvider = new();
    }

    partial void OnUseFlowChanged(bool oldValue, bool newValue)
    {
        // The following will always use the current value of UseFlow, which is == newValue
        SetYAxisVariable();
    }

    [RelayCommand]
    public void ComputeConfidenceLimits()
    {
        if (InputDataProvider.Data.Count == 0)
        {
            return;
        }
        if (CalcdPlotModel == null)
        {
            InitializePlotModel();
        }

        OutputDataProvider = new GraphicalDataProvider(); 

        LoadOutputDataTable(out PairedData upperExceedence, out PairedData centralTendency, out PairedData lowerExceedence);

        CalcdPlotModel.Series.Clear();
        AddLineSeriesToPlot(upperExceedence, true);
        AddLineSeriesToPlot(lowerExceedence, true);
        AddLineSeriesToPlot(centralTendency, false);
        CalcdPlotModel.InvalidatePlot(true);
    }

    private void LoadOutputDataTable(out PairedData upperNonExceedence, out PairedData centralTendency,out PairedData lowerNonExceedence)
    {
        if (OutputDataProvider.Data.Count > 0)
        {
            OutputDataProvider.Data.Clear();
        }

        GraphicalUncertainPairedData graphical = CreateGraphicalUncertainPairedData();
        upperNonExceedence = graphical.SamplePairedData(0.975);
        lowerNonExceedence = graphical.SamplePairedData(.025);
        centralTendency = graphical.SamplePairedData(int.MinValue, true);

        for (int i = 0; i < upperNonExceedence.Xvals.Length; i++)
        {
            GraphicalRow row = new(1 - upperNonExceedence.Xvals[i], centralTendency.Yvals[i], lowerNonExceedence.Yvals[i], upperNonExceedence.Yvals[i], false);
            OutputDataProvider.Data.Add(row);
        }
        OutputDataProvider.LinkList();
    }

    private void InitializePlotModel()
    {
        CalcdPlotModel = new()
        {
            Title = Name
        };
        CalcdPlotModel.Legends.Add(new Legend
        {
            LegendPosition = LegendPosition.BottomRight
        });
        SetAxis();
    }

    private void SetAxis()
    {
        LinearAxis xAxis = new()
        {
            Position = AxisPosition.Bottom,
            Title = StringConstants.EXCEEDANCE_PROBABILITY,
            LabelFormatter = ProbabilityFormatter,
            Maximum = 3.719, //probability of .9999
            Minimum = -3.719, //probability of .0001
            StartPosition = 1,
            EndPosition = 0
        };

        // Create Y axis based on UseFlow setting
        Axis yAxis;
        if (UseFlow)
        {
            yAxis = new LogarithmicAxis()
            {
                Position = AxisPosition.Left,
                Base = 10
            };
        }
        else
        {
            yAxis = new LinearAxis()
            {
                Position = AxisPosition.Left
            };
        }
        
        CalcdPlotModel.Axes.Add(xAxis);
        CalcdPlotModel.Axes.Add(yAxis);
        SetYAxisVariable();
    }

    // Updates the Y axis type and title based on UseFlow setting
    private void SetYAxisVariable()
    {
        if (CalcdPlotModel == null)
        {
            return;
        }
        
        // Remove existing Y axis
        if (CalcdPlotModel.Axes.Count > 1)
        {
            CalcdPlotModel.Axes.RemoveAt(1);
        }
        
        // Create new Y axis based on UseFlow setting
        Axis yAxis;
        if (UseFlow)
        {
            yAxis = new LogarithmicAxis()
            {
                Position = AxisPosition.Left,
                Title = StringConstants.DISCHARGE,
                Base = 10
            };
        }
        else
        {
            yAxis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = StringConstants.STAGE
            };
        }
        
        CalcdPlotModel.Axes.Add(yAxis);
        CalcdPlotModel.InvalidatePlot(true);
    }

    private static string ProbabilityFormatter(double d)
    {
        Normal standardNormal = new(0, 1);
        double value = standardNormal.CDF(d);
        string stringval = value.ToString("0.0000");
        return stringval;
    }

    private void AddLineSeriesToPlot(PairedData function, bool isConfidenceLimit = false)
    {
        LineSeries lineSeries = new()
        {
            TrackerFormatString = "X: {Probability:0.####}, Y: {4:F2} ",
            CanTrackerInterpolatePoints = false
        };

        NormalDataPoint[] points = new NormalDataPoint[function.Xvals.Length];
        for (int i = 0; i < function.Xvals.Length; i++)
        {
            double nonExceedenceProb = 1 - function.Xvals[i];
            double zScore = Normal.StandardNormalInverseCDF(nonExceedenceProb);
            double stageOrFlowVal = function.Yvals[i];
            points[i] = new NormalDataPoint(nonExceedenceProb, zScore, stageOrFlowVal);
        }

        if (isConfidenceLimit) { lineSeries.Color = OxyColors.Blue; lineSeries.LineStyle = LineStyle.Dash; }
        else { lineSeries.Color = OxyColors.Black; }

        lineSeries.ItemsSource = points;
        lineSeries.DataFieldX = nameof(NormalDataPoint.ZScore);
        lineSeries.DataFieldY = nameof(NormalDataPoint.Value);
        CalcdPlotModel.Series.Add(lineSeries);
    }
    public GraphicalUncertainPairedData CreateGraphicalUncertainPairedData()
    {
        CurveMetaData data = new(XLabel, YLabel, Name);
        return new GraphicalUncertainPairedData(InputDataProvider.Xs, InputDataProvider.Ys, EquivalentRecordLength, data, !UseFlow);
    }

    #region Saving and Loading
    //This was a dramatic refactoring of very old code, some of which was saved to disk, so this includes nonsense to be able to forward migrate
    // Backward Compatible Naming Conventions
    private const string GRAPHICALVM = "GraphicalVM";
    private const string TABLEWITHPLOTVM = "TableWithPlotVM";
    private const string DISTRIBUTIONPROVIDERTYPE = "DistributionProviderType";
    private const string NAME = "Name";

    public XElement ToXML()
    {
        XElement ele = new(GRAPHICALVM);
        ele.SetAttributeValue(nameof(EquivalentRecordLength), EquivalentRecordLength);
        ele.SetAttributeValue(nameof(UseFlow), UseFlow);

        UncertainPairedData upd = InputDataProvider.ToUncertainPairedData(XLabel, YLabel, Name, "Unspecified", "Unspecified", "Unspecified");
        XElement child = upd.WriteToXML();
        child.SetAttributeValue(DISTRIBUTIONPROVIDERTYPE, InputDataProvider.GetType().Name);

        ele.Add(child);
        return ele;
    }
    public void LoadFromXML(XElement element)
    {
        if(element.Name.LocalName.Equals(TABLEWITHPLOTVM))
        {
            element = element.Element(GRAPHICALVM);
        }
        EquivalentRecordLength = int.Parse(element.Attribute(nameof(EquivalentRecordLength)).Value);
        UseFlow = bool.Parse(element.Attribute(nameof(UseFlow)).Value);
        Name = element.Attribute(NAME)?.Value;

        XElement updEl = element.Elements().First();
        UncertainPairedData upd = UncertainPairedData.ReadFromXML(updEl);
        XLabel = upd.XLabel;
        YLabel = upd.YLabel;
        Name = upd.Name;

        InputDataProvider.UpdateFromUncertainPairedData(upd);
    }

    /// <summary>
    /// This loads a default GraphicalVM from a ProbabilityFunction Object which is the output of the FDA1.4Import Helper. 
    /// </summary>
    /// <param name="pf"></param>
    public void LoadFromProbabilityFunction(ProbabilityFunction pf)
    {
        Name = StringConstants.GRAPHICAL_FREQUENCY;
        XLabel = StringConstants.EXCEEDANCE_PROBABILITY;
        double[] probs = pf.ExceedanceProbability;
        double[] ys;
        if (pf.ProbabilityDataTypeId == ProbabilityFunction.ProbabilityDataType.DISCHARGE_FREQUENCY)
        {
            ys = pf.Discharge;
            UseFlow = true; //This will also set the YLabel
        }
        else
        {
            ys = pf.Stage;
            UseFlow = false;//This will also set the YLabel
        }
        IDistribution[] distYs = ys.Select(y => new Deterministic(y)).ToArray();
        UncertainPairedData upd = new(probs, distYs, new CurveMetaData(XLabel, YLabel, Name));
        InputDataProvider.UpdateFromUncertainPairedData(upd);

        EquivalentRecordLength = pf.EquivalentLengthOfRecord;
    }
    #endregion
}

