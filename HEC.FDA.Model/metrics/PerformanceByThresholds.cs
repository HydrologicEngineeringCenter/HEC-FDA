using System.Collections.Generic;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Model.Messaging;

namespace HEC.FDA.Model.metrics
{
    public class PerformanceByThresholds : ValidationErrorLogger
    {
        #region Properties 
        internal bool IsNull { get; }
        public List<Threshold> ListOfThresholds { get; set; }

        #endregion

        #region Constructors 

        public PerformanceByThresholds()
        {
            ListOfThresholds = new List<Threshold>();
        }
        public PerformanceByThresholds(bool isNull)
        {
            ListOfThresholds = new List<Threshold>();
            Threshold dummyThreshold = new();
            ListOfThresholds.Add(dummyThreshold);
            IsNull = isNull;
        }
        private PerformanceByThresholds(List<Threshold> thresholds)
        {
            ListOfThresholds = thresholds;
        }
        #endregion
        #region Methods 
        public void AddThreshold(Threshold threshold)
        {
            ListOfThresholds.Add(threshold);
        }
        public bool Equals(PerformanceByThresholds incomingPerformanceByThresholds)
        {
            bool success = true;
            foreach (Threshold threshold in ListOfThresholds)
            {
                foreach (Threshold inputThreshold in incomingPerformanceByThresholds.ListOfThresholds)
                {
                    if (threshold.ThresholdID == inputThreshold.ThresholdID)
                    {
                        success = threshold.Equals(inputThreshold);
                        if (!success)
                        {
                            break;
                        }
                    }
                }
            }
            return success;
        }
        public Threshold GetThreshold(int thresholdID)
        {
            foreach (Threshold threshold in ListOfThresholds)
            {
                if (threshold.ThresholdID.Equals(thresholdID))
                {
                    return threshold;
                }
            }
            Threshold dummyThreshold = new();
            string message = "The requested threshold could not be found so a dummy threshold is being returned";
            ErrorMessage errorMessage = new(message, MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            return dummyThreshold;
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new("Performance_By_Thresholds");
            foreach (Threshold threshold in ListOfThresholds)
            {
                XElement thresholdElement = threshold.WriteToXML();
                thresholdElement.Name = $"ID{threshold.ThresholdID}";
                masterElement.Add(thresholdElement);
            }
            return masterElement;
        }

        public static PerformanceByThresholds ReadFromXML(XElement xElement)
        {
            List<Threshold> thresholdList = new();
            foreach (XElement thresholdElement in xElement.Elements())
            {
                Threshold threshold = Threshold.ReadFromXML(thresholdElement);
                if(threshold != null)
                {
                    thresholdList.Add(threshold);
                }
            }
            return new PerformanceByThresholds(thresholdList);
        }

        #endregion
    }
}
