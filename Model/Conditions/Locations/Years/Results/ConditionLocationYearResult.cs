using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics;

namespace Model.Conditions.Locations.Years.Results
{
    public class ConditionLocationYearResult : IConditionLocationYearResult
    {
        public int Seed { get; }
        public IConditionLocationYearSummary ConditionLocationTime { get; }
        public IReadOnlyList<IConditionLocationYearRealizationSummary> Realizations { get; private set; }
        public IReadOnlyDictionary<IMetric, Statistics.IHistogram> Metrics { get; private set; }
        public IReadOnlyDictionary<IMetric, Statistics.IConvergenceResult> Convergence { get; private set; }

        public ConditionLocationYearResult(IConditionLocationYearSummary condition, IReadOnlyDictionary<IMetric, Statistics.IConvergenceCriteria> criteria, int seed)
        {
            //TODO: Validate
            Seed = seed;
            ConditionLocationTime = condition;
            Compute(criteria);
            // realizations from compute.
            // metrics from compute.
            // convergence from compute.
        }

        public void Compute(IReadOnlyDictionary<IMetric, Statistics.IConvergenceCriteria> criteria)
        {
            Random rng = new Random(Seed);
            /* Counts:
             *  N = max iterations
             *  n = iteration count
             *  i = iteration in set
             *  I = max batch or set size
             *  s = number of batches or sets
             * N <= n, at any point in the compute: n = (s - 1) * I + i
             * the number of complete sets * the size of those sets + the number of iterations in the current (incomplete) set.
             */
            var converged = new Dictionary<IMetric, IConvergenceResult>();
            var histograms = new Dictionary<IMetric, Statistics.IHistogram>();
            var realizations = new List<IConditionLocationYearRealizationSummary>();
            foreach (var i in criteria) converged[i.Key] = Statistics.IConvergenceResultFactory.Factory();
            int s = 0, n = 0, I = 1, N = 1; //I = 10000, N = 10000;
            while (!IsConverged(converged) && n < N)
            {
                s++;
                IReadOnlyDictionary<int, IReadOnlyDictionary<IParameterEnum, ISample>> setOfSamples = SetSamplePacket(rng,n, I);
                var setOfRealizations = new ConcurrentBag<IConditionLocationYearRealizationSummary>();
                Parallel.For(n, n + I, i =>
                {
                    setOfRealizations.Add(ConditionLocationTime.Compute(setOfSamples[i], i));
                });
                n += I;
                realizations.AddRange(setOfRealizations);
                if (histograms.Count == 0)
                {
                    histograms = UpdateHistograms(setOfRealizations, histograms);
                }
                else // test for convergence
                {
                    //TODO: Parallelize checks for convergence.
                    var updatedHistograms = UpdateHistograms(setOfRealizations, histograms);
                    foreach (var histogram in histograms)
                    {
                        if (criteria.ContainsKey(histogram.Key) && !converged[histogram.Key].Passed)
                            converged[histogram.Key] = criteria[histogram.Key].Test(histogram.Value, updatedHistograms[histogram.Key]);
                    }
                }
            }
            Metrics = histograms;
            Convergence = converged;
            Realizations = realizations;
        }
        private IReadOnlyDictionary<int, IReadOnlyDictionary<IParameterEnum, ISample>> SetSamplePacket(Random rng, int startIndex, int setCount = 1000)
        {
            Dictionary<int, IReadOnlyDictionary<IParameterEnum, ISample>> set = new Dictionary<int, IReadOnlyDictionary<IParameterEnum, ISample>>();
            for (int i = startIndex; i < startIndex + setCount; i++)
            {
                set.Add(i, ConditionLocationTime.SampleParametersPacket(rng, ConditionLocationTime.Parameters));
            }
            return set;
        }
        private Dictionary<IMetric, Statistics.IHistogram> UpdateHistograms(ConcurrentBag<IConditionLocationYearRealizationSummary> realizations, Dictionary<IMetric, Statistics.IHistogram> histograms)
        {
            //TODO: Parallelize adding lists to histograms
            
            /* Several steps
             *  1. check the metrics in the compute to ensure they are the expected metric types.
             *  2. gather list of metric realization values (doubles).
             *  3. add list of realization values to histograms
             *      a. generate new histograms if none have been generated before.
             *      b. otherwise just add to the existing ones.
             */
            realizations.TryPeek(out IConditionLocationYearRealizationSummary last);
            IEnumerable<IMetric> realizationMetrics = last.Metrics.Keys, histogramMetrics = histograms.Keys;
            //if (!Enumerable.SequenceEqual(realizationMetrics.OrderBy(i => i.ParameterType), histogramMetrics.OrderBy(j => j.ParameterType))) throw new ArgumentException($"The realization produced the following list of metrics: {PrintMetricList(realizationMetrics)} but the following list was expected: {PrintMetricList(histogramMetrics)}.");

            // step 2: gathering list of realization metrics...
            var metrics = new Dictionary<IMetric, List<double>>();
            foreach (var metric in realizationMetrics) metrics.Add(metric, new List<double>());
            foreach (var realization in realizations) foreach (var metric in realization.Metrics) metrics[metric.Key].Add(metric.Value);

            // step 3: add data to histograms... (a) if - new histograms (b) else - add to existing histograms
            if (histograms.Count == 0)
            {
                foreach (var metric in metrics)
                {
                    if (metric.Key.ParameterType != IParameterEnum.EAD)
                    {
                        histograms[metric.Key] = Statistics.IHistogramFactory.Factory(Statistics.IDataFactory.Factory(metric.Value, true), 0, 1, 100);
                    }
                    else
                    {
                        histograms[metric.Key] = Statistics.IHistogramFactory.Factory(Statistics.IDataFactory.Factory(metric.Value, true), 100);
                    }
                }
            }
            else
            {
                foreach (var metric in metrics)
                {
                    histograms[metric.Key] = Statistics.IHistogramFactory.Factory(histograms[metric.Key], Statistics.IDataFactory.Factory(metric.Value, true));
                }
            }
            return histograms;
        }
        private bool IsConverged(Dictionary<IMetric, IConvergenceResult> convergenceTests)
        {
            bool isConverged = true;
            foreach (var test in convergenceTests) if (!test.Value.Passed) isConverged = false;
            return isConverged;
        }
        private string PrintMetricList(IEnumerable<IMetric> list)
        {
            bool first = true;
            var toPrint = new StringBuilder("[");
            foreach (var metric in list)
            {
                if (first)
                {
                    toPrint.Append(metric.ParameterType.ToString());
                    first = false;
                }
                else toPrint.Append($", {metric.ParameterType.ToString()}");
            }
            return toPrint.Append("]").ToString();
        }
    }
}
