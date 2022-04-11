using System.Collections.Generic;
using Statistics.Distributions;
using Statistics.GraphicalRelationships;
using interfaces;
using System.Linq;
using System.Xml.Linq;
using System;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Implementations;

namespace paireddata
{
    public class GraphicalUncertainPairedData : HEC.MVVMFramework.Base.Implementations.Validation, IPairedDataProducer, ICanBeNull
    {
        #region Fields
        private int _EquivalentRecordLength;
        private double[] _ExceedanceProbabilities;
        private Normal[] _NonMontonicDistributions;
        private Normal[] _DistributionsMonotonicFromAbove;
        private Normal[] _DistributionsMonotonicFromBelow;
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
        public Normal[] NonMonotonicDistributions
        {
            get
            {
                return _NonMontonicDistributions;
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
        #endregion

        #region Constructors
        public GraphicalUncertainPairedData()
        {
            _metaData = new CurveMetaData();
            AddRules();
        }

        public GraphicalUncertainPairedData(double[] exceedanceProbabilities, double[] flowOrStageValues, int equivalentRecordLength, string xlabel, string ylabel, string name, bool usingStagesNotFlows = true, double maximumProbability = 0.9999, double minimumProbability = 0.0001)
        {
            Graphical graphical = new Graphical(exceedanceProbabilities, flowOrStageValues, equivalentRecordLength, usingStagesNotFlows, maximumProbability, minimumProbability);
            graphical.Validate();
            graphical.ComputeGraphicalConfidenceLimits();
            _ExceedanceProbabilities = graphical.ExceedanceProbabilities;
            _NonMontonicDistributions = graphical.StageOrLogFlowDistributions;
            _DistributionsMonotonicFromAbove = MakeMeMonotonicFromAbove(_NonMontonicDistributions);
            _DistributionsMonotonicFromBelow = MakeMeMonotonicFromBelow(_NonMontonicDistributions);
            _EquivalentRecordLength = equivalentRecordLength;
            _metaData = new CurveMetaData(xlabel, ylabel, name);
            AddRules();

        }
        #endregion

        #region Functions
        /// <summary>
        /// We have rules on monotonicity for exceedance probabilities. We expect the flow or stage distributions 
        /// to have situations where monotonicity is not satisfied. 
        /// Using monotonic decreasing because we use exceedance probabilities
        /// </summary>
        private void AddRules()
        {
            switch (_metaData.CurveType)
            {
                case CurveTypesEnum.StrictlyMonotonicallyDecreasing:
                    AddSinglePropertyRule(nameof(ExceedanceProbabilities), new Rule(() => IsArrayValid(ExceedanceProbabilities, (a, b) => (a >= b)), "X must be strictly monotonically decreasing"));
                    break;
                case CurveTypesEnum.MonotonicallyDecreasing:
                    AddSinglePropertyRule(nameof(ExceedanceProbabilities), new Rule(() => IsArrayValid(ExceedanceProbabilities, (a, b) => (a > b)), "X must be monotonically decreasing"));
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

        public IPairedData SamplePairedData(double probability)
        {
            double[] y = new double[_NonMontonicDistributions.Length];
            if (probability > 0.5)
            {
                for (int i = 0; i < _ExceedanceProbabilities.Length; i++)
                {
                    y[i] = _DistributionsMonotonicFromAbove[i].InverseCDF(probability);
                }
            }
            else
            {
                for (int i = 0; i < _ExceedanceProbabilities.Length; i++)
                {
                    y[i] = _DistributionsMonotonicFromBelow[i].InverseCDF(probability);
                }
            }
            PairedData pairedData = new PairedData(_ExceedanceProbabilities, y, _metaData);
            pairedData.Validate();
            if (pairedData.HasErrors)
            {
                if (pairedData.RuleMap[nameof(pairedData.Yvals)].ErrorLevel > ErrorLevel.Unassigned)
                {
                    pairedData.ForceMonotonic();
                }
                if (pairedData.RuleMap[nameof(pairedData.Xvals)].ErrorLevel > ErrorLevel.Unassigned)
                {
                    Array.Sort(pairedData.Xvals);//bad news.
                }
                pairedData.Validate();
                if (pairedData.HasErrors)
                {
                    //TODO: do something
                }
            }
            return pairedData;
        }
        private Normal[] MakeMeMonotonicFromBelow(Normal[] flowOrStageDistributions)
        {
                double[] probsForChecking = new double[] { .45, .2, .1, .05, .02, .01, .005, .002 };
                Normal[] monotonicDistributionArray = new Normal[flowOrStageDistributions.Length];
                monotonicDistributionArray[0] = flowOrStageDistributions[0];

                for (int j = 0; j < probsForChecking.Length; j++)
                {
                    double q = probsForChecking[j];
                    double qComplement = 1 - q;
                    for (int i = 1; i < flowOrStageDistributions.Length; i++)
                    {
                        double lowerBoundPreviousDistribution = monotonicDistributionArray[i - 1].InverseCDF(q);
                        double lowerBoundCurrentDistribution = flowOrStageDistributions[i].InverseCDF(q);
                        double lowerBoundDifference = lowerBoundCurrentDistribution - lowerBoundPreviousDistribution;

                        if (lowerBoundDifference < 0)
                        {
                            monotonicDistributionArray[i] = new Normal(flowOrStageDistributions[i].Mean, monotonicDistributionArray[i - 1].StandardDeviation, flowOrStageDistributions[i].SampleSize);
                        }
                        else
                        {
                            monotonicDistributionArray[i] = flowOrStageDistributions[i];
                        }
                    }
                }
                return monotonicDistributionArray;
            }

        private Normal[] MakeMeMonotonicFromAbove(Normal[] flowOrStageDistributions)
        {
            double[] probsForChecking = new double[] { .55, .8, .9, .95, .98, .99, .995, .998 };
            Normal[] monotonicDistributionArray = new Normal[flowOrStageDistributions.Length];
            monotonicDistributionArray[0] = flowOrStageDistributions[0];

            for (int j = 0; j < probsForChecking.Length; j++)
            {
                double q = probsForChecking[j];
                for (int i = 1; i < flowOrStageDistributions.Length; i++)
                {

                    double upperBoundPreviousDistribution = monotonicDistributionArray[i - 1].InverseCDF(q);
                    double upperBoundCurrentDistribution = flowOrStageDistributions[i].InverseCDF(q);
                    double upperBoundDifference = upperBoundCurrentDistribution - upperBoundPreviousDistribution;

                    if (upperBoundDifference < 0)
                    {
                        monotonicDistributionArray[i] = new Normal(flowOrStageDistributions[i].Mean, monotonicDistributionArray[i - 1].StandardDeviation, flowOrStageDistributions[i].SampleSize);
                    }
                    else
                    {
                        monotonicDistributionArray[i] = flowOrStageDistributions[i];
                    }
                }
            }
            return monotonicDistributionArray;
        }
        #endregion
    }
}
