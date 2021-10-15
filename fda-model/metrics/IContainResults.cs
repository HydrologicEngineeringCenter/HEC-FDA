using System;
namespace metrics
{
    public interface IContainResults
    {
        double AEPThreshold{get; set;}
        void AddEADEstimate(double eadEstimate, string category);
        void AddAEPEstimate(double aepEstimate);

    }
}