using compute;
using paireddata;
using Statistics;
using System.Collections.Generic;
using System.Linq;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.StageTransforms;
using static compute.Simulation;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
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

        private ContinuousDistribution GetFrequencyDistribution()
        {
            return _FreqElem.GetDistribution();
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
                stageDamages.Add(curve.Function);
            }
            return stageDamages;
        }

        public Simulation BuildSimulation()
        {
            //todo: cody commented out on 2/21/22

            //SimulationBuilder sb = Simulation.builder()
            //    .withFlowFrequency(GetFrequencyDistribution())
            //    .withFlowStage(_RatElem.Curve)
            //    .withStageDamages(GetStageDamagesAsPairedData());

            //if(_UseInOut)
            //{
            //    sb.withInflowOutflow(_InOutElem.Curve);
            //}
            //if(_UseExtInt)
            //{
            //    sb.withInteriorExterior(_ExtIntElem.Curve);
            //}
            //if(_UseLevee)
            //{
            //    sb.withLevee(_LeveeElem.Curve, _LeveeElem.Elevation);
            //}

            //return sb.build();

            return null;
        }
    }
}
