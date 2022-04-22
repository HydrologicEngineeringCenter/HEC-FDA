using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics;
using Statistics.Histograms;
using paireddata;
using System.Xml.Linq;

namespace metrics
{
    public class ProjectPerformanceResults
    {
        #region Fields
        private const double AEP_HISTOGRAM_DEFAULT_BINWIDTH = .0001;
        private const double CNEP_HISTOGRAM_DEFAULT_BINWIDTH = .01;
        private bool _calculatePerformanceForLevee;
        //TODO: handle performance by different threshold types 
        private ThresholdEnum _thresholdType;
        private double _thresholdValue;
        private ThreadsafeInlineHistogram _aep = null;
        private Dictionary<double, ThreadsafeInlineHistogram> _cnep;
        private UncertainPairedData _systemResponseFunction;
        private ConvergenceCriteria _ConvergenceCriteria;
        #endregion
        #region Properties
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
        #endregion
        #region Constructors 
        public ProjectPerformanceResults(ThresholdEnum thresholdType, double thresholdValue, ConvergenceCriteria convergenceCriteria)
        {
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _aep = new ThreadsafeInlineHistogram(AEP_HISTOGRAM_DEFAULT_BINWIDTH, convergenceCriteria);
            _aep.SetIterationSize(convergenceCriteria.MaxIterations);
            _cnep = new Dictionary<double, ThreadsafeInlineHistogram>();
            _ConvergenceCriteria = convergenceCriteria;

        }
        public ProjectPerformanceResults(ThresholdEnum thresholdType, double thresholdValue, UncertainPairedData systemResponseFunction, ConvergenceCriteria  convergenceCriteria)
        {
            _systemResponseFunction = systemResponseFunction;
            _calculatePerformanceForLevee = true;
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _aep = new ThreadsafeInlineHistogram(AEP_HISTOGRAM_DEFAULT_BINWIDTH, convergenceCriteria);
            _aep.SetIterationSize(convergenceCriteria.MaxIterations);
            _cnep = new Dictionary<double, ThreadsafeInlineHistogram>();
            _ConvergenceCriteria = convergenceCriteria;

        }
        public ProjectPerformanceResults(ThresholdEnum thresholdType, double thresholdValue, ConvergenceCriteria convergenceCriteria, ThreadsafeInlineHistogram aepHistogram, Dictionary<double, ThreadsafeInlineHistogram> cnepHistogramDictionary)
        {
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _aep = aepHistogram;
            //_aep = new ThreadsafeInlineHistogram(AEP_HISTOGRAM_DEFAULT_BINWIDTH, c);
            _aep.SetIterationSize(convergenceCriteria.MaxIterations);
            _cnep = cnepHistogramDictionary;
            _ConvergenceCriteria = convergenceCriteria;

        }
        public ProjectPerformanceResults(ThresholdEnum thresholdType, double thresholdValue, UncertainPairedData systemResponseFunction, ConvergenceCriteria convergenceCriteria, ThreadsafeInlineHistogram aepHistogram, Dictionary<double, ThreadsafeInlineHistogram> cnepHistogramDictionary)
        {
            _systemResponseFunction = systemResponseFunction;
            _calculatePerformanceForLevee = true;
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _aep = aepHistogram;
            _aep.SetIterationSize(convergenceCriteria.MaxIterations);
            _cnep = cnepHistogramDictionary;
            _ConvergenceCriteria = convergenceCriteria;

        }


        #endregion
        #region Methods

        public void AddAEPEstimate(double aepEstimate, Int64 iteration)
        {
            _aep.AddObservationToHistogram(aepEstimate, iteration);
        }
        public void AddConditionalNonExceedenceProbabilityKey(double standardNonExceedanceProbability, ConvergenceCriteria c)
        {
            if (!_cnep.ContainsKey(standardNonExceedanceProbability))
            {
                var histogram = new ThreadsafeInlineHistogram(CNEP_HISTOGRAM_DEFAULT_BINWIDTH, c);
                histogram.SetIterationSize(c.MaxIterations);
                _cnep.Add(standardNonExceedanceProbability, histogram);
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
            double key = 0.98;
            return _cnep[key].IsConverged;
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
        public double ConditionalNonExceedanceProbability(double standardNonExceedanceProbability)
        {
            if (_calculatePerformanceForLevee)
            {
                return CalculateConditionalNonExceedanceProbabilityForLevee(standardNonExceedanceProbability);
            }
            else
            {
                _cnep[standardNonExceedanceProbability].ForceDeQueue();
                double conditionalNonExceedanceProbability = _cnep[standardNonExceedanceProbability].CDF(_thresholdValue);
                return conditionalNonExceedanceProbability;
            }

        }

        private double CalculateConditionalNonExceedanceProbabilityForLevee(double standardNonExceedanceProbability)
        {
            IPairedData medianLeveeCurve = _systemResponseFunction.SamplePairedData(0.5);
            _cnep[standardNonExceedanceProbability].ForceDeQueue();
            double stageStep = _cnep[standardNonExceedanceProbability].BinWidth;
            int stageStepQuantity = _cnep[standardNonExceedanceProbability].BinCounts.Length;
            double[] stages = _systemResponseFunction.Xvals;
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
                currentCumulativeExceedanceProbability = 1-_cnep[standardNonExceedanceProbability].CDF(currentStage);
                nextCumulativeExceedanceProbability = 1-_cnep[standardNonExceedanceProbability].CDF(nextStage);
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
        public void ParallelTestForConvergence(double upperQuantile, double lowerQuantile)
        {
            double[] keys = new double[_cnep.Keys.Count];
            int index = 0;
            foreach (var keyvaluepair in _cnep)
            {
                keys[index] = keyvaluepair.Key;
                index++;
            }
            Parallel.For(0, keys.Length, i =>
            {
                _cnep[keys[i]].TestForConvergence(upperQuantile,lowerQuantile);//this will force dequeue also.
            });
        }
        public bool Equals(ProjectPerformanceResults projectPerformanceResults)
        {
            if(!_aep.Equals(projectPerformanceResults.HistogramOfAEPs)) { return false; }
            foreach (double key in _cnep.Keys) 
            {
                if(!_cnep[key].Equals(projectPerformanceResults.CNEPHistogramOfStages[key])) { return false; }
            }
            return true;
        }
        public XElement WriteToXML()
        {
            XElement masterElement = new XElement("Project_Performance_Results");
            masterElement.SetAttributeValue("Calculates_Performance_For_Levee", _calculatePerformanceForLevee);
            masterElement.SetAttributeValue("Threshold_Type", Convert.ToString(_thresholdType));
            masterElement.SetAttributeValue("Threshold_Value", _thresholdValue);
            if (_calculatePerformanceForLevee)
            {
                XElement systemResponseCurveElement = _systemResponseFunction.WriteToXML();
                systemResponseCurveElement.Name = "System_Response_Curve";
                masterElement.Add(systemResponseCurveElement);
            }
            XElement aepElement = _aep.WriteToXML();
            aepElement.Name = "AEP_Histogram";
            List<int> listOfKeys = new List<int>();
            foreach (int key in _cnep.Keys)
            {
                XElement cnepElement = new XElement($"{key}");
                cnepElement = _cnep[key].WriteToXML();
                cnepElement.Name = $"prob={key}";
                masterElement.Add(cnepElement);
                listOfKeys.Add(key);
            }
            masterElement.SetAttributeValue("CNEP_Keys", listOfKeys.ToString());
            XElement convergenceCriteriaElement = _ConvergenceCriteria.WriteToXML();
            convergenceCriteriaElement.Name = "Convergence_Criteria";
            return masterElement;
        }

        public static ProjectPerformanceResults ReadFromXML(XElement xElement)
        {
            Dictionary<double, ThreadsafeInlineHistogram> cnepHistogramDictionary = new Dictionary<double, ThreadsafeInlineHistogram>();
            string stringListOfKeys = xElement.Attribute("CNEP_Keys").Value;
            foreach (char key in stringListOfKeys)
            {
                double keyDouble = Convert.ToDouble(key);
                ThreadsafeInlineHistogram threadsafeInlineHistogram = ThreadsafeInlineHistogram.ReadFromXML(xElement.Element($"prob={key}"));
                cnepHistogramDictionary.Add(keyDouble, threadsafeInlineHistogram);
            }
            ThreadsafeInlineHistogram aepHistogram = ThreadsafeInlineHistogram.ReadFromXML(xElement.Element("AEP_Histogram"));
            ConvergenceCriteria convergenceCriteria = ConvergenceCriteria.ReadFromXML(xElement.Element("Convergence_Criteria"));
            bool calculatePerformanceForLevee = Convert.ToBoolean(xElement.Attribute("Calculates_Performance_For_Levee").Value);
            UncertainPairedData systemResponseCurve = new UncertainPairedData();
            if (calculatePerformanceForLevee)
            {
                systemResponseCurve = UncertainPairedData.ReadFromXML(xElement.Element("System_Response_Curve"));
            }
            ThresholdEnum thresholdType = (ThresholdEnum)Enum.Parse(typeof(ThresholdEnum), xElement.Attribute("Threshold_Type").Value);
            double thresholdValue = Convert.ToDouble(xElement.Attribute("Threshold_Value").Value);

            if (calculatePerformanceForLevee)
            {
                return new ProjectPerformanceResults(thresholdType, thresholdValue, systemResponseCurve, convergenceCriteria, aepHistogram, cnepHistogramDictionary);
            }
            else
            {
                return new ProjectPerformanceResults(thresholdType, thresholdValue, convergenceCriteria, aepHistogram, cnepHistogramDictionary);
            }
        }
        #endregion
    }
}
