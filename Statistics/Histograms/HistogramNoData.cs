using Statistics.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Statistics.Histograms
{
    internal class HistogramNoData: Histogram
    {
        #region Construtors      
        internal HistogramNoData(double min, double max, double width): base(InitializeBins(new Tuple<double, double>(min, max), width))
        {
        }
        #endregion
        private static IBin[] InitializeBins(Tuple<double, double> range, double width)
        {
            if (!HistogramValidator.IsConstructable(range.Item1, range.Item2, width, out IList<string> errors)) throw new InvalidConstructorArgumentsException(errors);
            else return InitializeEmptyBins(min: range.Item1, width, Increments(range.Item2 - range.Item1, width, Math.Ceiling)).ToArray();
        }

        public override double PDF(double x) => 0;
        public override double CDF(double x) => 0;
        public override double InverseCDF(double p) => throw new InvalidOperationException("The InverseCDF function cannot be performed because the histogram contains 0 binned observations.");
        public override double Sample(Random r = null) => throw new InvalidOperationException("The Sample function cannot be performed because the histogram contains 0 binned observations.");
        public override double[] Sample(int n, Random r = null) => throw new InvalidOperationException("The Sample function cannot be performed because the histogram contains 0 binned observations.");
        public override IDistribution SampleDistribution(Random r) => this;       
    }
    


    
}
