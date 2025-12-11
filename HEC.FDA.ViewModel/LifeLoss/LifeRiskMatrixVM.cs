using HEC.FDA.Model.paireddata;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace HEC.FDA.ViewModel.LifeLoss;

/// <summary>
/// ViewModel for the Life Risk Matrix chart that displays life loss data
/// with min/median/max series and zone boundary lines.
/// </summary>
public class LifeRiskMatrixVM : BaseViewModel
{
    private string _title = "Life Risk Matrix";

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
    /// Creates a LifeRiskMatrixVM with initial data.
    /// </summary>
    /// <param name="data">Pre-transformed UncertainPairedData where X = Average Life Loss, Y = AEP distribution</param>
    /// <param name="title">Chart title</param>
    public LifeRiskMatrixVM(UncertainPairedData data, string title = "Life Risk Matrix")
    {
        _title = title;
        InitializePlotModel();
        AddDataSeries(data);
    }

    /// <summary>
    /// Creates a LifeRiskMatrixVM without initial data (zone lines only).
    /// </summary>
    /// <param name="title">Chart title</param>
    public LifeRiskMatrixVM(string title = "Life Risk Matrix")
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

    private void AddZoneSeries()
    {
        // Individual Life Risk Line (horizontal at 1E-04)
        var individualRiskLine = new LineSeries
        {
            Title = "Individual Life Risk Line",
            LineStyle = LineStyle.Dash,
            Color = OxyColors.Red
        };
        individualRiskLine.Points.Add(new DataPoint(0.1, 1E-04));
        individualRiskLine.Points.Add(new DataPoint(10000, 1E-04));
        PlotModel.Series.Add(individualRiskLine);

        // Societal Life Risk Line (diagonal)
        // Line starts at (0.1, 1E-02) and maintains constant societal risk: AEP * LifeLoss = 0.001
        var societalRiskLine = new LineSeries
        {
            Title = "Societal Life Risk Line",
            LineStyle = LineStyle.Dash,
            Color = OxyColors.Blue
        };
        societalRiskLine.Points.Add(new DataPoint(0.1, 1E-02));
        societalRiskLine.Points.Add(new DataPoint(10000, 1E-07));
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

        AddLineSeries(lower, "Min (2.5%)", isConfidenceLimit: true);
        AddLineSeries(median, "Median", isConfidenceLimit: false);
        AddLineSeries(upper, "Max (97.5%)", isConfidenceLimit: true);

        PlotModel.InvalidatePlot(true);
    }

    private void AddLineSeries(PairedData function, string title, bool isConfidenceLimit)
    {
        var lineSeries = new LineSeries
        {
            Title = title,
            LineStyle = isConfidenceLimit ? LineStyle.Dash : LineStyle.Solid,
            Color = isConfidenceLimit ? OxyColors.Blue : OxyColors.Black,
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
