using compute;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using metrics;
using paireddata;
using Statistics;
using System.Collections.Generic;
using System.Linq;
using static compute.ImpactAreaScenarioSimulation;

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
            if (stageDamageCurves.Count == 0)
            {
                //todo: maybe get the impact area name for this message?
                vr.AddErrorMessage("The aggregated stage damage element '" + _StageDamageElem.Name + "' did not contain any curves that are associated " +
                    "with the impact area.");
            }
            return vr;
        }

        private void LoadSimulationBuilder()
        {
            _SimulationBuilder = ImpactAreaScenarioSimulation.builder(_ImpactAreaID)
            .withStageDamages(GetStageDamagesAsPairedData());
             
            if (_FreqElem.IsAnalytical)
            {
                _SimulationBuilder.withFlowFrequency(GetFrequencyDistribution());
                _SimulationBuilder.withFlowStage(_RatElem.ComputeComponentVM.SelectedItemToPairedData());
            }
            else
            {
                if(_FreqElem.MyGraphicalVM.UseFlow == true)
                {
                    _SimulationBuilder.withFlowFrequency(_FreqElem.MyGraphicalVM.ToGraphicalUncertainPairedData());
                    _SimulationBuilder.withFlowStage(_RatElem.ComputeComponentVM.SelectedItemToPairedData());
                }
                else
                {
                    _SimulationBuilder.withFrequencyStage(_FreqElem.MyGraphicalVM.ToGraphicalUncertainPairedData());
                }
            }
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
                if(_LeveeElem.IsDefaultCurveUsed)
                {
                    _SimulationBuilder.withLevee(_LeveeElem.CreateDefaultCurve(), _LeveeElem.Elevation);
                }
                else
                {
                    _SimulationBuilder.withLevee(_LeveeElem.ComputeComponentVM.SelectedItemToPairedData(), _LeveeElem.Elevation);
                }
            }
        }

        private ContinuousDistribution GetFrequencyDistribution()
        {
            return _FreqElem.CreateAnalyticalLP3Distribution();
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
                bool allZeroes = IsCurveYValuesAllZero(curve);
                if(!allZeroes)
                {
                    UncertainPairedData upd = curve.ComputeComponent.SelectedItemToPairedData(curve.DamCat);
                    stageDamages.Add(upd);
                }
            }

            return stageDamages;
        }

        private bool IsCurveYValuesAllZero(StageDamageCurve curve)
        {
            bool allZeroes = true;
            IDistribution[] yvals = curve.ComputeComponent.SelectedItemToPairedData().Yvals;
            List<double> ys = new List<double>();
            foreach (IDistribution yval in yvals)
            {
                ys.Add(yval.InverseCDF(.5));
            }

            foreach (double y in ys)
            {
                if (y != 0)
                {
                    allZeroes = false;
                    break;
                }
            }
            return allZeroes;
        }

        public ImpactAreaScenarioSimulation BuildSimulation()
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
