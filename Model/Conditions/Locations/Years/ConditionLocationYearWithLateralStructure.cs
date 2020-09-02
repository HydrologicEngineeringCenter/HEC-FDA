using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Functions;

namespace Model.Conditions.Locations.Years
{
    internal sealed class ConditionLocationYearWithLateralStructure: ConditionLocationYearBase<ILateralStructure>
    {
        public override IReadOnlyDictionary<IParameterEnum, bool> Parameters { get; }
        public override ILateralStructure LateralStructure { get; }

        internal ConditionLocationYearWithLateralStructure(ILocation location, int yr, IFrequencyFunction frequencyFx, IEnumerable<ITransformFunction> transformFxs, ILateralStructure lateralStructure, IEnumerable<IMetric> metrics, string label = ""): base(location, yr, frequencyFx, transformFxs, metrics, label)
        {
            //TODO: Validation
            LateralStructure = lateralStructure;
            Parameters = ParameterSamplePairsWithLateralStucture(); //contains failureFx and failureStageProb
        }
        private IReadOnlyDictionary<IParameterEnum, bool> ParameterSamplePairsWithLateralStucture()
        {
            Dictionary<IParameterEnum, bool> parameters = (Dictionary<IParameterEnum, bool>)ParameterSamplePairs();
            parameters.Add(IParameterEnum.LateralStructureFailure, LateralStructure.FailureFunction.IsConstant ? false : true);
            parameters.Add(IParameterEnum.LatralStructureFailureElevationFrequency, true);
            return parameters;
        }

        public override IConditionLocationYearRealization ComputePreview()
        {
            /* Differs from Compute() below by:
             *  1. No failure stage is sampled (although the failure function median curve is sampled).
             *  2. Damages from damage frequency curve are multiplied by failure probabilities.
             */
            Dictionary<IParameterEnum, ISample> parameters = new Dictionary<IParameterEnum, ISample>();
            foreach (var pair in Parameters) parameters.Add(pair.Key, new Samples.Sample());
            /* Nearly identical to LateralStructure.Compute(...) below this point except:
             *  3. Failure functions applied damage frequency function (as described above).
             */
            int metricIndex = 0;
            IList<IMetric> endPoints = Metrics.ToList();
            var sampledFxs = SampleFunctionsWithLateralStructure(parameters);
            Dictionary<IMetric, double> metrics = new Dictionary<IMetric, double>();
            IFrequencyFunction frequencyFx = (IFrequencyFunction)sampledFxs[EntryPoint.ParameterType].Parameter;
            foreach (var fx in TransformFunctions)
            {
                if (fx.ParameterType == IParameterEnum.InteriorStageDamage)
                {
                    sampledFxs[IParameterEnum.InteriorStageDamage] = new Samples.SampledFunction(sampledFxs[IParameterEnum.InteriorStageDamage], FailureAdjustedStageDamageFunctionGenerator(frequencyFx, (ITransformFunction)sampledFxs[IParameterEnum.InteriorStageDamage].Parameter, (ITransformFunction)sampledFxs[IParameterEnum.LateralStructureFailure].Parameter));
                }
                frequencyFx = frequencyFx.Compose((ITransformFunction)sampledFxs[fx.ParameterType].Parameter);
                sampledFxs.Add(frequencyFx.ParameterType, new Samples.SampledFunction(new Samples.Sample(), frequencyFx));
                while (frequencyFx.ParameterType == endPoints[metricIndex].TargetFunction)
                {
                    metrics.Add(endPoints[metricIndex], endPoints[metricIndex].Compute(frequencyFx));
                    metricIndex++;
                }
            }
            var lateralStructure = new Samples.SampledOrdinate(IParameterFactory.Factory(double.NaN, IParameterEnum.ExteriorElevation), new Samples.Sample());
            return new ConditionLocationYearRealizationWithLateralStructure(sampledFxs, lateralStructure, metrics, -1);
        }
        public override IConditionLocationYearRealization Compute(IReadOnlyDictionary<IParameterEnum, ISample> sampleParameters = null, int id = -1)
        {
            /* Differs from LateralStructure.Compute(...) without lateral structure in a couple key ways.
             * 1. Lateral structure parameters are sampled...
             *          - failure function, and 
             *          - failure stage) must be sampled.\
             *    FetchSamples() on the parent class ensures (through IsValidComputeParameters()) that these are sampled.
             * 2. The sampled lateral structure parameters are either:
             *      a. used to generate an interior exterior function 
             *          - with 0 interior stage before failure stage or top of levee,
             *          - interior stage = exterior stage at or above failure stage or top of levee 
             *      b. used to edit the provided interior exterior function:
             *          - with 0 interior stage below failure stage or top of levee,
             *          - interior stage = provided interior exterior relationship stage above failure stage or top of levee.
             */
            
            int metricIndex = 0;
            IList<IMetric> endPoints = Metrics.ToList();
            sampleParameters = FetchSamples(sampleParameters);
            ISampledParameter<IParameterOrdinate> failElevation = null;
            var sampledFxs = SampleFunctionsWithLateralStructure(sampleParameters);
            Dictionary<IMetric, double> metrics = new Dictionary<IMetric, double>();
            IFrequencyFunction frequencyFx = (IFrequencyFunction)sampledFxs[EntryPoint.ParameterType].Parameter;
            foreach (var fx in TransformFunctions) // should NOT include failure function (so this ends in the right spot).
            {
                if (frequencyFx.ParameterType == IParameterEnum.ExteriorStageFrequency)
                {
                    failElevation = new Samples.SampledOrdinate(
                        IParameterFactory.Factory(frequencyFx.F(IOrdinateFactory.Factory(sampleParameters[IParameterEnum.LatralStructureFailureElevationFrequency].Probability)).Value(), IParameterEnum.ExteriorElevation, frequencyFx.YSeries.Units),
                        sampleParameters[IParameterEnum.LatralStructureFailureElevationFrequency]);
                    if (!sampleParameters.ContainsKey(IParameterEnum.ExteriorInteriorStage))
                    {
                        ITransformFunction eiFx = ExteriorInteriorFunctionGenerator(frequencyFx, failElevation.Parameter.Ordinate.Value());
                        sampledFxs[IParameterEnum.ExteriorInteriorStage] = new Samples.SampledFunction(new Samples.Sample(), eiFx);
                    }
                    else // an exterior interior relationship already exists.
                    {
                        ITransformFunction eiFx = ExteriorInteriorFunctionGenerator((ITransformFunction)sampledFxs[IParameterEnum.ExteriorInteriorStage].Parameter, failElevation.Parameter.Ordinate.Value());
                        sampledFxs[IParameterEnum.ExteriorInteriorStage] = new Samples.SampledFunction(sampledFxs[IParameterEnum.ExteriorInteriorStage], eiFx);
                    }
                }
                frequencyFx = frequencyFx.Compose((ITransformFunction)sampledFxs[fx.ParameterType].Parameter);
                sampledFxs.Add(frequencyFx.ParameterType, new Samples.SampledFunction(new Samples.Sample(), frequencyFx));
                while (frequencyFx.ParameterType == endPoints[metricIndex].TargetFunction)
                {
                    metrics.Add(endPoints[metricIndex], endPoints[metricIndex].Compute(frequencyFx));
                    metricIndex++;
                } 
            }
            return new ConditionLocationYearRealizationWithLateralStructure(sampledFxs, failElevation, metrics, id);
        }
        private Dictionary<IParameterEnum, ISampledParameter<IFdaFunction>> SampleFunctionsWithLateralStructure(IReadOnlyDictionary<IParameterEnum, ISample> sampleParameters)
        {
            /* Extends parent class SampleFunctions() to include failure function.
             * 1. samples transform functions using SampleFunctions() on parent class.
             * 2. samples Lateral Structure Failure Function
             * Note: failure stage is not yet sampled.
             */
            Dictionary<IParameterEnum, ISampledParameter<IFdaFunction>> fxs = SampleFunctions(sampleParameters);
            fxs.Add(IParameterEnum.LateralStructureFailure, new Samples.SampledFunction(sampleParameters[LateralStructure.FailureFunction.ParameterType], LateralStructure.FailureFunction.Sample(sampleParameters[LateralStructure.FailureFunction.ParameterType].Probability)));
            return fxs;
        }
        private ITransformFunction ExteriorInteriorFunctionGenerator(IFrequencyFunction extFrequencyFx, double failureElevation)
        {
            List<ICoordinate> coordinates = new List<ICoordinate>();
            foreach (var pair in extFrequencyFx.Coordinates)
                coordinates.Add(ICoordinateFactory.Factory(pair.Y.Value(), pair.Y.Value() < failureElevation ? 0.0 : pair.Y.Value()));
            return ITransformFunctionFactory.Factory(IFunctionFactory.Factory(coordinates, extFrequencyFx.Interpolator), IParameterEnum.ExteriorInteriorStage, label: $"Automatically Generated {IParameterEnum.ExteriorInteriorStage.Print(true)}", extFrequencyFx.YSeries.Units, extFrequencyFx.YSeries.Label, extFrequencyFx.YSeries.Units);
        }
        private ITransformFunction ExteriorInteriorFunctionGenerator(ITransformFunction eiFx, double failureElevation)
        {
            List<ICoordinate> coordinates = new List<ICoordinate>();
            foreach (var pair in eiFx.Coordinates)
            {
                coordinates.Add(ICoordinateFactory.Factory(pair.X.Value(), pair.X.Value() < failureElevation ? 0.0 : pair.Y.Value()));
            }
            return ITransformFunctionFactory.Factory(IFunctionFactory.Factory(coordinates, eiFx.Interpolator), eiFx.ParameterType, eiFx.Label, eiFx.XSeries.Units, eiFx.XSeries.Label, eiFx.YSeries.Units, eiFx.YSeries.Label);
        }
        private ITransformFunction FailureAdjustedStageDamageFunctionGenerator(IFrequencyFunction frequencyFx, ITransformFunction stageDamageFx, ITransformFunction failureFx)
        {
            List<ICoordinate> coordinates = new List<ICoordinate>();
            foreach(var pair in frequencyFx.Coordinates)
            {
                coordinates.Add(ICoordinateFactory.Factory(pair.Y.Value(), stageDamageFx.F(pair.Y).Value() * failureFx.F(pair.Y).Value()));
            }
            return ITransformFunctionFactory.Factory(IFunctionFactory.Factory(coordinates, frequencyFx.Interpolator), IParameterEnum.InteriorStageDamage, stageDamageFx.Label, stageDamageFx.XSeries.Units, stageDamageFx.XSeries.Label, stageDamageFx.YSeries.Units, stageDamageFx.YSeries.Label);
        }
    }
}
