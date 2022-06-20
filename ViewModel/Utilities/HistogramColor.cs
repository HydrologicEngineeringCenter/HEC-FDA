using HEC.Plotting.SciChart2D.DataModel;
using System.Windows.Media;

namespace HEC.FDA.ViewModel.Utilities
{
    internal static class HistogramColor
    {

        internal static void SetHistogramColor(HistogramData2D histogram)
        {
            histogram.StrokeColor = Colors.Blue;
            histogram.FillColor = Colors.Blue;
        }

    }
}
