using System;
using System.Collections.Generic;
using Statistics;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;

namespace metrics
{ //TODO: I THINK SOME OR ALL OF THIS CLASS SHOULD BE INTERNAL 
    public class ConsequenceResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields
        private List<ConsequenceResult> _consequenceResultList;
        //impact area to be string?
        private int _regionID;//impact area ID or census block ID
        private bool _isNull;
        #endregion

        #region Properties 
        public List<ConsequenceResult> ConsequenceResultList
        {
            get
            {
                return _consequenceResultList;
            }
        }
        public int RegionID
        {
            get
            {
                return _regionID;
            }
        }
        //this needs to be an error report
        public event MessageReportedEventHandler MessageReport;
        public bool IsNull
        {
            get
            {
                return _isNull;
            }
        }
        #endregion
        #region Constructors
        public ConsequenceResults()
        {
            _consequenceResultList = new List<ConsequenceResult>();
            _regionID = 0;
            _isNull = true;
        }
        public ConsequenceResults(int impactAreaID){
            _consequenceResultList = new List<ConsequenceResult>();
            _regionID = impactAreaID;
            _isNull = false;
        }
        private ConsequenceResults(List<ConsequenceResult> damageResults, int impactAreaID)
        {
            _consequenceResultList = damageResults;
            _regionID = impactAreaID;
            _isNull = false;

        }
        #endregion

        #region Methods 
        public void AddConsequenceResultObject(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID)
        {
            ConsequenceResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            if (damageResult.IsNull)
            {
                ConsequenceResult newDamageResult = new ConsequenceResult(damageCategory, assetCategory, convergenceCriteria, impactAreaID);
                _consequenceResultList.Add(newDamageResult);
            }
        }
        public void AddConsequenceResult(ConsequenceResult consequenceResultToAdd)
        {
            ConsequenceResult consequenceResult = GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.AssetCategory, consequenceResultToAdd.RegionID);
            if (consequenceResult.IsNull)
            {
                _consequenceResultList.Add(consequenceResultToAdd);
            }
        }
        public void AddConsequenceRealization(double dammageEstimate, string damageCategory, string assetCategory, int impactAreaID, Int64 iteration)
        {
            ConsequenceResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            damageResult.AddConsequenceRealization(dammageEstimate, iteration);

        }
        public double MeanDamage(string damageCategory, string assetCategory, int impactAreaID)
        {
            ConsequenceResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            return damageResult.MeanExpectedAnnualConsequences();
        }

        public double ConsequenceExceededWithProbabilityQ(string damageCategory, double exceedanceProbability, string assetCategory, int impactAreaID)
        {
            ConsequenceResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            double quantileRequested = damageResult.ConsequenceExceededWithProbabilityQ(exceedanceProbability);
            return quantileRequested;

        }
        public ConsequenceResult GetConsequenceResult(string damageCategory, string assetCategory, int impactAreaID)
        {
            foreach (ConsequenceResult damageResult in _consequenceResultList)
            {
                //The impact area should always be equal because a consequence result reflects 1 impact area and a consequence resultS reflects 1 impact area   
                if (damageResult.RegionID.Equals(impactAreaID))
                {
                    if (damageResult.DamageCategory.Equals(damageCategory))
                    {
                        if (damageResult.AssetCategory.Equals(assetCategory))
                        {
                            return damageResult;
                        } 
                    }
                }
            }
            ReportMessage(this, new MessageEventArgs(new Message("The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned.")));
            ConsequenceResult dummyResult = new ConsequenceResult();
            return dummyResult;
        }
        public bool Equals(ConsequenceResults inputDamageResults)
        {
           foreach (ConsequenceResult damageResult in _consequenceResultList)
           {
               ConsequenceResult inputDamageResult = inputDamageResults.GetConsequenceResult(damageResult.DamageCategory, damageResult.AssetCategory, damageResult.RegionID);
               bool resultsMatch = damageResult.Equals(inputDamageResult);
               if (!resultsMatch)
               {
                   return false;
               }
            }
            return true;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        public XElement WriteToXML()
        {
            XElement masterElem = new XElement("EAD_Results");
            foreach (ConsequenceResult damageResult in _consequenceResultList)
            {
                XElement damageResultElement = damageResult.WriteToXML();
                damageResultElement.Name = $"{damageResult.DamageCategory}-{damageResult.AssetCategory}";
                masterElem.Add(damageResultElement);
            }
            masterElem.SetAttributeValue("ImpactAreaID", _regionID);
            return masterElem;
        }

        public static ConsequenceResults ReadFromXML(XElement xElement)
        {
            List<ConsequenceResult> damageResults = new List<ConsequenceResult>();
            foreach (XElement histogramElement in xElement.Elements())
            {
                damageResults.Add(ConsequenceResult.ReadFromXML(histogramElement));
            }
            int impactAreaID = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new ConsequenceResults(damageResults,impactAreaID);
        }

        #endregion
    }
}