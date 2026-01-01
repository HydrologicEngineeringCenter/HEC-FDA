using System;
using System.Collections.Generic;
using System.Linq;
using Statistics.Distributions;
using Statistics;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Implementations;
using System.Xml.Linq;
using HEC.FDA.Model.utilities;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Model.Messaging;
using HEC.FDA.Model.paireddata;

namespace HEC.FDA.Model.extensions
{
    [StoredProperty("GraphicalDistribution")]
    public class GraphicalDistribution : ValidationErrorLogger
    {
        #region Properties
        [StoredProperty("StageOrLoggedFlowValues")]
        public double[] StageOrLoggedFlowValues { get; internal set; }
        [StoredProperty("UsingStagesNotFlows")]
        public bool UsingStagesNotFlows { get; }
        [StoredProperty("EquivalentRecordLength")]
        public int EquivalentRecordLength { get; }
        [StoredProperty("ExceedanceProbabilities")]
        public double[] ExceedanceProbabilities { get; internal set; }
        [StoredProperty("StageOrLogFlowDistributions")]
        public ContinuousDistribution[] StageOrLogFlowDistributions { get; internal set; }

        #endregion

        #region Constructor 
        public GraphicalDistribution()
        {
            EquivalentRecordLength = 10;
            UsingStagesNotFlows = true;
            ExceedanceProbabilities = new double[] { 0 };
            StageOrLogFlowDistributions = new Normal[] { new Normal(0, 1) };
            StageOrLoggedFlowValues = new double[] { 0 };

        }
        /// <summary>
        /// Graphical Distribution implements Beth Faber's Less Simple Method for calculating uncertainty about the distribution
        /// See the HEC-FDA Technical Reference for more information on the Less Simple Method
        /// This constructor assumes that exceedance probabilities and flow or stage values have a strictly monotonic relationships.
        /// </summary>
        /// <param name="userInputExceedanceProbabilities"></param> User-provided exceedance probabilities. There should be at least 8.
        /// <param name="stageOrUnloggedFlowValues"></param> User-provided flow or stage values. A value should correspond to a probability. 
        /// <param name="equivalentRecordLength"></param> The equivalent record length in years.

        public GraphicalDistribution(double[] userInputExceedanceProbabilities, double[] stageOrUnloggedFlowValues, int equivalentRecordLength, bool usingStagesNotFlows = true)
        {
            EquivalentRecordLength = equivalentRecordLength;
            UsingStagesNotFlows = usingStagesNotFlows;
            (double[] exceedenceProbs, ContinuousDistribution[] stageOrLogFlowDists) = GraphicalFrequencyUncertaintyCalculators.LessSimpleMethod(userInputExceedanceProbabilities, stageOrUnloggedFlowValues, UsingStagesNotFlows, EquivalentRecordLength);

            ExceedanceProbabilities = exceedenceProbs;
            StageOrLoggedFlowValues = stageOrLogFlowDists.Select((x) => x.InverseCDF(.5)).ToArray();
            AddRules(userInputExceedanceProbabilities);
            Validate();
            if (ErrorLevel >= ErrorLevel.Major)
            {
                string message = $"There are major or worse errors associated with a graphical frequency function, confidence intervals cannot be computed." + Environment.NewLine;
                ErrorMessage errorMessage = new(message, ErrorLevel.Major);
                ReportMessage(this, new MessageEventArgs(errorMessage));
            }
            else
            {
                //then we compute uncertainty
                StageOrLogFlowDistributions = stageOrLogFlowDists;
            }
        }
        private GraphicalDistribution(double[] stageOrLoggedFlowValues, bool usingStagesNotFlows, int equivalentRecordLength, double[] exceedanceProbabilities, ContinuousDistribution[] stageOrLogFlowDistributions)
        {
            StageOrLoggedFlowValues = stageOrLoggedFlowValues;
            UsingStagesNotFlows = usingStagesNotFlows;
            EquivalentRecordLength = equivalentRecordLength;
            ExceedanceProbabilities = exceedanceProbabilities;
            StageOrLogFlowDistributions = stageOrLogFlowDistributions;
        }

        #endregion

        #region Methods
        private void AddRules(double[] exceedanceProbabilities)
        {
            AddSinglePropertyRule(nameof(EquivalentRecordLength), new Rule(() => EquivalentRecordLength > 0, "Equivalent record length must be greater than 0."));
            AddSinglePropertyRule(nameof(exceedanceProbabilities), new Rule(() => IsArrayValid(exceedanceProbabilities, (a, b) => (a >= b)), "Exceedance Probabilities must be strictly monotonically decreasing"));
            AddSinglePropertyRule(nameof(StageOrLoggedFlowValues), new Rule(() => IsArrayValid(StageOrLoggedFlowValues, (a, b) => (a <= b)), "Y must be strictly monotonically decreasing"));
        }
        private static bool IsArrayValid(double[] arrayOfData, Func<double, double, bool> comparison)
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
        #endregion

        #region XML Methods
        public static GraphicalDistribution ReadFromXML(XElement xElement)
        {
            GraphicalDistribution graphicalInError = new();

            Type graphicalDistributionType = typeof(GraphicalDistribution);

            string erlTag = Serialization.GetXMLTagFromProperty(graphicalDistributionType, nameof(EquivalentRecordLength));
            if (!int.TryParse(xElement.Attribute(erlTag)?.Value, out int erl))
                return graphicalInError;

            string boolTag = Serialization.GetXMLTagFromProperty(graphicalDistributionType, nameof(UsingStagesNotFlows));
            if (!bool.TryParse(xElement.Attribute(boolTag)?.Value, out bool usesStageNotFlows))
                return graphicalInError;

            string probsTag = Serialization.GetXMLTagFromProperty(graphicalDistributionType, nameof(ExceedanceProbabilities));
            List<double> exceedanceProbabilities = new();
            int i = 0;
            foreach (XElement exceedanceProbability in xElement.Element(probsTag).Elements())
            {
                if (!double.TryParse(exceedanceProbability.Attribute(probsTag)?.Value, out double prob))
                {
                    //serves as null object to be returned in case of error
                    GraphicalDistribution graphical = new();
                    return graphical;
                }
                exceedanceProbabilities.Add(prob);
                i++;
            }

            string distsTag = Serialization.GetXMLTagFromProperty(graphicalDistributionType, nameof(StageOrLogFlowDistributions));
            List<Normal> stageOrFlowDistributions = new();
            foreach (XElement stageOrFlowDistribution in xElement.Element(distsTag).Elements())
            {
                Normal normal = (Normal)ContinuousDistribution.FromXML(stageOrFlowDistribution);
                if (normal != null)
                {
                    stageOrFlowDistributions.Add(normal);
                }
            }

            string valsTag = Serialization.GetXMLTagFromProperty(graphicalDistributionType, nameof(StageOrLoggedFlowValues));
            List<double> inputStageFlowVals = new();

            int j = 0;
            foreach (XElement stageOrFlowValue in xElement.Element(valsTag).Elements())
            {
                if (!double.TryParse(stageOrFlowValue.Attribute(valsTag)?.Value, out double val))
                {
                    //serves as null object to be returned in case of error
                    GraphicalDistribution graphical = new();
                    return graphical;
                }
                inputStageFlowVals.Add(val);
                j++;
            }

            return new GraphicalDistribution(inputStageFlowVals.ToArray(), usesStageNotFlows, erl, exceedanceProbabilities.ToArray(), stageOrFlowDistributions.ToArray()); 
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new(GetType().Name);

            string boolTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(UsingStagesNotFlows));
            masterElement.SetAttributeValue(boolTag, UsingStagesNotFlows);

            string erlTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(EquivalentRecordLength));
            masterElement.SetAttributeValue(erlTag, EquivalentRecordLength);

            string probsTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(ExceedanceProbabilities));
            XElement exceedanceProbabilities = new(probsTag);
            for (int i = 0; i < ExceedanceProbabilities.Length; i++)
            {
                //the name of rowElement does not matter, only the name of the attribute does 
                XElement rowElement = new("Probability");
                rowElement.SetAttributeValue(probsTag, ExceedanceProbabilities[i]);
                exceedanceProbabilities.Add(rowElement);
            }
            masterElement.Add(exceedanceProbabilities);

            string valsTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(StageOrLoggedFlowValues));
            XElement inputStageFlowValues = new(valsTag);
            for (int i = 0; i < StageOrLoggedFlowValues.Length; i++)
            {
                XElement rowElement = new("StageFlow");
                rowElement.SetAttributeValue(valsTag, StageOrLoggedFlowValues[i]);
                inputStageFlowValues.Add(rowElement);
            }
            masterElement.Add(inputStageFlowValues);

            string distsTag = Serialization.GetXMLTagFromProperty(GetType(), nameof(StageOrLogFlowDistributions));
            XElement stageOrLogFlowDistributions = new(distsTag);
            for (int i = 0; i < StageOrLogFlowDistributions.Length; i++)
            {
                stageOrLogFlowDistributions.Add(StageOrLogFlowDistributions[i].ToXML());
            }
            masterElement.Add(stageOrLogFlowDistributions);


            return masterElement;
        }
        #endregion
    }
}
