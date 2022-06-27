using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statistics;
using Statistics.Histograms;
using paireddata;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Enumerations;

namespace metrics
{
    public class SystemPerformanceResults : HEC.MVVMFramework.Base.Implementations.Validation, IReportMessage
    {
        #region Fields
        private const double AEP_HISTOGRAM_DEFAULT_BINWIDTH = .001;
        private const double CNEP_HISTOGRAM_DEFAULT_BINWIDTH = .5;
        private const string AEP_ASSURANCE_TYPE = "AEP";
        private const string STAGE_ASSURANCE_TYPE = "STAGE";
        private bool _calculatePerformanceForLevee;
        //TODO: handle performance by different threshold types 
        private ThresholdEnum _thresholdType;
        private double _thresholdValue;
        private List<AssuranceResultStorage> _assuranceList;
        private UncertainPairedData _systemResponseFunction;
        private ConvergenceCriteria _ConvergenceCriteria;

        #endregion
        #region Properties
        public List<AssuranceResultStorage> Assurances
        {
            get
            {
                return _assuranceList;
            }
        }
        public event MessageReportedEventHandler MessageReport;
        #endregion
        #region Constructors 
        public SystemPerformanceResults(ThresholdEnum thresholdType, double thresholdValue, ConvergenceCriteria convergenceCriteria)
        {
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _ConvergenceCriteria = convergenceCriteria;
            _assuranceList = new List<AssuranceResultStorage>();
            AssuranceResultStorage aepAssurance = new AssuranceResultStorage(AEP_ASSURANCE_TYPE, convergenceCriteria, AEP_HISTOGRAM_DEFAULT_BINWIDTH);
            _assuranceList.Add(aepAssurance);

        }
        public SystemPerformanceResults(ThresholdEnum thresholdType, double thresholdValue, UncertainPairedData systemResponseFunction, ConvergenceCriteria  convergenceCriteria)
        {
            _systemResponseFunction = systemResponseFunction;
            _calculatePerformanceForLevee = true;
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _assuranceList = new List<AssuranceResultStorage>();
            AssuranceResultStorage aepAssurance = new AssuranceResultStorage(AEP_ASSURANCE_TYPE, convergenceCriteria, AEP_HISTOGRAM_DEFAULT_BINWIDTH);
            _assuranceList.Add(aepAssurance);
            _ConvergenceCriteria = convergenceCriteria;

        }
        private SystemPerformanceResults(ThresholdEnum thresholdType, double thresholdValue, ConvergenceCriteria convergenceCriteria, List<AssuranceResultStorage> assurances)
        {
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _assuranceList = assurances;
            _ConvergenceCriteria = convergenceCriteria;

        }
        private SystemPerformanceResults(ThresholdEnum thresholdType, double thresholdValue, UncertainPairedData systemResponseFunction, ConvergenceCriteria convergenceCriteria, List<AssuranceResultStorage> assurances)
        {
            _systemResponseFunction = systemResponseFunction;
            _calculatePerformanceForLevee = true;
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _assuranceList = assurances;
            _ConvergenceCriteria = convergenceCriteria;

        }


        #endregion
        #region Methods
        public void AddAssuranceHistogram(double standardNonExceedanceProbability)
        {
            AssuranceResultStorage assurance = new AssuranceResultStorage(STAGE_ASSURANCE_TYPE, _ConvergenceCriteria, CNEP_HISTOGRAM_DEFAULT_BINWIDTH, standardNonExceedanceProbability);
            if (!_assuranceList.Contains(assurance))
            {
                _assuranceList.Add(assurance);
            }
        }
        /// <summary>
        /// This method returns the thread safe inline histogram of AEPs
        /// </summary>
        /// <returns></returns>
        public ThreadsafeInlineHistogram GetAEPHistogram()
        {
            ThreadsafeInlineHistogram aepHistogram = GetAssurance(AEP_ASSURANCE_TYPE).AssuranceHistogram;
            return aepHistogram;
        }
        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageReport?.Invoke(sender, e);
        }
        public void AddAEPForAssurance(double aep, int iteration)
        {
            GetAssurance(AEP_ASSURANCE_TYPE).AssuranceHistogram.AddObservationToHistogram(aep, iteration);
        }
        public void AddStageForAssurance(double standardNonExceedanceProbability, double stage, int iteration)
        {
            GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AssuranceHistogram.AddObservationToHistogram(stage, iteration);
        }

        public double MeanAEP()
        {
            return GetAssurance(AEP_ASSURANCE_TYPE).AssuranceHistogram.Mean;
        }

        public double MedianAEP()
        {
            return GetAssurance(AEP_ASSURANCE_TYPE).AssuranceHistogram.InverseCDF(0.5);
        }

        public double AssuranceOfAEP(double exceedanceProbability)
        {   //assurance of AEP is a non-exceedance probability so we use CDF as is 
            IHistogram aepHistogram = GetAEPHistogram();
            double assuranceOfAEP = aepHistogram.CDF(exceedanceProbability);
            //double assuranceOfAEP = GetAssurance(AEP_ASSURANCE_TYPE).AssuranceHistogram.CDF(exceedanceProbability);
            return assuranceOfAEP;
        }
        public bool AssuranceIsConverged()
        {
            double standardNonExceedanceProbability = 0.98;
            ThreadsafeInlineHistogram assuranceHistogram = GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AssuranceHistogram;
            return assuranceHistogram.IsConverged;
        }
        public bool AssuranceTestForConvergence(double upperConfidenceLimitProb, double lowerConfidenceLimitProb)
        {
            double standardNonExceedanceProbability = 0.98;
            ThreadsafeInlineHistogram assuranceHistogram = GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AssuranceHistogram;
            bool assuranceIsConverged = assuranceHistogram.IsHistogramConverged(upperConfidenceLimitProb, lowerConfidenceLimitProb);
            return assuranceIsConverged;
        }
        public int AssuranceRemainingIterations(double upperConfidenceLimitProb, double lowerConfidenceLimitProb)
        {
            double standardNonExceedanceProbability = 0.98;
            ThreadsafeInlineHistogram assuranceHistogram = GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AssuranceHistogram;
            int iterationsRemaining = assuranceHistogram.EstimateIterationsRemaining(upperConfidenceLimitProb, lowerConfidenceLimitProb);
            return iterationsRemaining;
        }
        public double AssuranceOfEvent(double standardNonExceedanceProbability)
        {
            if (_calculatePerformanceForLevee)
            {
                return CalculateAssuranceForLevee(standardNonExceedanceProbability);
            }
            else
            {
                GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AssuranceHistogram.ForceDeQueue();
                ThreadsafeInlineHistogram assuranceHistogram = GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AssuranceHistogram;
                double assurance = assuranceHistogram.CDF(_thresholdValue);
                return assurance;
            }

        }

        private double CalculateAssuranceForLevee(double standardNonExceedanceProbability)
        {
            ThreadsafeInlineHistogram assuranceHistogram = GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AssuranceHistogram;
            IPairedData medianLeveeCurve = _systemResponseFunction.SamplePairedData(0.5);
            assuranceHistogram.ForceDeQueue();
            double stageStep = assuranceHistogram.BinWidth;
            int stageStepQuantity = assuranceHistogram.BinCounts.Length;
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
                currentCumulativeExceedanceProbability = 1- assuranceHistogram.CDF(currentStage);
                nextCumulativeExceedanceProbability = 1- assuranceHistogram.CDF(nextStage);
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

        public double LongTermExceedanceProbability(int years)
        {
            double ltep = 1 - Math.Pow((1 - MeanAEP()), years);
            return ltep;
        }
        /// <summary>
        /// The parallel test for convergence will test for convergence in the histograms of stages and in the histogram of aeps
        /// </summary>
        /// <param name="upperQuantile"></param>
        /// <param name="lowerQuantile"></param>
        public void ParallelResultsAreConverged(double upperQuantile, double lowerQuantile)
        {
            double[] assuranceQuantity = new double[_assuranceList.Count];
            Parallel.For(0, assuranceQuantity.Length, i =>
            {
                _assuranceList.ElementAt(i).AssuranceHistogram.IsHistogramConverged(upperQuantile, lowerQuantile);
            });
        }
        public bool Equals(SystemPerformanceResults projectPerformanceResults)
        {
            foreach (AssuranceResultStorage assuranceResultStorage in _assuranceList)
            {
                bool areEqual = assuranceResultStorage.Equals(projectPerformanceResults.GetAssurance(assuranceResultStorage.AssuranceType, assuranceResultStorage.StandardNonExceedanceProbability));
                if (!areEqual)
                {
                    return false;
                }
            }
            return true;
        }
        internal AssuranceResultStorage GetAssurance(string type, double standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee = 0)
        {
            foreach (AssuranceResultStorage assurance in _assuranceList)
            {
                if (assurance.AssuranceType == type)
                {
                    if (assurance.StandardNonExceedanceProbability == standardNonExceedanceProbabilityForAssuranceOfTargetOrLevee)
                    {
                        return assurance;
                    }
                }
            }
            string message = $"The requested type and standardNonExceedanceProbability were not found. a dummy assurance object is being returned";
            HEC.MVVMFramework.Model.Messaging.ErrorMessage errorMessage = new HEC.MVVMFramework.Model.Messaging.ErrorMessage(message, HEC.MVVMFramework.Base.Enumerations.ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            AssuranceResultStorage dummyAssurance = new AssuranceResultStorage();
            return dummyAssurance;

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
            foreach (AssuranceResultStorage assuranceResultStorage in _assuranceList)
            {
                XElement assuranceElement = assuranceResultStorage.WriteToXML();
                assuranceElement.Name = "AssuranceResultStorage";
                masterElement.Add(assuranceElement);
            }
            XElement convergenceCriteriaElement = _ConvergenceCriteria.WriteToXML();
            convergenceCriteriaElement.Name = "Convergence_Criteria";
            masterElement.Add(convergenceCriteriaElement);
            return masterElement;
        }

        public static SystemPerformanceResults ReadFromXML(XElement xElement)
        {
            List<AssuranceResultStorage> histogramList = new List<AssuranceResultStorage>();
            foreach (XElement element in xElement.Elements())
            {
                if (element.Name == "AssuranceResultStorage")
                {
                    AssuranceResultStorage assuranceResultStorage = AssuranceResultStorage.ReadFromXML(element);
                    histogramList.Add(assuranceResultStorage);
                }
            }
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
                return new SystemPerformanceResults(thresholdType, thresholdValue, systemResponseCurve, convergenceCriteria, histogramList);
            }
            else
            {
                return new SystemPerformanceResults(thresholdType, thresholdValue, convergenceCriteria, histogramList);
            }
        }
        #endregion
    }
}
