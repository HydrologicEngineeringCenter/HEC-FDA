using System;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Interfaces;
namespace Statistics.Histograms
{
    public interface IHistogram: IReportMessage 
    {
        #region Properties 
        bool IsConverged { get; }
        bool HistogramIsZeroValued { get; set; }
        int ConvergedIteration { get; }
        double BinWidth { get; }
        Int32[] BinCounts { get; }
        double Min { get;  }
        double Max { get;  }
        double Mean { get; }
        double Variance { get; }
        double StandardDeviation { get; }
        int SampleSize { get; }
        ConvergenceCriteria ConvergenceCriteria { get; }
        string MyType { get; }


        #endregion

        #region Methods
        double PDF(double x);
        double CDF(double x);
        double InverseCDF(double p);
        void AddObservationToHistogram(double observation, int iterationIndex);
        void ForceDeQueue();
        XElement WriteToXML();
        bool IsHistogramConverged(double upperq, double lowerq);
        int EstimateIterationsRemaining(double upperq, double lowerq);
        bool Equals(IHistogram histogramForComparison);
        int FindBinCount(double x, bool cumulative = true);
        #endregion
    }
}
