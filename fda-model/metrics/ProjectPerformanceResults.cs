using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics;
using Statistics.Histograms;

namespace metrics
{
    public class ProjectPerformance
{
        //TODO: Everything should be written in terms on non-exceedance probability 
    private const double AEP_HISTOGRAM_DEFAULT_BINWIDTH = .0001;
    private const double CNEP_HISTOGRAM_DEFAULT_BINWIDTH = .01;
        private ThresholdEnum _thresholdType;
        private double _thresholdValue;
    private Histogram _aep = null;
    private Dictionary<double, Histogram> _cnep;



    public ProjectPerformance(ThresholdEnum thresholdType, double thresholdValue)
    {
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _aep = null; //is this necessary?
            _cnep = new Dictionary<double, Histogram>();

    }
        public void AddAEPEstimate(double aepEstimate)
        {
            double[] data = new double[1] { aepEstimate };
            IData aep = IDataFactory.Factory(data);
            if (_aep == null)
            {
                var histo = new Histogram(null, AEP_HISTOGRAM_DEFAULT_BINWIDTH);
                _aep = histo;
                
            }
            _aep.AddObservationToHistogram(aep);
        }

        public void AddStageForCNEP(double standardNonExceedanceProbability, double stageForCNEP)
        {
            double[] data = new double[1] { stageForCNEP };
            IData stage = IDataFactory.Factory(data);
            if (_cnep.ContainsKey(standardNonExceedanceProbability))
            {
                _cnep[standardNonExceedanceProbability].AddObservationToHistogram(stage);
            }
            else
            {
                var histo = new Histogram(stage, CNEP_HISTOGRAM_DEFAULT_BINWIDTH);
                _cnep.Add(standardNonExceedanceProbability, histo);
            }
        }

        public double MeanAEP()
        {
            return _aep.Mean;
        }

        public double MedianAEP()
        {
            return _aep.Median;
        }

        public double AssuranceOfAEP(double exceedanceProbability)
        {   //assurance of AEP is a non-exceedance probability
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
