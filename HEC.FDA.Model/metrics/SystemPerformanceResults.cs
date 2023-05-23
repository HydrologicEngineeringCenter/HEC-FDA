using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Statistics;
using Statistics.Histograms;
using System.Xml.Linq;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.FDA.Model.paireddata;
using HEC.MVVMFramework.Model.Messaging;

namespace HEC.FDA.Model.metrics
{
    public class SystemPerformanceResults : ValidationErrorLogger
    {
        #region Fields
        private const string AEP_ASSURANCE_TYPE = "AEP";
        private const string STAGE_ASSURANCE_TYPE = "STAGE";
        private const string AEP_ASSURANCE_FOR_PLOTTING = "AEP_PLOT";
        private const double AEP_BIN_WIDTH = 0.0002;
        private const double STAGE_BIN_WIDTH = 0.001;
        private const double AEP_FOR_PLOTTING_BIN_WIDTH = 0.02;
        private bool _calculatePerformanceForLevee;
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
        #endregion
        #region Constructors 
        ///
        public SystemPerformanceResults()
        {
            _thresholdType = ThresholdEnum.DefaultExteriorStage;
            _thresholdValue = 0;
            _ConvergenceCriteria = new ConvergenceCriteria();
            _assuranceList = new List<AssuranceResultStorage>();
            AssuranceResultStorage dummyAEP = new AssuranceResultStorage(AEP_ASSURANCE_TYPE, 0);
            _assuranceList.Add(dummyAEP);
            AssuranceResultStorage dummyPlottingAEP = new AssuranceResultStorage(AEP_ASSURANCE_FOR_PLOTTING, 0);
            _assuranceList.Add(dummyPlottingAEP);
            double[] standardNonExceedanceProbabilities = new double[] { .9, .96, .98, .99, .996, .998 };
            foreach (double probability in standardNonExceedanceProbabilities)
            {
                AssuranceResultStorage dummyAssurance = new AssuranceResultStorage(STAGE_ASSURANCE_TYPE, probability);
                _assuranceList.Add(dummyAssurance);
            }
        }
        public SystemPerformanceResults(ThresholdEnum thresholdType, double thresholdValue, ConvergenceCriteria convergenceCriteria)
        {
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _ConvergenceCriteria = convergenceCriteria;
            _assuranceList = new List<AssuranceResultStorage>();
            AssuranceResultStorage aepAssurance = new AssuranceResultStorage(AEP_ASSURANCE_TYPE, AEP_BIN_WIDTH, convergenceCriteria);
            _assuranceList.Add(aepAssurance);
            AssuranceResultStorage aepAssuranceForPlotting = new AssuranceResultStorage(AEP_ASSURANCE_FOR_PLOTTING, AEP_FOR_PLOTTING_BIN_WIDTH, convergenceCriteria);
            _assuranceList.Add(aepAssuranceForPlotting);
        }
        public SystemPerformanceResults(ThresholdEnum thresholdType, double thresholdValue, UncertainPairedData systemResponseFunction, ConvergenceCriteria convergenceCriteria)
        {
            _systemResponseFunction = systemResponseFunction;
            //If the system response function is the default function
            if (_systemResponseFunction.Xvals.Length <= 2)
            {
                _calculatePerformanceForLevee = false;
            } else
            {
                _calculatePerformanceForLevee = true;
            }
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _assuranceList = new List<AssuranceResultStorage>();
            AssuranceResultStorage aepAssurance = new AssuranceResultStorage(AEP_ASSURANCE_TYPE, AEP_BIN_WIDTH, convergenceCriteria);
            _assuranceList.Add(aepAssurance);
            AssuranceResultStorage aepAssuranceForPlotting = new AssuranceResultStorage(AEP_ASSURANCE_FOR_PLOTTING, AEP_FOR_PLOTTING_BIN_WIDTH, convergenceCriteria);
            _assuranceList.Add(aepAssuranceForPlotting);
            _ConvergenceCriteria = convergenceCriteria;
        }

        //TODO: these two constructors don't seem that useful - delete?
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
            if (_systemResponseFunction.Xvals.Length <= 2)
            {
                _calculatePerformanceForLevee = false;
            }
            else
            {
                _calculatePerformanceForLevee = true;
            }
            _thresholdType = thresholdType;
            _thresholdValue = thresholdValue;
            _assuranceList = assurances;
            _ConvergenceCriteria = convergenceCriteria;
        }


        #endregion
        #region Methods
        /// <summary>
        /// The standard non-exceedance probabilities are one of the double[] { .9, .96, .98, .99, .996, .998 };
        /// For now, bin width for histograms of stages will be 0.001 - so 1/1000 of a foot
        /// </summary>
        /// <param name="standardNonExceedanceProbability"></param>
        public void AddStageAssuranceHistogram(double standardNonExceedanceProbability, double binWidth = STAGE_BIN_WIDTH)
        {
            AssuranceResultStorage assurance = new AssuranceResultStorage(STAGE_ASSURANCE_TYPE, binWidth, _ConvergenceCriteria, standardNonExceedanceProbability);
            if (!_assuranceList.Contains(assurance))
            {
                _assuranceList.Add(assurance);
            }
        }
        /// <summary>
        /// This method returns the thread safe inline histogram of AEPs
        /// This method is only used to get the histogram for plotting purposes. 
        /// </summary>
        /// <returns></returns>
        public Histogram GetAEPHistogram()
        {
            Histogram aepHistogram = GetAssurance(AEP_ASSURANCE_FOR_PLOTTING).AssuranceHistogram;
            return aepHistogram;
        }
        public Histogram GetAssuranceOfThresholdHistogram(double standardNonExceedanceProbability)
        {
            Histogram stageHistogram = GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AssuranceHistogram;
            return stageHistogram;
        }
        internal Histogram GetAEPHistogramForMetrics()
        {
            Histogram aepHistogram = GetAssurance(AEP_ASSURANCE_TYPE).AssuranceHistogram;
            return aepHistogram;
        }

        public void AddAEPForAssurance(double aep, int iteration)
        {
            GetAssurance(AEP_ASSURANCE_TYPE).AddObservation(aep, iteration);
            GetAssurance(AEP_ASSURANCE_FOR_PLOTTING).AddObservation(aep, iteration);
        }
        public void AddStageForAssurance(double standardNonExceedanceProbability, double stage, int iteration)
        {
            GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AddObservation(stage, iteration);
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
            double assuranceOfAEP = GetAssurance(AEP_ASSURANCE_TYPE).AssuranceHistogram.CDF(exceedanceProbability);
            return assuranceOfAEP;
        }
        public bool AssuranceIsConverged()
        {
            double standardNonExceedanceProbability = 0.98;
            Histogram assuranceHistogram = GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AssuranceHistogram;
            return assuranceHistogram.IsConverged;
        }
        public bool AssuranceTestForConvergence(double upperConfidenceLimitProb, double lowerConfidenceLimitProb)
        {
            double standardNonExceedanceProbability = 0.98;
            Histogram assuranceHistogram = GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AssuranceHistogram;
            bool assuranceIsConverged = assuranceHistogram.IsHistogramConverged(upperConfidenceLimitProb, lowerConfidenceLimitProb);
            return assuranceIsConverged;
        }
        public long AssuranceRemainingIterations(double upperConfidenceLimitProb, double lowerConfidenceLimitProb)
        {
            double standardNonExceedanceProbability = 0.98;
            Histogram assuranceHistogram = GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AssuranceHistogram;
            long iterationsRemaining = assuranceHistogram.EstimateIterationsRemaining(upperConfidenceLimitProb, lowerConfidenceLimitProb);
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
                Histogram assuranceHistogram = GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AssuranceHistogram;
                double assurance = assuranceHistogram.CDF(_thresholdValue);
                return assurance;
            }

        }

        private double CalculateAssuranceForLevee(double standardNonExceedanceProbability)
        {
            Histogram assuranceHistogram = GetAssurance(STAGE_ASSURANCE_TYPE, standardNonExceedanceProbability).AssuranceHistogram;
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
                currentCumulativeExceedanceProbability = 1 - assuranceHistogram.CDF(currentStage);
                nextCumulativeExceedanceProbability = 1 - assuranceHistogram.CDF(nextStage);
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
            double ltep = 1 - Math.Pow(1 - MeanAEP(), years);
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
            ErrorMessage errorMessage = new ErrorMessage(message, ErrorLevel.Fatal);
            ReportMessage(this, new MessageEventArgs(errorMessage));
            AssuranceResultStorage dummyAssurance = new AssuranceResultStorage(STAGE_ASSURANCE_TYPE, .98);
            return dummyAssurance;

        }
        public void PutDataIntoHistograms()
        {
            foreach (AssuranceResultStorage assuranceResultStorage in  _assuranceList)
            {
                assuranceResultStorage.PutDataIntoHistogram();
            }
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
