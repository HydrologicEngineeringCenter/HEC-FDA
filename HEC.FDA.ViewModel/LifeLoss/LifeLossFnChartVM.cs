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
    public LifeLossFnChartVM(UncertainPairedData data, string title = "Life Loss Function")
    {
        _title = title;
        InitializePlotModel();
        AddDataSeries(data);
    }

    /// <summary>
    /// Creates a LifeLossFnChartVM without initial data (zone lines only).
    /// </summary>
    /// <param name="title">Chart title</param>
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
        AddZoneRegions();
        AddZoneSeries();
    }

    private void AddAxes()
    {
        var xAxis = new LogarithmicAxis
        {
            Position = AxisPosition.Bottom,
            Title = "Average Life Loss",
            Minimum = 0.1,
            Maximum = 10000,
            AbsoluteMinimum = 0.1,
            AbsoluteMaximum = 10000,
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
            Minimum = 1E-07,
            Maximum = 1,
            AbsoluteMinimum = 1E-07,
            AbsoluteMaximum = 1,
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

    private void AddZoneRegions()
    {
        // Define key values
        double xMin = 0.1;
        double xMax = 10000;
        double yMin = 1E-07;
        double yMax = 1;

        // Define colors (matching reference image, semi-transparent)
        var greenColor = OxyColor.FromAColor(150, OxyColor.FromRgb(209, 226, 175));   // Sage/olive green from reference
        var yellowColor = OxyColor.FromAColor(150, OxyColor.FromRgb(255, 242, 175));  // Pale yellow from reference
        var orangeColor = OxyColor.FromAColor(150, OxyColor.FromRgb(244, 204, 158));  // Peach/orange from reference
        var blueColor = OxyColor.FromAColor(150, OxyColor.FromRgb(180, 210, 230));    // Light blue/teal from reference


        // The societal line defines the boundary:
        // Points: (0.1, 1E-02) -> (10, 1E-04) -> (1000, 1E-06) -> (10000, 1E-06)
        // Orange zone: Above the societal line
        var orangeZone = new PolygonAnnotation
        {
            Fill = orangeColor,
            StrokeThickness = 0,
            Layer = AnnotationLayer.BelowSeries
        };
        // Start at societal line and go up to top of chart
        orangeZone.Points.Add(new DataPoint(0.1, 1E-02));
        orangeZone.Points.Add(new DataPoint(10, 1E-04));
        orangeZone.Points.Add(new DataPoint(1000, 1E-06));
        orangeZone.Points.Add(new DataPoint(10000, 1E-06));
        orangeZone.Points.Add(new DataPoint(xMax, yMax));
        orangeZone.Points.Add(new DataPoint(xMin, yMax));
        PlotModel.Annotations.Add(orangeZone);

        // Yellow zone: Triangle between (0.1, 1E-02) -> (10, 1E-04) -> (0.1, 1E-04)
        var yellowZone = new PolygonAnnotation
        {
            Fill = yellowColor,
            StrokeThickness = 0,
            Layer = AnnotationLayer.BelowSeries
        };
        yellowZone.Points.Add(new DataPoint(0.1, 1E-02));
        yellowZone.Points.Add(new DataPoint(10, 1E-04));
        yellowZone.Points.Add(new DataPoint(0.1, 1E-04));
        PlotModel.Annotations.Add(yellowZone);

        // Green zone: Below the societal line
        var greenZone = new PolygonAnnotation
        {
            Fill = greenColor,
            StrokeThickness = 0,
            Layer = AnnotationLayer.BelowSeries
        };
        greenZone.Points.Add(new DataPoint(0.1, 1E-04));
        greenZone.Points.Add(new DataPoint(10, 1E-04));
        greenZone.Points.Add(new DataPoint(1000, 1E-06));
        greenZone.Points.Add(new DataPoint(10000, 1E-06));
        greenZone.Points.Add(new DataPoint(xMax, yMin));
        greenZone.Points.Add(new DataPoint(xMin, yMin));
        PlotModel.Annotations.Add(greenZone);

        // Blue zone: Rectangle with upper left at (1000, 1E-06) and lower right at (10000, 1E-07)
        var blueZone = new PolygonAnnotation
        {
            Fill = blueColor,
            StrokeThickness = 0,
            Layer = AnnotationLayer.BelowSeries
        };
        blueZone.Points.Add(new DataPoint(1000, 1E-06));
        blueZone.Points.Add(new DataPoint(10000, 1E-06));
        blueZone.Points.Add(new DataPoint(10000, 1E-07));
        blueZone.Points.Add(new DataPoint(1000, 1E-07));
        PlotModel.Annotations.Add(blueZone);
    }

    private void AddZoneSeries()
    {
        // Individual Life Risk Line (horizontal at 1E-04)
        var individualRiskLine = new LineSeries
        {
            Title = "Individual Life Risk Line",
            LineStyle = LineStyle.Dash,
            Color = OxyColors.Red,
            StrokeThickness = 3
        };
        individualRiskLine.Points.Add(new DataPoint(0.1, 1E-04));
        individualRiskLine.Points.Add(new DataPoint(10000, 1E-04));
        PlotModel.Series.Add(individualRiskLine);

        // Societal Life Risk Line (diagonal)
        var societalRiskLine = new LineSeries
        {
            Title = "Societal Life Risk Line",
            LineStyle = LineStyle.Dash,
            Color = OxyColors.Blue,
            StrokeThickness = 3
        };
        societalRiskLine.Points.Add(new DataPoint(0.1, 1E-02));
        societalRiskLine.Points.Add(new DataPoint(1000, 1E-06));
        PlotModel.Series.Add(societalRiskLine);
    }

    /// <summary>
    /// Adds min/median/max data series from UncertainPairedData.
    /// </summary>
    /// <param name="data">Pre-transformed UncertainPairedData where X = Average Life Loss, Y = AEP distribution</param>
    public void AddDataSeries(UncertainPairedData data)
    {
        if (data == null || data.IsNull) return;

        var lower = data.SamplePairedData(0.025);
        var median = data.SamplePairedData(0.5);
        var upper = data.SamplePairedData(0.975);

        AddLineSeries(lower, "2.5 Percentile", isConfidenceLimit: true);
        AddLineSeries(median, "Median", isConfidenceLimit: false);
        AddLineSeries(upper, "97.5 Percentile", isConfidenceLimit: true);

        PlotModel.InvalidatePlot(true);
    }

    private void AddLineSeries(PairedData function, string title, bool isConfidenceLimit)
    {
        var lineSeries = new LineSeries
        {
            Title = title,
            LineStyle = isConfidenceLimit ? LineStyle.Dash : LineStyle.Solid,
            Color = isConfidenceLimit ? OxyColors.ForestGreen : OxyColors.Black,
            StrokeThickness = isConfidenceLimit ? 1.5 : 2.0
        };

        for (int i = 0; i < function.Xvals.Length; i++)
        {
            double lifeLoss = function.Xvals[i];
            double aep = function.Yvals[i];

            // Only add valid points (positive values for log scale)
            if (lifeLoss > 0 && aep > 0)
            {
                lineSeries.Points.Add(new DataPoint(lifeLoss, aep));
            }
        }

        PlotModel.Series.Add(lineSeries);
    }

    /// <summary>
    /// Updates the chart with new data, clearing existing data series but keeping zone lines.
    /// </summary>
    /// <param name="data">Pre-transformed UncertainPairedData where X = Average Life Loss, Y = AEP distribution</param>
    public void UpdateData(UncertainPairedData data)
    {
        // Remove data series but keep zone boundary lines (first 2 series)
        while (PlotModel.Series.Count > 2)
        {
            PlotModel.Series.RemoveAt(PlotModel.Series.Count - 1);
        }

        AddDataSeries(data);
    }

    /// <summary>
    /// Clears all data series, keeping only the zone boundary lines.
    /// </summary>
    public void ClearDataSeries()
    {
        while (PlotModel.Series.Count > 2)
        {
            PlotModel.Series.RemoveAt(PlotModel.Series.Count - 1);
        }
        PlotModel.InvalidatePlot(true);
    }
}
