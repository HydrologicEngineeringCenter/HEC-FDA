using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Model.Locations
{
    internal sealed class ConditionLocation : IConditionLocation
    {
        private readonly bool _HasLateralStructure;

        public ILocation Location { get; }
        public IFrequencyFunction EntryPoint { get; }
        public IOrderedEnumerable<ITransformFunction> TransformFunctions { get; }
        public ILateralStructure LateralStructure { get; }
        public IOrderedEnumerable<IMetric> Metrics { get; }
        internal IDictionary<IParameterEnum, bool> Parameters { get; }

        internal ConditionLocation(ILocation location, IFrequencyFunction frequencyFx, IEnumerable<ITransformFunction> transformFxs, IEnumerable<IMetric> metrics, ILateralStructure lateralStructure = null)
        {
            //ToDo: Validation
            Location = location;
            EntryPoint = frequencyFx;
            TransformFunctions = transformFxs.OrderBy(i => i.ParameterType);
            LateralStructure = lateralStructure;
            _HasLateralStructure = LateralStructure.IsNull() ? false : true;
            Metrics = metrics.OrderBy(m => m.TargetFunction);
            Parameters = ParameterIsVariablePairs();
        }
        public IDictionary<IParameterEnum, bool> ParameterIsVariablePairs()
        {
            Dictionary<IParameterEnum, bool> parameters = new Dictionary<IParameterEnum, bool>();
            parameters.Add(EntryPoint.ParameterType, !EntryPoint.IsConstant);
            foreach (var fx in TransformFunctions) parameters.Add(fx.ParameterType, !fx.IsConstant);
            if (_HasLateralStructure)
            {
                parameters.Add(LateralStructure.TopElevation.ParameterType, !LateralStructure.TopElevation.IsConstant);
                parameters.Add(LateralStructure.FailureFunction.ParameterType, !LateralStructure.FailureFunction.IsConstant);
            }
            return parameters;
        }
        public IEnumerable<IParameterEnum> ConstantParameters()
        {
            List<IParameterEnum> constants = new List<IParameterEnum>();
            foreach (var pair in Parameters) constants.Add(pair.Key);
            return constants;
        }
        public IDictionary<IParameterEnum, ISampleRecord> SamplePacket(Random rng, IDictionary<IParameterEnum, bool> sampleParameters = null)
        {
            IDictionary<IParameterEnum, ISampleRecord> sample = new Dictionary<IParameterEnum, ISampleRecord>();
            foreach (var parameter in sampleParameters) sample.Add(parameter.Key, parameter.Value ? new SampleRecord(rng.NextDouble()) : new SampleRecord());
            return sample;
        }
        public IConditionLocationRealization Compute(IDictionary<IParameterEnum, ISampleRecord> sampleProbabilities)
        {
            int i = 0, I = Metrics.Count();
            IList<IMetric> metrics = Metrics.ToList();
            IFrequencyFunction frequencyFx = EntryPoint;
            IDictionary<IMetric, double> metricRealizations = new Dictionary<IMetric, double>();
            foreach (ITransformFunction transformFx in TransformFunctions)
            {
                frequencyFx = frequencyFx.Compose(transformFx, sampleProbabilities[frequencyFx.ParameterType].NonExceedanceProbability, sampleProbabilities[transformFx.ParameterType].NonExceedanceProbability);
                while (frequencyFx.ParameterType == metrics[i].TargetFunction) metricRealizations.Add(metrics[i], metrics[i].Compute(frequencyFx));
            }
            return new ConditionLocationRealization(metricRealizations, sampleProbabilities);
        }
    }
}
