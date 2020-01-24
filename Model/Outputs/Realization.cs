//using System;
//using System.Collections.Generic;

//using Model.Inputs.Functions;
//using Model.Inputs.Conditions;
//using Functions;
//using Model.Inputs.Functions.ImpactAreaFunctions;

//namespace Model.Outputs
//{
//    public static class Realization
//    {
//        #region Properties
//        //private ICondition _Condition;
//        //public double[] SampleProbabilities { get; }
//        //public ITransformFunction[] TransformFunctions { get; }
//        //public IFrequencyFunction[] FrequencyFunctions { get; private set; }
//        //public IDictionary<IMetric, double> Metrics { get; private set; }  
//        #endregion

//        #region Constructor
//        //private Realization(ICondition condition, int seed, bool test)
//        //{
//        //    _Condition = condition;
//        //    SampleProbabilities = test ? SampleTestRealizaitionProbabilities(seed) : SampleRealizationProbabilities(seed);
//        //    //FrequencyFunctions = new IFrequencyFunction[2];
//        //    //TransformFunctions = new ITransformFunction[Condition.TransformFunctions.Count];
//        //    Metrics = new Dictionary<IMetric, double>();  
//        //}
//        #endregion

//        #region Methods
//        //internal double[] SampleRealizationProbabilities(int seed)
//        //{
//        //    Random numberGenerator = new Random(seed);
//        //    double[] p = new double[_Condition.TransformFunctions.Count + 1];
//        //    for (int n = 0; n < p.Length; n++) p[n] = numberGenerator.NextDouble();
//        //    return p;
//        //}
//        //internal double[] SampleTestRealizaitionProbabilities(int seed)
//        //{
//        //    Random numberGenerator = new Random(seed);
//        //    double[] p = new double[_Condition.TransformFunctions.Count * 2];
//        //    for (int n = 0; n < p.Length; n++) p[n] = numberGenerator.Next(0, 1) == 0 ? 0.0001 : 0.9999;
//        //    return p;
//        //}
//        public static IDictionary<IMetric, double> Compute(ICondition condition, int seed, bool test = false)
//        {
//            IRealization R = new Realization(condition, seed, test);
//            //the dummy probability gets used for functions that we know are already a constant.
//            double dummyProbability = .5;
//            int j = 0;
//            int J = R.Condition.Metrics.Count;


//            IFrequencyFunction frequencyFunction = R.Condition.EntryPoint;
    
//            bool isFirstFreqFunc = true;
//            for (int i = 0; i < R.Condition.TransformFunctions.Count; i++)
//            {
//                ITransformFunction transformFunc = R.Condition.TransformFunctions[i];
                
//                //we only want to pull a random number for the first frequency function because we do not know if it is a constant or not
//                double freqFuncProb;
//                if (isFirstFreqFunc)
//                {
//                    freqFuncProb = R.SampleProbabilities[i];
//                    isFirstFreqFunc = false;
//                }
//                else
//                {
//                    freqFuncProb = dummyProbability;
//                }
//                    frequencyFunction = frequencyFunction.Compose(transformFunc, freqFuncProb, R.SampleProbabilities[i + 1]);
//                while (frequencyFunction.Type == R.Condition.Metrics[j].TargetFunction())
//                {
//                    //right now we are just using ".5" for the probability because we know that it won't ever be used
//                    //The frequencyFunction has already been sampled during the compose above
//                    R.Metrics.Add(R.Condition.Metrics[j], R.Condition.Metrics[j].Compute(frequencyFunction, dummyProbability));
//                    if (j + 1 < R.Condition.Metrics.Count) j++; else break;
//                }
//            }
//             return R;
//        }
//        #endregion
//    }
//}
