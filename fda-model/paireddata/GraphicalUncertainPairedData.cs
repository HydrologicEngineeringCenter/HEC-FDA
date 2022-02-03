using System.Collections.Generic;
using Statistics.Distributions;
using Statistics.GraphicalRelationships;
using interfaces;
using System.Linq;
using System.Xml.Linq;
using System;

namespace paireddata
{
    class GraphicalUncertainPairedData : IPairedDataProducer, ICanBeNull
    {
        #region Fields
        private int _EquivalentRecordLength;
        private double[] _ExceedanceProbabilities;
        private Normal[] _UnrevisedDistributions;
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
        public string Name
        {
            get { return _metaData.Name; }
        }
        public string Description
        {
            get { return _metaData.Description; }
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
        }

        public GraphicalUncertainPairedData(double[] exceedanceProbabilities, double[] flowOrStageValues, int equivalentRecordLength, string xlabel, string ylabel, string name, string description, double maximumProbability = 0.9999, double minimumProbability = 0.0001, bool usingFlows = false, bool flowsAreNotLogged = false)
        {
            Graphical graphical = new Graphical(exceedanceProbabilities, flowOrStageValues, equivalentRecordLength, maximumProbability, minimumProbability, usingFlows, flowsAreNotLogged);
            graphical.ComputeGraphicalConfidenceLimits();
            _ExceedanceProbabilities = graphical.ExceedanceProbabilities;
            _UnrevisedDistributions = graphical.FlowOrStageDistributions;
            _DistributionsMonotonicFromAbove = MakeMeMonotonicFromAbove(_UnrevisedDistributions);
            _DistributionsMonotonicFromBelow = MakeMeMonotonicFromBelow(_UnrevisedDistributions);
            _EquivalentRecordLength = equivalentRecordLength;
            _metaData = new CurveMetaData(xlabel, ylabel, name, description);
        }
        #endregion

        #region Functions
        IPairedData IPairedDataProducer.SamplePairedData(double probability)
        {
            double[] y = new double[_UnrevisedDistributions.Length];
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
                if (pairedData.RuleMap[nameof(pairedData.Yvals)].ErrorLevel > Base.Enumerations.ErrorLevel.Unassigned)
                {
                    Array.Sort(pairedData.Yvals);//sorts but doesnt solve the problem of repeated values.
                }
                if (pairedData.RuleMap[nameof(pairedData.Xvals)].ErrorLevel > Base.Enumerations.ErrorLevel.Unassigned)
                {
                    Array.Sort(pairedData.Xvals);//bad news.
                }
                pairedData.Validate();
                if (pairedData.HasErrors)
                {
                    // throw new Exception("the produced paired data is not monotonically increasing.");
                }
            }
            return pairedData;
        }
        private Normal[] MakeMeMonotonicFromBelow(Normal[] flowOrStageDistributions)
        {
            throw new NotImplementedException();
        }

        private Normal[] MakeMeMonotonicFromAbove(Normal[] flowOrStageDistributions)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
