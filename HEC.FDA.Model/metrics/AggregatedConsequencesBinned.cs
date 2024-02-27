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
    public class AggregatedConsequencesBinned : IReportMessage, IProgressReport
    {
        #region Fields
        private readonly double[] _TempResults;
        private readonly double[] _TempCounts;
        private bool _HistogramNotConstructed = false;
        #endregion

        #region Properties
        public event MessageReportedEventHandler MessageReport;
        public event ProgressReportedEventHandler ProgressReport;
        public IHistogram ConsequenceHistogram { get; private set; }
        public IHistogram DamagedElementQuantityHistogram { get; private set; }
        public string DamageCategory { get; }
        public string AssetCategory { get; }
        public int RegionID { get; } = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE;
        public bool IsNull { get; }
        public ConvergenceCriteria ConvergenceCriteria { get; }
        #endregion 

        #region Constructors
        /// <summary>
        /// This constructor is only used for handling errors. 
        /// </summary>
        public AggregatedConsequencesBinned()
        {
            DamageCategory = "unassigned - this is a dummy object";
            AssetCategory = "unassigned - this is a dummy object";
            RegionID = 0;
            ConvergenceCriteria = new ConvergenceCriteria();
            ConsequenceHistogram = new Histogram();
            DamagedElementQuantityHistogram = new Histogram();
            IsNull = true;
            _TempResults = new double[ConvergenceCriteria.IterationCount];
            _TempCounts = new double[ConvergenceCriteria.IterationCount];
            MessageHub.Register(this);

        }
        public AggregatedConsequencesBinned(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID)
        {
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            ConvergenceCriteria = convergenceCriteria;
            IsNull = false;
            RegionID = impactAreaID;
            _TempResults = new double[ConvergenceCriteria.IterationCount];
            _TempCounts = new double[ConvergenceCriteria.IterationCount];
            _HistogramNotConstructed = true;
            MessageHub.Register(this);

        }
        /// <summary>
        /// This constructor can accept wither a Histogram or a ThreadsageInlineHistogram
        /// as such can be used for both compute types
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="assetCategory"></param>
        /// <param name="histogram"></param>
        /// <param name="impactAreaID"></param>
        public AggregatedConsequencesBinned(string damageCategory, string assetCategory, IHistogram histogram, int impactAreaID)
        {
            DamageCategory = damageCategory;
            AssetCategory = assetCategory;
            ConsequenceHistogram = histogram;
            ConvergenceCriteria = ConsequenceHistogram.ConvergenceCriteria;
            RegionID = impactAreaID;
            IsNull = false;
            MessageHub.Register(this);
            _TempResults = new double[ConvergenceCriteria.IterationCount];
            _TempCounts = new double[ConvergenceCriteria.IterationCount];

        }
        #endregion

        #region Methods
        internal void PutDataIntoHistogram()
        {
            if(_HistogramNotConstructed)
            {
                //this bin quantity only appears relevant for bin width 
                int initialBinQuantity = 100;
                double max = _TempResults.Max();
                double min = _TempResults.Min();
                double range = max - min;
                double binWidth;
                //deterministic result all falls into one bin, binWidth of 1 is sufficient 
                if (range == 0)
                {
                    binWidth = 1;
                }
                else
                {
                    binWidth = initialBinQuantity / range;
                    //don't let the bin width get too small or the histo gets too big
                    //if (binWidth < 0.5)
                    //{
                    //    binWidth = 0.5;
                    //}
                }
                ConsequenceHistogram = new Histogram(binWidth, ConvergenceCriteria);
                DamagedElementQuantityHistogram = new Histogram(binWidth:1, ConvergenceCriteria);
                _HistogramNotConstructed = false;
            }
            ConsequenceHistogram.AddObservationsToHistogram(_TempResults);
            DamagedElementQuantityHistogram.AddObservationsToHistogram(_TempCounts);
            Array.Clear(_TempResults);
        }

        internal void AddConsequenceRealization(double damageRealization, long iteration = 1, int damagedElementsCount = 0)
        {
             _TempResults[iteration] = (damageRealization);
            _TempCounts[iteration] = (damagedElementsCount);

        }

        internal double MeanExpectedAnnualConsequences()
        {
            return ConsequenceHistogram.Mean;
        }

        internal double ConsequenceExceededWithProbabilityQ(double exceedanceProbability)
        {
            double nonExceedanceProbability = 1 - exceedanceProbability;
            double quantile = ConsequenceHistogram.InverseCDF(nonExceedanceProbability);
            return quantile;
        }

        internal double QuantityExceededWithProbabilityQ(double exceedanceProbability)
        {
            double nonExceedanceProbability = 1- exceedanceProbability;
            double quantile = DamagedElementQuantityHistogram.InverseCDF(nonExceedanceProbability);
            return quantile;
        }

        internal bool Equals(AggregatedConsequencesBinned damageResult)
        {
            bool histogramsMatch = ConsequenceHistogram.Equals(damageResult.ConsequenceHistogram);
            if (!histogramsMatch)
            {
                return false;
            }
            return true;
        }
        public static AggregatedConsequencesByQuantile ConvertToSingleEmpiricalDistributionOfConsequences(AggregatedConsequencesBinned consequenceDistributionResult)
        {
            Empirical empirical = Histogram.ConvertToEmpiricalDistribution(consequenceDistributionResult.ConsequenceHistogram);
            return new AggregatedConsequencesByQuantile(consequenceDistributionResult.DamageCategory, consequenceDistributionResult.AssetCategory, empirical, consequenceDistributionResult.RegionID);
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

        public static AggregatedConsequencesBinned ReadFromXML(XElement xElement)
        {
            IHistogram damageHistogram = Histogram.ReadFromXML(xElement.Element("DamageHistogram"));
            string damageCategory = xElement.Attribute("DamageCategory").Value;
            string assetCategory = xElement.Attribute("AssetCategory").Value;
            int id = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new AggregatedConsequencesBinned(damageCategory, assetCategory, damageHistogram, id);
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
