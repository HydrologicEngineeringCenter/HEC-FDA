using System;
using HEC.FDA.Statistics.Convergence;
using HEC.FDA.Statistics.Distributions;
using HEC.MVVMFramework.Base.Interfaces;

namespace HEC.FDA.Statistics.Histograms
{
    public interface IHistogram : IReportMessage, IDistribution
    {
        #region Properties 
        bool IsConverged { get; }
        bool HistogramIsZeroValued { get; set; }
        bool HistogramIsSingleValued { get; }
        long ConvergedIteration { get; }
        double BinWidth { get; }
        long[] BinCounts { get; }
        double Min { get; }
        double Max { get; }
        double Mean { get; }
        double Variance { get; }
        double StandardDeviation { get; }
        ConvergenceCriteria ConvergenceCriteria { get; }
        string TypeOfIHistogram { get; }


        #endregion

        #region Methods
        void AddObservationToHistogram(double observation, long iterationIndex);
        void ForceDeQueue();
        bool IsHistogramConverged(double upperq, double lowerq);
        long EstimateIterationsRemaining(double upperq, double lowerq);
        long FindBinCount(double x, bool cumulative = true);
        #endregion
    }
}
