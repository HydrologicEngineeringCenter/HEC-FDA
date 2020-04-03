using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics.Histograms
{
    public interface IHistogramBuilder
    {
        int BinCount { get; }
        int SampleSize { get; }
        double BinWidth { get; }
        List<IBin> Bins { get; }
        IRange<double> Range { get; }

        IHistogramBuilder InitializeWithDataAndWidth(IEnumerable<double> data, double width);
        IHistogramBuilder InitializeWithBinsCountWidthAndMin(int nBins, double min, double width);
        IHistogramBuilder InitializeWithBinsCountAndRange(int nBins, double min, double max);
        
        

        //void InitializeWithDataAndBinsCount(IEnumerable<double> data, int nBins);
        //void InitializeWithBetaDistributionAndWidth(IEnumerable<double> data, double width);
        //void InitializeWithBetaDistributionAndBinsCount(IEnumerable<double> data, int nBins);




        //void AddData(IEnumerable<double> data);
        //void AddData(double data);

        //void AddBinCount(int nBins);
        //void AddBinWidth(double binWidth);
        //void AddHistogramRange(double min, double max);
        //void BuildFromDataRange(IEnumerable<double> data);
        //void BuildFromBetaDistribution(IEnumerable<double> data);

        //IHistogram Build();
    }
}
