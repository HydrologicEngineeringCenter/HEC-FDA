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
        //public int TimeStampSeed { get; private set; }
       // public Random SeedGenerator { get; private set; }
        public int IterationCount { get; private set; } = 0;
        public bool Converged { get; private set; } = false;
        public int MaxIterations { get; set; } = 500000;
        //public IList<int> IterationSeedContainer { get; private set; } = new List<int>();
        //public Statistics.Histogram Aep { get; private set; } = new Statistics.Histogram(50, 0.5, 0.001, false);
        //public Statistics.Histogram Ead { get; private set; } = new Statistics.Histogram(50, 0, 1000000, false);
        public IDictionary<MetricEnum, Statistics.IHistogram> Metrics = new Dictionary<MetricEnum, Statistics.IHistogram>();
        public System.Collections.Concurrent.ConcurrentDictionary<int, IDictionary<IMetric, double>> Realizations { get; }
        public int Seed { get; }
        //public List<IRealization> RealizationIds { get; }
        //public int PacketSize { get; }

        #endregion

        #region Constructor
        public Result(ICondition condition)
        {
            int TimeStampSeed = (int)new DateTime().Ticks;

            new Result(condition, TimeStampSeed);
        }
       
        public Result(ICondition condition, int seed)
        {
            Condition = condition;
            Metrics = new Dictionary<MetricEnum, Statistics.IHistogram>();
            Realizations = new System.Collections.Concurrent.ConcurrentDictionary<int, IDictionary<IMetric, double>>();
            Seed = seed;
        }
        #endregion

        #region Methods
        public void Compute()
        {
            if (Condition.IsValid == false) { Condition.ReportValidationErrors(); return; }

            IterationCount = 0;
            int randomPacketSize = Condition.TransformFunctions.Count + 1;
            //TimeStampSeed = (int)new DateTime().Ticks;
            Random randomNumberGenerator = new Random(Seed);
            int localIteration, batchCount = 1000;

            List<List<double>> allProbabilities = CreateAllProbabilities(randomNumberGenerator, batchCount, randomPacketSize);
            while (Converged == false &&
                   IterationCount < MaxIterations)
            {
                localIteration = IterationCount;

                //todo: John, why are we not starting at zero here.
                //from 1,000 inclusive to 2,000 exclusive.
                Parallel.For(localIteration, localIteration + batchCount, i =>
                {
                    //List<double> randomNumbers = new List<double>();
                    //for(int k = 0;k<randomPacketSize;k++)
                    //{
                    //    randomNumbers.Add(randomNumberGenerator.NextDouble());
                    //}
                    bool success = Realizations.TryAdd(i, Condition.Compute(allProbabilities[i]));
                    if(!success)
                    {
                        throw new Exception("The result compute tried to put the same id into the dictionary a second time.");
                    }
                    IterationCount++;
                });
            }
        }

        private List<List<double>> CreateAllProbabilities(Random RNG, int numComputes, int packetSize)
        {
            List<List<double>> allProbabilities = new List<List<double>>();
            for(int i = 0;i<numComputes;i++)
            {
                List<double> probs = new List<double>();
                for(int j=0;j<packetSize;j++)
                {
                    probs.Add(RNG.NextDouble());
                }
                allProbabilities.Add(probs);
            }
            return allProbabilities;
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
