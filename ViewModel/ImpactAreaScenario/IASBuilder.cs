using ViewModel.Utilities;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario
{
    internal class IASBuilder
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

        private IMetricEnum _metricType;
        private double _thresholdValue;

        private int _impactAreaID;
        private int _flowFreqID;
        private int _inflowOutflowID;
        private int _ratingID;
        private int _leveeFailureID;
        private int _extIntStageID;
        private int _stageDamageID;
        public IASBuilder(string name, string desc, int analysisYear, int impactAreaID,  IMetricEnum metricType, double thresholdValue)
        {
            _name = name;
            _description = desc;
            _analysisYear = analysisYear;
            _impactAreaID = impactAreaID;
            _metricType = metricType;
            _thresholdValue = thresholdValue;
            _usesThreshold = true;
        }

        public IASBuilder WithAnalyticalFreqElem(FrequencyRelationships.AnalyticalFrequencyElement analyticalFreqElem)
        {
            _usesAnalyticalFlowFreq = true;
            _flowFreqID = analyticalFreqElem.GetElementID();
            return this;
        }

        public IASBuilder WithInflowOutflowElem(FlowTransforms.InflowOutflowElement inflowOutflowElem)
        {
            _usesInflowOutflow = true;
            _inflowOutflowID = inflowOutflowElem.GetElementID();
            return this;
        }

        public IASBuilder WithRatingCurveElem(StageTransforms.RatingCurveElement ratingElem)
        {
            _usesRating = true;
            _ratingID = ratingElem.GetElementID();
            return this;
        }

        public IASBuilder WithExtIntStageElem(StageTransforms.ExteriorInteriorElement extIntElem)
        {
            _usesExtIntStage = true;
            _extIntStageID = extIntElem.GetElementID();
            return this;
        }

        public IASBuilder WithLevee(GeoTech.LeveeFeatureElement leveeElem)
        {
            _usesLevee = true;
            _leveeFailureID = leveeElem.GetElementID();
            return this;
        }



        public IASBuilder WithAggStageDamageElem(AggregatedStageDamage.AggregatedStageDamageElement stageDamageElem)
        {
            _usesAggStageDamage = true;
            _stageDamageID = stageDamageElem.GetElementID();
            return this;
        }

        //public ConditionBuilder WithThreshold(string thresholdType, double thresholdValue)
        //{
        //    _usesThreshold = true;
        //    _thresholdType = thresholdType;
        //    _thresholdValue = thresholdValue;
        //    return this;
        //}

        public IASElement build()
        {
            return new IASElement(_name, _description,_analysisYear,_impactAreaID,_flowFreqID,
                _inflowOutflowID,_ratingID,_extIntStageID ,_leveeFailureID, _stageDamageID ,_metricType,_thresholdValue);
        }

    }
}
