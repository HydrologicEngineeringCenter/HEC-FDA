using System;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Interfaces;
namespace Statistics.Histograms
{
    public interface IHistogram:  IDistribution 
    {
        #region Properties 
        bool IsConverged { get; }
        bool HistogramIsZeroValued { get; }
        bool HistogramIsSingleValued { get; }
        Int64 ConvergedIteration { get; }
        double BinWidth { get; }
        Int64[] BinCounts { get; }
        double Min { get;  }
        double Max { get;  }
        double Mean { get; }
        double Variance { get; }
        double StandardDeviation { get; }
        ConvergenceCriteria ConvergenceCriteria { get; }
        string TypeOfIHistogram { get; }


        #endregion

        #region Methods
        void AddObservationToHistogram(double observation, Int64 iterationIndex);
        void AddObservationsToHistogram(double[] observations);
        void ForceDeQueue();
        bool IsHistogramConverged(double upperq, double lowerq);
        Int64 EstimateIterationsRemaining(double upperq, double lowerq);
        Int64 FindBinCount(double x, bool cumulative = true);
        #endregion
    }
}
