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

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;

public partial class NewGraphicalVM : ObservableObject
{
    [ObservableProperty]
    int _equivalentRecordLength;

    [ObservableProperty]
    bool _useFlow;

    [ObservableProperty]
    string _xLabel;

    [ObservableProperty]
    string _yLabel;

    [ObservableProperty]
    string _name;

    [ObservableProperty]
    DeterministicDataProvider _inputDataProvider;

    [ObservableProperty]
    GraphicalDataProvider _outputDataProvider;

    [ObservableProperty]
    ViewResolvingPlotModel _calcdPlotModel;

    public NewGraphicalVM(string name, string xlabel, string ylabel)
    {
        Name = name;
        XLabel = xlabel;
        YLabel = ylabel;
        UseFlow = true;
    }
    public NewGraphicalVM(XElement vmEle)
    {
        LoadFromXML(vmEle);
    }
    public NewGraphicalVM(ProbabilityFunction probabilityFunction)
    {
        LoadFromProbabilityFunction(probabilityFunction);
    }


    [RelayCommand]
    public void ComputeConfidenceLimits()
    {
        if (CalcdPlotModel == null)
        {
            InitializePlotModel();
        }

        GraphicalUncertainPairedData graphical = GraphicalUncertainPairedData;
        OutputDataProvider = new GraphicalDataProvider(UseFlow);

        PairedData upperNonExceedence = graphical.SamplePairedData(0.975);
        PairedData lowerNonExceedence = graphical.SamplePairedData(.025);
        PairedData centralTendency = graphical.SamplePairedData(int.MinValue, true);

        CalcdPlotModel.Series.Clear();
        AddLineSeriesToPlot(upperNonExceedence, true);
        AddLineSeriesToPlot(lowerNonExceedence, true);
        AddLineSeriesToPlot(centralTendency, true);
    }

    private void InitializePlotModel()
    {
        CalcdPlotModel = new();
        CalcdPlotModel.Title = Name;
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
        LinearAxis yAxis = new()
        {
            Position = AxisPosition.Left,
            Title = StringConstants.DISCHARGE,
            Unit = "cfs",
        };
        CalcdPlotModel.Axes.Add(xAxis);
        CalcdPlotModel.Axes.Add(yAxis);
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
            double zScore = Normal.StandardNormalInverseCDF(function.Xvals[i]);
            double stageOrFlowVal = function.Yvals[i];
            points[i] = new NormalDataPoint(function.Xvals[i], zScore, stageOrFlowVal);
        }

        if (isConfidenceLimit) { lineSeries.Color = OxyColors.Blue; lineSeries.LineStyle = LineStyle.Dash; }
        else { lineSeries.Color = OxyColors.Black; }

        lineSeries.ItemsSource = points;
        lineSeries.DataFieldX = nameof(NormalDataPoint.ZScore);
        lineSeries.DataFieldY = nameof(NormalDataPoint.Value);
        CalcdPlotModel.Series.Add(lineSeries);
    }

    public GraphicalUncertainPairedData GraphicalUncertainPairedData
    {
        get { return new GraphicalUncertainPairedData(InputDataProvider.Xs, InputDataProvider.Ys, EquivalentRecordLength, new CurveMetaData(XLabel, YLabel, Name), !UseFlow); }

    }

    #region Saving and Loading
    //This was a dramatic refactoring of very old code, some of which was saved to disk, so this includes nonsense to be able to forward migrate
    // Backward Compatible Naming Conventions
    private const string GRAPHICALVM = "GraphicalVM";
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

