using System;
namespace metrics
{
    public interface IContainResults
    {
        double AEPThreshold{get; set;}
        void AddEADEstimate(double eadEstimate);
        void AddAEPEstimate(double aepEstimate);

    }
}