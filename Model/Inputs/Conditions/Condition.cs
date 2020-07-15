using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Model.Outputs;
using Model.Inputs.Functions;
using Model.Inputs.Functions.ImpactAreaFunctions;

namespace Model.Inputs.Conditions
{
    internal sealed class Condition : ICondition
    {
        #region Properties

        public string Id
        {
            get
            {
                return new StringBuilder(Name).Append(Year).ToString();
            }
        }
        public string Name { get; } = string.Empty;
        public int Year { get; } = DateTime.Now.Year;
        public IFrequencyFunction EntryPoint { get; }
        public IList<ITransformFunction> TransformFunctions { get; }
        public IList<IMetric> Metrics { get; }
        public bool IsValid { get; } = false;
        #endregion

        #region Constructor
        public Condition(string name, int year, IFrequencyFunction frequencyFunction, IList<ITransformFunction> transformFunctions, IList<IMetric> metrics)
        {
            Name = name;
            Year = year;
            EntryPoint = frequencyFunction;
            TransformFunctions = transformFunctions.OrderBy(i => i.Type).ToList();
            Metrics = metrics.OrderBy(x=>x.TargetFunction).ToList();
            IsValid = Validate();
        }
        #endregion

        #region Methods
        public IDictionary<IMetric, double> Compute(List<double> randomNums)
        {
            IDictionary<IMetric, double> metricsDictionary = new Dictionary<IMetric, double>();

        //IRealization R = new Realization(condition, seed, test);
        //the dummy probability gets used for functions that we know are already a constant.
        double dummyProbability = .5;
            int j = 0;
            int J = Metrics.Count;


            IFrequencyFunction frequencyFunction = EntryPoint;

            bool isFirstFreqFunc = true;
            for (int i = 0; i < TransformFunctions.Count; i++)
            {
                ITransformFunction transformFunc = TransformFunctions[i];

                //we only want to pull a random number for the first frequency function because we do not know if it is a constant or not
                double freqFuncProb;
                if (isFirstFreqFunc)
                {
                    freqFuncProb = randomNums[i];
                    isFirstFreqFunc = false;
                }
                else
                {
                    freqFuncProb = dummyProbability;
                }
                frequencyFunction = frequencyFunction.Compose(transformFunc, freqFuncProb, randomNums[i + 1]);
                while (frequencyFunction.Type == Metrics[j].TargetFunction)
                {
                    //right now we are just using ".5" for the probability because we know that it won't ever be used
                    //The frequencyFunction has already been sampled during the compose above
                    metricsDictionary.Add(Metrics[j], Metrics[j].Compute(frequencyFunction, dummyProbability));
                    if (j + 1 < Metrics.Count) j++; else break;
                }
            }
            return metricsDictionary;
        }

        

        public bool TestCompute(List<double> randomNums, int nTests = 100)
        {
            for (int i = 0; i < nTests; i++)
            {
                try
                {
                    Compute(randomNums);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        //private bool RunTestCompute(int nTests = 100)
        //{
        //    //InitializeMetricsRange();
        //    try
        //    {
        //        ApproximateMetricRange(InnerCompute(GenerateTestProbabilities(TransformFunctions.Count * 2)), true);

        //        for (int i = 1; i < nTests; i++)
        //        {
        //            ApproximateMetricRange(InnerCompute(GenerateTestProbabilities(TransformFunctions.Count * 2)), false);
        //        }
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}        
        //private void ApproximateMetricRange(IDictionary<MetricEnum, double> result, bool firstPass)
        //{
        //    if (firstPass == false)
        //    {
        //        foreach (var metric in result)
        //        {
                  
        //            Math.Min(MetricsRange[metric.Key][0], metric.Value);
        //            Math.Max(MetricsRange[metric.Key][0], metric.Value);
        //        }
        //    }
        //    else
        //    {
        //        foreach (var metric in result)
        //        {
        //            MetricsRange[metric.Key][0] = metric.Value;
        //            MetricsRange[metric.Key][1] = metric.Value;
        //        }    
        //    }
        //}
        //private double[] SampleRealizationProbabilities(int seed)
        //{
        //    Random r = new Random(seed);

        //    int N = TransformFunctions.Count * 2;
        //    double[] P = new double[N];
        //    for (int n = 0; n < N; n++) P[n] = r.NextDouble();
        //    return P;
        //}
        //private double[] SampleRealizationTestProbabilities(int seed = 0)
        //{
        //    Random r = (seed == 0) ? new Random() : new Random(seed);
        //    int N = TransformFunctions.Count * 2;
        //    double[] P = new double[N];
        //    for (int n = 0; n < N; n++) P[n] = r.Next(0, 1) == 0 ? 0.0001 : 0.9999;
        //    return P;
        //}
        //public IDictionary<MetricEnum, double> ComputeRealization(int seed, bool testCompute = false)
        //{
        //    int j = 0, J = ComputePoints.Count, n = 0;
        //    IFunctionCompose frequencyFunction = EntryPoint;
        //    IDictionary<MetricEnum, double> metrics = new Dictionary<MetricEnum, double>();
        //    double[] P = testCompute ? SampleRealizationProbabilities(seed) : SampleRealizationTestProbabilities(seed);
        //    for (int i = 0; i < TransformFunctions.Count; i++)
        //    {
        //        frequencyFunction = frequencyFunction.Compose(TransformFunctions[i], P[n], P[n + 1]);
        //        while (frequencyFunction.Type == ComputePoints[j].FunctionType)
        //        {
        //            if (ComputePoints[j].Unit < MetricEnum.ExpectedAnnualDamage)
        //                metrics.Add(ComputePoints[j].Unit, frequencyFunction.GetXFromY(ComputePoints[j].TargetValue));
        //            else
        //                metrics.Add(ComputePoints[j].Unit, frequencyFunction.Integrate());
        //            if (j + 1 < ComputePoints.Count) j++;
        //            else break;
        //        }
        //    }
        //    return metrics;
        //}
        #region Old Compute Style (delete if ComputeRealization() passes tests)
        //public IDictionary<MetricEnum, double> Compute(int seed)
        //{
        //    Random numberGenerator = new Random();
        //    int N = TransformFunctions.Count * 2; double[] randoms = new double[N];
        //    for (int i = 0; i < N; i++) randoms[i] = numberGenerator.NextDouble();
        //    return InnerCompute(randoms);
        //}
        //private IDictionary<MetricEnum, double> InnerCompute(double[] P)
        //{
        //    int j = 0, J = ComputePoints.Count, n = 0;
        //    IDictionary<MetricEnum, double> metrics = new Dictionary<MetricEnum, double>();
        //    for (int i = 0; i < TransformFunctions.Count; i++)
        //    {
        //        n = i * 2;
        //        FrequencyFunctions.Add(FrequencyFunctions[i].Compose(TransformFunctions[i], P[n], P[n + 1]));
        //        while (FrequencyFunctions[FrequencyFunctions.Count - 1].Type == ComputePoints[j].FunctionType)
        //        {
        //            if (ComputePoints[j].Unit < MetricEnum.ExpectedAnnualDamage)
        //                metrics.Add(ComputePoints[j].Unit, FrequencyFunctions[FrequencyFunctions.Count - 1].GetXFromY(ComputePoints[j].TargetValue));
        //            else
        //                metrics.Add(ComputePoints[j].Unit, FrequencyFunctions[FrequencyFunctions.Count - 1].Integrate());
        //            if (j + 1 == J) break;
        //            else j++;
        //        }
        //    }
        //    return metrics;
        //}
        #endregion
        #region Recursion (In Development)
        //public void RecursiveCompute(int seed)
        //{
        //    // so much to do here....
        //}
        //private double ComputeMetricsRecursively(Random r, IFunctionCompose frequency, int transform, int computePoint)
        //{
        //    if (ComputePoints[computePoint].Unit == MetricEnum.ExpectedAnnualDamage)
        //        return frequency.Integrate();
        //    else
        //        return frequency.GetXFromY(ComputePoints[computePoint].TargetValue);
        //    return ComputeMetricsRecursively(r, RecursiveCompose(r, EntryPoint, transform, computePoint), transform, computePoint + 1);
        //}
        //private IFunctionCompose RecursiveCompose(Random r, IFunctionCompose frequency, int transform, int computePoint)
        //{
        //    if (frequency.Type == ComputePoints[computePoint].FunctionType) return frequency;
        //    else return RecursiveCompose(r, frequency.Compose(TransformFunctions[transform], r.NextDouble(), r.NextDouble()), transform + 1, computePoint);
        //}
        #endregion
        #endregion

        #region IValidate Members
        public bool Validate()
        {
            if (ValidateYear() && ValidateFunctionOrder()) return true;
            else { ReportValidationErrors(); return false; }
        }
        internal bool ValidateYear()
        {
            if (Year < 1849 || //Swamp Lands Act of 1849 was the first major US flood control act.
                Year > DateTime.Today.Year + 200) return false;
            else return true;
        }
        internal bool ValidateFunctionOrder()
        {
            TransformFunctions.OrderBy(t => t.Type);
            if (Metrics.Count == 0) return false;
            Metrics.OrderBy(t => t.Type);
            foreach (ITransformFunction tFunction in TransformFunctions)
            {
                if (tFunction.Type < EntryPoint.Type ||
                    tFunction.Type > Metrics[Metrics.Count - 1].TargetFunction) return false;
            }
            foreach (IMetric metric in Metrics)
            {
                ImpactAreaFunctionEnum exit = metric.TargetFunction;
                if (exit < EntryPoint.Type ||
                    exit > TransformFunctions[TransformFunctions.Count - 1].Type + 1) return false;
            }
            return true;
        }
        internal bool ValidateCompute()
        {
            int seed = 1;
            int randomPacketSize = TransformFunctions.Count + 1;
            List<double> randomNumbers = new List<double>();
            Random randomNumberGenerator = new Random(seed);
            for (int k = 0; k < randomPacketSize; k++)
            {
                randomNumbers.Add(randomNumberGenerator.NextDouble());
            }
            return TestCompute(randomNumbers);
        }
        public IEnumerable<string> ReportValidationErrors()
        {
            //IsValid = true;
            StringBuilder messages = ReportAnalysisYearErrors()
                                    .Append(ReportTransformFunctionErrors())
                                    .Append(ReportMetricErrors());

            IList<string> report = new List<string>();
            char[] delimiter = Environment.NewLine.ToCharArray();
            string[] lines = messages.ToString()
                                     .Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                report.Add(line);
            }
            return report;
        }
        private StringBuilder ReportAnalysisYearErrors()
        {
            StringBuilder analysisYearMessage = new StringBuilder();            
            if (Year < 1849 || //Swamp Lands Act of 1849 was the first major US flood control act.
                Year > DateTime.Today.Year + 200)
            {
                //IsValid = false;
                return analysisYearMessage.Append("The condition year must be between 1849 and ")
                                          .Append(DateTime.Today.Year + 200)
                                          .Append(".");
            }
            else return analysisYearMessage;
        }
        private StringBuilder ReportTransformFunctionErrors()
        {
            StringBuilder transformFunctionMessages = new StringBuilder();
            if (Metrics.Count == 0)
            {
                //IsValid = false;
                transformFunctionMessages.AppendLine("The condition can not be computed without a targeted performance metric or expected annual damages.");
            }
            foreach (ITransformFunction function in TransformFunctions)
            {
                if (function.Type < EntryPoint.Type &&
                    function.Type > Metrics[Metrics.Count - 1].TargetFunction)
                {
                    TransformFunctions.Remove(function);
                    transformFunctionMessages.AppendLine("A ")
                                             .Append(function.Type)
                                             .Append(" transform function was removed from the compute sequence because it appears before the condition's computational entry point or after the condition's computational exit point.");
                }
            }
            return transformFunctionMessages;
        }
        private StringBuilder ReportMetricErrors()
        {
            StringBuilder thresholdMessages = new StringBuilder();
            foreach (IMetric metric in Metrics)
            {
                ImpactAreaFunctionEnum exit = metric.TargetFunction;
                if (exit < EntryPoint.Type ||
                    exit > TransformFunctions[TransformFunctions.Count - 1].Type + 1)
                {
                    //IsValid = false;
                    thresholdMessages.AppendLine("The ")
                                     .Append(metric.Type)
                                     .Append(" metric can not be calculated because it preceeds the computational entry point (a ")
                                     .Append(EntryPoint.Type).Append(" function). ")
                                     .Append("or follows the last function that can be calculated in the compute sequence (a ")
                                     .Append(TransformFunctions[TransformFunctions.Count - 1].Type + 1).Append(" function).");
                }
            }
            return thresholdMessages;    
        }
        #endregion
    }
}
