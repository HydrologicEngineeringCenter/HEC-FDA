using Model.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Model.Conditions.Locations.Years
{
    internal abstract class ConditionLocationYearBase<T> : IConditionLocationYear<T>
    {
        #region Properties
        public int Year { get; }
        public ILocation Location { get; }
        public string Label { get; }
        public IFrequencyFunction EntryPoint { get; }
        public IOrderedEnumerable<IMetric> Metrics { get; }
        public IOrderedEnumerable<ITransformFunction> TransformFunctions { get; }
        public abstract IReadOnlyDictionary<IParameterEnum, bool> Parameters { get; }
        public abstract T LateralStructure { get; }
        
        #endregion
        #region Constructor
        internal ConditionLocationYearBase(ILocation location, int yr, IFrequencyFunction entryPoint, IEnumerable<ITransformFunction> transformFxs, IEnumerable<IMetric> metrics, string label = "")
        {
            //TODO: Validation
            Year = yr;
            Location = location;
            Label = label == "" ? Location.Name + Year.ToString() : label; 
            EntryPoint = entryPoint;
            Metrics = metrics.OrderBy(i => i.TargetFunction);
            TransformFunctions = transformFxs.OrderBy(i => i.ParameterType);
        }
        #endregion
        #region Functions
        public IReadOnlyDictionary<IParameterEnum, ISample> SampleParametersPacket(Random rng = null, IReadOnlyDictionary<IParameterEnum, bool> sampleParameters = null)
        {
            /* This generates a dictionary of compute parameters sample value pairs:
             *     KEY: IParameterEnum VALUE: ISample Record (bool DoSample, double probability) 
             * 
             * Notes:
             * - If a random number is not provided one is generated using the time stamp as a seed value. In this case the compute will be untraceable.
             * - If a set of parameters (to sample) are not provided the Parameters property is used (all non-constant parameters are sampled).
             */
            if (rng.IsNull()) rng = new Random();
            if (sampleParameters.IsNull()) sampleParameters = Parameters;
            Dictionary<IParameterEnum, ISample> sample = new Dictionary<IParameterEnum, ISample>();
            foreach (var parameter in sampleParameters) sample.Add(parameter.Key, parameter.Value ? new Sample(rng.NextDouble()) : new Sample());
            return sample;
        }
        public abstract IConditionLocationYearRealization ComputePreview();
        public abstract IConditionLocationYearRealization Compute(IReadOnlyDictionary<IParameterEnum, ISample> parameterSamplePs, int id = -1);
        protected Dictionary<IParameterEnum, ISampledParameter<IFdaFunction>> SampleFunctions(IReadOnlyDictionary<IParameterEnum, ISample> sampleParameters)
        {
            Dictionary<IParameterEnum, ISampledParameter<IFdaFunction>> sampledFxs = new Dictionary<IParameterEnum, ISampledParameter<IFdaFunction>>()
            {
                [EntryPoint.ParameterType] = new Samples.SampledFunction(sampleParameters[EntryPoint.ParameterType], EntryPoint.Sample(sampleParameters[EntryPoint.ParameterType].Probability))
            };
            foreach (var fx in TransformFunctions) sampledFxs.Add(fx.ParameterType, new Samples.SampledFunction(sampleParameters[fx.ParameterType], fx.Sample(sampleParameters[fx.ParameterType].Probability)));
            return sampledFxs;
        }

        public IEnumerable<IParameterEnum> ConstantParameters()
        {
            List<IParameterEnum> constants = new List<IParameterEnum>();
            foreach (var pair in Parameters) if (!pair.Value) constants.Add(pair.Key);
            return constants;
        }
        protected IReadOnlyDictionary<IParameterEnum, bool> ParameterSamplePairs()
        {
            Dictionary<IParameterEnum, bool> pairs = new Dictionary<IParameterEnum, bool>
            {
                [EntryPoint.ParameterType] = EntryPoint.IsConstant
            };
            foreach(var fx in TransformFunctions)
            {
                pairs.Add(fx.ParameterType, fx.IsConstant);
            }
            return pairs;
        }
        protected IReadOnlyDictionary<IParameterEnum, ISample> FetchSamples(IReadOnlyDictionary<IParameterEnum, ISample> sampleParameters = null)
        {
            if (sampleParameters.IsNull()) return SampleParametersPacket();
            else
            {
                if (!IsValidComputeParameters(sampleParameters, out List<IParameterEnum> missingParameters)) throw new ArgumentException($"The following required parameters: {PrintParameterList(missingParameters)} are not included in the {nameof(sampleParameters)} argument causing an error.");
                else return sampleParameters;
            }  
        }
        protected bool IsValidComputeParameters<U>(IReadOnlyDictionary<IParameterEnum, U> parameters, out List<IParameterEnum> missingParameters)
        {
            missingParameters = new List<IParameterEnum>();
            foreach (var pair in Parameters) if (!parameters.ContainsKey(pair.Key)) missingParameters.Add(pair.Key);
            return missingParameters.Count == 0;
        }
        protected string PrintParameterList(List<IParameterEnum> parameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in parameters)
            {
                if (sb.Length == 0) sb.Append($"[{p.Print(true)}");
                else sb.Append($", {p.Print(true)}");
            }
            sb.Append("]");
            return sb.ToString();
        }
        #endregion
    }
}
