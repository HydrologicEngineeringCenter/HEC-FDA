using System;
using System.Collections.Generic;
using scenarios;
using System.Linq;
namespace alternatives
{
    public class Alternative
    {
        private Scenario _currentYear;
        private Scenario _futureYear;
        private Int64 _periodOfAnalysis;

        //probably need getters and setters
        public Alternative(Scenario currentYear, Scenario futureYear, Int64 periodOfAnalysis){
            _currentYear = currentYear;
            _futureYear = futureYear;
            _periodOfAnalysis = periodOfAnalysis;
        }
        public double ComputeEEAD(interfaces.IProvideRandomNumbers rp, Int64 iterations, double discountRate){
            //probably instantiate a rng to seed each impact area differently
            IList<metrics.IContainResults> baseYearResults = _currentYear.Compute(rp,iterations);//this is a list of impact area-specific ead
            IList<metrics.IContainResults> mlfYearResults = _futureYear.Compute(rp, iterations);
            //I am not sure how to use the IContainResults
            double baseEAD = baseYearResults.Last().MeanEAD("Total");
            double mlfEAD = mlfYearResults.Last().MeanEAD("Total");
            double[] interpolatedEADs = Interpolate(baseEAD, mlfEAD, _currentYear.Year, _futureYear.Year, _periodOfAnalysis);
            double sumPresentValueEAD = PresentValueCompute(interpolatedEADs, discountRate);
            double averageAnnualEquivalentDamage = IntoAverageAnnualEquivalentTerms(sumPresentValueEAD, _periodOfAnalysis, discountRate);
            return averageAnnualEquivalentDamage;
        }
        private double IntoAverageAnnualEquivalentTerms(double sumPresentValueEAD, Int64 periodOfAnalysis, double discountRate)
        {
            double presentValueInterestFactorOfAnnuity = (1 - (1 / Math.Pow(1 + discountRate, periodOfAnalysis))) / discountRate;
            double averageAnnualEquivalentDamage = sumPresentValueEAD / presentValueInterestFactorOfAnnuity;
            return averageAnnualEquivalentDamage;
        }
        private double PresentValueCompute(double[] interpolatedEADs, double discountRate)
        {
            Int64 periodOfAnalysis = interpolatedEADs.Length;
            double[] presentValueInterestFactor = new double[periodOfAnalysis];
            double sumPresentValueEAD = 0;
            for (int i=0; i<periodOfAnalysis; i++)
            {
                presentValueInterestFactor[i] = 1 / Math.Pow(1 + discountRate, i+1);
                sumPresentValueEAD += interpolatedEADs[i] * presentValueInterestFactor[i];
            }
            return sumPresentValueEAD;
        }
        private double[] Interpolate(double baseEAD, double mlfEAD, Int64 baseYear, Int64 mlfYear, Int64 periodOfAnalysis)
        {
            Int64 yearsBetweenBaseAndMLFInclusive = mlfYear - baseYear;
            Int64 yearsAfterMLF = periodOfAnalysis - mlfYear;
            double[] interpolatedEADs = new double[periodOfAnalysis];
            for (Int64 i =0; i<yearsBetweenBaseAndMLFInclusive; i++)
            {
                interpolatedEADs[i] = baseEAD + (i / yearsBetweenBaseAndMLFInclusive) * (baseEAD - mlfEAD);
            }
            for (Int64 i = yearsBetweenBaseAndMLFInclusive; i<periodOfAnalysis; i++)
            {
                interpolatedEADs[i] = mlfEAD;
            }
            return interpolatedEADs;
        }



    }
}