using System.Collections.Generic;
using System.Linq;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace HEC.FDA.ViewModel.LifeLoss;

/// <summary>
/// ViewModel for the Life Loss Function chart that displays life loss data
/// with min/median/max series and zone boundary lines.
/// </summary>
public class LifeLossFnChartVM : BaseViewModel
{
    private const string DataSeriesTag = "DataSeries";

    private string _title = "Life Loss Function";

    public ViewResolvingPlotModel PlotModel { get; } = new();

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            PlotModel.Title = value;
            NotifyPropertyChanged();
        }
    }


    /// <summary>
    /// Creates a LifeLossFnChartVM with initial data.
    /// </summary>
    /// <param name="data">Pre-transformed UncertainPairedData where X = Average Life Loss, Y = AEP distribution</param>
    /// <param name="title">Chart title</param>
    /// <param name="showZones">Whether to display zone regions and boundary lines</param>
    public LifeLossFnChartVM(UncertainPairedData data, string title = "Life Loss Function"): this(title)
    {
        AddDataSeries(data);
    }

    /// <summary>
    /// Creates a LifeLossFnChartVM without initial data.
    /// </summary>
    /// <param name="title">Chart title</param>
    /// <param name="showZones">Whether to display zone regions and boundary lines</param>
    public LifeLossFnChartVM(string title = "Life Loss Function")
    {
        _title = title;
        InitializePlotModel();
    }


    private void InitializePlotModel()
    {
        PlotModel.Title = Title;
        PlotModel.Legends.Add(new OxyPlot.Legends.Legend
        {
            LegendPosition = OxyPlot.Legends.LegendPosition.TopRight,
            LegendPlacement = OxyPlot.Legends.LegendPlacement.Inside
        });
        AddAxes();
    }

    private void AddAxes()
    {
        var xAxis = new LogarithmicAxis
        {
            Position = AxisPosition.Bottom,
            Title = "Average Annual Life Loss",
            Base = 10,
            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = OxyColor.FromRgb(200, 200, 200),
            MinorTickSize = 0,
            MinorGridlineStyle = LineStyle.None,
            IntervalLength = 100
        };

        var yAxis = new LogarithmicAxis
        {
            Position = AxisPosition.Left,
            Title = "Annual Exceedance Probability",
            Base = 10,
            StartPosition = 0,
            EndPosition = 1,
            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = OxyColor.FromRgb(200, 200, 200),
            MinorTickSize = 0,
            MinorGridlineStyle = LineStyle.None,
            StringFormat = "0.0E+00"
        };

        PlotModel.Axes.Add(xAxis);
        PlotModel.Axes.Add(yAxis);
    }

    /// <summary>
    /// Adds min/median/max data series from CategoriedUncertainPairedData.
    /// </summary>
    /// <param name="data">CategoriedUncertainPairedData where Xvals = AEP and YHistograms = life loss distributions</param>
    public void AddDataSeries(UncertainPairedData data)
    {
        if (data == null) return;

        // Get quantile curves - these have X = AEP, Y = life loss
        var lower = data.SamplePairedData(0.025);
        var median = data.SamplePairedData(0.5);
        var upper = data.SamplePairedData(0.975);

        // Swap X and Y for the chart (chart expects X = life loss, Y = AEP)
        AddLineSeriesSwapped(lower, data.Xvals, "2.5 Percentile", isConfidenceLimit: true);
        AddLineSeriesSwapped(median, data.Xvals, "Median", isConfidenceLimit: false);
        AddLineSeriesSwapped(upper, data.Xvals, "97.5 Percentile", isConfidenceLimit: true);

        PlotModel.InvalidatePlot(true);
    }

    private void AddLineSeriesSwapped(PairedData function, IReadOnlyList<double> aepValues, string title, bool isConfidenceLimit)
    {
        var lineSeries = new LineSeries
        {
            Title = title,
            LineStyle = isConfidenceLimit ? LineStyle.Dash : LineStyle.Solid,
            Color = isConfidenceLimit ? OxyColors.ForestGreen : OxyColors.Black,
            StrokeThickness = isConfidenceLimit ? 1.5 : 2.0,
            Tag = DataSeriesTag
        };

        // function.Xvals = AEP (from original data), function.Yvals = life loss (quantile values)
        // We want chart X = life loss, chart Y = AEP
        for (int i = 0; i < function.Yvals.Count; i++)
        {
            double lifeLoss = function.Yvals[i];  // Y values are life loss
            double aep = aepValues[i];// X values are AEP
            lineSeries.Points.Add(new DataPoint(lifeLoss,1-aep)); //exceedence to non exceedence. 

        }

        PlotModel.Series.Add(lineSeries);
    }

    /// <summary>
    /// Updates the chart with new data, clearing existing data series but keeping zone lines.
    /// </summary>
    /// <param name="data">Pre-transformed UncertainPairedData where X = Average Life Loss, Y = AEP distribution</param>
    public void UpdateData(UncertainPairedData data)
    {
        RemoveDataSeries();
        AddDataSeries(data);
    }

    /// <summary>
    /// Clears all data series, keeping only the zone boundary lines.
    /// </summary>
    public void ClearDataSeries()
    {
        RemoveDataSeries();
        PlotModel.InvalidatePlot(true);
    }

    private void RemoveDataSeries()
    {
        var dataSeries = PlotModel.Series.Where(s => DataSeriesTag.Equals(s.Tag)).ToList();
        foreach (var series in dataSeries)
        {
            PlotModel.Series.Remove(series);
        }
    }
}
