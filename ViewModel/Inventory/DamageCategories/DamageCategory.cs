using System;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Inventory.DamageCategory
{
    internal class DamageCategory : IDamageCategory
    {
        public string Name 
        { 
            get;
            set; 
        }
        public string Description { get; set; }
        public int RebuildPeriod { get; set; }
        public double CostFactor { get; set; }

        public DamageCategory(string name)
        {
            Name = name;
        }

        public double CalculateNewValue(double value, double percentDamage, DateTime lastDamageDate, DateTime currentDamageDate)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IDamageCategory damageCategory)
        {
            throw new NotImplementedException();
        }

        public void ReadFromXml(XElement elem)
        {
            throw new NotImplementedException();
        }

        public XElement writeToXmlElement()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The display member path is not working in the ManualStageDamageControl.xaml 
        /// This is needed to display the name in the UI.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
