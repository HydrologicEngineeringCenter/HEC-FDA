using HEC.FDA.Model.metrics;
using HEC.FDA.Model.metrics.Extensions;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Logging;
using Utility.Progress;

namespace HEC.FDA.Model.alternativeComparisonReport;

public enum AlternativeComparisonReportType
{
    BaseYearEADReduced,
    FutureYearEADReduced
}

public static class AlternativeComparisonReport
{
    public static AlternativeComparisonReportResults ComputeAlternativeComparisonReport(AlternativeResults withoutProjectAlternativeResults, IEnumerable<AlternativeResults> withProjectAlternativesResults, ProgressReporter pr = null)
    {
        List<StudyAreaConsequencesByQuantile> _EqadResults;
        List<StudyAreaConsequencesByQuantile> _BaseYearEADResults;
        List<StudyAreaConsequencesByQuantile> _FutureYearEADResults;

        pr = pr ?? ProgressReporter.None();
        pr.ReportMessage("Starting alternative comparison report results processing." + Environment.NewLine);
        pr.ReportProgress(1);

        OperationResult success = ValidateAlternativeResults(withoutProjectAlternativeResults, withProjectAlternativesResults);
        if (!success)
        {
            pr.ReportMessage(success.GetConcatenatedMessages());
            return null;
        }

        _EqadResults = ComputeDistributionOfEqadReduced(withoutProjectAlternativeResults, withProjectAlternativesResults, pr);
        pr.ReportProgress(33);

        _BaseYearEADResults = ComputeDistributionEADReduced(withoutProjectAlternativeResults, withProjectAlternativesResults, AlternativeComparisonReportType.BaseYearEADReduced, pr);
        pr.ReportProgress(67);

        _FutureYearEADResults = ComputeDistributionEADReduced(withoutProjectAlternativeResults, withProjectAlternativesResults, AlternativeComparisonReportType.FutureYearEADReduced, pr);
        pr.ReportMessage("Alternative comparison report results processing complete." + Environment.NewLine);
        pr.ReportProgress(100);

        return new AlternativeComparisonReportResults(withProjectAlternativesResults, withoutProjectAlternativeResults, _EqadResults, _BaseYearEADResults, _FutureYearEADResults);
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

    private static List<StudyAreaConsequencesByQuantile> ComputeDistributionOfEqadReduced(AlternativeResults withoutProjectAlternativeResults, IEnumerable<AlternativeResults> withProjectAlternativesResults, ProgressReporter pr)
    {
        //We calculate a list of many empirical distributions of consequences - one for each with-project alternative 
        List<StudyAreaConsequencesByQuantile> damagesReducedAllAlternatives = [];

        foreach (AlternativeResults withProjectAlternativeResults in withProjectAlternativesResults)
        {

            StudyAreaConsequencesByQuantile damageReducedOneAlternative = new(withProjectAlternativeResults.AlternativeID);

            List<AggregatedConsequencesByQuantile> withoutProjectConsequenceDistList = [.. withoutProjectAlternativeResults.EqadResults.ConsequenceResultList];

            foreach (AggregatedConsequencesByQuantile withProjectDamageResult in withProjectAlternativeResults.EqadResults.ConsequenceResultList)
            {
                //A with-project levee introduces Non_Fail consequences that the without-project condition has
                //no counterpart for. That is an ordinary difference between the two conditions, not a defect,
                //so use the lookup that returns null without logging a Fatal, and stand a zero damage
                //counterpart in its place. The counterpart carries the with-project row's identity; the
                //IsNull placeholder GetConsequenceResult hands back carries none and defaults to RiskType.Fail.
                AggregatedConsequencesByQuantile withoutProjectDamageResult = withoutProjectAlternativeResults.EqadResults.GetConsequenceResultOrNull(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID, withProjectDamageResult.ConsequenceType, withProjectDamageResult.RiskType); //GetEqadHistogram;
                withoutProjectConsequenceDistList.Remove(withoutProjectDamageResult);

                withoutProjectDamageResult ??= withProjectDamageResult.ZeroDamageCounterpart();

                AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, pr, true);
                damageReducedOneAlternative.AddExistingConsequenceResultObject(damageReducedResult);
            }
            if (withoutProjectConsequenceDistList.Count > 0)
            {
                foreach (AggregatedConsequencesByQuantile withoutProjectDamageResult in withoutProjectConsequenceDistList)
                {
                    AggregatedConsequencesByQuantile withProjectDamageResult = withProjectAlternativeResults.EqadResults.GetConsequenceResultOrNull(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID, withoutProjectDamageResult.ConsequenceType, withoutProjectDamageResult.RiskType);
                    withProjectDamageResult ??= withoutProjectDamageResult.ZeroDamageCounterpart();

                    AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, pr, false);
                    damageReducedOneAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                }
            }
            damagesReducedAllAlternatives.Add(damageReducedOneAlternative);
        }
        return damagesReducedAllAlternatives;
    }

    private static AggregatedConsequencesByQuantile IterateOnConsequenceDistributionResult(AggregatedConsequencesByQuantile withProjectDamageResult, AggregatedConsequencesByQuantile withoutProjectDamageResult, ProgressReporter pr, bool iterateOnWithProject = true)
    {
        List<Empirical> empiricalList =
        [
            withoutProjectDamageResult.ConsequenceDistribution,
            withProjectDamageResult.ConsequenceDistribution
        ];
        Empirical empirical = Empirical.StackEmpiricalDistributions(empiricalList, Empirical.Subtract);
        AggregatedConsequencesByQuantile singleEmpiricalDistributionOfConsequences = new();


        if (iterateOnWithProject)
        {
            singleEmpiricalDistributionOfConsequences = new AggregatedConsequencesByQuantile(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, empirical, withProjectDamageResult.RegionID, withProjectDamageResult.ConsequenceType, withProjectDamageResult.RiskType);

        }
        else
        {
            singleEmpiricalDistributionOfConsequences = new AggregatedConsequencesByQuantile(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, empirical, withoutProjectDamageResult.RegionID, withoutProjectDamageResult.ConsequenceType, withoutProjectDamageResult.RiskType);

        }
        return singleEmpiricalDistributionOfConsequences;
    }

    /// <summary>
    /// Computes the distribution of Expected Annual Damages (EAD) reduced for a given alternative
    /// by comparing with-project and without-project scenario results.
    /// </summary>
    /// <param name="alternativeID">The ID of the alternative being evaluated.</param>
    /// <param name="withoutProjectScenarioResults">Scenario results for the without-project condition.</param>
    /// <param name="withProjectScenarioResultsList">A collection of scenario results for each with-project condition.</param>
    /// <param name="pr">Progress reporter for logging and progress updates.</param>
    /// <returns>A list of StudyAreaConsequencesByQuantile representing the EAD reduced for each alternative.</returns>
    private static List<StudyAreaConsequencesByQuantile> ComputeDistributionEADReduced(
        AlternativeResults withoutProjectAlternativeResults, IEnumerable<AlternativeResults> withProjectAlternativesResults,
        AlternativeComparisonReportType type, ProgressReporter pr)
    {
        ScenarioResults withoutProjectScenarioResults;
        IEnumerable<ScenarioResults> withProjectScenarioResultsList;

        switch (type)
        {
            case AlternativeComparisonReportType.BaseYearEADReduced:
                List<ScenarioResults> withProj = withProjectAlternativesResults.Select(x => x.BaseYearScenarioResults).ToList();
                withoutProjectScenarioResults = withoutProjectAlternativeResults.BaseYearScenarioResults;
                withProjectScenarioResultsList = withProj;
                break;
            case AlternativeComparisonReportType.FutureYearEADReduced:
                List<ScenarioResults> withProjFut = withProjectAlternativesResults.Select(x => x.FutureYearScenarioResults).ToList();
                withoutProjectScenarioResults = withoutProjectAlternativeResults.FutureYearScenarioResults;
                withProjectScenarioResultsList = withProjFut;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        pr = pr ?? ProgressReporter.None();
        // List to hold the EAD reduced results for all alternatives
        List<StudyAreaConsequencesByQuantile> damageReducedAlternatives = [];

        // Loop through each with-project scenario result
        foreach (AlternativeResults withProjResults in withProjectAlternativesResults)
        {
            int alternativeID = withProjResults.AlternativeID;
            ScenarioResults withProjResultsList = type switch
            {
                AlternativeComparisonReportType.BaseYearEADReduced => withProjResults.BaseYearScenarioResults,
                AlternativeComparisonReportType.FutureYearEADReduced => withProjResults.FutureYearScenarioResults,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };

            // Create a new StudyAreaConsequencesByQuantile for the current alternative
            StudyAreaConsequencesByQuantile damageReducedAlternative = new(alternativeID);

            // Loop through each impact area scenario result in the with-project scenario
            foreach (ImpactAreaScenarioResults withProjectResults in withProjResultsList.ResultsList)
            {
                // Get the corresponding without-project results for the same impact area
                ImpactAreaScenarioResults withoutProjectResults = withoutProjectScenarioResults.GetResults(withProjectResults.ImpactAreaID);
                StudyAreaConsequencesBinned withprojectDamageResults = withProjectResults.ConsequenceResults;
                StudyAreaConsequencesBinned withoutProjectDamageResults = withoutProjectResults.ConsequenceResults;

                // Create a list of all consequence results for the without-project scenario
                List<AggregatedConsequencesBinned> withoutProjectDamageResultsList = [.. withoutProjectDamageResults.ConsequenceResultList];

                // Loop through each consequence result in the with-project scenario
                foreach (AggregatedConsequencesBinned withProjectDamageResult in withprojectDamageResults.ConsequenceResultList)
                {
                    // Find the matching consequence result in the without-project scenario
                    AggregatedConsequencesBinned withoutProjectDamageResult = withoutProjectDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID, withProjectDamageResult.ConsequenceType, withProjectDamageResult.RiskType);
                    // Remove the matched result from the list to track unmatched results
                    withoutProjectDamageResultsList.Remove(withoutProjectDamageResult);

                    if (withoutProjectDamageResult == null)
                    {
                        withoutProjectDamageResult = withProjectDamageResult.ZeroDamageCounterpart();
                    }

                    // Compute the reduced damage result by subtracting with- and without-project distributions
                    AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(
                        AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult),
                        AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult),
                        pr, true);
                    // Add the result to the current alternative's results
                    damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                }

                // Handle any remaining consequence results that exist only in the without-project scenario
                if (withoutProjectDamageResultsList.Count > 0)
                {
                    foreach (AggregatedConsequencesBinned withoutProjectDamageResult in withoutProjectDamageResultsList)
                    {
                        // Try to find a matching with-project result (may be null)
                        AggregatedConsequencesBinned withProjectDamageResult = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID, withoutProjectDamageResult.ConsequenceType, withoutProjectDamageResult.RiskType);
                        // Compute the reduced damage result (with-project may be null)

                        if (withProjectDamageResult == null)
                        {
                            withProjectDamageResult = withoutProjectDamageResult.ZeroDamageCounterpart();
                        }

                        AggregatedConsequencesByQuantile damageReducedResult = IterateOnConsequenceDistributionResult(
                            AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult),
                            AggregatedConsequencesBinned.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult),
                            pr, false);
                        // Add the result to the current alternative's results
                        damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                }
            }
            // Add the results for this alternative to the overall list
            damageReducedAlternatives.Add(damageReducedAlternative);
        }
        // Return the list of EAD reduced results for all alternatives
        return damageReducedAlternatives;
    }


}
