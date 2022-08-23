using System;
using Statistics.Histograms;
using Statistics;
using System.Xml.Linq;
using System.Runtime.Remoting;
using System.Reflection;
using Statistics.Distributions;
namespace metrics
{ //TODO: I THINK SOME OR ALL OF THIS CLASS SHOULD BE INTERNAL 
    public class ConsequenceResult
    {
        #region Fields
        private double _consequenceResult;
        private string _damageCategory;
        private string _assetCategory;
        private int _regionID;
        private bool _isNull;
        #endregion

        #region Properties
        public double Consequence
        {
            get
            {
                return _consequenceResult;
            }
        }
        public string DamageCategory
        {
            get
            {
                return _damageCategory;
            }
        }
        public string AssetCategory
        {
            get
            {
                return _assetCategory;
            }
        }
        public int RegionID
        {
            get
            {
                return _regionID;
            }
        }
        public bool IsNull
        {
            get
            {
                return _isNull;
            }
        }
        #endregion

        #region Constructors

        public ConsequenceResult()
        {
            _damageCategory = "unassigned";
            _assetCategory = "unassigned";
            _regionID = 0;
            _isNull = true;
            _consequenceResult = 0;
        }

        internal ConsequenceResult(string damageCategory, string assetCategory, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _regionID = impactAreaID;
            _isNull = false;
            _consequenceResult = 0;
        }        
        #endregion

        #region Methods
        internal void IncrementConsequence(double increment)
        {
            _consequenceResult += increment;
        }

        internal bool Equals(ConsequenceResult damageResult)
        {
            bool valuesMatch = _consequenceResult.Equals(damageResult.Consequence);
            if (!valuesMatch)
            {
                return false;
            }
            bool damageCategoriesMatch = _damageCategory.Equals(damageResult.DamageCategory);
            if (!damageCategoriesMatch)
            {
                return false;
            }
            bool assetCategoriesMatch = _assetCategory.Equals(damageResult.AssetCategory);
            if (!assetCategoriesMatch)
            {
                return false;
            }
            bool regionIDMatch = _regionID.Equals(damageResult.RegionID);
            if (!regionIDMatch)
            {
                return false;
            }
            return true;
        }
        internal XElement WriteToXML()
        {
            XElement masterElement = new XElement("ConsequenceResult");
            masterElement.SetAttributeValue("Value", _consequenceResult);
            masterElement.SetAttributeValue("DamageCategory", _damageCategory);
            masterElement.SetAttributeValue("AssetCategory", _assetCategory);
            masterElement.SetAttributeValue("ImpactAreaID", _regionID);
            return masterElement;
        }

        internal static ConsequenceResult ReadFromXML(XElement xElement)
        {
            string damageCategory = xElement.Attribute("DamageCategory").Value;
            string assetCategory = xElement.Attribute("AssetCategory").Value;
            int id = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            ConsequenceResult consequenceResult = new ConsequenceResult(damageCategory, assetCategory, id);
            double consequenceValue = Convert.ToDouble(xElement.Attribute("Value").Value);
            consequenceResult._consequenceResult = consequenceValue;
            return consequenceResult;
        }
        #endregion
    }
}
