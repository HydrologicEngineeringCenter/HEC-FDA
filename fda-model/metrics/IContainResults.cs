using System;
using System.Xml.Linq;

namespace metrics
{
    public interface IContainResults
    {
        PerformanceByThresholds PerformanceByThresholds { get; }
        DamageResults DamageResults { get; }
        int ImpactAreaID { get; }

    }
}