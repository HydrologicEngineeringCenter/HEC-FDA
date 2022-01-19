using System;
namespace metrics
{
    public interface IContainResults
    {
        PerformanceByThresholds PerformanceByThresholds { get; }
        ExpectedAnnualDamageResults ExpectedAnnualDamageResults { get; }

    }
}