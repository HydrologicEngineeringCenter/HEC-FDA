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
            ConsequenceResult dummyConsequenceResult = new ConsequenceResult();
            _consequenceResultList.Add(dummyConsequenceResult);
            _isNull = false;
            MessageHub.Register(this);
        }
        internal ConsequenceResults(bool isNull)
        {
            _consequenceResultList = new List<ConsequenceResult>();
            ConsequenceResult dummyConsequenceResult = new ConsequenceResult();
            _consequenceResultList.Add(dummyConsequenceResult);
            _isNull = isNull;
            MessageHub.Register(this);
        }
        private ConsequenceResults(List<ConsequenceResult> damageResults)
        {
            _consequenceResultList = damageResults;
            _isNull = false;
            MessageHub.Register(this);

        }
        #endregion

        #region Methods 
        public void AddNewConsequenceResultObject(string damageCategory, int impactAreaID, double structureDamage = 0, double contentDamage = 0, double otherDamage = 0, double vehicleDamage = 0)
        {
            ConsequenceResult damageResult = GetConsequenceResult(damageCategory, impactAreaID);
            if (damageResult.IsNull)
            {
                ConsequenceResult newDamageResult = new ConsequenceResult(damageCategory, impactAreaID);
                newDamageResult.IncrementConsequence(structureDamage, contentDamage, vehicleDamage, otherDamage);
                _consequenceResultList.Add(newDamageResult);
            }
        }
        public void AddExistingConsequenceResultObject(ConsequenceResult consequenceResultToAdd)
        {
            ConsequenceResult consequenceResult = GetConsequenceResult(consequenceResultToAdd.DamageCategory, consequenceResultToAdd.RegionID);
            if (consequenceResult.IsNull)
            {
                _consequenceResultList.Add(consequenceResultToAdd);
            }
            else
            {
                consequenceResult.IncrementConsequence(consequenceResultToAdd.StructureDamage, consequenceResultToAdd.ContentDamage, consequenceResultToAdd.VehicleDamage, consequenceResultToAdd.OtherDamage);
            }
        }

        /// <summary>
        /// This method returns a consequence result for the given damage category, asset category, and impact area 
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="impactAreaID"></param>
        /// <returns></returns>
        public ConsequenceResult GetConsequenceResult(string damageCategory, int impactAreaID)
        {
            foreach (ConsequenceResult damageResult in _consequenceResultList)
            {
                //The impact area should always be equal because a consequence result reflects 1 impact area and a consequence resultS reflects 1 impact area   
                if (damageResult.RegionID.Equals(impactAreaID))
                {
                    if (damageResult.DamageCategory.Equals(damageCategory))
                    {
                        return damageResult;
                    }
                }
            }
            string message = "The requested damage category - impact area combination could not be found. An arbitrary object is being returned";
            ErrorMessage errorMessage = new ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            ConsequenceResult dummyResult = new ConsequenceResult();
            return dummyResult;
        }

        public bool Equals(ConsequenceResults inputDamageResults)
        {
            foreach (ConsequenceResult damageResult in _consequenceResultList)
            {
                ConsequenceResult inputDamageResult = inputDamageResults.GetConsequenceResult(damageResult.DamageCategory, damageResult.RegionID);
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

        #endregion
    }
}