using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Functions;

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
        public IDictionary<IParameterEnum, bool> Parameters { get; }

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
        internal IDictionary<IParameterEnum, bool> ParameterIsVariablePairs()
        {
            Dictionary<IParameterEnum, bool> parameters = new Dictionary<IParameterEnum, bool>();
            parameters.Add(EntryPoint.ParameterType, !EntryPoint.IsConstant);
            foreach (var fx in TransformFunctions) parameters.Add(fx.ParameterType, !fx.IsConstant);
            if (_HasLateralStructure)
            {
                parameters.Add(LateralStructure.ParameterType, !LateralStructure.IsConstant);
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
        public IDictionary<IParameterEnum, ISampleRecord> SamplePacket(Random rng = null, IDictionary<IParameterEnum, bool> parameters = null)
        {
            /* This generates a dictionary of compute parameters sample value pairs:
             *     KEY: IParameterEnum VALUE: ISample Record (bool DoSample, double probability) 
             * 
             * Notes:
             * - If a random number is not provided one is generated using the time stamp as a seed value. In this case the compute will be untraceable.
             * - If a set of parameters (to sample) are not provided the Parameters property is used (all non-constant parameters are sampled).
             */
            if (rng.IsNull()) rng = new Random();
            if (parameters.IsNull()) parameters = Parameters;
            IDictionary<IParameterEnum, ISampleRecord> sample = new Dictionary<IParameterEnum, ISampleRecord>();
            foreach (var parameter in parameters) sample.Add(parameter.Key, parameter.Value ? new SampleRecord(rng.NextDouble()) : new SampleRecord());
            return sample;
        }
        public IConditionLocationRealization Compute(IDictionary<IParameterEnum, ISampleRecord> parameters = null)
        {
            if (parameters.IsNull()) parameters = SamplePacket();
            else
            {
                if (!IsValidParameterDictionary(parameters, out List<IParameterEnum> missingParameters)) throw new ArgumentException($"The following required parameters: {PrintList(missingParameters)} are missing from the specified compute parameters.");
            }
            if (_HasLateralStructure) 
                return ComputeWithLateralStructure(parameters);
            else
            {
                int i = 0, I = Metrics.Count();
                IList<IMetric> metrics = Metrics.ToList();
                IFrequencyFunction frequencyFx = EntryPoint;
                IDictionary<IMetric, double> metricRealizations = new Dictionary<IMetric, double>();
                //if (parameters.ContainsKey(IParameterEnum.ExteriorInteriorStage)) //need to use this else don't need to.
                foreach (ITransformFunction transformFx in TransformFunctions)
                {
                    frequencyFx = frequencyFx.Compose(transformFx, parameters[frequencyFx.ParameterType].Probability, parameters[transformFx.ParameterType].Probability);
                    while (frequencyFx.ParameterType == metrics[i].TargetFunction) metricRealizations.Add(metrics[i], metrics[i].Compute(frequencyFx));
                }
                return new ConditionLocationRealization(metricRealizations, parameters);
            }
            
        }
        private IConditionLocationRealization ComputeWithLateralStructure(IDictionary<IParameterEnum, ISampleRecord> parameters)
        {
            int i = 0, I = Metrics.Count();
            IList<IMetric> metrics = Metrics.ToList();
            IFrequencyFunction frequencyFx = EntryPoint;
            ILateralStructureRealization lateralStructure;
            IDictionary<IMetric, double> metricRealizations = new Dictionary<IMetric, double>();
            foreach (ITransformFunction transformFx in TransformFunctions)
            {
                if (frequencyFx.ParameterType == IParameterEnum.ExteriorStageFrequency)
                {
                    //In case it has not been sampled (if it has this will not matter).
                    frequencyFx = IFrequencyFunctionFactory.Factory(frequencyFx.Sample(parameters[IParameterEnum.ExteriorStageFrequency].Probability), frequencyFx.ParameterType, frequencyFx.Label, frequencyFx.XSeries.Label, frequencyFx.YSeries.Label, frequencyFx.YSeries.Units);
                    lateralStructure = LateralStructure.Compute(parameters[LateralStructure.FailureFunction.ParameterType].Probability, frequencyFx, parameters[IParameterEnum.LatralStructureFailureElevationFrequency].Probability);
                    if (transformFx.ParameterType == IParameterEnum.ExteriorInteriorStage)
                    {
                        
                        ITransformFunction intExtFx = lateralStructure.InteriorExteriorGenerator(ITransformFunctionFactory.Factory(transformFx.Sample(parameters[IParameterEnum.ExteriorInteriorStage].Probability), transformFx.ParameterType, transformFx.Label, transformFx.XSeries.Units, transformFx.XSeries.Label, transformFx.YSeries.Units, transformFx.YSeries.Label));
                        frequencyFx = frequencyFx.Compose(intExtFx, parameters[frequencyFx.ParameterType].Probability, parameters[transformFx.ParameterType].Probability); // the transform probability is not really getting used since intExt is a constant.
                        while (frequencyFx.ParameterType == metrics[i].TargetFunction) metricRealizations.Add(metrics[i], metrics[i].Compute(frequencyFx));
                    }
                    else
                    {
                        // this yields a interior stage frequency function.
                        ITransformFunction intExtFx = lateralStructure.InteriorExteriorGenerator(frequencyFx);
                        frequencyFx = frequencyFx.Compose(intExtFx, parameters[frequencyFx.ParameterType].Probability, parameters[transformFx.ParameterType].Probability); // the transform probability is not really getting used since intExt is a constant.
                        while (frequencyFx.ParameterType == metrics[i].TargetFunction) metricRealizations.Add(metrics[i], metrics[i].Compute(frequencyFx));
                        // this yields the next frequency function ... at the moment this must be a stage damage transform and therefore a damage frequency function.
                        frequencyFx = frequencyFx.Compose(transformFx, parameters[frequencyFx.ParameterType].Probability, parameters[transformFx.ParameterType].Probability); // the transform probability is not really getting used since intExt is a constant.
                        while (frequencyFx.ParameterType == metrics[i].TargetFunction) metricRealizations.Add(metrics[i], metrics[i].Compute(frequencyFx));
                    }
                }
                frequencyFx = frequencyFx.Compose(transformFx, parameters[frequencyFx.ParameterType].Probability, parameters[transformFx.ParameterType].Probability); // the transform probability is not really getting used since intExt is a constant.
                while (frequencyFx.ParameterType == metrics[i].TargetFunction) metricRealizations.Add(metrics[i], metrics[i].Compute(frequencyFx));
            }
            return new ConditionLocationRealization(metricRealizations, parameters);
        }


        private bool IsValidParameterDictionary<T>(IDictionary<IParameterEnum, T> parameters, out List<IParameterEnum> missingParameters)
        {
            missingParameters = new List<IParameterEnum>();
            foreach (var pair in Parameters) if (!(parameters.ContainsKey(pair.Key))) missingParameters.Add(pair.Key);
            return missingParameters.Count == 0;
        }
        private string PrintList(List<IParameterEnum> parameters)
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
    }
}
