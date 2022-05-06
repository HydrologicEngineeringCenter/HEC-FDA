using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics.Histograms;
using Statistics;
using System.Xml.Linq;


namespace metrics
{
 public class DamageResult
    {
        #region Fields

        private const double EAD_HISTOGRAM_BINWIDTH = 10;
        private ThreadsafeInlineHistogram _damageHistogram;
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
                return _damageHistogram;
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
        public DamageResult()
        {
            _damageCategory = "unassigned";
            _assetCategory = "unassigned";
            _impactAreaID = 0;
            _convergenceCriteria = new ConvergenceCriteria();
            _damageHistogram = new ThreadsafeInlineHistogram(EAD_HISTOGRAM_BINWIDTH, _convergenceCriteria);
            _isNull = true;
        }
        public DamageResult(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _impactAreaID = impactAreaID;
            _convergenceCriteria = convergenceCriteria;
            _damageHistogram = new ThreadsafeInlineHistogram(EAD_HISTOGRAM_BINWIDTH, _convergenceCriteria);
            _isNull = false;
        }
        public DamageResult(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID, double binWidth)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _impactAreaID = impactAreaID;
            _convergenceCriteria = convergenceCriteria;
            _damageHistogram = new ThreadsafeInlineHistogram(binWidth, _convergenceCriteria);
            _isNull = false;
        }
        private DamageResult(string damageCategory, string assetCategory, ThreadsafeInlineHistogram eadHistogram, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _damageHistogram = eadHistogram;
            _convergenceCriteria = _damageHistogram.ConvergenceCriteria;
            _impactAreaID = impactAreaID;
            _isNull = false;

        }
        #endregion

        #region Methods
        public void AddDamageRealization(double damageRealization, Int64 iteration)
        {
            _damageHistogram.AddObservationToHistogram(damageRealization, iteration);
        }

        public double MeanDamage()
        {
            return _damageHistogram.Mean;
        }

        public double DamageExceededWithProbabilityQ(double exceedanceProbability)
        {
            double nonExceedanceProbability = 1 - exceedanceProbability;
            double quartile = _damageHistogram.InverseCDF(nonExceedanceProbability);
            return quartile;
        }

        public bool Equals(DamageResult damageResult)
        {
                bool histogramsMatch = _damageHistogram.Equals(damageResult.DamageHistogram);
                if (!histogramsMatch)
                {
                    return false;
                }
            return true;
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Damage");
            XElement histogramElement = _damageHistogram.WriteToXML();
            histogramElement.Name = "DamageHistogram";
            masterElement.Add(histogramElement);
            masterElement.SetAttributeValue("DamageCategory", _damageCategory);
            masterElement.SetAttributeValue("AssetCategory", _assetCategory);
            masterElement.SetAttributeValue("ImpactAreaID", _impactAreaID);
            return masterElement;
        }

        public static DamageResult ReadFromXML(XElement xElement)
        {
            ThreadsafeInlineHistogram damageHistogram = ThreadsafeInlineHistogram.ReadFromXML(xElement.Element("DamageHistogram"));
            string damageCategory = xElement.Attribute("DamageCategory").Value;
            string assetCategory = xElement.Attribute("AssetCategory").Value;
            int id = Convert.ToInt32(xElement.Attribute("ImpactAreaID").Value);
            return new DamageResult(damageCategory, assetCategory, damageHistogram, id);
        }
        #endregion
    }
}
