using System.Collections.Generic;
using Statistics.Distributions;
using Statistics.GraphicalRelationships;
using Statistics;
using interfaces;
using System.Linq;
using System.Xml.Linq;
using System;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Enumerations;
namespace paireddata
{
    public class GraphicalUncertainPairedData : HEC.MVVMFramework.Base.Implementations.Validation, IPairedDataProducer, ICanBeNull, IReportMessage
    {
        #region Fields
        private int _EquivalentRecordLength;
        private double[] _ExceedanceProbabilities;
        private double[] _NonExceedanceProbabilities;
        private Statistics.ContinuousDistribution[] _StageOrLogFlowDistributions;
        private CurveMetaData _metaData;
        #endregion

        #region Properties
        public string XLabel
        {
            get { return _metaData.XLabel; }
        }
        public string YLabel
        {
            get { return _metaData.YLabel; }
        }
        public Statistics.ContinuousDistribution[] StageOrLogFlowDistributions
        {
            get
            {
                return _StageOrLogFlowDistributions;
            }
        }
        public string Name
        {
            get { return _metaData.Name; }
        }
        public string Category
        {
            get { return _metaData.Category; }
        }
        public bool IsNull
        {
            get { return _metaData.IsNull; }
        }
        public double[] ExceedanceProbabilities
        {
            get
            {
                return _ExceedanceProbabilities;
            }
        }
        public int EquivalentRecordLength
        {
            get
            {
                return _EquivalentRecordLength;
            }

        }
        public event MessageReportedEventHandler MessageReport;

        #endregion

        #region Constructors
        public GraphicalUncertainPairedData()
        {
            _metaData = new CurveMetaData();
            AddRules();
        }
        [Obsolete("This constructor is deprecated. Please use the constructor that accepts Curve Meta Data as an argument")]
        public GraphicalUncertainPairedData(double[] exceedanceProbabilities, double[] flowOrStageValues, int equivalentRecordLength, string xlabel, string ylabel, string name, bool usingStagesNotFlows = true, double maximumProbability = 0.9999, double minimumProbability = 0.0001)
        {
            Graphical graphical = new Graphical(exceedanceProbabilities, flowOrStageValues, equivalentRecordLength, usingStagesNotFlows, maximumProbability, minimumProbability);
            graphical.Validate();
            graphical.ComputeGraphicalConfidenceLimits();
            _ExceedanceProbabilities = graphical.ExceedanceProbabilities;
            _NonExceedanceProbabilities = ExceedanceToNonExceedance(graphical.ExceedanceProbabilities);
            _StageOrLogFlowDistributions = graphical.StageOrLogFlowDistributions;
            _EquivalentRecordLength = equivalentRecordLength;
            _metaData = new CurveMetaData(xlabel, ylabel, name, CurveTypesEnum.StrictlyMonotonicallyIncreasing);
            AddRules();

        }
        public GraphicalUncertainPairedData(double[] exceedanceProbabilities, double[] flowOrStageValues, int equivalentRecordLength, CurveMetaData curveMetaData, bool usingStagesNotFlows = true, double maximumProbability = 0.9999, double minimumProbability = 0.0001)
        {
            Graphical graphical = new Graphical(exceedanceProbabilities, flowOrStageValues, equivalentRecordLength, usingStagesNotFlows, maximumProbability, minimumProbability);
            graphical.Validate();
            graphical.ComputeGraphicalConfidenceLimits();
            _ExceedanceProbabilities = graphical.ExceedanceProbabilities;
            _NonExceedanceProbabilities = ExceedanceToNonExceedance(graphical.ExceedanceProbabilities);
            _StageOrLogFlowDistributions = graphical.StageOrLogFlowDistributions;
            _EquivalentRecordLength = equivalentRecordLength;
            _metaData = curveMetaData;
            AddRules();

        }
        #endregion

        #region Functions
        /// <summary>
        /// We have rules on monotonicity for non-exceedance probabilities. So we test for strict monotonic decreasing. 
        /// This means that the exceecance probabilities are strictly monotonically increasing
        /// Satisfying the curve type enum.
        /// </summary>
        private void AddRules()
        {
            switch (_metaData.CurveType)
            {
                case CurveTypesEnum.StrictlyMonotonicallyIncreasing:
                    AddSinglePropertyRule(nameof(_NonExceedanceProbabilities), new Rule(() => IsArrayValid(_NonExceedanceProbabilities, (a, b) => (a > b)), "X must be strictly monotonically decreasing"));
                    break;
                default:
                    break;
            }

        }
        private bool IsArrayValid(double[] arrayOfData, Func<double, double, bool> comparison)
        {
            if (arrayOfData == null) return false;
            for (int i = 0; i < arrayOfData.Length - 1; i++)
            {
                if (comparison(arrayOfData[i], arrayOfData[i + 1]))
                {
                    return false;
                }
            }
            return true;
        }
        private double[] ExceedanceToNonExceedance(double[] exceedanceProbabilities)
        {
            double[] nonExceedanceProbabilities = new double[exceedanceProbabilities.Length];
            for (int i = 0; i < nonExceedanceProbabilities.Length; i++)
            {
                nonExceedanceProbabilities[i] = 1 - exceedanceProbabilities[i];
            }
            return nonExceedanceProbabilities;
        }

        public IPairedData SamplePairedData(double probability)
        {
            double[] y = new double[_StageOrLogFlowDistributions.Length];
            for (int i = 0; i < _NonExceedanceProbabilities.Length; i++)
            {
                y[i] = _StageOrLogFlowDistributions[i].InverseCDF(probability);
            }
            PairedData pairedData = new PairedData(_NonExceedanceProbabilities, y, _metaData);
            pairedData.Validate();
            if (pairedData.HasErrors)
            {
                if (pairedData.RuleMap[nameof(pairedData.Yvals)].ErrorLevel > ErrorLevel.Unassigned)
                {
                    pairedData.ForceMonotonic();
                    ReportMessage(this, new MessageEventArgs(new Message("Sampled Y Values were not monotonically increasing as required and were forced to be monotonic")));
                }
                if (pairedData.RuleMap[nameof(pairedData.Xvals)].ErrorLevel > ErrorLevel.Unassigned)
                {
                    ReportMessage(this, new MessageEventArgs(new Message("X values are not monotonically decreasing as required")));
                }
                pairedData.Validate();
                if (pairedData.HasErrors)
                {
                    //TODO: do something
                }
            }
            return pairedData;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }

        #endregion
    }
}
