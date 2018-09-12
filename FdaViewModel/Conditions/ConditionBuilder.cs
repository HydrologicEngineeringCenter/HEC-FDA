using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Conditions
{
    class ConditionBuilder
    {
        private string _name;
        private string _description;
        private int _analysisYear;
        private ImpactArea.ImpactAreaElement _impactAreaElem;
        private ImpactArea.ImpactAreaRowItem _indexLocation;

        private bool _usesAnalyticalFlowFreq = false;
        private FrequencyRelationships.AnalyticalFrequencyElement _analyticalFreqElem;

        private bool _usesInflowOutflow = false;
        private FlowTransforms.InflowOutflowElement _inflowOutflowElem;

        private bool _usesRating = false;
        private StageTransforms.RatingCurveElement _ratingElem;

        private bool _usesExtIntStage = false;
        private StageTransforms.ExteriorInteriorElement _extIntElem;

        private bool _usesLevee = false;
        private GeoTech.LeveeFeatureElement _leveeElem;

        private bool _usesFailureFunction = false;
        private GeoTech.FailureFunctionElement _failureFunctionElem;

        private bool _usesAggStageDamage = false;
        private AggregatedStageDamage.AggregatedStageDamageElement _stageDamageElem;

        private bool _usesThreshold = false;
        private string _thresholdType;
        private double _thresholdValue;

        private OwnerElement _owner;

        public ConditionBuilder(string name, string desc, int analysisYear, ImpactArea.ImpactAreaElement impactAreaElem, 
            ImpactArea.ImpactAreaRowItem indexLocation, OwnerElement owner)
        {
            _name = name;
            _description = desc;
            _analysisYear = analysisYear;
            _impactAreaElem = impactAreaElem;
            _indexLocation = indexLocation;
            _owner = owner;
        }

        public ConditionBuilder WithAnalyticalFreqElem(FrequencyRelationships.AnalyticalFrequencyElement analyticalFreqElem)
        {
            _usesAnalyticalFlowFreq = true;
            _analyticalFreqElem = analyticalFreqElem;
            return this;
        }

        public ConditionBuilder WithInflowOutflowElem(FlowTransforms.InflowOutflowElement inflowOutflowElem)
        {
            _usesInflowOutflow = true;
            _inflowOutflowElem = inflowOutflowElem;
            return this;
        }

        public ConditionBuilder WithRatingCurveElem(StageTransforms.RatingCurveElement ratingElem)
        {
            _usesRating = true;
            _ratingElem = ratingElem;
            return this;
        }

        public ConditionBuilder WithExtIntStageElem(StageTransforms.ExteriorInteriorElement extIntElem)
        {
            _usesExtIntStage = true;
            _extIntElem = extIntElem;
            return this;
        }

        public ConditionBuilder WithLevee(GeoTech.LeveeFeatureElement leveeElem)
        {
            _usesLevee = true;
            _leveeElem = leveeElem;
            return this;
        }

        public ConditionBuilder WithFailureFunctionElem(GeoTech.FailureFunctionElement failureFunctionElem)
        {
            _usesFailureFunction = true;
            _failureFunctionElem = failureFunctionElem;
            return this;
        }

        public ConditionBuilder WithAggStageDamageElem(AggregatedStageDamage.AggregatedStageDamageElement stageDamageElem)
        {
            _usesAggStageDamage = true;
            _stageDamageElem = stageDamageElem;
            return this;
        }

        public ConditionBuilder WithThreshold(string thresholdType, double thresholdValue)
        {
            _usesThreshold = true;
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            return this;
        }

        public ConditionsElement build()
        {
            return new ConditionsElement(_name, _description,_analysisYear,_impactAreaElem,_indexLocation,_usesAnalyticalFlowFreq,
                _analyticalFreqElem,_usesInflowOutflow,_inflowOutflowElem,_usesRating,_ratingElem,_usesExtIntStage,_extIntElem,
                _usesLevee,_leveeElem,_usesFailureFunction,_failureFunctionElem,_usesAggStageDamage,_stageDamageElem,
                _usesThreshold,_thresholdType,_thresholdValue,_owner);
        }

    }
}
