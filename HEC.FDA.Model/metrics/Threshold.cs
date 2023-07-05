using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.utilities;
using Statistics;
using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Utility.Extensions.Attributes;

namespace HEC.FDA.Model.metrics
{
    [StoredProperty("Threshold")]
    public class Threshold
    {
        #region Properties
        [StoredProperty("Threshold_Type")]
        public ThresholdEnum ThresholdType { get; set; }
        [StoredProperty("Threshold_Value")]
        public double ThresholdValue { get; set; }
        [StoredProperty("Project_Performance_Results")]
        public SystemPerformanceResults SystemPerformanceResults { get; set; }
        [StoredProperty("Threshold_ID")]
        /// <summary>
        /// Threshold ID should be an integer greater than or equal to 1. 
        /// The threshold ID = 0 is reserved for the default threshold.
        /// </summary>
        public int ThresholdID { get; }
        public bool IsNull { get; }
        #endregion

        #region Constructors 
        public Threshold()
        {
            ThresholdType = ThresholdEnum.DefaultExteriorStage;
            ThresholdID = 9999;
            SystemPerformanceResults = new SystemPerformanceResults();
            IsNull = true;
        }
        public Threshold(int thresholdID, ConvergenceCriteria c, ThresholdEnum thresholdType = 0, double thresholdValue = 0)
        {
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
            SystemPerformanceResults = new SystemPerformanceResults(c);
            ThresholdID = thresholdID;
            IsNull = false;
        }

        public Threshold(int thresholdID, UncertainPairedData systemResponseCurve, ConvergenceCriteria c, ThresholdEnum thresholdType = 0, double thresholdValue = 0)
        {
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
            SystemPerformanceResults = new SystemPerformanceResults(systemResponseCurve, c);
            ThresholdID = thresholdID;
            IsNull = false;

        }

        private Threshold(int thresholdID, ThresholdEnum thresholdType, double thresholdValue, SystemPerformanceResults projectPerformanceResults)
        {
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
            ThresholdID = thresholdID;
            SystemPerformanceResults = projectPerformanceResults;
            IsNull = false;

        }
        #endregion
        #region Methods
        public bool Equals(Threshold incomingThreshold)
        {
            bool thresholdTypeIsTheSame = ThresholdType.Equals(incomingThreshold.ThresholdType);
            bool thresholdValueIsTheSame = ThresholdValue.Equals(incomingThreshold.ThresholdValue);
            bool thresholdIDIsTheSame = ThresholdID.Equals(incomingThreshold.ThresholdID);
            bool projectPerformanceIsTheSame = SystemPerformanceResults.Equals(incomingThreshold.SystemPerformanceResults);
            if (!thresholdTypeIsTheSame || !thresholdIDIsTheSame || !thresholdValueIsTheSame || !thresholdIDIsTheSame || !projectPerformanceIsTheSame)
            {
                return false;
            }
            return true;
        }

        #region Serialization
        public XElement WriteToXML()
        {
            StoredPropertyAttribute attribute = (StoredPropertyAttribute)Attribute.GetCustomAttribute(typeof(Threshold), typeof(StoredPropertyAttribute));
            string thresholdMasterTag = attribute.SerializedName;
            XElement masterElement = new(thresholdMasterTag);
            string thresholdTypeTag = GetXMLTagFromProperty(nameof(ThresholdType));
            masterElement.SetAttributeValue(thresholdTypeTag, ThresholdType);
            string thresholdValueTag = GetXMLTagFromProperty(nameof(ThresholdValue));
            masterElement.SetAttributeValue(thresholdValueTag, ThresholdValue);
            string thresholdIDTag = GetXMLTagFromProperty(nameof(ThresholdID));
            masterElement.SetAttributeValue(thresholdIDTag, ThresholdID);
            XElement projectPerformanceElement = SystemPerformanceResults.WriteToXML();
            masterElement.Add(projectPerformanceElement);
            return masterElement;
        }

        public static Threshold ReadFromXML(XElement xElement)
        {
            string thresholdTypeTag = GetXMLTagFromProperty(nameof(ThresholdType));
            ThresholdEnum thresholdType = ThresholdEnumFromString(xElement.Attribute(thresholdTypeTag)?.Value);

            string thresholdValueTag = GetXMLTagFromProperty(nameof(ThresholdValue));
            if (!double.TryParse(xElement.Attribute(thresholdValueTag)?.Value, out double thresholdValue))
                return null;

            string thresholdIDTag = GetXMLTagFromProperty(nameof(ThresholdID));
            if (!int.TryParse(xElement.Attribute(thresholdIDTag)?.Value, out int thresholdID))
                return null;

            string performanceResultsTag = GetXMLTagFromProperty(nameof(SystemPerformanceResults));
            SystemPerformanceResults projectPerformanceResults = SystemPerformanceResults.ReadFromXML(xElement.Element(performanceResultsTag));
            if (projectPerformanceResults != null)
                return new Threshold(thresholdID, thresholdType, thresholdValue, projectPerformanceResults);

            return null;
        }

        private static string GetXMLTagFromProperty(string propertyName)
        {
            return typeof(Threshold).GetProperty(propertyName).GetCustomAttribute<StoredPropertyAttribute>().SerializedName;
        }

        private static ThresholdEnum ThresholdEnumFromString(string value)
        {
            foreach (ThresholdEnum option in Enum.GetValues(typeof(ThresholdEnum)))
            {
                var attribute = option.GetAttribute<StoredPropertyAttribute>();

                if (attribute.SerializedName.Equals(value))
                    return option;

                if (attribute.AlsoKnownAs == null)
                    continue;

                foreach (string alias in attribute.AlsoKnownAs)
                    if (alias.Equals(value))
                        return option;
            }
            return ThresholdEnum.NotSupported;
        }
        #endregion
        #endregion


    }
}