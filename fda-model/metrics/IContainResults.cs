using System;
using System.Xml.Linq;

namespace metrics
{
    public interface IContainResults
    {
        PerformanceByThresholds PerformanceByThresholds { get; }
        ExpectedAnnualDamageResults ExpectedAnnualDamageResults { get; }

    }
}