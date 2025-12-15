using System;
using System.Linq;
using System.Text;

namespace Statistics.Histograms
{
    /// <summary>
    /// Quick debugging helper to visualize histogram data as ASCII charts in the console.
    /// Useful for rapid debugging during Monte Carlo computations.
    /// </summary>
    public static class HistogramDebugVisualizer
    {
        /// <summary>
        /// Prints the histogram as an ASCII bar chart to the console.
        /// </summary>
        /// <param name="histogram">The histogram to visualize.</param>
        /// <param name="title">Optional title for the chart. Default is "Histogram".</param>
        /// <param name="maxWidth">Maximum width of the chart in characters. Default is 80.</param>
        /// <param name="maxBins">Maximum number of bins to display. If histogram has more bins, it will be downsampled. Default is 30.</param>
        public static void PrintToConsole(this IHistogram histogram, string title = "Histogram", int maxWidth = 80, int maxBins = 30)
        {
            string chart = CreateAsciiChart(histogram, title, maxWidth, maxBins);
            Console.WriteLine(chart);
        }

        /// <summary>
        /// Returns the histogram as an ASCII bar chart string.
        /// </summary>
        /// <param name="histogram">The histogram to visualize.</param>
        /// <param name="title">Optional title for the chart. Default is "Histogram".</param>
        /// <param name="maxWidth">Maximum width of the chart in characters. Default is 80.</param>
        /// <param name="maxBins">Maximum number of bins to display. If histogram has more bins, it will be downsampled. Default is 30.</param>
        /// <returns>ASCII chart as a string.</returns>
        public static string CreateAsciiChart(this IHistogram histogram, string title = "Histogram", int maxWidth = 80, int maxBins = 30)
        {
            if (histogram == null)
            {
                throw new ArgumentNullException(nameof(histogram));
            }

            if (maxWidth < 20)
            {
                throw new ArgumentException("Max width must be at least 20 characters.", nameof(maxWidth));
            }

            if (maxBins < 1)
            {
                throw new ArgumentException("Max bins must be at least 1.", nameof(maxBins));
            }

            StringBuilder sb = new StringBuilder();

            // Title and header
            sb.AppendLine();
            sb.AppendLine(new string('=', maxWidth));
            sb.AppendLine(CenterText(title, maxWidth));
            sb.AppendLine(new string('=', maxWidth));
            sb.AppendLine();

            // Statistics summary
            sb.AppendLine($"  Sample Size: {histogram.SampleSize:N0}");
            sb.AppendLine($"  Mean:        {histogram.SampleMean:F4}");
            sb.AppendLine($"  Std Dev:     {histogram.StandardDeviation:F4}");
            sb.AppendLine($"  Min:         {histogram.Min:F4}");
            sb.AppendLine($"  Max:         {histogram.Max:F4}");
            sb.AppendLine($"  Bins:        {histogram.BinCounts.Length}");
            sb.AppendLine($"  Bin Width:   {histogram.BinWidth:F4}");
            if (histogram.IsConverged)
            {
                sb.AppendLine($"  Converged:   Yes (iteration {histogram.ConvergedIteration})");
            }
            else
            {
                sb.AppendLine($"  Converged:   No");
            }
            sb.AppendLine();

            // Get bin data
            long[] binCounts = histogram.BinCounts;
            if (binCounts == null || binCounts.Length == 0)
            {
                sb.AppendLine("  No data to display.");
                sb.AppendLine(new string('=', maxWidth));
                return sb.ToString();
            }

            // Downsample if needed
            long[] displayBins;
            double[] displayBinCenters;
            if (binCounts.Length > maxBins)
            {
                (displayBins, displayBinCenters) = DownsampleBins(histogram, maxBins);
                sb.AppendLine($"  (Downsampled to {maxBins} bins for display)");
                sb.AppendLine();
            }
            else
            {
                displayBins = binCounts;
                displayBinCenters = GetBinCenters(histogram);
            }

            // Calculate chart dimensions
            long maxCount = displayBins.Max();
            if (maxCount == 0)
            {
                sb.AppendLine("  All bins are empty.");
                sb.AppendLine(new string('=', maxWidth));
                return sb.ToString();
            }

            // Reserve space for labels (bin center value + count + spacing)
            int labelWidth = 25; // Space for "123456.12 |" and " | 123456"
            int chartWidth = Math.Max(maxWidth - labelWidth, 20);

            // Draw bars
            for (int i = 0; i < displayBins.Length; i++)
            {
                double binCenter = displayBinCenters[i];
                long count = displayBins[i];

                // Calculate bar width
                int barWidth = maxCount > 0 ? (int)Math.Round((double)count / maxCount * chartWidth) : 0;

                // Format: "  123.45 | ████████████ | 1234"
                string bar = new string('█', barWidth);
                string label = $"{binCenter,10:F2} |";
                string countStr = $"| {count,6}";

                sb.Append($"  {label} ");
                sb.Append(bar.PadRight(chartWidth));
                sb.AppendLine($" {countStr}");
            }

            sb.AppendLine();
            sb.AppendLine(new string('=', maxWidth));

            return sb.ToString();
        }

        /// <summary>
        /// Prints a comparison of multiple histograms side by side.
        /// </summary>
        /// <param name="histograms">Array of histograms to compare.</param>
        /// <param name="labels">Labels for each histogram.</param>
        /// <param name="maxWidth">Maximum width of each chart. Default is 60.</param>
        public static void PrintComparison(IHistogram[] histograms, string[] labels, int maxWidth = 60)
        {
            if (histograms == null || histograms.Length == 0)
            {
                throw new ArgumentException("Must provide at least one histogram.", nameof(histograms));
            }

            if (labels != null && labels.Length != histograms.Length)
            {
                throw new ArgumentException("Number of labels must match number of histograms.", nameof(labels));
            }

            int headerWidth = maxWidth * Math.Min(histograms.Length, 2);
            Console.WriteLine();
            Console.WriteLine(new string('=', headerWidth));
            Console.WriteLine(CenterText("HISTOGRAM COMPARISON", headerWidth));
            Console.WriteLine(new string('=', headerWidth));
            Console.WriteLine();

            for (int i = 0; i < histograms.Length; i++)
            {
                string title = labels != null && i < labels.Length ? labels[i] : $"Histogram {i + 1}";
                histograms[i].PrintToConsole(title, maxWidth, 20);
            }
        }

        /// <summary>
        /// Prints a summary table of statistics for multiple histograms.
        /// </summary>
        /// <param name="histograms">Array of histograms to summarize.</param>
        /// <param name="labels">Labels for each histogram.</param>
        public static void PrintSummaryTable(IHistogram[] histograms, string[] labels)
        {
            if (histograms == null || histograms.Length == 0)
            {
                throw new ArgumentException("Must provide at least one histogram.", nameof(histograms));
            }

            if (labels != null && labels.Length != histograms.Length)
            {
                throw new ArgumentException("Number of labels must match number of histograms.", nameof(labels));
            }

            Console.WriteLine();
            Console.WriteLine(new string('=', 120));
            Console.WriteLine("HISTOGRAM SUMMARY");
            Console.WriteLine(new string('=', 120));
            Console.WriteLine();

            // Header
            Console.WriteLine($"{"Name",-20} {"Sample Size",15} {"Mean",12} {"Std Dev",12} {"Min",12} {"Max",12} {"Conv?",8}");
            Console.WriteLine(new string('-', 120));

            // Data rows
            for (int i = 0; i < histograms.Length; i++)
            {
                IHistogram h = histograms[i];
                string name = labels != null && i < labels.Length ? labels[i] : $"Histogram {i + 1}";
                string converged = h.IsConverged ? "Yes" : "No";

                Console.WriteLine($"{name,-20} {h.SampleSize,15:N0} {h.SampleMean,12:F4} {h.StandardDeviation,12:F4} {h.Min,12:F4} {h.Max,12:F4} {converged,8}");
            }

            Console.WriteLine(new string('=', 120));
            Console.WriteLine();
        }

        #region Private Helper Methods

        private static string CenterText(string text, int width)
        {
            if (text.Length >= width)
                return text.Substring(0, width);

            int leftPadding = (width - text.Length) / 2;
            return text.PadLeft(leftPadding + text.Length).PadRight(width);
        }

        private static double[] GetBinCenters(IHistogram histogram)
        {
            int numBins = histogram.BinCounts.Length;
            double[] centers = new double[numBins];

            for (int i = 0; i < numBins; i++)
            {
                centers[i] = histogram.Min + (i + 0.5) * histogram.BinWidth;
            }

            return centers;
        }

        private static (long[] bins, double[] centers) DownsampleBins(IHistogram histogram, int targetBins)
        {
            long[] originalBins = histogram.BinCounts;
            int originalCount = originalBins.Length;

            if (originalCount <= targetBins)
            {
                return (originalBins, GetBinCenters(histogram));
            }

            long[] downsampledBins = new long[targetBins];
            double[] downsampledCenters = new double[targetBins];

            // Calculate how many original bins per downsampled bin
            double binsPerTarget = (double)originalCount / targetBins;

            for (int i = 0; i < targetBins; i++)
            {
                int startIdx = (int)Math.Floor(i * binsPerTarget);
                int endIdx = (int)Math.Floor((i + 1) * binsPerTarget);

                // Sum the bins in this range
                long sum = 0;
                double centerSum = 0;
                int count = 0;

                for (int j = startIdx; j < endIdx && j < originalCount; j++)
                {
                    sum += originalBins[j];
                    centerSum += histogram.Min + (j + 0.5) * histogram.BinWidth;
                    count++;
                }

                downsampledBins[i] = sum;
                downsampledCenters[i] = count > 0 ? centerSum / count : histogram.Min + (startIdx + 0.5) * histogram.BinWidth;
            }

            return (downsampledBins, downsampledCenters);
        }

        #endregion
    }
}
