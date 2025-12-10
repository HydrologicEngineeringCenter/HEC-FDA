using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.LifeLoss;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using Statistics;
using System.Collections.Generic;
using System.Linq;
using static HEC.FDA.Model.compute.ImpactAreaScenarioSimulation;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class SimulationCreator
    {
        private readonly FrequencyElement _FreqElem;
        private readonly InflowOutflowElement _InOutElem;
        private readonly StageDischargeElement _RatElem;
        private readonly ExteriorInteriorElement _ExtIntElem;
        private readonly LateralStructureElement _LeveeElem;
        private readonly bool _UseInOut;
        private readonly bool _UseExtInt;
        private readonly bool _UseLevee;
        private readonly int _ImpactAreaID;
        private readonly bool _HasFailureStageDamage;
        private readonly AggregatedStageDamageElement _StageDamageElem;
        private readonly bool _HasNonFailureStageDamage;
        private readonly AggregatedStageDamageElement _NonFailureStageDamageElem;
        private readonly bool _HasFailureStageLifeLoss;
        private readonly StageLifeLossElement _FailureStageLifeLossElement;
        private readonly bool _HasNonFailureStageLifeLoss;
        private readonly StageLifeLossElement _NonFailureStageLifeLossElement;


        private SimulationBuilder _SimulationBuilder;

        public SimulationCreator(FrequencyElement freqElem, InflowOutflowElement inOutElem, StageDischargeElement ratElem,
            ExteriorInteriorElement extIntElem, LateralStructureElement levElem, int currentImpactAreaID,
            bool hasFailureStageDamage, AggregatedStageDamageElement stageDamElem,
            bool hasNonFailureStageDamage, AggregatedStageDamageElement nonFailureStageDamElem,
            bool hasFailureStageLifeLoss, StageLifeLossElement failureStageLifeLossElem,
            bool hasNonFailureStageLifeLoss, StageLifeLossElement nonFailureStageLifeLossELem)
        {
            _FreqElem = freqElem;
            _InOutElem = inOutElem;
            _RatElem = ratElem;
            _ExtIntElem = extIntElem;
            _LeveeElem = levElem;

            _UseInOut = inOutElem != null;
            _UseExtInt = extIntElem != null;
            _UseLevee = levElem != null;

            _ImpactAreaID = currentImpactAreaID;

            _HasFailureStageDamage = hasFailureStageDamage;
            _StageDamageElem = stageDamElem;
            _HasNonFailureStageDamage = hasNonFailureStageDamage;
            _NonFailureStageDamageElem = nonFailureStageDamElem;
            _HasFailureStageLifeLoss = hasFailureStageLifeLoss;
            _FailureStageLifeLossElement = failureStageLifeLossElem;
            _HasNonFailureStageLifeLoss = hasNonFailureStageLifeLoss;
            _NonFailureStageLifeLossElement = nonFailureStageLifeLossELem;

            LoadSimulationBuilder();
        }

        public FdaValidationResult IsConfigurationValid()
        {
            FdaValidationResult vr = IsStageDamageValid();
            if (_HasNonFailureStageDamage)
            {
                vr.AddErrorMessage(IsNonFailureStageDamageValid().ErrorMessage);
            }
            return vr;
        }

        private FdaValidationResult IsStageDamageValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (_StageDamageElem == null)
            {
                vr.AddErrorMessage("The selected aggregated stage damage element no longer exists. Edit the senario and select a new one.");
            }
            else
            {
                List<StageDamageCurve> stageDamageCurves = _StageDamageElem.Curves.Where(curve => curve.ImpArea.ID == _ImpactAreaID).ToList();
                if (stageDamageCurves.Count == 0)
                {
                    vr.AddErrorMessage("The aggregated stage damage element '" + _StageDamageElem.Name + "' did not contain any curves that are associated " +
                        "with the impact area.");
                }
            }
            return vr;
        }

        private FdaValidationResult IsNonFailureStageDamageValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (_NonFailureStageDamageElem == null)
            {
                vr.AddErrorMessage("The selected non-failure stage damage element no longer exists. Edit the senario and select a new one.");
            }
            else
            {
                List<StageDamageCurve> stageDamageCurves = _NonFailureStageDamageElem.Curves.Where(curve => curve.ImpArea.ID == _ImpactAreaID).ToList();
                if (stageDamageCurves.Count == 0)
                {
                    vr.AddErrorMessage("The non-failure stage damage element '" + _NonFailureStageDamageElem.Name + "' did not contain any curves that are associated " +
                        "with the impact area.");
                }
            }
            return vr;
        }

        private void LoadSimulationBuilder()
        {
            _SimulationBuilder = ImpactAreaScenarioSimulation.Builder(_ImpactAreaID);

            if (_HasFailureStageDamage)
            {
                _SimulationBuilder.WithStageDamages(GetStageDamagesAsPairedData(_StageDamageElem));
            }

            if (_HasNonFailureStageDamage)
            {
                _SimulationBuilder.WithNonFailureStageDamage(GetStageDamagesAsPairedData(_NonFailureStageDamageElem));
            }

            if (_HasFailureStageLifeLoss)
            {
                _SimulationBuilder.WithFakeStageLifeLoss();
            }

            if (_FreqElem.IsAnalytical)
            {
                _SimulationBuilder.WithFlowFrequency(GetFrequencyDistribution());
                _SimulationBuilder.WithFlowStage(_RatElem.CurveComponentVM.SelectedItemToPairedData());
            }
            else
            {
                if (_FreqElem.GraphicalUsesFlow == true)
                {
                    _SimulationBuilder.WithFlowFrequency(_FreqElem.GraphicalUncertainPairedData);
                    _SimulationBuilder.WithFlowStage(_RatElem.CurveComponentVM.SelectedItemToPairedData());
                }
                else
                {
                    _SimulationBuilder.WithFrequencyStage(_FreqElem.GraphicalUncertainPairedData);
                }
            }
            if (_UseInOut)
            {
                _SimulationBuilder.WithInflowOutflow(_InOutElem.CurveComponentVM.SelectedItemToPairedData());
            }
            if (_UseExtInt)
            {
                _SimulationBuilder.WithInteriorExterior(_ExtIntElem.CurveComponentVM.SelectedItemToPairedData());
            }
            if (_UseLevee)
            {
                if (_LeveeElem.IsDefaultCurveUsed)
                {
                    _SimulationBuilder.WithLevee(_LeveeElem.CreateDefaultCurve(), _LeveeElem.Elevation);
                }
                else
                {
                    _SimulationBuilder.WithLevee(_LeveeElem.CurveComponentVM.SelectedItemToPairedData(), _LeveeElem.Elevation);
                }
            }
        }

        private ContinuousDistribution GetFrequencyDistribution()
        {
            return _FreqElem.LPIII;
        }

        private List<StageDamageCurve> GetStageDamageCurves(AggregatedStageDamageElement stageDamageElement)
        {
            List<StageDamageCurve> stageDamageCurves = new List<StageDamageCurve>();
            if (stageDamageElement != null)
            {
                stageDamageCurves = stageDamageElement.Curves.Where(curve => curve.ImpArea.ID == _ImpactAreaID).ToList();
            }
            return stageDamageCurves;
        }

        // returns an empty list if no damages
        private List<UncertainPairedData> GetStageDamagesAsPairedData(AggregatedStageDamageElement stageDamageElement)
        {
            List<UncertainPairedData> stageDamages = new List<UncertainPairedData>();
            List<StageDamageCurve> stageDamageCurves = GetStageDamageCurves(stageDamageElement);
            foreach (StageDamageCurve curve in stageDamageCurves)
            {
                bool allZeroes = IsCurveYValuesAllZero(curve);
                if (!allZeroes)
                {
                    UncertainPairedData upd = curve.ComputeComponent.SelectedItemToPairedData(curve.DamCat, curve.AssetCategory);
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
            return _SimulationBuilder.Build();
        }

        public void WithAdditionalThreshold(Threshold additionalThreshold)
        {
            _SimulationBuilder.WithAdditionalThreshold(additionalThreshold);
        }

    }
}
