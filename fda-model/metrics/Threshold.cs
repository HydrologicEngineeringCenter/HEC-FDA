using paireddata;
using Statistics;
using System;
using System.Xml.Linq;

namespace metrics
{
    public class Threshold
    {
        public ThresholdEnum ThresholdType { get; set; }
        public double ThresholdValue { get; set; }
        public ProjectPerformanceResults ProjectPerformanceResults { get; set; }
        /// <summary>
        /// Threshold ID should be an integer greater than or equal to 1. 
        /// The threshold ID = 0 is reserved for the default threshold.
        /// </summary>
        public int ThresholdID { get; }

        public Threshold(int thresholdID,ConvergenceCriteria c, ThresholdEnum thresholdType=0, double thresholdValue=0 )
        {
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
            ProjectPerformanceResults = new ProjectPerformanceResults(thresholdType, thresholdValue, c); 
            ThresholdID = thresholdID;
        }

        public Threshold(int thresholdID, UncertainPairedData systemResponseCurve, ConvergenceCriteria c, ThresholdEnum thresholdType = 0, double thresholdValue = 0)
        {
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
            ProjectPerformanceResults = new ProjectPerformanceResults(thresholdType, thresholdValue, systemResponseCurve, c);
            ThresholdID = thresholdID;
        }

        private Threshold(int thresholdID, ThresholdEnum thresholdType, double thresholdValue, ProjectPerformanceResults projectPerformanceResults)
        {
            ThresholdType = thresholdType;
            ThresholdValue = thresholdValue;
            ThresholdID = thresholdID;
            ProjectPerformanceResults = projectPerformanceResults;
        }
        public bool Equals(Threshold incomingThreshold)
        {
            bool thresholdTypeIsTheSame = ThresholdType.Equals(incomingThreshold.ThresholdType);
            bool thresholdValueIsTheSame = ThresholdValue.Equals(incomingThreshold.ThresholdValue);
            bool thresholdIDIsTheSame = ThresholdID.Equals(incomingThreshold.ThresholdID);
            bool projectPerformanceIsTheSame = ProjectPerformanceResults.Equals(incomingThreshold.ProjectPerformanceResults);
            if (!thresholdIDIsTheSame || !thresholdValueIsTheSame || !thresholdIDIsTheSame || !projectPerformanceIsTheSame)
            {
                return false;
            }
            return true;
        }

        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Threshold");
            masterElement.SetAttributeValue("Threshold_Type", Convert.ToString(ThresholdType));
            masterElement.SetAttributeValue("Threshold_Value", ThresholdValue);
            masterElement.SetAttributeValue("Threshold_ID", ThresholdID);
            XElement projectPerformanceElement = ProjectPerformanceResults.WriteToXML();
            projectPerformanceElement.Name= "Project_Performance_Results";
            masterElement.Add(projectPerformanceElement);
            return masterElement;
        }

        public static Threshold ReadFromXML(XElement xElement)
        {
            ThresholdEnum thresholdType = (ThresholdEnum)Enum.Parse(typeof(ThresholdEnum), xElement.Attribute("Threshold_Type").Value);
            double thresholdValue = Convert.ToDouble(xElement.Attribute("Threshold_Value").Value);
            int thresholdID = Convert.ToInt32(xElement.Attribute("Threshold_ID").Value);
            ProjectPerformanceResults projectPerformanceResults = ProjectPerformanceResults.ReadFromXML(xElement.Element("Project_Performance_Results"));
            return new Threshold(thresholdID, thresholdType, thresholdValue, projectPerformanceResults);
        }
    }
}