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
    public class GraphicalUncertainPairedData : HEC.MVVMFramework.Base.Implementations.Validation, IPairedDataProducer, ICanBeNull, IReportMessage, IMetaData
    {
        #region Fields
        private int _EquivalentRecordLength;
        private double[] _ExceedanceProbabilities;
        private double[] _NonExceedanceProbabilities;
        private Statistics.ContinuousDistribution[] _StageOrLogFlowDistributions;
        private CurveMetaData _metaData;
        private bool _UsingStagesNotFlows;
        private double _MaximumProbability;
        private double _MinimumProbability;

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

        public string DamageCategory
        {
            get { return _metaData.DamageCategory; }
        }
        public string AssetCategory
        {
            get { return _metaData.AssetCategory; }
        }
        public bool IsNull
        {
            get { return _metaData.IsNull; }
        }
        public CurveMetaData CurveMetaData
        {
            get
            {
                return _metaData;
            }
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
        [Obsolete("This constructor is deprecated. Construct a CurveMetaData, then inject into constructor")]
        public GraphicalUncertainPairedData(double[] exceedanceProbabilities, double[] flowOrStageValues, int equivalentRecordLength, string xlabel, string ylabel, string name, bool usingStagesNotFlows = true, double maximumProbability = 0.9999, double minimumProbability = 0.0001)
        {
            _UsingStagesNotFlows = usingStagesNotFlows;
            _MaximumProbability = maximumProbability;
            _MinimumProbability = minimumProbability;
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
            _UsingStagesNotFlows = usingStagesNotFlows;
            _MaximumProbability = maximumProbability;
            _MinimumProbability = minimumProbability;
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
        private GraphicalUncertainPairedData(double[] exceedanceProbabilities, Normal[] flowOrStageDistributions, int equivalentRecordLength, CurveMetaData curveMetaData, bool usingStagesNotFlows = true, double maximumProbability = 0.9999, double minimumProbability = 0.0001)
        {
            _UsingStagesNotFlows = usingStagesNotFlows;
            _MaximumProbability = maximumProbability;
            _MinimumProbability = minimumProbability;
            _ExceedanceProbabilities = exceedanceProbabilities;
            _NonExceedanceProbabilities = ExceedanceToNonExceedance(exceedanceProbabilities);
            _StageOrLogFlowDistributions = flowOrStageDistributions;
            _EquivalentRecordLength = equivalentRecordLength;
            _metaData = curveMetaData;
            AddRules();
        }
        #endregion

        #region Methods
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
        public bool Equals(GraphicalUncertainPairedData incomingGraphicalUncertainPairedData)
        {
            bool nullMatches = CurveMetaData.IsNull.Equals(incomingGraphicalUncertainPairedData.CurveMetaData.IsNull);
            if(nullMatches && IsNull)
            {
                return true;
            }
            bool nameIsTheSame = Name.Equals(incomingGraphicalUncertainPairedData.Name);
            bool erlIsTheSame = EquivalentRecordLength.Equals(incomingGraphicalUncertainPairedData.EquivalentRecordLength);
            if (!nameIsTheSame || !erlIsTheSame)
            {
                return false;
            }
            for (int i = 0; i < _ExceedanceProbabilities.Length; i++)
            {
                bool probabilityIsTheSame = _ExceedanceProbabilities[i].Equals(incomingGraphicalUncertainPairedData.ExceedanceProbabilities[i]);
                bool distributionIsTheSame = _StageOrLogFlowDistributions[i].Equals(incomingGraphicalUncertainPairedData.StageOrLogFlowDistributions[i]);
                if(!probabilityIsTheSame || !distributionIsTheSame)
                {
                    return false;
                }
            }
            return true;
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Graphical_Uncertain_Paired_Data");
            XElement curveMetaDataElement = CurveMetaData.WriteToXML();
            curveMetaDataElement.Name = "CurveMetaData";
            masterElement.Add(curveMetaDataElement);
            if(CurveMetaData.IsNull)
            {
                return masterElement;
            }
            else
            {
                masterElement.SetAttributeValue("Ordinate_Count", _ExceedanceProbabilities.Length);
                masterElement.SetAttributeValue("ERL", EquivalentRecordLength);
                masterElement.SetAttributeValue("Using_Stages_Not_Flows", _UsingStagesNotFlows);
                masterElement.SetAttributeValue("Maximum_Probability", _MaximumProbability);
                masterElement.SetAttributeValue("Minimum_Probability", _MinimumProbability);
                XElement pairedDataElement = new XElement("Coordinates");
                for (int i = 0; i < _ExceedanceProbabilities.Length; i++)
                {
                    XElement rowElement = new XElement("Coordinate");
                    XElement xElement = new XElement("X");
                    xElement.SetAttributeValue("Value", _ExceedanceProbabilities[i]);
                    XElement yElement = StageOrLogFlowDistributions[i].ToXML();
                    rowElement.Add(xElement);
                    rowElement.Add(yElement);
                    pairedDataElement.Add(rowElement);
                }
                masterElement.Add(pairedDataElement);
                return masterElement;
            }

        }

        public static GraphicalUncertainPairedData ReadFromXML(XElement xElement)
        {
            CurveMetaData metaData = CurveMetaData.ReadFromXML(xElement.Element("CurveMetaData"));
            if (metaData.IsNull)
            {
                return new GraphicalUncertainPairedData();
            }
            else
            {
                int equivalentRecordLength = Convert.ToInt32(xElement.Attribute("ERL").Value);
                bool usingStagesNotFlows = Convert.ToBoolean(xElement.Attribute("Using_Stages_Not_Flows").Value);
                double minimumProbability = Convert.ToDouble(xElement.Attribute("Minimum_Probability").Value);
                double maximumProbability = Convert.ToDouble(xElement.Attribute("Maximum_Probability").Value);
                int size = Convert.ToInt32(xElement.Attribute("Ordinate_Count").Value);
                double[] exceedanceProbabilities = new double[size];
                Normal[] stageOrFlowDistributions = new Normal[size];
                int i = 0;
                foreach (XElement coordinateElement in xElement.Element("Coordinates").Elements())
                {
                    foreach (XElement ordinateElements in coordinateElement.Elements())
                    {
                        if (ordinateElements.Name.ToString().Equals("X"))
                        {
                            exceedanceProbabilities[i] = Convert.ToDouble(ordinateElements.Attribute("Value").Value);
                        }
                        else
                        {
                            stageOrFlowDistributions[i] = (Normal)Statistics.ContinuousDistribution.FromXML(ordinateElements);
                        }
                    }
                    i++;
                }
                return new GraphicalUncertainPairedData(exceedanceProbabilities, stageOrFlowDistributions, equivalentRecordLength, metaData, usingStagesNotFlows, maximumProbability, minimumProbability);
            }

        }
        #endregion
    }
    }


