using HEC.FDA.Model.metrics;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class ThresholdRowItem
    {
        private double? _ThresholdValue = null;

        public int ID { get; set; }
        public List<ThresholdType> ThresholdTypes { get; } = new List<ThresholdType>();
        public ThresholdType ThresholdType { get; set; }
        public double? ThresholdValue
        {
            get { return _ThresholdValue; }
            set { _ThresholdValue = value;  }
        }

        public ThresholdRowItem(int id, ThresholdEnum thresholdType, double? value)
        {
            ID = id;
            ThresholdType = new ThresholdType(ThresholdEnum.AdditionalExteriorStage, "Additional Exterior Stage");
            ThresholdValue = value;
        }

        public ThresholdRowItem(XElement elem, int index)
        {
            ID = index;
            ThresholdType = ConvertStringToMetricEnum(elem.Attribute("Type").Value);
            ThresholdValue = Double.Parse(elem.Attribute("Value").Value);
        }

        public Threshold GetThreshold()
        {
            Statistics.ConvergenceCriteria convergenceCriteria = new Statistics.ConvergenceCriteria();
            return new Threshold(ID, convergenceCriteria, ThresholdType.Metric, ThresholdValue.Value);
        }

        public XElement ToXML()
        {
            XElement rowElement = new XElement("Row");
            rowElement.SetAttributeValue("Type", ThresholdType.Metric);
            rowElement.SetAttributeValue("Value", ThresholdValue);
            return rowElement;
        }

        private ThresholdType ConvertStringToMetricEnum(string metric)
        {
            switch (metric)
            {
                case nameof(ThresholdEnum.NotSupported):
                    {
                        return new ThresholdType(ThresholdEnum.NotSupported, "Not Supported");
                    }
                case nameof(ThresholdEnum.DefaultExteriorStage):
                    {
                        return new ThresholdType(ThresholdEnum.DefaultExteriorStage, "Default Exterior Stage");
                    }
                case nameof(ThresholdEnum.TopOfLevee):
                    {
                        return new ThresholdType(ThresholdEnum.TopOfLevee, "Top Of Levee");
                    }
                case nameof(ThresholdEnum.LeveeSystemResponse):
                    {
                        return new ThresholdType(ThresholdEnum.LeveeSystemResponse, "Levee System Response");
                    }
                case nameof(ThresholdEnum.AdditionalExteriorStage):
                    {
                        return new ThresholdType(ThresholdEnum.AdditionalExteriorStage, "Additional Exterior Stage");
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException("Could not convert string: " + metric + " to an IMetricEnum.");
                    }
            }
        }
    }
}
