using System;
namespace metrics
{
    public interface IContainResults
    {

        void AddEADEstimate(double eadEstimate, string category);

        double MeanEAD(string category);

        double EADExceededWithProbabilityQ(string category, double exceedanceProbability);

    }
}