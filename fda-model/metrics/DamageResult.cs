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
        #endregion

        #region Constructors
        public DamageResult()
        {
            _damageCategory = "unassigned";
            _assetCategory = "unassigned";
            _impactAreaID = 0;
            ConvergenceCriteria convergenceCriteria = new ConvergenceCriteria();
            _damageHistogram = new ThreadsafeInlineHistogram(EAD_HISTOGRAM_BINWIDTH, convergenceCriteria);
        }
        public DamageResult(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _impactAreaID = impactAreaID;
            _damageHistogram = new ThreadsafeInlineHistogram(EAD_HISTOGRAM_BINWIDTH,convergenceCriteria);
        }
        private DamageResult(string damageCategory, string assetCategory, ThreadsafeInlineHistogram eadHistogram, int impactAreaID)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _damageHistogram = eadHistogram;
            _impactAreaID = impactAreaID;
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
