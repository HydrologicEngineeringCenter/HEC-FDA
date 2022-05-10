using System;
using System.Xml.Linq;

namespace metrics
{
    public interface IContainResults
    {
        PerformanceByThresholds PerformanceByThresholds { get; }
        ConsequenceResults DamageResults { get; }
        int ImpactAreaID { get; }

    }
}