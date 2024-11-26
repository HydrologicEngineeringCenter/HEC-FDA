using HEC.FDA.Model.metrics;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Model.Messaging;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.Model.alternativeComparisonReport
{
    public class AlternativeComparisonReport: ValidationErrorLogger, IProgressReport
    {
        #region Fields
        private List<StudyAreaConsequencesByQuantile> _AAEqResults;
        private List<StudyAreaConsequencesByQuantile> _BaseYearEADResults;
        private List<StudyAreaConsequencesByQuantile> _FutureYearEADResults;
        #endregion

        #region Properties
        public event ProgressReportedEventHandler ProgressReport;
        #endregion

        #region Constructor 
        public AlternativeComparisonReport()
        {
        }
        #endregion 

        public AlternativeComparisonReportResults ComputeAlternativeComparisonReport(AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            ReportMessage(this, new MessageEventArgs(new ErrorMessage("Starting alternative comparison report results processing." + Environment.NewLine, ErrorLevel.Info)));
            
            ReportProgress(this, new ProgressReportEventArgs(1));
            ComputeDistributionOfAAEQDamageReduced(withoutProjectAlternativeResults, withProjectAlternativesResults);
            ReportProgress(this, new ProgressReportEventArgs(33));

            ComputeDistributionEADReducedBaseYear(withoutProjectAlternativeResults, withProjectAlternativesResults);
            ReportProgress(this, new ProgressReportEventArgs(67));

            ComputeDistributionEADReducedFutureYear(withoutProjectAlternativeResults, withProjectAlternativesResults);

            ReportMessage(this, new MessageEventArgs(new ErrorMessage("Alternative comparison report results processing complete." + Environment.NewLine, ErrorLevel.Info)));
            ReportProgress(this, new ProgressReportEventArgs(100));

            return new AlternativeComparisonReportResults(withProjectAlternativesResults, withoutProjectAlternativeResults, _AAEqResults, _BaseYearEADResults , _FutureYearEADResults);
        }
        private  void ComputeDistributionOfAAEQDamageReduced(AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            //We calculate a list of many empirical distributions of consequences - one for each with-project alternative 
            List<StudyAreaConsequencesByQuantile> damagesReducedAllAlternatives = new();

            foreach (AlternativeResults withProjectAlternativeResults in withProjectAlternativesResults)
            {
                
                StudyAreaConsequencesByQuantile damageReducedOneAlternative = new(withProjectAlternativeResults.AlternativeID);

                List<AggregatedConsequencesByQuantile> withoutProjectConsequenceDistList = new();
                foreach (AggregatedConsequencesByQuantile consequenceDistributionResult in withoutProjectAlternativeResults.AAEQDamageResults.ConsequenceResultList)
                {
                    withoutProjectConsequenceDistList.Add(consequenceDistributionResult);
                }

                foreach (AggregatedConsequencesByQuantile withProjectDamageResult in withProjectAlternativeResults.AAEQDamageResults.ConsequenceResultList)
                {
                    AggregatedConsequencesByQuantile withoutProjectDamageResult = withoutProjectAlternativeResults.AAEQDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID); //GetAAEQDamageHistogram;
                    withoutProjectConsequenceDistList.Remove(withoutProjectDamageResult);


                    AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, true);
                    damageReducedOneAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                }
                if (withoutProjectConsequenceDistList.Count > 0)
                {
                    foreach (AggregatedConsequencesByQuantile withoutProjectDamageResult in withoutProjectConsequenceDistList)
                    {
                        AggregatedConsequencesByQuantile withProjectDamageResult = withProjectAlternativeResults.AAEQDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                        AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, false);
                        damageReducedOneAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                }
                damagesReducedAllAlternatives.Add(damageReducedOneAlternative);
            }
            _AAEqResults = damagesReducedAllAlternatives;
        }

        private static AggregatedConsequencesByQuantile IterateOnConsequenceDistributionResult(AggregatedConsequencesByQuantile withProjectDamageResult, AggregatedConsequencesByQuantile withoutProjectDamageResult, bool iterateOnWithProject = true)
        {
            List<Empirical> empiricalList = new()
            {
                withoutProjectDamageResult.ConsequenceDistribution,
                withProjectDamageResult.ConsequenceDistribution
            };
            Empirical empirical = Empirical.StackEmpiricalDistributions(empiricalList, Empirical.Subtract);
            AggregatedConsequencesByQuantile singleEmpiricalDistributionOfConsequences = new();


            if (iterateOnWithProject)
            {
                singleEmpiricalDistributionOfConsequences = new AggregatedConsequencesByQuantile(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, empirical, withProjectDamageResult.RegionID);

            }
            else
            {
                singleEmpiricalDistributionOfConsequences = new AggregatedConsequencesByQuantile(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, empirical, withoutProjectDamageResult.RegionID);

            }
            return singleEmpiricalDistributionOfConsequences;
        }

        private void ComputeDistributionEADReducedBaseYear(AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            List<StudyAreaConsequencesByQuantile> damageReducedAllAlternatives = new();
            foreach (AlternativeResults withProjectResults in withProjectAlternativesResults)
            {
                StudyAreaConsequencesByQuantile damageReducedAlternative = new(withProjectResults.AlternativeID);

                foreach (ImpactAreaScenarioResults withProjectIAS in withProjectResults.BaseYearScenarioResults.ResultsList.Cast<ImpactAreaScenarioResults>())
                {
                    //TODO at this level, we have assumed that there are results for a given impact area in both with and without conditions 
                    ImpactAreaScenarioResults withoutProjectIAS = withoutProjectAlternativeResults.BaseYearScenarioResults.GetResults(withProjectIAS.ImpactAreaID);
                    StudyAreaConsequencesBinned withprojectDamageResults = withProjectIAS.ConsequenceResults;
                    StudyAreaConsequencesBinned withoutProjectDamageResults = withoutProjectIAS.ConsequenceResults;

                    List<AggregatedConsequencesBinned> withoutProjectDamageResultsList = new();

                    foreach (AggregatedConsequencesBinned withoutProjectDistributionResult in withoutProjectDamageResults.ConsequenceResultList)
                    {
                        withoutProjectDamageResultsList.Add(withoutProjectDistributionResult);
                    }

                    foreach (AggregatedConsequencesBinned withProjectDamageResult in withprojectDamageResults.ConsequenceResultList)
                    {
                        AggregatedConsequencesBinned withoutProjectDamageResult = withoutProjectDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID);
                        withoutProjectDamageResultsList.Remove(withoutProjectDamageResult);

                        AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult), AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult), true);
                        damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                    if (withoutProjectDamageResultsList.Count > 0)
                    {
                        foreach (AggregatedConsequencesBinned withoutProjectDamageResult in withoutProjectDamageResultsList)
                        {
                            AggregatedConsequencesBinned withProjectDamageResult = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                            //I have to be able to handle null with-project results here 
                            AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult), AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult), false);
                            damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                        }
                    }
                }
                damageReducedAllAlternatives.Add(damageReducedAlternative);
            }
            _BaseYearEADResults = damageReducedAllAlternatives;
        }


        private void ComputeDistributionEADReducedFutureYear(AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            List<StudyAreaConsequencesByQuantile> damageReducedAlternatives = new();
            foreach (AlternativeResults alternative in withProjectAlternativesResults)
            {
                StudyAreaConsequencesByQuantile damageReducedAlternative = new(alternative.AlternativeID);
                MessageEventArgs beginComputeMessageArgs = new(new Message($"Compute of the distribution of EqAD reduced for alternative ID {damageReducedAlternative.AlternativeID} has been initiated." + Environment.NewLine));
                ReportMessage(this, beginComputeMessageArgs);

                foreach (ImpactAreaScenarioResults withProjectResults in alternative.FutureYearScenarioResults.ResultsList.Cast<ImpactAreaScenarioResults>())
                {
                    //TODO at this level, we have assumed that there are results for a given impact area in both with and without conditions 
                    ImpactAreaScenarioResults withoutProjectResults = withoutProjectAlternativeResults.FutureYearScenarioResults.GetResults(withProjectResults.ImpactAreaID);
                    StudyAreaConsequencesBinned withprojectDamageResults = withProjectResults.ConsequenceResults;
                    StudyAreaConsequencesBinned withoutProjectDamageResults = withoutProjectResults.ConsequenceResults;

                    List<AggregatedConsequencesBinned> withoutProjectDamageResultsList = new();

                    foreach (AggregatedConsequencesBinned withoutProjectDistributionResult in withoutProjectDamageResults.ConsequenceResultList)
                    {
                        withoutProjectDamageResultsList.Add(withoutProjectDistributionResult);
                    }

                    foreach (AggregatedConsequencesBinned withProjectDamageResult in withprojectDamageResults.ConsequenceResultList)
                    {
                        AggregatedConsequencesBinned withoutProjectDamageResult = withoutProjectDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID);
                        withoutProjectDamageResultsList.Remove(withoutProjectDamageResult);

                        AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult), AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult), true);
                        damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                    if (withoutProjectDamageResultsList.Count > 0)
                    {
                        foreach (AggregatedConsequencesBinned withoutProjectDamageResult in withoutProjectDamageResultsList)
                        {
                            AggregatedConsequencesBinned withProjectDamageResult = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                            AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult), AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult), false);
                            damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                        }
                    }
                }
                damageReducedAlternatives.Add(damageReducedAlternative);
                MessageEventArgs endComputeMessageArgs = new(new Message($"Compute of the distribution of EqAD reduced for alternative ID {damageReducedAlternative.AlternativeID} has completed." + Environment.NewLine));
                ReportMessage(this, endComputeMessageArgs);
            }
            _FutureYearEADResults = damageReducedAlternatives;

        }

        public void ReportProgress(object sender, ProgressReportEventArgs e)
        {
            ProgressReport?.Invoke(sender, e);
        }
    }
}
