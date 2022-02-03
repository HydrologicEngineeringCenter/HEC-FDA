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
        private double[] _ExceedanceProbabilities;
        private Normal[] _FlowOrStageDistributions;
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
        public double[] ExceedanceProbabilities() //why do we have the () here?
        {
            return _ExceedanceProbabilities;
        }
        public Normal[] FlowOrStageDistributions()
        {
            return _FlowOrStageDistributions;
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
            _FlowOrStageDistributions = graphical.FlowOrStageDistributions;
            _metaData = new CurveMetaData(xlabel, ylabel, name, description);
        }




        #region Functions
        IPairedData IPairedDataProducer.SamplePairedData(double probability)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
