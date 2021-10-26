using System;
namespace metrics
{
    public interface IContainResults
    {
        double AEPThreshold{get; set;}
        void AddEADEstimate(double eadEstimate, string category);
        void AddAEPEstimate(double aepEstimate);
        void AddStageForCNEP(double standardProbability, double stageForCNEP);
        double MeanEAD(string category);
        double MeanAEP();
        double MedianAEP();
        double[] AssuranceOfAEP();
        double[] EADExceededWithProbability(string category);
        double[] LongTermRisk();
    }
}