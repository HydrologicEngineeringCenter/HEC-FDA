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
using metrics;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.Base.Events;

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

        private SimulationBuilder _SimulationBuilder;

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

            LoadSimulationBuilder();
        }

        public FdaValidationResult IsConfigurationValid()
        {
            FdaValidationResult vr = IsStageDamageValid();

            return vr;
        }
        private FdaValidationResult IsStageDamageValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            //stage damages
            List<StageDamageCurve> stageDamageCurves = _StageDamageElem.Curves.Where(curve => curve.ImpArea.ID == _ImpactAreaID).ToList();
            if(stageDamageCurves.Count == 0)
            {
                //todo: maybe get the impact area name for this message?
                vr.AddErrorMessage("The aggregated stage damage element '" + _StageDamageElem.Name + "' did not contain any curves that are associated " +
                    "with the impact area.");
            }
            return vr;
        }

        private void LoadSimulationBuilder()
        {
            _SimulationBuilder = Simulation.builder()
                .withFlowFrequency(GetFrequencyDistribution())
                .withFlowStage(_RatElem.ComputeComponentVM.SelectedItemToPairedData())
                .withStageDamages(GetStageDamagesAsPairedData());

            if (_UseInOut)
            {
                _SimulationBuilder.withInflowOutflow(_InOutElem.ComputeComponentVM.SelectedItemToPairedData());
            }
            if (_UseExtInt)
            {
                _SimulationBuilder.withInteriorExterior(_ExtIntElem.ComputeComponentVM.SelectedItemToPairedData());
            }
            if (_UseLevee)
            {
                _SimulationBuilder.withLevee(_LeveeElem.ComputeComponentVM.SelectedItemToPairedData(), _LeveeElem.Elevation);
            }
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
                stageDamages.Add(curve.ComputeComponent.SelectedItemToPairedData(curve.DamCat));
            }
            return stageDamages;
        }

        public Simulation BuildSimulation()
        {
            return _SimulationBuilder.build();
        }

        public void WithAdditionalThresholds(List<Threshold> additionalThresholds)
        {
            foreach (Threshold threshold in additionalThresholds)
            {
                _SimulationBuilder.withAdditionalThreshold(threshold);
            }
        }

        public void WithAdditionalThreshold(Threshold additionalThreshold)
        {
            _SimulationBuilder.withAdditionalThreshold(additionalThreshold);
        }

    }
}
