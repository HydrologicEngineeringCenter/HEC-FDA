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
        private ThreadsafeInlineHistogram _aep = null;
        private Dictionary<double, ThreadsafeInlineHistogram> _cnep;
        private UncertainPairedData _leveeCurve;

        public ThreadsafeInlineHistogram HistogramOfAEPs
        {
            get
            {
                return _aep;
            }
        }
        public Dictionary<double, ThreadsafeInlineHistogram> CNEPHistogramOfStages
        {
            get
            {
                return _cnep;
            }
        }
        public ProjectPerformanceResults(ThresholdEnum thresholdType, double thresholdValue, ConvergenceCriteria c)
        {
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _aep = new ThreadsafeInlineHistogram(AEP_HISTOGRAM_DEFAULT_BINWIDTH, c);
            _aep.SetIterationSize(c.MaxIterations);
            _cnep = new Dictionary<double, ThreadsafeInlineHistogram>();

        }
        public ProjectPerformanceResults(ThresholdEnum thresholdType, double thresholdValue, UncertainPairedData leveeCurve, ConvergenceCriteria  c)
        {
            _leveeCurve = leveeCurve;
            _calculatePerformanceForLevee = true;
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _aep = new ThreadsafeInlineHistogram(AEP_HISTOGRAM_DEFAULT_BINWIDTH, c);
            _aep.SetIterationSize(c.MaxIterations);
            _cnep = new Dictionary<double, ThreadsafeInlineHistogram>();

        }
        public void AddAEPEstimate(double aepEstimate, Int64 iteration)
        {
            _aep.AddObservationToHistogram(aepEstimate, iteration);
        }
        public void AddConditionalNonExceedenceProbabilityKey(double standardNonExceedanceProbability, ConvergenceCriteria c)
        {
            if (!_cnep.ContainsKey(standardNonExceedanceProbability))
            {
                var histo = new ThreadsafeInlineHistogram(CNEP_HISTOGRAM_DEFAULT_BINWIDTH, c);
                histo.SetIterationSize(c.MaxIterations);
                _cnep.Add(standardNonExceedanceProbability, histo);
            }
        }

        public void AddStageForCNEP(double standardNonExceedanceProbability, double stageForCNEP, Int64 iteration)
        {
            _cnep[standardNonExceedanceProbability].AddObservationToHistogram(stageForCNEP, iteration);
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
        public bool ConditionalNonExceedanceProbabilityIsConverged()
        {
            //dont like this.
            //couldn't we nix the foreach and just return _cnep[0.98].IsConverged?
            foreach (double key in _cnep.Keys)
            {
                if (key == 0.98)
                {
                    return _cnep[key].IsConverged;
                }
            }
            return false;
        }
        public bool ConditionalNonExceedanceProbabilityTestForConvergence(double upperConfidenceLimitProb, double lowerConfidenceLimitProb)
        {
            //dont like this.
            foreach( double key in _cnep.Keys)
            {
                if(key == 0.98)
                {
                    return _cnep[key].TestForConvergence(upperConfidenceLimitProb,lowerConfidenceLimitProb);
                }
            }
            return false;
        }
        public Int64 ConditionalNonExceedanceProbabilityRemainingIterations(double upper, double lower)
        {
            //dont like this
            foreach (double key in _cnep.Keys)
            {
                if (key == 0.98)
                {
                    return _cnep[key].EstimateIterationsRemaining(upper, lower);
                }
            }
            return 0;
        }
        public double ConditionalNonExceedanceProbability(double exceedanceProbability)
        {
            if (_calculatePerformanceForLevee)
            {
                return CalculateConditionalNonExceedanceProbabilityForLevee(exceedanceProbability);
            }
            else
            {
                _cnep[exceedanceProbability].ForceDeQueue();
                double conditionalNonExceedanceProbability = _cnep[exceedanceProbability].CDF(_thresholdValue);
                return conditionalNonExceedanceProbability;
            }

        }

        private double CalculateConditionalNonExceedanceProbabilityForLevee(double exceedanceProbability)
        {
            IPairedData medianLeveeCurve = _leveeCurve.SamplePairedData(0.5);
            _cnep[exceedanceProbability].ForceDeQueue();
            double stageStep = _cnep[exceedanceProbability].BinWidth;
            int stageStepQuantity = _cnep[exceedanceProbability].BinCounts.Length;
            double[] stages = _leveeCurve.Xvals;
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
        public void ParallelTestForConvergence(double upper, double lower)
        {
            double[] keys = new double[_cnep.Keys.Count];
            int idx = 0;
            foreach (var keyvaluepair in _cnep)
            {
                keys[idx] = keyvaluepair.Key;
                idx++;
            }
            Parallel.For(0, keys.Length, i =>
            {
                _cnep[keys[i]].TestForConvergence(upper,lower);//this will force dequeue also.
            });
        }
    }
}
