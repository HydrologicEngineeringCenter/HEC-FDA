using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics;
using Statistics.Histograms;

namespace metrics
{
    public class Performance
{
    private const double AEP_HISTOGRAM_BINWIDTH = .0001;
    private const double CNEP_HISTOGRAM_BINWIDTH = .01;
    private Threshold _threshold;
    private Histogram _aep = null;
    private Dictionary<string, Histogram> _ead;
    private Dictionary<double, Histogram> _cnep;



    public Performance(Threshold threshold)
    {
            _threshold = threshold;
            _aep = null; //is this necessary?
            _cnep = new Dictionary<double, Histogram>();

    }
        public void AddAEPEstimate(double aepEstimate)
        {
            double[] data = new double[1] { aepEstimate };
            IData aep = IDataFactory.Factory(data);
            if (_aep != null)
            {
                _aep.AddObservationToHistogram(aep);
            }
            else
            {
                var histo = new Histogram(aep, AEP_HISTOGRAM_BINWIDTH);
                _aep = histo;
            }


        }

        public void AddStageForCNEP(double standardProbability, double stageForCNEP)
        {
            double[] data = new double[1] { stageForCNEP };
            IData stage = IDataFactory.Factory(data);
            if (_cnep.ContainsKey(standardProbability))
            {
                _cnep[standardProbability].AddObservationToHistogram(stage);
            }
            else
            {
                var histo = new Histogram(stage, CNEP_HISTOGRAM_BINWIDTH);
                _cnep.Add(standardProbability, histo);
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
            double conditionalNonExceedanceProbability = 1 - _cnep[exceedanceProbability].CDF(_threshold.ThresholdValue);
            return conditionalNonExceedanceProbability;
        }

        public double LongTermExceedanceProbability(double years)
        {
            double ltep = 1 - Math.Pow((1 - MeanAEP()), years);
            return ltep;
        }
    }
}
