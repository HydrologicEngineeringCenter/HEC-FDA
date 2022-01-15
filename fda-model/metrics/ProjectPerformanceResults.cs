using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics;
using Statistics.Histograms;

namespace metrics
{
    public class ProjectPerformanceResults
{
    private const double AEP_HISTOGRAM_DEFAULT_BINWIDTH = .0001;
    private const double CNEP_HISTOGRAM_DEFAULT_BINWIDTH = .01;
        //TODO: handle performance by different threshold types 
        private ThresholdEnum _thresholdType;
        private double _thresholdValue;
    private Histogram _aep = null;
    private Dictionary<double, Histogram> _cnep;

    public Histogram HistogramOfAEPs
        {
            get
            {
                return _aep;
            }
        }

    public ProjectPerformanceResults(ThresholdEnum thresholdType, double thresholdValue)
    {
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _aep = null; //is this necessary?
            _cnep = new Dictionary<double, Histogram>();

    }
        public void AddAEPEstimate(double aepEstimate)
        {
            if (_aep == null)
            {
                var histo = new Histogram(null, AEP_HISTOGRAM_DEFAULT_BINWIDTH);
                _aep = histo;
                
            }
            _aep.AddObservationToHistogram(aepEstimate);
        }

        public void AddStageForCNEP(double standardNonExceedanceProbability, double stageForCNEP)
        {
            if (!_cnep.ContainsKey(standardNonExceedanceProbability))
            {
                var histo = new Histogram(null, CNEP_HISTOGRAM_DEFAULT_BINWIDTH);
                _cnep.Add(standardNonExceedanceProbability, histo);
            }
            _cnep[standardNonExceedanceProbability].AddObservationToHistogram(stageForCNEP);
        }

        public double MeanAEP()
        {
            return _aep.Mean;
        }

        public double MedianAEP()
        {
            return _aep.InverseCDF(0.5);
        }

        public double AssuranceOfAEP(double exceedanceProbability)
        {   //assurance of AEP is a non-exceedance probability so we use CDF as is 
            double assuranceOfAEP = _aep.CDF(exceedanceProbability);
            return assuranceOfAEP;
        }

        public double ConditionalNonExceedanceProbability(double exceedanceProbability)
        {
            double conditionalNonExceedanceProbability = _cnep[exceedanceProbability].CDF(_thresholdValue);
            return conditionalNonExceedanceProbability;
        }

        public double LongTermExceedanceProbability(double years)
        {
            double ltep = 1 - Math.Pow((1 - MeanAEP()), years);
            return ltep;
        }
    }
}
