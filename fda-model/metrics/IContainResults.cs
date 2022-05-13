using System;
using System.Xml.Linq;

namespace metrics
{
    public interface IContainResults
    {
        PerformanceByThresholds PerformanceByThresholds { get; }
        ConsequenceResults ConsequenceResults { get; }
        int ImpactAreaID { get; }

    }
}