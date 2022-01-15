using System;
namespace metrics
{
    public interface IContainResults
    {
        PerformanceByThresholds PerformanceByThresholds { get; set; }
        ExpectedAnnualDamageResults ExpectedAnnualDamageResults { get; set; }

    }
}