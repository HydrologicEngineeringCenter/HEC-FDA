using System.Collections.Generic;
using System.Xml.Linq;

namespace metrics
{
    public class PerformanceByThresholds
{
        #region Fields
        private List<Threshold> _thresholds;
        #endregion

        #region Properties 
        public List<Threshold> ListOfThresholds
        {
            get
            {
                return _thresholds;
            }
            set
            {
                _thresholds = value;
            }
        }
        #endregion

        #region Constructors 

        public PerformanceByThresholds()
        {
            _thresholds = new List<Threshold>();
                   
        }      
        private PerformanceByThresholds(List<Threshold> thresholds)
        {
            _thresholds = thresholds;
        }
        #endregion
        #region Methods 
        public void AddThreshold(Threshold threshold)
        {
            _thresholds.Add(threshold);
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
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Performance_By_Thresholds");
            foreach (Threshold threshold in ListOfThresholds)
            {
                XElement thresholdElement = threshold.WriteToXML();
                thresholdElement.Name = $"{threshold.ThresholdID}";
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
        #endregion
    }
}
