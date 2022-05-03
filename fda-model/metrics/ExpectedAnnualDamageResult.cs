using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics.Histograms;
using Statistics;

namespace metrics
{
 public class ExpectedAnnualDamageResult
{
        #region Fields

        private const double EAD_HISTOGRAM_BINWIDTH = 10;
        private ThreadsafeInlineHistogram _eadHistogram;
        private string _damageCategory;
        private string _assetCategory;
        #endregion

        #region Properties
        public ThreadsafeInlineHistogram EADHistogram
        {
            get
            {
                return _eadHistogram;
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
        #endregion

        #region Constructors
        public ExpectedAnnualDamageResult(string damageCategory, string assetCategory, ConvergenceCriteria convergenceCriteria)
        {
            _damageCategory = damageCategory;
            _assetCategory = assetCategory;
            _eadHistogram = new ThreadsafeInlineHistogram(EAD_HISTOGRAM_BINWIDTH,convergenceCriteria);
        }
        #endregion

        #region Methods
        public void AddEADRealization(double ead, int iteration)
        {
            _eadHistogram.AddObservationToHistogram(ead, iteration);
        }

        public double MeanEAD()
        {
            return _eadHistogram.Mean;
        }

        public double EADExceededWithProbabilityQ(double exceedanceProbability)
        {
            double nonExceedanceProbability = 1 - exceedanceProbability;
            double quartile = _eadHistogram.InverseCDF(nonExceedanceProbability);
            return quartile;
        }

        public bool Equals(ExpectedAnnualDamageResult expectedAnnualDamageResult)
        {
                bool histogramsMatch = _eadHistogram.Equals(expectedAnnualDamageResult.EADHistogram);
                if (!histogramsMatch)
                {
                    return false;
                }
            return true;
        }
        #endregion
    }
}
