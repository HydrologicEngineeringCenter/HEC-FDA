using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace OccupancyTypes
{
    public interface IDamageCategory
    {

        string Name { get; set; }
        string Description { get; set; }
        int RebuildPeriod { get; set; }
        double CostFactor { get; set; }

        double CalculateNewValue(double value, double percentDamage, DateTime lastDamageDate, DateTime currentDamageDate);
        XElement writeToXmlElement();

        void ReadFromXml(XElement elem);

        Boolean Equals(IDamageCategory damageCategory);
    }
}
