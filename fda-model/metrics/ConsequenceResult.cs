using System;
using Statistics.Histograms;
using Statistics;
using System.Xml.Linq;


namespace metrics
{ //TODO: I THINK SOME OR ALL OF THIS CLASS SHOULD BE INTERNAL 
    public class ConsequenceResult
    {
        #region Fields
        //TODO: hard-wiring the bin width is no good
        private const double HISTOGRAM_BINWIDTH = 10;
        private ThreadsafeInlineHistogram _consequenceHistogram;
        private string _damageCategory;
        private string _assetCategory;
        private int _impactAreaID;
        private ConvergenceCriteria _convergenceCriteria;
        private bool _isNull;
        #endregion

        #region Properties
        public ThreadsafeInlineHistogram DamageHistogram
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
        public int ImpactAreaID
        {
            get
            {
                return _impactAreaID;
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
        public ConsequenceResult()
        {
            _damageCategory = "unassigned";
            _assetCategory = "unassigned";
            _impactAreaID = 0;
            _convergenceCriteria = new ConvergenceCriteria();
            _consequenceHistogram = new ThreadsafeInlineHistogram(HISTOGRAM_BINWIDTH, _convergenceCriteria);
            _isNull = true;
        }
        public ConsequenceResult(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _impactAreaID = impactAreaID;
            _convergenceCriteria = convergenceCriteria;
            _consequenceHistogram = new ThreadsafeInlineHistogram(HISTOGRAM_BINWIDTH, _convergenceCriteria);
            _isNull = false;
        }
        public ConsequenceResult(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID, double binWidth)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _impactAreaID = impactAreaID;
            _convergenceCriteria = convergenceCriteria;
            _consequenceHistogram = new ThreadsafeInlineHistogram(binWidth, _convergenceCriteria);
            _isNull = false;
        }
        private ConsequenceResult(string damageCategory, string assetCategory, ThreadsafeInlineHistogram histogram, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _consequenceHistogram = histogram;
            _convergenceCriteria = _consequenceHistogram.ConvergenceCriteria;
            _impactAreaID = impactAreaID;
            _isNull = false;

        }
        #endregion

        #region Methods
        internal void AddConsequenceRealization(double damageRealization, Int64 iteration)
        {
            _consequenceHistogram.AddObservationToHistogram(damageRealization, iteration);
        }

        internal double MeanConsequences()
        {
            return _consequenceHistogram.Mean;
        }

        internal double ConsequenceExceededWithProbabilityQ(double exceedanceProbability)
        {
            double nonExceedanceProbability = 1 - exceedanceProbability;
            double quartile = _consequenceHistogram.InverseCDF(nonExceedanceProbability);
            return quartile;
        }

        public bool Equals(ConsequenceResult damageResult)
        {
                bool histogramsMatch = _consequenceHistogram.Equals(damageResult.DamageHistogram);
                if (!histogramsMatch)
                {
                    return false;
                }
            return true;
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Consequence");
            XElement histogramElement = _consequenceHistogram.WriteToXML();
            histogramElement.Name = "DamageHistogram";
            masterElement.Add(histogramElement);
            masterElement.SetAttributeValue("DamageCategory", _damageCategory);
            masterElement.SetAttributeValue("AssetCategory", _assetCategory);
            masterElement.SetAttributeValue("ImpactAreaID", _impactAreaID);
            return masterElement;
        }

        public static ConsequenceResult ReadFromXML(XElement xElement)
        {
            ThreadsafeInlineHistogram damageHistogram = ThreadsafeInlineHistogram.ReadFromXML(xElement.Element("DamageHistogram"));
            string damageCategory = xElement.Attribute("DamageCategory").Value;
            string assetCategory = xElement.Attribute("AssetCategory").Value;
            int id = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new ConsequenceResult(damageCategory, assetCategory, damageHistogram, id);
        }
        #endregion
    }
}
