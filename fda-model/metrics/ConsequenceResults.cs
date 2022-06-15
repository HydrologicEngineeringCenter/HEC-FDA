using System;
using System.Collections.Generic;
using Statistics;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Model.Messaging;
using Statistics.Histograms;

namespace metrics
{ 
    public class ConsequenceResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields
        private List<ConsequenceResult> _consequenceResultList;
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
            _isNull = true;
        }
        private ConsequenceResults(List<ConsequenceResult> damageResults)
        {
            _consequenceResultList = damageResults;
            _isNull = false;

        }
        #endregion

        #region Methods 
        public void AddNewConsequenceResultObject(string damageCategory, string assetCategory, int impactAreaID)
        {
            ConsequenceResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            if (damageResult.IsNull)
            {
                ConsequenceResult newDamageResult = new ConsequenceResult(damageCategory, assetCategory, impactAreaID);
                _consequenceResultList.Add(newDamageResult);
            }
        }
        public void AddExistingConsequenceResultObject(ConsequenceResult consequenceResultToAdd)
        {
            ConsequenceResult consequenceResult = GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.AssetCategory, consequenceResultToAdd.RegionID);
            if (consequenceResult.IsNull)
            {
                _consequenceResultList.Add(consequenceResultToAdd);
            }
        }
        public void IncrementConsequenceRealization(double damageEstimate, string damageCategory, string assetCategory, int impactAreaID)
        {
            ConsequenceResult damageResult = GetConsequenceResult(damageCategory, assetCategory, impactAreaID);
            damageResult.IncrementConsequence(damageEstimate);
        }

        /// <summary>
        /// This method returns a consequence result for the given damage category, asset category, and impact area 
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <param name="impactAreaID"></param>
        /// <returns></returns>
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
            string message = "The requested damage category - asset category - impact area combination could not be found. An arbitrary object is being returned";
            ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
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
            return masterElem;
        }

        public static ConsequenceResults ReadFromXML(XElement xElement)
        {
            List<ConsequenceResult> damageResults = new List<ConsequenceResult>();
            foreach (XElement histogramElement in xElement.Elements())
            {
                damageResults.Add(ConsequenceResult.ReadFromXML(histogramElement));
            }
            return new ConsequenceResults(damageResults);
        }

        #endregion
    }
}