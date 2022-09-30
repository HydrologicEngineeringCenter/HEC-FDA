using System.Collections.Generic;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;

namespace HEC.FDA.Model.metrics
{
    public class PerformanceByThresholds : Validation, IReportMessage
    {
        #region Fields
        private List<Threshold> _Thresholds;
        private bool _IsNull;
        #endregion

        #region Properties 
        internal bool IsNull
        {
            get
            {
                return _IsNull;
            }
        }
        public List<Threshold> ListOfThresholds
        {
            get
            {
                return _Thresholds;
            }
            set
            {
                _Thresholds = value;
            }
        }
        public event MessageReportedEventHandler MessageReport;

        #endregion

        #region Constructors 

        public PerformanceByThresholds()
        {
            _Thresholds = new List<Threshold>();
            MessageHub.Register(this);

        }
        public PerformanceByThresholds(bool isNull)
        {
            _Thresholds = new List<Threshold>();
            Threshold dummyThreshold = new Threshold();
            _Thresholds.Add(dummyThreshold);
            _IsNull = isNull;
            MessageHub.Register(this);

        }
        private PerformanceByThresholds(List<Threshold> thresholds)
        {
            _Thresholds = thresholds;
            MessageHub.Register(this);
        }
        #endregion
        #region Methods 
        public void AddThreshold(Threshold threshold)
        {
            _Thresholds.Add(threshold);
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
            foreach (Threshold threshold in _Thresholds)
            {
                if (threshold.ThresholdID.Equals(thresholdID))
                {
                    return threshold;
                }
            }
            Threshold dummyThreshold = new Threshold();
            string message = "The requested threshold could not be found so a dummy threshold is being returned";
            MVVMFramework.Model.Messaging.ErrorMessage errorMessage = new MVVMFramework.Model.Messaging.ErrorMessage(message, MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            return dummyThreshold;

        }
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Performance_By_Thresholds");
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
            List<Threshold> thresholdList = new List<Threshold>();
            foreach (XElement thresholdElement in xElement.Elements())
            {
                Threshold threshold = Threshold.ReadFromXML(thresholdElement);
                thresholdList.Add(threshold);
            }
            return new PerformanceByThresholds(thresholdList);
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }

        internal void ForceDeQueue()
        {
            foreach (Threshold threshold in ListOfThresholds)
            {
                foreach (AssuranceResultStorage assuranceResultStorage in threshold.SystemPerformanceResults.Assurances)
                {
                    assuranceResultStorage.AssuranceHistogram.ForceDeQueue();
                }
            }
        }
        #endregion
    }
}
