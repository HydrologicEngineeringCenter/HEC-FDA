using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Functions;
using Model.Conditions.Locations.Years.Realizations;
using Functions.CoordinatesFunctions;

namespace Model.Conditions.Locations.Years
{
    public class ConditionLocationYearNoLateralStructure : ConditionLocationYearBase<string>
    {
        #region Properties
        
        public override IReadOnlyDictionary<IParameterEnum, bool> Parameters { get; }      
        public override string LateralStructure => "No Lateral Structure";
        #endregion

        #region Constructor
        public ConditionLocationYearNoLateralStructure(ILocation location, int yr, IFrequencyFunction frequencyFx, IEnumerable<ITransformFunction> transformFxs, IEnumerable<IMetric> metrics, string label = ""): base(location, yr, frequencyFx, transformFxs, metrics, label)
        {
            //TODO: Validation            
            Parameters = ParameterSamplePairs();
        }
        #endregion
        #region Functions
        public override IConditionLocationYearRealization ComputePreview()
        {
            Dictionary<IParameterEnum, ISample> parameters = new Dictionary<IParameterEnum, ISample>();
            foreach (var pair in Parameters) parameters.Add(pair.Key, new Samples.Sample());
            return Compute(parameters); 
        }
        public override IConditionLocationYearRealization Compute(IReadOnlyDictionary<IParameterEnum, ISample> sampleParameters = null, int id = -1)
        {
            int metricIndex = 0;
            IList<IMetric> endPoints = Metrics.ToList();
            var sampledFxs = SampleFunctions(FetchSamples(sampleParameters));
            Dictionary<IMetric, double> metrics = new Dictionary<IMetric, double>();
            IFrequencyFunction frequencyFx = (IFrequencyFunction)sampledFxs[EntryPoint.ParameterType].Parameter;
            //todo: delete me, just for testing
            Utilities.WriteToConsole.WriteCoordinatesToConsole(frequencyFx, "Freq func from CondLocYearNoLatStruct ln 41");
            /////////////////////

            foreach (var fx in TransformFunctions)
            {
                ITransformFunction transformFunction = (ITransformFunction)sampledFxs[fx.ParameterType].Parameter;
                frequencyFx = frequencyFx.Compose(transformFunction);

                //todo: delete me, just for testing
                Utilities.WriteToConsole.WriteCoordinatesToConsole(frequencyFx, "Freq func from CondLocYearNoLatStruct ln 50");
                Utilities.WriteToConsole.WriteCoordinatesToConsole(transformFunction, "transform func from CondLocYearNoLatStruct ln 51");
                /////////////////////

                sampledFxs.Add(frequencyFx.ParameterType, new Samples.SampledFunction(new Samples.Sample(), frequencyFx));
                while (metricIndex < endPoints.Count && frequencyFx.ParameterType == endPoints[metricIndex].TargetFunction)
                {
                    metrics.Add(endPoints[metricIndex], endPoints[metricIndex].Compute(frequencyFx));
                    metricIndex++;
                }
            }
            return new ConditionLocationYearRealizationNoLateralStructure(sampledFxs, metrics, id);
        }
        #endregion



        //public IConditionLocationRealization Compute(IReadOnlyDictionary<IParameterEnum, ISample> parameters = null)
        //{
        //    if (sampleParameters.IsNull()) sampleParameters = SamplePacket();
        //    else if (!IsValidComputeParameters(sampleParameters, out List<IParameterEnum> missingParameters)) throw new ArgumentException($"The following required parameters: {PrintParameterList(missingParameters)} are not included in the {nameof(sampleParameters)} argument causing an error.");

        //    int metricIndex = 0;
        //    List<IMetric> metrics = Metrics.ToList();
        //    IFrequencyFunction frequencyFx = EntryPoint;
        //    Dictionary<IMetric, double> realizationMetrics = new Dictionary<IMetric, double>();
        //    Dictionary<IParameterEnum, IFdaFunction> realizationFxs = new Dictionary<IParameterEnum, IFdaFunction>();
        //    foreach (var transformFx in TransformFunctions)
        //    {
        //        //realizationFxs.Add(frequencyFx.ParameterType, IFdaFunctionFactory.Factory(frequencyFx.ParameterType, frequencyFx.Sample(parameters[frequencyFx.ParameterType].Probability), frequencyFx.Label, frequencyFx.XSeries.Units, frequencyFx.XSeries.Label, frequencyFx.YSeries.Units, frequencyFx.YSeries.Label));
        //        //realizationFxs.Add(transformFx.ParameterType, IFdaFunctionFactory.Factory(transformFx.ParameterType, transformFx.Sample(parameters[transformFx.ParameterType].Probability), transformFx.Label, transformFx.XSeries.Units, transformFx.XSeries.Label, transformFx.YSeries.Units, transformFx.YSeries.Label));
        //        transformFx.Sample(sampleParameters[transformFx.ParameterType].Probability);
        //        frequencyFx.Compose(transformFx, sampleParameters[frequencyFx.ParameterType].Probability, sampleParameters[transformFx.ParameterType].Probability);
        //        while (frequencyFx.ParameterType == metrics[metricIndex].TargetFunction)
        //        {
        //            realizationMetrics.Add(metrics[metricIndex], metrics[metricIndex].Compute(frequencyFx));
        //            metricIndex++;
        //        }
        //    }


        //    if (sampleParameters.IsNull()) sampleParameters = SamplePacket();
        //    else
        //    {
        //        if (!IsValidParameterDictionary(sampleParameters, out List<IParameterEnum> missingParameters)) throw new ArgumentException($"The following required parameters: {PrintList(missingParameters)} are missing from the specified compute parameters.");
        //    }
        //    if (_HasLateralStructure)
        //        return ComputeWithLateralStructure(sampleParameters);
        //    else
        //    {
        //        int i = 0, I = Metrics.Count();
        //        IList<IMetric> metrics = Metrics.ToList();
        //        IFrequencyFunction frequencyFx = EntryPoint;
        //        Dictionary<IMetric, double> metricRealizations = new Dictionary<IMetric, double>();
        //        //if (parameters.ContainsKey(IParameterEnum.ExteriorInteriorStage)) //need to use this else don't need to.
        //        foreach (ITransformFunction transformFx in TransformFunctions)
        //        {
        //            frequencyFx = frequencyFx.Compose(transformFx, sampleParameters[frequencyFx.ParameterType].Probability, sampleParameters[transformFx.ParameterType].Probability);
        //            while (frequencyFx.ParameterType == metrics[i].TargetFunction) metricRealizations.Add(metrics[i], metrics[i].Compute(frequencyFx));
        //        }
        //        return new ConditionLocationRealization(metricRealizations, sampleParameters);
        //    }

        //}
        //private IConditionLocationRealization ComputeWithLateralStructure(IDictionary<IParameterEnum, ISample> parameters)
        //{
        //    int i = 0, I = Metrics.Count();
        //    IList<IMetric> metrics = Metrics.ToList();
        //    IFrequencyFunction frequencyFx = EntryPoint;
        //    ILateralStructureRealization lateralStructure;
        //    IDictionary<IMetric, double> metricRealizations = new Dictionary<IMetric, double>();
        //    foreach (ITransformFunction transformFx in TransformFunctions)
        //    {
        //        if (frequencyFx.ParameterType == IParameterEnum.ExteriorStageFrequency)
        //        {
        //            //In case it has not been sampled (if it has this will not matter).
        //            frequencyFx = IFrequencyFunctionFactory.Factory(frequencyFx.Sample(parameters[IParameterEnum.ExteriorStageFrequency].Probability), frequencyFx.ParameterType, frequencyFx.Label, frequencyFx.XSeries.Label, frequencyFx.YSeries.Label, frequencyFx.YSeries.Units);
        //            lateralStructure = LateralStructure.Compute(parameters[LateralStructure.FailureFunction.ParameterType].Probability, frequencyFx, parameters[IParameterEnum.LatralStructureFailureElevationFrequency].Probability);
        //            if (transformFx.ParameterType == IParameterEnum.ExteriorInteriorStage)
        //            {

        //                ITransformFunction intExtFx = lateralStructure.InteriorExteriorGenerator(ITransformFunctionFactory.Factory(transformFx.Sample(parameters[IParameterEnum.ExteriorInteriorStage].Probability), transformFx.ParameterType, transformFx.Label, transformFx.XSeries.Units, transformFx.XSeries.Label, transformFx.YSeries.Units, transformFx.YSeries.Label));
        //                frequencyFx = frequencyFx.Compose(intExtFx, parameters[frequencyFx.ParameterType].Probability, parameters[transformFx.ParameterType].Probability); // the transform probability is not really getting used since intExt is a constant.
        //                while (frequencyFx.ParameterType == metrics[i].TargetFunction) metricRealizations.Add(metrics[i], metrics[i].Compute(frequencyFx));
        //            }
        //            else
        //            {
        //                // this yields a interior stage frequency function.
        //                ITransformFunction intExtFx = lateralStructure.InteriorExteriorGenerator(frequencyFx);
        //                frequencyFx = frequencyFx.Compose(intExtFx, parameters[frequencyFx.ParameterType].Probability, parameters[transformFx.ParameterType].Probability); // the transform probability is not really getting used since intExt is a constant.
        //                while (frequencyFx.ParameterType == metrics[i].TargetFunction) metricRealizations.Add(metrics[i], metrics[i].Compute(frequencyFx));
        //                // this yields the next frequency function ... at the moment this must be a stage damage transform and therefore a damage frequency function.
        //                frequencyFx = frequencyFx.Compose(transformFx, parameters[frequencyFx.ParameterType].Probability, parameters[transformFx.ParameterType].Probability); // the transform probability is not really getting used since intExt is a constant.
        //                while (frequencyFx.ParameterType == metrics[i].TargetFunction) metricRealizations.Add(metrics[i], metrics[i].Compute(frequencyFx));
        //            }
        //        }
        //        frequencyFx = frequencyFx.Compose(transformFx, parameters[frequencyFx.ParameterType].Probability, parameters[transformFx.ParameterType].Probability); // the transform probability is not really getting used since intExt is a constant.
        //        while (frequencyFx.ParameterType == metrics[i].TargetFunction) metricRealizations.Add(metrics[i], metrics[i].Compute(frequencyFx));
        //    }
        //    return new ConditionLocationRealization(metricRealizations, parameters);
        //}
    }
}
