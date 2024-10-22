using HEC.FDA.Model.metrics;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class ThresholdRowItem
    {
        public int ID { get; set; }
        public ThresholdEnum ThresholdType { get; set; }
        public string ThresholdTypeDisplayName { get => ThresholdType.GetDisplayName(); }
        public double ThresholdValue { get; set; }

        public ThresholdRowItem(int id, ThresholdEnum thresholdType, double value)
        {
            ID = id;
            ThresholdType = thresholdType;
            ThresholdValue = value;
        }

        public ThresholdRowItem(XElement elem, int index)
        {
            ID = index;
            try
            {
                ThresholdType = Enum.Parse<ThresholdEnum>(elem.Attribute("Type").Value);
                ThresholdValue = double.Parse(elem.Attribute("Value").Value);
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to parse Threshold from XML", ex);
            }
           
        }

        public Threshold GetThreshold()
        {
            Statistics.ConvergenceCriteria convergenceCriteria = new Statistics.ConvergenceCriteria();
            return new Threshold(ID, convergenceCriteria, ThresholdType, ThresholdValue);
        }

        public XElement ToXML()
        {
            XElement rowElement = new XElement("Row");
            rowElement.SetAttributeValue("Type", ThresholdType);
            rowElement.SetAttributeValue("Value", ThresholdValue);
            return rowElement;
        }
    }
}
