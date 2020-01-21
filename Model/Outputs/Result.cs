using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Inputs;
using Model.Inputs.Conditions;

namespace Model.Outputs
{
    internal class Result
    {
        #region Properties
        public ICondition Condition { get; }
        public int TimeStampSeed { get; private set; }
        public Random SeedGenerator { get; private set; }
        public int IterationCount { get; private set; } = 0;
        public bool Converged { get; private set; } = false;
        public int MaxIterations { get; set; } = 500000;
        public IList<int> IterationSeedContainer { get; private set; } = new List<int>();
        //public Statistics.Histogram Aep { get; private set; } = new Statistics.Histogram(50, 0.5, 0.001, false);
        //public Statistics.Histogram Ead { get; private set; } = new Statistics.Histogram(50, 0, 1000000, false);
        public IDictionary<MetricEnum, Statistics.IHistogram> Metrics = new Dictionary<MetricEnum, Statistics.IHistogram>();

        #endregion

        #region Constructor
        public Result(ICondition condition)
        {
            Condition = condition;
            Metrics = new Dictionary<MetricEnum, Statistics.IHistogram>();
        }
        #endregion

        #region Methods
        public void Compute()
        {
            if (Condition.IsValid == false) { Condition.ReportValidationErrors(); return; }

            IterationCount = 0;
            TimeStampSeed = (int)new DateTime().Ticks;
            SeedGenerator = new Random(TimeStampSeed);
            int localIteration, batchCount = 1000;

            while (Converged == false &&
                   IterationCount < MaxIterations)
            {
                localIteration = IterationCount;
                for (int i = 0; i < batchCount; i++) IterationSeedContainer.Add(SeedGenerator.Next());
              
                Parallel.For(localIteration, localIteration + batchCount, i =>
                {
                    Condition.Compute(IterationSeedContainer[i]);
                    IterationCount++;
                });
            }
        }
        //private IDictionary<ComputePointUnitTypeEnum, Statistics.Histogram> InitializeMetrics()
        //{
        //    Metrics = new Dictionary<ComputePointUnitTypeEnum, Statistics.Histogram>();
        //    foreach (var point in Condition.ComputePoints)
        //    {
        //        Metrics.Add(point.Unit, new Statistics.Histogram(50, ))
        //    }
        //}
        //private void AddMetricsTestConvergence()
        //{
        //    foreach (var metric in Condition.Metrics)
        //    {
        //        if (Metrics.ContainsKey(metric.Key)) Metrics[metric.Key] = 
        //    }
        //}
        #endregion
    }
}
