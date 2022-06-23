using System;
using Statistics.Histograms;
using Statistics;
using System.Xml.Linq;
using System.Runtime.Remoting;
using System.Reflection;
using Statistics.Distributions;
namespace metrics
{ //TODO: I THINK SOME OR ALL OF THIS CLASS SHOULD BE INTERNAL 
    public class ConsequenceDistributionResult
    {
        #region Fields
        //TODO: hard-wiring the bin width is no good
        private IHistogram _consequenceHistogram;
        private string _damageCategory;
        private string _assetCategory;
        private int _regionID;
        private ConvergenceCriteria _convergenceCriteria;
        private bool _isNull;
        #endregion

        #region Properties
        public IHistogram ConsequenceHistogram
        {
            get
            {
                return _consequenceHistogram;
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
        public ConvergenceCriteria ConvergenceCriteria
        {
            get
            {
                return _convergenceCriteria;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This constructor builds a ThreadsafeInlineHistogram. Only use for parallel computes. 
        /// </summary>
        public ConsequenceDistributionResult()
        {
            _damageCategory = "unassigned";
            _assetCategory = "unassigned";
            _regionID = 0;
            _convergenceCriteria = new ConvergenceCriteria();
            _consequenceHistogram = new ThreadsafeInlineHistogram(_convergenceCriteria);
            _isNull = true;
        }
        /// <summary>
        /// This constructor builds a ThreadsafeInlineHistogram. Only use for parallel computes. 
        /// </summary>
        public ConsequenceDistributionResult(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _regionID = impactAreaID;
            _convergenceCriteria = convergenceCriteria;
            _consequenceHistogram = new ThreadsafeInlineHistogram(_convergenceCriteria);
            _isNull = false;
        }
        /// <summary>
        /// This constructor builds a ThreadsafeInlineHistogram. Only use for parallel computes. 
        /// </summary>
        public ConsequenceDistributionResult(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID, double binWidth)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _regionID = impactAreaID;
            _convergenceCriteria = convergenceCriteria;
            _consequenceHistogram = new ThreadsafeInlineHistogram(binWidth, _convergenceCriteria);
            _isNull = false;
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
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _consequenceHistogram = histogram;
            _convergenceCriteria = _consequenceHistogram.ConvergenceCriteria;
            _regionID = impactAreaID;
            _isNull = false;

        }
        #endregion

        #region Methods
        internal void AddConsequenceRealization(double damageRealization, int iteration)
        {
            _consequenceHistogram.AddObservationToHistogram(damageRealization, iteration);
        }

        internal double MeanExpectedAnnualConsequences()
        {
            return _consequenceHistogram.Mean;
        }

        internal double ConsequenceExceededWithProbabilityQ(double exceedanceProbability)
        {
            double nonExceedanceProbability = 1 - exceedanceProbability;
            double quartile = _consequenceHistogram.InverseCDF(nonExceedanceProbability);
            return quartile;
        }

        public bool Equals(ConsequenceDistributionResult damageResult)
        {
                bool histogramsMatch = _consequenceHistogram.Equals(damageResult.ConsequenceHistogram);
                if (!histogramsMatch)
                {
                    return false;
                }
            return true;
        }

        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("ConsequenceResult");
            masterElement.SetAttributeValue("Type", _consequenceHistogram.MyType);
            XElement histogramElement = _consequenceHistogram.WriteToXML();
            histogramElement.Name = "DamageHistogram";
            masterElement.Add(histogramElement);
            masterElement.SetAttributeValue("DamageCategory", _damageCategory);
            masterElement.SetAttributeValue("AssetCategory", _assetCategory);
            masterElement.SetAttributeValue("ImpactAreaID", _regionID);
            return masterElement;
        }

        public static ConsequenceDistributionResult ReadFromXML(XElement xElement)
        {
            string type = xElement.Attribute("Type").Value;
            IHistogram damageHistogram;
            if (type.Equals("Histogram"))
            {
                damageHistogram = Histogram.ReadFromXML(xElement.Element("DamageHistogram"));
            } 
            else
            {
                damageHistogram = ThreadsafeInlineHistogram.ReadFromXML(xElement.Element("DamageHistogram"));

            }
            string damageCategory = xElement.Attribute("DamageCategory").Value;
            string assetCategory = xElement.Attribute("AssetCategory").Value;
            int id = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new ConsequenceDistributionResult(damageCategory, assetCategory, damageHistogram, id);
        }
        #endregion
    }
}
