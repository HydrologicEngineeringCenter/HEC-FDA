using System;
using System.Xml.Linq;

namespace metrics
{
    public interface IContainImpactAreaScenarioResults
    {
        PerformanceByThresholds PerformanceByThresholds { get; }
        ConsequenceDistributionResults ConsequenceResults { get; }
        int ImpactAreaID { get; }

    }
}