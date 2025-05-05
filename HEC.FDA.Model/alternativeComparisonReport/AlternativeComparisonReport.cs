using HEC.FDA.Model.metrics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Logging;
using Utility.Progress;

namespace HEC.FDA.Model.alternativeComparisonReport;

public class AlternativeComparisonReport
{
    #region Fields
    private List<StudyAreaConsequencesByQuantile> _AAEqResults;
    private List<StudyAreaConsequencesByQuantile> _BaseYearEADResults;
    private List<StudyAreaConsequencesByQuantile> _FutureYearEADResults;
    #endregion

    public AlternativeComparisonReport()
    {
    }
    public AlternativeComparisonReportResults ComputeAlternativeComparisonReport(AlternativeResults withoutProjectAlternativeResults, IEnumerable<AlternativeResults> withProjectAlternativesResults, ProgressReporter pr = null)
    {
        pr = pr ?? ProgressReporter.None();
        pr.ReportMessage("Starting alternative comparison report results processing." + Environment.NewLine);
        pr.ReportProgress(1);

        OperationResult success = ValidateAlternativeResults(withoutProjectAlternativeResults,withProjectAlternativesResults);
        if(!success){
            pr.ReportMessage(success.GetConcatenatedMessages());
            return null;
        }

        ComputeDistributionOfAAEQDamageReduced(withoutProjectAlternativeResults, withProjectAlternativesResults, pr);
        pr.ReportProgress(33);

        ComputeDistributionEADReducedBaseYear(withoutProjectAlternativeResults, withProjectAlternativesResults, pr);
        pr.ReportProgress(67);

        ComputeDistributionEADReducedFutureYear(withoutProjectAlternativeResults, withProjectAlternativesResults, pr);
        pr.ReportMessage("Alternative comparison report results processing complete." + Environment.NewLine);
        pr.ReportProgress(100);

        return new AlternativeComparisonReportResults(withProjectAlternativesResults, withoutProjectAlternativeResults, _AAEqResults, _BaseYearEADResults, _FutureYearEADResults);
    }

    private static OperationResult ValidateAlternativeResults(AlternativeResults withoutProjectAlternativeResults, IEnumerable<AlternativeResults> withProjectAlternativesResults)
    {
        
        if (withoutProjectAlternativeResults == null)
        {
            return OperationResult.Fail("Without Project Alternative failed to compute");
        }
        foreach (AlternativeResults res in withProjectAlternativesResults)
        {
            if (res == null)
            {
                return OperationResult.Fail("One or more With Project Alternatives failed to compute");
            }
        }
        return OperationResult.Success();
    }

    private  void ComputeDistributionOfAAEQDamageReduced(AlternativeResults withoutProjectAlternativeResults, IEnumerable<AlternativeResults> withProjectAlternativesResults, ProgressReporter pr)
    {
        //We calculate a list of many empirical distributions of consequences - one for each with-project alternative 
        List<StudyAreaConsequencesByQuantile> damagesReducedAllAlternatives = [];

        foreach (AlternativeResults withProjectAlternativeResults in withProjectAlternativesResults)
        {
            
            StudyAreaConsequencesByQuantile damageReducedOneAlternative = new(withProjectAlternativeResults.AlternativeID);

            List<AggregatedConsequencesByQuantile> withoutProjectConsequenceDistList = [.. withoutProjectAlternativeResults.AAEQDamageResults.ConsequenceResultList];

            foreach (AggregatedConsequencesByQuantile withProjectDamageResult in withProjectAlternativeResults.AAEQDamageResults.ConsequenceResultList)
            {
                AggregatedConsequencesByQuantile withoutProjectDamageResult = withoutProjectAlternativeResults.AAEQDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID); //GetAAEQDamageHistogram;
                withoutProjectConsequenceDistList.Remove(withoutProjectDamageResult);


                AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, pr,true);
                damageReducedOneAlternative.AddExistingConsequenceResultObject(damageReducedResult);
            }
            if (withoutProjectConsequenceDistList.Count > 0)
            {
                foreach (AggregatedConsequencesByQuantile withoutProjectDamageResult in withoutProjectConsequenceDistList)
                {
                    AggregatedConsequencesByQuantile withProjectDamageResult = withProjectAlternativeResults.AAEQDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                    AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult,pr, false);
                    damageReducedOneAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                }
            }
            damagesReducedAllAlternatives.Add(damageReducedOneAlternative);
        }
        _AAEqResults = damagesReducedAllAlternatives;
    }

    private static AggregatedConsequencesByQuantile IterateOnConsequenceDistributionResult(AggregatedConsequencesByQuantile withProjectDamageResult, AggregatedConsequencesByQuantile withoutProjectDamageResult, ProgressReporter pr,bool iterateOnWithProject = true)
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

    private void ComputeDistributionEADReducedBaseYear(AlternativeResults withoutProjectAlternativeResults, IEnumerable<AlternativeResults> withProjectAlternativesResults, ProgressReporter pr)
    {
        List<StudyAreaConsequencesByQuantile> damageReducedAllAlternatives = [];
        foreach (AlternativeResults withProjectResults in withProjectAlternativesResults)
        {
            StudyAreaConsequencesByQuantile damageReducedAlternative = new(withProjectResults.AlternativeID);

            foreach (ImpactAreaScenarioResults withProjectIAS in withProjectResults.BaseYearScenarioResults.ResultsList.Cast<ImpactAreaScenarioResults>())
            {
                //TODO at this level, we have assumed that there are results for a given impact area in both with and without conditions 
                ImpactAreaScenarioResults withoutProjectIAS = withoutProjectAlternativeResults.BaseYearScenarioResults.GetResults(withProjectIAS.ImpactAreaID);
                StudyAreaConsequencesBinned withprojectDamageResults = withProjectIAS.ConsequenceResults;
                StudyAreaConsequencesBinned withoutProjectDamageResults = withoutProjectIAS.ConsequenceResults;

                List<AggregatedConsequencesBinned> withoutProjectDamageResultsList = [.. withoutProjectDamageResults.ConsequenceResultList];

                foreach (AggregatedConsequencesBinned withProjectDamageResult in withprojectDamageResults.ConsequenceResultList)
                {
                    AggregatedConsequencesBinned withoutProjectDamageResult = withoutProjectDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID);
                    withoutProjectDamageResultsList.Remove(withoutProjectDamageResult);

                    AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult), AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult), pr,true);
                    damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                }
                if (withoutProjectDamageResultsList.Count > 0)
                {
                    foreach (AggregatedConsequencesBinned withoutProjectDamageResult in withoutProjectDamageResultsList)
                    {
                        AggregatedConsequencesBinned withProjectDamageResult = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                        //I have to be able to handle null with-project results here 
                        AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult), AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult),pr, false);
                        damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                }
            }
            damageReducedAllAlternatives.Add(damageReducedAlternative);
        }
        _BaseYearEADResults = damageReducedAllAlternatives;
    }


    private void ComputeDistributionEADReducedFutureYear(AlternativeResults withoutProjectAlternativeResults, IEnumerable<AlternativeResults> withProjectAlternativesResults, ProgressReporter pr)
    {
        List<StudyAreaConsequencesByQuantile> damageReducedAlternatives = new();
        foreach (AlternativeResults alternative in withProjectAlternativesResults)
        {
            StudyAreaConsequencesByQuantile damageReducedAlternative = new(alternative.AlternativeID);
            pr.ReportMessage($"Compute of the distribution of EqAD reduced for alternative ID {damageReducedAlternative.AlternativeID} has been initiated." + Environment.NewLine);

            foreach (ImpactAreaScenarioResults withProjectResults in alternative.FutureYearScenarioResults.ResultsList.Cast<ImpactAreaScenarioResults>())
            {
                //TODO at this level, we have assumed that there are results for a given impact area in both with and without conditions 
                ImpactAreaScenarioResults withoutProjectResults = withoutProjectAlternativeResults.FutureYearScenarioResults.GetResults(withProjectResults.ImpactAreaID);
                StudyAreaConsequencesBinned withprojectDamageResults = withProjectResults.ConsequenceResults;
                StudyAreaConsequencesBinned withoutProjectDamageResults = withoutProjectResults.ConsequenceResults;

                List<AggregatedConsequencesBinned> withoutProjectDamageResultsList = [.. withoutProjectDamageResults.ConsequenceResultList];

                foreach (AggregatedConsequencesBinned withProjectDamageResult in withprojectDamageResults.ConsequenceResultList)
                {
                    AggregatedConsequencesBinned withoutProjectDamageResult = withoutProjectDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID);
                    withoutProjectDamageResultsList.Remove(withoutProjectDamageResult);

                    AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult), AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult), pr, true);
                    damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                }
                if (withoutProjectDamageResultsList.Count > 0)
                {
                    foreach (AggregatedConsequencesBinned withoutProjectDamageResult in withoutProjectDamageResultsList)
                    {
                        AggregatedConsequencesBinned withProjectDamageResult = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                        AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult), AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult), pr, false);
                        damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                }
            }
            damageReducedAlternatives.Add(damageReducedAlternative);
            pr.ReportMessage($"Compute of the distribution of EqAD reduced for alternative ID {damageReducedAlternative.AlternativeID} has completed." + Environment.NewLine);
        }
        _FutureYearEADResults = damageReducedAlternatives;

    }

}