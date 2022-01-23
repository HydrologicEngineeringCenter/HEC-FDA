using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics;
using Statistics.Histograms;
using paireddata;

namespace metrics
{
    public class ProjectPerformanceResults
    {
        private const double AEP_HISTOGRAM_DEFAULT_BINWIDTH = .0001;
        private const double CNEP_HISTOGRAM_DEFAULT_BINWIDTH = .01;
        private bool _calculatePerformanceForLevee;
        //TODO: handle performance by different threshold types 
        private ThresholdEnum _thresholdType;
        private double _thresholdValue;
        private Histogram _aep = null;
        private Dictionary<double, Histogram> _cnep;
        private UncertainPairedData _leveeCurve;
        private object _aepLock = new object();
        private object _cneplock = new object();

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
        public ProjectPerformanceResults(ThresholdEnum thresholdType, double thresholdValue, UncertainPairedData leveeCurve)
        {
            _leveeCurve = leveeCurve;
            _calculatePerformanceForLevee = true;
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _aep = null; //is this necessary?
            _cnep = new Dictionary<double, Histogram>();

        }
        public void AddAEPEstimate(double aepEstimate)
        {
            lock (_aepLock)
            {
                if (_aep == null)
                {
                    var histo = new Histogram(aepEstimate, AEP_HISTOGRAM_DEFAULT_BINWIDTH);
                    _aep = histo;
                
                }
                _aep.AddObservationToHistogram(aepEstimate);
            }

        }

        public void AddStageForCNEP(double standardNonExceedanceProbability, double stageForCNEP)
        {
            lock (_cneplock)
            {
                if (!_cnep.ContainsKey(standardNonExceedanceProbability))
                {
                    var histo = new Histogram(stageForCNEP, CNEP_HISTOGRAM_DEFAULT_BINWIDTH);
                    _cnep.Add(standardNonExceedanceProbability, histo);
                }
                _cnep[standardNonExceedanceProbability].AddObservationToHistogram(stageForCNEP);
            }

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
            if (_calculatePerformanceForLevee)
            {
                return CalculateConditionalNonExceedanceProbabilityForLevee(exceedanceProbability);
            }
            else
            {
                double conditionalNonExceedanceProbability = _cnep[exceedanceProbability].CDF(_thresholdValue);
                return conditionalNonExceedanceProbability;
            }

        }

        private double CalculateConditionalNonExceedanceProbabilityForLevee(double exceedanceProbability)
        {
            IPairedData medianLeveeCurve = _leveeCurve.SamplePairedData(0.5);
            double stageStep = _cnep[exceedanceProbability].BinWidth;
            int stageStepQuantity = _cnep[exceedanceProbability].BinCounts.Length;
            double[] stages = _leveeCurve.xs();
            double firstStage = stages[0];

            double currentStage;
            double nextStage;
            double currentCumulativeExceedanceProbability = 0;
            double nextCumulativeExceedanceProbability = 0;
            double incrementalProbability = 0;
            double averageStage = 0;
            double geotechnicalFailureAtAverageStage = 0;
            double incrementalProbabilityWithFailure = 0;
            double exceedanceProbabilityWithFailure = 0;
            double conditionalNonExceedanceProbability;
            int i = 0;
            //calculate from the bottom of the fragility curve up, until certain failure  
            while (geotechnicalFailureAtAverageStage < 1)
            { 
                currentStage = firstStage + i * stageStep;
                nextStage = currentStage + stageStep;
                currentCumulativeExceedanceProbability = 1-_cnep[exceedanceProbability].CDF(currentStage);
                nextCumulativeExceedanceProbability = 1-_cnep[exceedanceProbability].CDF(nextStage);
                incrementalProbability = currentCumulativeExceedanceProbability - nextCumulativeExceedanceProbability;
                averageStage = (currentStage + nextStage) / 2;
                geotechnicalFailureAtAverageStage = medianLeveeCurve.f(averageStage);
                incrementalProbabilityWithFailure = incrementalProbability * geotechnicalFailureAtAverageStage;
                exceedanceProbabilityWithFailure += incrementalProbabilityWithFailure;
                i++;
            }
            //correct cumulative probability with failure by removing incorrect incremental probability with failure 
            exceedanceProbabilityWithFailure -= incrementalProbabilityWithFailure;
            //the incremental probability with failure for the stage at which prob(failure) = 1 is the current cumulative exceedance probability
            exceedanceProbabilityWithFailure += currentCumulativeExceedanceProbability;
            conditionalNonExceedanceProbability = 1 - exceedanceProbabilityWithFailure;
            return conditionalNonExceedanceProbability;
        }

        public double LongTermExceedanceProbability(double years)
        {
            double ltep = 1 - Math.Pow((1 - MeanAEP()), years);
            return ltep;
        }
    }
}
