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
        bool HistogramIsSingleValued { get; }
        Int64 ConvergedIteration { get; }
        double BinWidth { get; }
        Int64[] BinCounts { get; }
        double Min { get;  }
        double Max { get;  }
        double Mean { get; }
        double Variance { get; }
        double StandardDeviation { get; }
        Int64 SampleSize { get; }
        ConvergenceCriteria ConvergenceCriteria { get; }
        string MyType { get; }


        #endregion

        #region Methods
        double PDF(double x);
        double CDF(double x);
        double InverseCDF(double p);
        void AddObservationToHistogram(double observation, Int64 iterationIndex);
        void ForceDeQueue();
        XElement WriteToXML();
        bool IsHistogramConverged(double upperq, double lowerq);
        Int64 EstimateIterationsRemaining(double upperq, double lowerq);
        bool Equals(IHistogram histogramForComparison);
        Int64 FindBinCount(double x, bool cumulative = true);
        #endregion
    }
}
