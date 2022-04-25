using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace metrics
{
    public class PerformanceByThresholds
{        
        private Dictionary<int,Threshold> _thresholds; 
        public Dictionary<int,Threshold> ThresholdsDictionary
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

        public PerformanceByThresholds()
        {
            _thresholds = new Dictionary<int,Threshold>();
                   
        }      
        private PerformanceByThresholds(Dictionary<int,Threshold> thresholdDictionary)
        {
            _thresholds = thresholdDictionary;
        }
        public void AddThreshold(Threshold threshold)
        {
            _thresholds.Add(threshold.ThresholdID,threshold);
        }

        public void RemoveThreshold(Threshold threshold)
        {
            _thresholds.Remove(threshold.ThresholdID);
        }
        public bool Equals(PerformanceByThresholds incomingPerformanceByThresholds)
        {
            foreach (int key in ThresholdsDictionary.Keys)
            {
                bool success = ThresholdsDictionary[key].Equals(incomingPerformanceByThresholds.ThresholdsDictionary[key]);
                if (!success)
                {
                    return false;
                }
            }
            return true;
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Performance_By_Thresholds");
            int keyCount = ThresholdsDictionary.Keys.ToArray().Length;
            masterElement.SetAttributeValue("Key_Count", keyCount);

            for (int i = 0; i < keyCount; i++)
            {
                int key = ThresholdsDictionary.Keys.ToArray()[i];
                XElement thresholdElement = ThresholdsDictionary[key].WriteToXML();
                thresholdElement.Name = $"Threshold_{key}";
                masterElement.Add(thresholdElement);
                masterElement.SetAttributeValue($"key{i}", key);
            }
            return masterElement;
        }

        public static PerformanceByThresholds ReadFromXML(XElement xElement)
        {
            Dictionary<int, Threshold> thresholdDictionary = new Dictionary<int, Threshold>();
            int keyCount = Convert.ToInt32(xElement.Attribute("Key_Count").Value);
            for (int i = 0; i < keyCount; i++)
            {
                int key = Convert.ToInt32(xElement.Attribute($"key{i}").Value);
                Threshold threshold = Threshold.ReadFromXML(xElement.Element($"Threshold_{key}"));
                thresholdDictionary.Add(key, threshold);
            }
            return new PerformanceByThresholds(thresholdDictionary);

        }

    }
}
