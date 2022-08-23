using System;
using System.Collections.Generic;
using alternatives;
using Statistics.Histograms;
using metrics;
using Statistics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using compute;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;

namespace alternativeComparisonReport
{
    public class AlternativeComparisonReport
{        
        public static AlternativeComparisonReportResults ComputeAlternativeComparisonReport(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            MessageEventArgs beginComputeMessageArgs = new MessageEventArgs(new Message("The alternative results are being processed for the alternative comparison report."));
            withoutProjectAlternativeResults.ReportMessage(withoutProjectAlternativeResults,beginComputeMessageArgs);
            List<ConsequenceDistributionResults> aaeqResults = ComputeDistributionOfAAEQDamageReduced(randomProvider, convergenceCriteria, withoutProjectAlternativeResults, withProjectAlternativesResults);
            MessageEventArgs aaeqResultsMessageArgs = new MessageEventArgs(new Message("The distributions of AAEQ Damage Reduced for the given with-project conditions have been computed."));
            withoutProjectAlternativeResults.ReportMessage(withoutProjectAlternativeResults, aaeqResultsMessageArgs);
            List<ConsequenceDistributionResults> baseYearEADResults = ComputeDistributionEADReducedBaseYear(randomProvider, convergenceCriteria, withoutProjectAlternativeResults, withProjectAlternativesResults);
            MessageEventArgs baseYearEADReducedMessageArgs = new MessageEventArgs(new Message("THe distributions of base year EAD reduced for the given with-project conditions have been computed."));
            withoutProjectAlternativeResults.ReportMessage(withoutProjectAlternativeResults, baseYearEADReducedMessageArgs);
            List<ConsequenceDistributionResults> futureYearEADResults = ComputeDistributionEADReducedFutureYear(randomProvider, convergenceCriteria, withoutProjectAlternativeResults, withProjectAlternativesResults);
            MessageEventArgs futureYearEADReducedMessageArgs = new MessageEventArgs(new Message("The distributions of future year EAD reduced for the given with-project conditions have been computed."));
            withoutProjectAlternativeResults.ReportMessage(withoutProjectAlternativeResults,futureYearEADReducedMessageArgs);
            return new AlternativeComparisonReportResults(withProjectAlternativesResults, withoutProjectAlternativeResults, aaeqResults, baseYearEADResults, futureYearEADResults);
        }
        private static List<ConsequenceDistributionResults> ComputeDistributionOfAAEQDamageReduced(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            List<ConsequenceDistributionResults> damagesReducedAllAlternatives = new List<ConsequenceDistributionResults>();
            foreach (AlternativeResults withProjectAlternativeResults in withProjectAlternativesResults)
            {
                ConsequenceDistributionResults damageReducedOneAlternative = new ConsequenceDistributionResults(withProjectAlternativeResults.AlternativeID);
                MessageEventArgs beginComputeMessageArgs = new MessageEventArgs(new Message($"Compute of the distribution of AAEQ damage reduced for alternative ID {withProjectAlternativeResults.AlternativeID} has been initiated."));
                damageReducedOneAlternative.ReportMessage(damageReducedOneAlternative, beginComputeMessageArgs);

                List<ConsequenceDistributionResult> withoutProjectConsequenceDistList = new List<ConsequenceDistributionResult>();
                foreach (ConsequenceDistributionResult consequenceDistributionResult in withoutProjectAlternativeResults.AAEQDamageResults.ConsequenceResultList)
                {
                    withoutProjectConsequenceDistList.Add(consequenceDistributionResult);
                }

                foreach (ConsequenceDistributionResult withProjectDamageResult in withProjectAlternativeResults.AAEQDamageResults.ConsequenceResultList)
                {
                        ConsequenceDistributionResult withoutProjectDamageResult = withoutProjectAlternativeResults.AAEQDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID); //GetAAEQDamageHistogram;
                        withoutProjectConsequenceDistList.Remove(withoutProjectDamageResult);


                    ConsequenceDistributionResult damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, randomProvider);
                    damageReducedOneAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                }
                if (withoutProjectConsequenceDistList.Count > 0)
                {
                    foreach (ConsequenceDistributionResult withoutProjectDamageResult in withoutProjectConsequenceDistList)
                    {
                        ConsequenceDistributionResult withProjectDamageResult = withProjectAlternativeResults.AAEQDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                        ConsequenceDistributionResult damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, randomProvider, false);
                        damageReducedOneAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                }
                damagesReducedAllAlternatives.Add(damageReducedOneAlternative);
            }
            return damagesReducedAllAlternatives;
        }
        
        private static ConsequenceDistributionResult IterateOnConsequenceDistributionResult(ConsequenceDistributionResult withProjectDamageResult, ConsequenceDistributionResult withoutProjectDamageResult, interfaces.IProvideRandomNumbers randomProvider, bool iterateOnWithProject = true)
        {
            if (iterateOnWithProject)
            {
                MessageEventArgs beginComputeMessageArgs = new MessageEventArgs(new Message($"Damage reduced distribution compute for damage category {withProjectDamageResult.DamageCategory}, asset category {withProjectDamageResult.AssetCategory}, and impact area ID {withProjectDamageResult.RegionID} has been initiated."));
                withProjectDamageResult.ReportMessage(withProjectDamageResult, beginComputeMessageArgs);
            }
            else
            {
                MessageEventArgs beginComputeMessageArgs = new MessageEventArgs(new Message($"Damage reduced distribution compute for damage category {withoutProjectDamageResult.DamageCategory}, asset category {withoutProjectDamageResult.AssetCategory}, and impact area ID {withoutProjectDamageResult.RegionID} has been initiated."));
                withoutProjectDamageResult.ReportMessage(withoutProjectDamageResult, beginComputeMessageArgs);
            }
            IHistogram withoutProjectHistogram = withoutProjectDamageResult.ConsequenceHistogram;
            IHistogram withProjectHistogram = withProjectDamageResult.ConsequenceHistogram;
            ConsequenceDistributionResult damageReducedResult = new ConsequenceDistributionResult();
            ConvergenceCriteria convergenceCriteria;
            bool bothHistogramsAreZeroValued = withoutProjectHistogram.HistogramIsZeroValued && withProjectHistogram.HistogramIsZeroValued;
            if (!bothHistogramsAreZeroValued)
            {
                if (iterateOnWithProject)
                {
                    convergenceCriteria = withProjectDamageResult.ConvergenceCriteria;
                }
                else
                {
                    convergenceCriteria = withoutProjectDamageResult.ConvergenceCriteria;
                }
                List<double> resultCollection = new List<double>();
                Int64 iterations = convergenceCriteria.MinIterations;
                bool converged = false;
                Int64 progressChunks = 1;
                Int64 _completedIterations = 0;
                Int64 _ExpectedIterations = convergenceCriteria.MaxIterations;
                if (_ExpectedIterations > 100)
                {
                    progressChunks = _ExpectedIterations / 100;
                }
                while (!converged)
                {
                    for (int i = 0; i < iterations; i++) 
                    {
                        double withProjectDamage = withProjectHistogram.InverseCDF(randomProvider.NextRandom());
                        double withoutProjectDamage = withoutProjectHistogram.InverseCDF(randomProvider.NextRandom());
                        double damagesReduced = withoutProjectDamage - withProjectDamage;
                        resultCollection.Add(damagesReduced);
                        _completedIterations++;
                        if (_completedIterations % progressChunks == 0)//need an atomic integer count here.
                        {
                            double percentcomplete = ((double)_completedIterations) / ((double)_ExpectedIterations) * 100;
                            damageReducedResult.ReportProgress(damageReducedResult, new ProgressReportEventArgs((int)percentcomplete));
                        }
                    }
                    Histogram histogram = new Histogram(resultCollection, convergenceCriteria);
                    converged = histogram.IsHistogramConverged(.95, .05);
                    if (!converged)
                    {
                        iterations = histogram.EstimateIterationsRemaining(.95, .05);
                    }
                    else
                    {
                        if (iterateOnWithProject)
                        {
                            damageReducedResult = new ConsequenceDistributionResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, histogram, withProjectDamageResult.RegionID);

                        }
                        else
                        {
                            damageReducedResult = new ConsequenceDistributionResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, histogram, withoutProjectDamageResult.RegionID);

                        }
                        iterations = 0;
                        break;
                    }
                }
            }
            MessageEventArgs endComputeMessageArgs = new MessageEventArgs(new Message($"Damage reduced distribution compute for damage category {damageReducedResult.DamageCategory}, asset category {damageReducedResult.AssetCategory}, and impact area ID {damageReducedResult.RegionID} has completed."));
            withProjectDamageResult.ReportMessage(withProjectDamageResult, endComputeMessageArgs);
            return damageReducedResult;
        }

        private static List<ConsequenceDistributionResults> ComputeDistributionEADReducedBaseYear(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            List<ConsequenceDistributionResults> damageReducedAlternatives = new List<ConsequenceDistributionResults>();
            foreach (AlternativeResults withProjectResults in withProjectAlternativesResults)
            {
                ConsequenceDistributionResults damageReducedAlternative = new ConsequenceDistributionResults(withProjectResults.AlternativeID);
                MessageEventArgs beginComputeMessageArgs = new MessageEventArgs(new Message($"Compute of the distribution of base year EAD reduced for alternative ID {damageReducedAlternative.AlternativeID} has been initiated."));
                damageReducedAlternative.ReportMessage(damageReducedAlternative, beginComputeMessageArgs);

                foreach (ImpactAreaScenarioResults withProjectIAS in withProjectResults.BaseYearScenarioResults.ResultsList)
                {
                    //TODO at this level, we have assumed that there are results for a given impact area in both with and without conditions 
                    ImpactAreaScenarioResults withoutProjectIAS = withoutProjectAlternativeResults.BaseYearScenarioResults.GetResults(withProjectIAS.ImpactAreaID);
                    ConsequenceDistributionResults withprojectDamageResults = withProjectIAS.ConsequenceResults;
                    ConsequenceDistributionResults withoutProjectDamageResults = withoutProjectIAS.ConsequenceResults;

                    List<ConsequenceDistributionResult> withoutProjectDamageResultsList = new List<ConsequenceDistributionResult>();

                    foreach (ConsequenceDistributionResult withoutProjectDistributionResult in withoutProjectDamageResults.ConsequenceResultList)
                    {
                        withoutProjectDamageResultsList.Add(withoutProjectDistributionResult);
                    }

                    foreach (ConsequenceDistributionResult withProjectDamageResult in withprojectDamageResults.ConsequenceResultList) 
                    {
                        ConsequenceDistributionResult withoutProjectDamageResult = withoutProjectDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID);
                        withoutProjectDamageResultsList.Remove(withoutProjectDamageResult);
                        ConsequenceDistributionResult damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, randomProvider);
                        damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                    if(withoutProjectDamageResultsList.Count > 0)
                    {
                        foreach(ConsequenceDistributionResult withoutProjectDamageResult in withoutProjectDamageResultsList)
                        {
                            ConsequenceDistributionResult withProjectDamageResult = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                            ConsequenceDistributionResult damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, randomProvider, false);
                            damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                        }
                    }
                }
                damageReducedAlternatives.Add(damageReducedAlternative);
            }
            return damageReducedAlternatives;

        }


        private static List<ConsequenceDistributionResults> ComputeDistributionEADReducedFutureYear(interfaces.IProvideRandomNumbers randomProvider, ConvergenceCriteria convergenceCriteria, AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            List<ConsequenceDistributionResults> damageReducedAlternatives = new List<ConsequenceDistributionResults>();
            foreach (AlternativeResults alternative in withProjectAlternativesResults)
            {
                ConsequenceDistributionResults damageReducedAlternative = new ConsequenceDistributionResults(alternative.AlternativeID);
                MessageEventArgs beginComputeMessageArgs = new MessageEventArgs(new Message($"Compute of the distribution of AAEQ damage reduced for alternative ID {damageReducedAlternative.AlternativeID} has been initiated."));
                damageReducedAlternative.ReportMessage(damageReducedAlternative, beginComputeMessageArgs);

                foreach (ImpactAreaScenarioResults withProjectResults in alternative.FutureYearScenarioResults.ResultsList)
                {
                    //TODO at this level, we have assumed that there are results for a given impact area in both with and without conditions 
                    ImpactAreaScenarioResults withoutProjectResults = withoutProjectAlternativeResults.FutureYearScenarioResults.GetResults(withProjectResults.ImpactAreaID);
                    ConsequenceDistributionResults withprojectDamageResults = withProjectResults.ConsequenceResults;
                    ConsequenceDistributionResults withoutProjectDamageResults = withoutProjectResults.ConsequenceResults;

                    List<ConsequenceDistributionResult> withoutProjectDamageResultsList = new List<ConsequenceDistributionResult>();

                    foreach (ConsequenceDistributionResult withoutProjectDistributionResult in withoutProjectDamageResults.ConsequenceResultList)
                    {
                        withoutProjectDamageResultsList.Add(withoutProjectDistributionResult);
                    }

                    foreach (ConsequenceDistributionResult withProjectDamageResult in withprojectDamageResults.ConsequenceResultList)
                    {
                        ConsequenceDistributionResult withoutProjectDamageResult = withoutProjectDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID);
                        withoutProjectDamageResultsList.Remove(withoutProjectDamageResult);
                        ConsequenceDistributionResult damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, randomProvider);
                        damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                    if (withoutProjectDamageResultsList.Count > 0)
                    {
                        foreach (ConsequenceDistributionResult withoutProjectDamageResult in withoutProjectDamageResultsList)
                        {
                            ConsequenceDistributionResult withProjectDamageResult = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                            ConsequenceDistributionResult damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, randomProvider, false);
                            damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                        }
                    }
                }
                damageReducedAlternatives.Add(damageReducedAlternative);
            }
            return damageReducedAlternatives;

        }

    }
}
