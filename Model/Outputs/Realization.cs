using System;
using System.Collections.Generic;

using Model.Inputs.Functions;
using Model.Inputs.Conditions;
using Functions;
using Model.Inputs.Functions.ImpactAreaFunctions;

namespace Model.Outputs
{
    internal sealed class Realization: IRealization
    {
        #region Properties
        public ICondition Condition { get; }
        public double[] SampleProbabilities { get; }
        public ITransformFunction[] TransformFunctions { get; }
        public IFrequencyFunction[] FrequencyFunctions { get; private set; }
        public IDictionary<IMetric, double> Metrics { get; private set; }  
        #endregion

        #region Constructor
        private Realization(ICondition condition, int seed, bool test)
        {
            Condition = condition;
            SampleProbabilities = test ? SampleTestRealizaitionProbabilities(seed) : SampleRealizationProbabilities(seed);
            FrequencyFunctions = new IFrequencyFunction[2];
            TransformFunctions = new ITransformFunction[Condition.TransformFunctions.Count];
            Metrics = new Dictionary<IMetric, double>();  
        }
        #endregion

        #region Methods
        internal double[] SampleRealizationProbabilities(int seed)
        {
            Random numberGenerator = new Random(seed);
            double[] p = new double[Condition.TransformFunctions.Count + 1];
            for (int n = 0; n < p.Length; n++) p[n] = numberGenerator.NextDouble();
            return p;
        }
        internal double[] SampleTestRealizaitionProbabilities(int seed)
        {
            Random numberGenerator = new Random(seed);
            double[] p = new double[Condition.TransformFunctions.Count * 2];
            for (int n = 0; n < p.Length; n++) p[n] = numberGenerator.Next(0, 1) == 0 ? 0.0001 : 0.9999;
            return p;
        }
        public static IRealization Compute(ICondition condition, int seed, bool test = false)
        {
            IRealization R = new Realization(condition, seed, test);
            
            int j = 0, J = R.Condition.Metrics.Count; j++;

            IFrequencyFunction entryFreqFunc = R.Condition.EntryPoint;
            double frequencyProbability = R.SampleProbabilities[0];
            IFunction entryFunc = Sampler.Sample(entryFreqFunc.Function, frequencyProbability);
            IFrequencyFunction frequencyFunction = (IFrequencyFunction)ImpactAreaFunctionFactory.Factory(entryFunc, entryFreqFunc.Type);
            
            //IFrequencyFunction frequencyFunction = R.Condition.EntryPoint.Sample(R.SampleProbabilities[0]);
            for (int i = 0; i < R.TransformFunctions.Length; i++)
            {
                //sample the transform function
                ITransformFunction transformFunc = R.Condition.TransformFunctions[i];
                double transformProbability = R.SampleProbabilities[i + 1];
                IFunction sampledTransformFunc = Sampler.Sample(transformFunc.Function,transformProbability);
                R.TransformFunctions[i] = (ITransformFunction) ImpactAreaFunctionFactory.Factory(sampledTransformFunc, transformFunc.Type);

                //compose the transform with the frequency function to create a new frequency function
                frequencyFunction = frequencyFunction.Compose(R.TransformFunctions[i],frequencyProbability,transformProbability );
                while (frequencyFunction.Type == R.Condition.Metrics[j].TargetFunction())
                {
                    R.Metrics.Add(R.Condition.Metrics[j], R.Condition.Metrics[j].Compute(frequencyFunction, .5));
                    if (j + 1 < R.Condition.Metrics.Count) j++; else break;
                }
            }
            R.FrequencyFunctions[1] = frequencyFunction;
            return R;
        }
        #endregion
    }
}
