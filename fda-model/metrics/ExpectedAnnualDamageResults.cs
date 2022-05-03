using System;
using System.Collections.Generic;
using Statistics;
using Statistics.Histograms;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Enumerations;

namespace metrics
{
    public class ExpectedAnnualDamageResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields
        private List<ExpectedAnnualDamageResult> _eadResultList;
        #endregion

        #region Properties 
        public List<ExpectedAnnualDamageResult> EADResultList
        {
            get
            {
                return _eadResultList;
            }
        }
        public event MessageReportedEventHandler MessageReport;

        #endregion
        #region Constructors
        public ExpectedAnnualDamageResults(){
            _eadResultList = new List<ExpectedAnnualDamageResult>();
        }
        private ExpectedAnnualDamageResults(List<ExpectedAnnualDamageResult> expectedAnnualDamageResults)
        {
            _eadResultList = expectedAnnualDamageResults;
        }
        #endregion

        #region Methods 
        public void AddEADResultObject(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria)
        {
            foreach (ExpectedAnnualDamageResult expectedAnnualDamageResult in _eadResultList)
            {
                if (expectedAnnualDamageResult.DamageCategory != damageCategory)
                {
                    if (expectedAnnualDamageResult.AssetCategory != assetCategory)
                    {
                        ExpectedAnnualDamageResult newExpectedAnnualDamageResult = new ExpectedAnnualDamageResult(damageCategory, assetCategory, convergenceCriteria);
                        _eadResultList.Add(newExpectedAnnualDamageResult);
                    }
                }
            }
        }
        public void AddEADEstimate(double eadEstimate, string damageCategory, string assetCategory, Int64 iteration)
        {

            foreach (ExpectedAnnualDamageResult expectedAnnualDamageResult in _eadResultList)
            {
                if (expectedAnnualDamageResult.DamageCategory == damageCategory)
                {
                    if (expectedAnnualDamageResult.AssetCategory == assetCategory)
                    {
                        expectedAnnualDamageResult.AddEADRealization(eadEstimate, iteration);
                    }
                }
            }
        }
        public double MeanEAD(string damageCategory, string assetCategory = "unassigned")
        {
            foreach (ExpectedAnnualDamageResult expectedAnnualDamageResult in _eadResultList)
            {
                if (expectedAnnualDamageResult.DamageCategory == damageCategory)
                {
                    if (expectedAnnualDamageResult.AssetCategory == assetCategory)
                    {
                        return expectedAnnualDamageResult.MeanEAD();
                    }
                }
 
            }
            ReportMessage(this, new MessageEventArgs(new Message("The requested damage category - asset category combination could not be found. An arbitrary result of 0 is being returned.")));
            return 0;
        }

        public double EADExceededWithProbabilityQ(string damageCategory, double exceedanceProbability, string assetCategory = "unassigned")
        {
            foreach (ExpectedAnnualDamageResult expectedAnnualDamageResult in _eadResultList)
            {
                if (expectedAnnualDamageResult.DamageCategory == damageCategory)
                {
                    if (expectedAnnualDamageResult.AssetCategory == assetCategory)
                    {
                        double quartile = expectedAnnualDamageResult.EADExceededWithProbabilityQ(exceedanceProbability);
                        return quartile;
                    }
                }

            }
            ReportMessage(this, new MessageEventArgs(new Message("The requested damage category - asset category combination could not be found. An arbitrary result of 0 is being returned.")));
            return 0;
        }
        public ExpectedAnnualDamageResult GetExpectedAnnualDamageResult(string damageCategory, string assetCategory)
        {
            foreach (ExpectedAnnualDamageResult expectedAnnualDamageResult in _eadResultList)
            {
                if (expectedAnnualDamageResult.DamageCategory == damageCategory)
                {
                    if (expectedAnnualDamageResult.AssetCategory == assetCategory)
                    {
                        return expectedAnnualDamageResult;
                    }
                }

            }
            ReportMessage(this, new MessageEventArgs(new Message("The requested damage category - asset category combination could not be found. An arbitrary result of 0 is being returned.")));
            ExpectedAnnualDamageResult dummyResult = new ExpectedAnnualDamageResult();
            return dummyResult;
        }
        public bool Equals(ExpectedAnnualDamageResults expectedAnnualDamageResults)
        {
           foreach (ExpectedAnnualDamageResult expectedAnnualDamageResult in _eadResultList)
           {
               foreach (ExpectedAnnualDamageResult inputExpectedAnnualDamageResult in expectedAnnualDamageResults.EADResultList)
                {
                    if (expectedAnnualDamageResult.DamageCategory == inputExpectedAnnualDamageResult.DamageCategory)
                    {
                        if (expectedAnnualDamageResult.AssetCategory == inputExpectedAnnualDamageResult.AssetCategory)
                        {
                            bool resultsMatch = expectedAnnualDamageResult.Equals(inputExpectedAnnualDamageResult);
                            if (!resultsMatch)
                            {
                                return false;
                            }
                        }
                    }
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
            foreach (ExpectedAnnualDamageResult expectedAnnualDamageResult in _eadResultList)
            {
                XElement expectedAnnualDamageResultElement = expectedAnnualDamageResult.WriteToXML();
                expectedAnnualDamageResultElement.Name = $"{expectedAnnualDamageResult.DamageCategory}-{expectedAnnualDamageResult.AssetCategory}";
                masterElem.Add(expectedAnnualDamageResultElement);
            }
            return masterElem;
        }

        public static ExpectedAnnualDamageResults ReadFromXML(XElement xElement)
        {
            List<ExpectedAnnualDamageResult> expectedAnnualDamageResults = new List<ExpectedAnnualDamageResult>();
            foreach (XElement histogramElement in xElement.Elements())
            {
                expectedAnnualDamageResults.Add(ExpectedAnnualDamageResult.ReadFromXML(histogramElement));
            }
            return new ExpectedAnnualDamageResults(expectedAnnualDamageResults);
        }

        #endregion
    }
}