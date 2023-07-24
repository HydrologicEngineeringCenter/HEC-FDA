using System;
using Statistics.Histograms;
using Statistics;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using System.Collections.Generic;
using System.Linq;
using Statistics.Distributions;
using System.Collections.Concurrent;

namespace HEC.FDA.Model.metrics
{ 
    public class ConsequenceDistributionResult : IReportMessage, IProgressReport
    {
        #region Fields
        private readonly double[] _TempResults;
        private bool _HistogramNotConstructed = false;
        #endregion

        #region Properties
        public event MessageReportedEventHandler MessageReport;
        public event ProgressReportedEventHandler ProgressReport;
        public IHistogram ConsequenceHistogram { get; private set; }
        public string DamageCategory { get; }
        public string AssetCategory { get; }
        public int RegionID { get; } = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE;
        public bool IsNull { get; }
        public ConvergenceCriteria ConvergenceCriteria { get; }
        #endregion 

        #region Constructors
        /// <summary>
        /// This constructor builds a ThreadsafeInlineHistogram. Only use for parallel computes. 
        /// </summary>
        public ConsequenceDistributionResult()
        {
            DamageCategory = "unassigned";
            AssetCategory = "unassigned";
            RegionID = 0;
            ConvergenceCriteria = new ConvergenceCriteria();
            ConsequenceHistogram = new ThreadsafeInlineHistogram();
            IsNull = true;
            _TempResults = new double[ConvergenceCriteria.IterationCount];
            MessageHub.Register(this);

        }
        /// <summary>
        /// This constructor builds a ThreadsafeInlineHistogram. Only use for parallel computes. 
        /// This constructor is used only for simulation compute and does not track impact area ID
        /// </summary>
        public ConsequenceDistributionResult(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID)
        {
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            ConvergenceCriteria = convergenceCriteria;
            IsNull = false;
            RegionID = impactAreaID;
            _TempResults = new double[ConvergenceCriteria.IterationCount];
            _HistogramNotConstructed = true;
            MessageHub.Register(this);

        }
        public ConsequenceDistributionResult(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, List<double> consequences, int impactAreaID)
        {
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            ConvergenceCriteria = convergenceCriteria;
            ConsequenceHistogram = new Histogram(consequences, convergenceCriteria);
            RegionID = impactAreaID;
            _TempResults = new double[ConvergenceCriteria.IterationCount];

        }
        /// <summary>
        /// This constructor can accept wither a Histogram or a ThreadsageInlineHistogram
        /// as such can be used for both compute types
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <param name="histogram"></param>
        /// <param name="impactAreaID"></param>
        public ConsequenceDistributionResult(string damageCategory, string assetCategory, IHistogram histogram, int impactAreaID)
        {
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            ConsequenceHistogram = histogram;
            ConvergenceCriteria = ConsequenceHistogram.ConvergenceCriteria;
            RegionID = impactAreaID;
            IsNull = false;
            MessageHub.Register(this);
            _TempResults = new double[ConvergenceCriteria.IterationCount];
        }
        #endregion

        #region Methods
        public void PutDataIntoHistogram()
        {
            if(_HistogramNotConstructed)
            {
                double max = _TempResults.Max();
                double binWidth = max / 1000;
                if (binWidth == 0)
                {
                    binWidth = 1;
                }
                ConsequenceHistogram = new Histogram(binWidth, ConvergenceCriteria);
                _HistogramNotConstructed = false;
            }
            ConsequenceHistogram.AddObservationsToHistogram(_TempResults);
            Array.Clear(_TempResults);
        }

        internal void AddConsequenceRealization(double damageRealization, long iteration = 1)
        {
             _TempResults[iteration] = (damageRealization);
        }

        internal double MeanExpectedAnnualConsequences()
        {
            return ConsequenceHistogram.Mean;
        }

        internal double ConsequenceExceededWithProbabilityQ(double exceedanceProbability)
        {
            double nonExceedanceProbability = 1 - exceedanceProbability;
            double quartile = ConsequenceHistogram.InverseCDF(nonExceedanceProbability);
            return quartile;
        }

        public bool Equals(ConsequenceDistributionResult damageResult)
        {
            bool histogramsMatch = ConsequenceHistogram.Equals(damageResult.ConsequenceHistogram);
            if (!histogramsMatch)
            {
                return false;
            }
            return true;
        }
        public static SingleEmpiricalDistributionOfConsequences ConvertToSingleEmpiricalDistributionOfConsequences(ConsequenceDistributionResult consequenceDistributionResult)
        {
            Empirical empirical = Histogram.ConvertToEmpiricalDistribution(consequenceDistributionResult.ConsequenceHistogram);
            return new SingleEmpiricalDistributionOfConsequences(consequenceDistributionResult.DamageCategory, consequenceDistributionResult.AssetCategory, empirical, consequenceDistributionResult.RegionID);
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new("ConsequenceResult");
            XElement histogramElement = ConsequenceHistogram.ToXML();
            histogramElement.Name = "DamageHistogram";
            masterElement.Add(histogramElement);
            masterElement.SetAttributeValue("DamageCategory", DamageCategory);
            masterElement.SetAttributeValue("AssetCategory", AssetCategory);
            masterElement.SetAttributeValue("ImpactAreaID", RegionID);
            return masterElement;
        }

        public static ConsequenceDistributionResult ReadFromXML(XElement xElement)
        {
            IHistogram damageHistogram = Histogram.ReadFromXML(xElement.Element("DamageHistogram"));
            string damageCategory = xElement.Attribute("DamageCategory").Value;
            string assetCategory = xElement.Attribute("AssetCategory").Value;
            int id = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new ConsequenceDistributionResult(damageCategory, assetCategory, damageHistogram, id);
        }

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }

        public void ReportProgress(object sender, ProgressReportEventArgs e)
        {
            ProgressReport?.Invoke(sender, e);
        }
        #endregion
    }
}
