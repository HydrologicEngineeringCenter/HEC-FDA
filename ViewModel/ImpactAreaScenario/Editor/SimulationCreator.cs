using ead;
using Model;
using paireddata;
using Statistics;
using System.Collections.Generic;
using System.Linq;
using ViewModel.AggregatedStageDamage;
using ViewModel.FlowTransforms;
using ViewModel.FrequencyRelationships;
using ViewModel.GeoTech;
using ViewModel.StageTransforms;
using static ead.Simulation;

namespace ViewModel.ImpactAreaScenario.Editor
{
    public class SimulationCreator
    {
        private readonly AnalyticalFrequencyElement _FreqElem;
        private readonly InflowOutflowElement _InOutElem;
        private readonly RatingCurveElement _RatElem;
        private readonly ExteriorInteriorElement _ExtIntElem;
        private readonly LeveeFeatureElement _LeveeElem;
        private readonly AggregatedStageDamageElement _StageDamageElem;
        private readonly bool _UseInOut;
        private readonly bool _UseExtInt;
        private readonly bool _UseLevee;
        private readonly int _ImpactAreaID;

        public SimulationCreator(AnalyticalFrequencyElement freqElem, InflowOutflowElement inOutElem, RatingCurveElement ratElem,
            ExteriorInteriorElement extIntElem, LeveeFeatureElement levElem, AggregatedStageDamageElement stageDamElem, int currentImpactAreaID)
        {
            _FreqElem = freqElem;
            _InOutElem = inOutElem;
            _RatElem = ratElem;
            _ExtIntElem = extIntElem;
            _LeveeElem = levElem;
            _StageDamageElem = stageDamElem;

            _UseInOut = inOutElem != null;
            _UseExtInt = extIntElem != null;
            _UseLevee = levElem != null;

            _ImpactAreaID = currentImpactAreaID;
        }

        private IDistribution GetFrequencyDistribution()
        {
            IDistribution freqDistribution = _FreqElem.GetDistribution();
            //todo: using dummy flow-freq right now
            IDistribution flow_frequency = IDistributionFactory.FactoryUniform(0, 100000, 1000);
            return flow_frequency;
        }

        private List<StageDamageCurve> GetStageDamageCurves()
        {
            List<StageDamageCurve> stageDamageCurves = _StageDamageElem.Curves.Where(curve => curve.ImpArea.ID == _ImpactAreaID).ToList();
            return stageDamageCurves;
        }

        private List<UncertainPairedData> GetStageDamagesAsPairedData()
        {
            List<UncertainPairedData> stageDamages = new List<UncertainPairedData>();
            List<StageDamageCurve> stageDamageCurves = GetStageDamageCurves();
            foreach (StageDamageCurve curve in stageDamageCurves)
            {
                //todo: i don't like this. Is there an easier way to get a paired data from the stage damage curve.
                IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.InteriorStageDamage, curve.Function);
                stageDamages.Add(fdaFunction.ToUncertainPairedData());
            }
            return stageDamages;
        }

        public Simulation BuildSimulation()
        {
            SimulationBuilder sb = Simulation.builder()
                .withFlowFrequency(GetFrequencyDistribution())
                .withFlowStage(_RatElem.Curve.ToUncertainPairedData())
                .withStageDamages(GetStageDamagesAsPairedData());

            if(_UseInOut)
            {
                sb.withInflowOutflow(_InOutElem.Curve.ToUncertainPairedData());
            }
            if(_UseExtInt)
            {
                sb.withInteriorExterior(_ExtIntElem.Curve.ToUncertainPairedData());
            }
            if(_UseLevee)
            {
                sb.withLevee(_LeveeElem.Curve.ToUncertainPairedData());
            }

            return sb.build();
        }
    }
}
