using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Statistics;

namespace Model
{
    public interface IConditionLocationTimeResult
    {
        int Seed { get; }
        IConditionLocationTimeSummary ConditionLocationTime { get; }
        IReadOnlyList<IConditionLocationTimeRealizationSummary> Realizations { get; }
        IReadOnlyDictionary<IMetric, Statistics.IHistogram> Metrics { get; }
        IReadOnlyDictionary<IMetric, Statistics.IConvergenceResult> Convergence { get; }
    }

    
}
