using System.Collections.Generic;
using Statistics.Histograms;
using Statistics;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.interfaces;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Model.Messaging;
using HEC.MVVMFramework.Base.Enumerations;
using Statistics.Distributions;
using System.Threading.Tasks;
using System.Linq;

namespace HEC.FDA.Model.alternativeComparisonReport
{
    public class AlternativeComparisonReport: ValidationErrorLogger, IProgressReport
    {
        #region Fields
        private List<ManyEmpiricalDistributionsOfConsequences> _AAEqResults;
        private List<ManyEmpiricalDistributionsOfConsequences> _BaseYearEADResults;
        private List<ManyEmpiricalDistributionsOfConsequences> _FutureYearEADResults;
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
            ReportMessage(this, new MessageEventArgs(new ErrorMessage("Starting alternative comparison report compute", ErrorLevel.Info)));
            ReportProgress(this, new ProgressReportEventArgs(10));

            Parallel.Invoke(
                () => ComputeDistributionOfAAEQDamageReduced(withoutProjectAlternativeResults, withProjectAlternativesResults),
                () => ComputeDistributionEADReducedBaseYear(withoutProjectAlternativeResults, withProjectAlternativesResults),
                () => ComputeDistributionEADReducedFutureYear(withoutProjectAlternativeResults, withProjectAlternativesResults)
                );

            //TODO: Fix the hacked in progress reporting 
            ReportProgress(this, new ProgressReportEventArgs(100));
            return new AlternativeComparisonReportResults(withProjectAlternativesResults, withoutProjectAlternativeResults, _AAEqResults, _BaseYearEADResults , _FutureYearEADResults);
        }
        private  void ComputeDistributionOfAAEQDamageReduced(AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            //We calculate a list of many empirical distributions of consequences - one for each with-project alternative 
            List<ManyEmpiricalDistributionsOfConsequences> damagesReducedAllAlternatives = new();

            foreach (AlternativeResults withProjectAlternativeResults in withProjectAlternativesResults)
            {
                
                ManyEmpiricalDistributionsOfConsequences damageReducedOneAlternative = new(withProjectAlternativeResults.AlternativeID);
                MessageEventArgs beginComputeMessageArgs = new(new Message($"Compute of the distribution of AAEQ damage reduced for alternative ID {withProjectAlternativeResults.AlternativeID} has been initiated."));
                ReportMessage(this, beginComputeMessageArgs);

                List<SingleEmpiricalDistributionOfConsequences> withoutProjectConsequenceDistList = new();
                foreach (SingleEmpiricalDistributionOfConsequences consequenceDistributionResult in withoutProjectAlternativeResults.AAEQDamageResults.ConsequenceResultList)
                {
                    withoutProjectConsequenceDistList.Add(consequenceDistributionResult);
                }

                foreach (SingleEmpiricalDistributionOfConsequences withProjectDamageResult in withProjectAlternativeResults.AAEQDamageResults.ConsequenceResultList)
                {
                    SingleEmpiricalDistributionOfConsequences withoutProjectDamageResult = withoutProjectAlternativeResults.AAEQDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID); //GetAAEQDamageHistogram;
                    withoutProjectConsequenceDistList.Remove(withoutProjectDamageResult);


                    SingleEmpiricalDistributionOfConsequences damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, true);
                    damageReducedOneAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                }
                if (withoutProjectConsequenceDistList.Count > 0)
                {
                    foreach (SingleEmpiricalDistributionOfConsequences withoutProjectDamageResult in withoutProjectConsequenceDistList)
                    {
                        SingleEmpiricalDistributionOfConsequences withProjectDamageResult = withProjectAlternativeResults.AAEQDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                        SingleEmpiricalDistributionOfConsequences damageReducedResult = IterateOnConsequenceDistributionResult(withProjectDamageResult, withoutProjectDamageResult, false);
                        damageReducedOneAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                }
                damagesReducedAllAlternatives.Add(damageReducedOneAlternative);
            }
            _AAEqResults = damagesReducedAllAlternatives;
        }

        private SingleEmpiricalDistributionOfConsequences IterateOnConsequenceDistributionResult(SingleEmpiricalDistributionOfConsequences withProjectDamageResult, SingleEmpiricalDistributionOfConsequences withoutProjectDamageResult, bool iterateOnWithProject = true)
        {
            List<Empirical> empiricalList = new();
            if (iterateOnWithProject)
            {
                MessageEventArgs beginComputeMessageArgs = new(new Message($"Damage reduced distribution compute for damage category {withProjectDamageResult.DamageCategory}, asset category {withProjectDamageResult.AssetCategory}, and impact area ID {withProjectDamageResult.RegionID} has been initiated."));
                ReportMessage(this, beginComputeMessageArgs);
            }
            else
            {
                MessageEventArgs beginComputeMessageArgs = new(new Message($"Damage reduced distribution compute for damage category {withoutProjectDamageResult.DamageCategory}, asset category {withoutProjectDamageResult.AssetCategory}, and impact area ID {withoutProjectDamageResult.RegionID} has been initiated."));
                ReportMessage(this, beginComputeMessageArgs);
            }

            empiricalList.Add(withoutProjectDamageResult.ConsequenceDistribution);
            empiricalList.Add(withProjectDamageResult.ConsequenceDistribution);
            Empirical empirical = Empirical.StackEmpiricalDistributions(empiricalList, Empirical.Subtract);
            SingleEmpiricalDistributionOfConsequences singleEmpiricalDistributionOfConsequences = new();


            if (iterateOnWithProject)
            {
                singleEmpiricalDistributionOfConsequences = new SingleEmpiricalDistributionOfConsequences(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, empirical, withProjectDamageResult.RegionID);

            }
            else
            {
                singleEmpiricalDistributionOfConsequences = new SingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, empirical, withoutProjectDamageResult.RegionID);

            }

            MessageEventArgs endComputeMessageArgs = new(new Message($"Damage reduced distribution compute for damage category {singleEmpiricalDistributionOfConsequences.DamageCategory}, asset category {singleEmpiricalDistributionOfConsequences.AssetCategory}, and impact area ID {singleEmpiricalDistributionOfConsequences.RegionID} has completed."));
            ReportMessage(this, endComputeMessageArgs);
            return singleEmpiricalDistributionOfConsequences;
        }

        private void ComputeDistributionEADReducedBaseYear(AlternativeResults withoutProjectAlternativeResults, List<AlternativeResults> withProjectAlternativesResults)
        {
            List<ManyEmpiricalDistributionsOfConsequences> damageReducedAllAlternatives = new();
            foreach (AlternativeResults withProjectResults in withProjectAlternativesResults)
            {
                ManyEmpiricalDistributionsOfConsequences damageReducedAlternative = new(withProjectResults.AlternativeID);
                MessageEventArgs beginComputeMessageArgs = new(new Message($"Compute of the distribution of base year EAD reduced for alternative ID {damageReducedAlternative.AlternativeID} has been initiated."));
                ReportMessage(this, beginComputeMessageArgs);

                foreach (ImpactAreaScenarioResults withProjectIAS in withProjectResults.BaseYearScenarioResults.ResultsList.Cast<ImpactAreaScenarioResults>())
                {
                    //TODO at this level, we have assumed that there are results for a given impact area in both with and without conditions 
                    ImpactAreaScenarioResults withoutProjectIAS = withoutProjectAlternativeResults.BaseYearScenarioResults.GetResults(withProjectIAS.ImpactAreaID);
                    ConsequenceDistributionResults withprojectDamageResults = withProjectIAS.ConsequenceResults;
                    ConsequenceDistributionResults withoutProjectDamageResults = withoutProjectIAS.ConsequenceResults;

                    List<ConsequenceDistributionResult> withoutProjectDamageResultsList = new();

                    foreach (ConsequenceDistributionResult withoutProjectDistributionResult in withoutProjectDamageResults.ConsequenceResultList)
                    {
                        withoutProjectDamageResultsList.Add(withoutProjectDistributionResult);
                    }

                    foreach (ConsequenceDistributionResult withProjectDamageResult in withprojectDamageResults.ConsequenceResultList)
                    {
                        ConsequenceDistributionResult withoutProjectDamageResult = withoutProjectDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID);
                        withoutProjectDamageResultsList.Remove(withoutProjectDamageResult);

                        SingleEmpiricalDistributionOfConsequences damageReducedResult = IterateOnConsequenceDistributionResult(ConsequenceDistributionResult.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult), ConsequenceDistributionResult.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult), true);
                        damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                    if (withoutProjectDamageResultsList.Count > 0)
                    {
                        foreach (ConsequenceDistributionResult withoutProjectDamageResult in withoutProjectDamageResultsList)
                        {
                            ConsequenceDistributionResult withProjectDamageResult = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                            //I have to be able to handle null with-project results here 
                            SingleEmpiricalDistributionOfConsequences damageReducedResult = IterateOnConsequenceDistributionResult(ConsequenceDistributionResult.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult), ConsequenceDistributionResult.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult), false);
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
            List<ManyEmpiricalDistributionsOfConsequences> damageReducedAlternatives = new();
            foreach (AlternativeResults alternative in withProjectAlternativesResults)
            {
                ManyEmpiricalDistributionsOfConsequences damageReducedAlternative = new(alternative.AlternativeID);
                MessageEventArgs beginComputeMessageArgs = new(new Message($"Compute of the distribution of AAEQ damage reduced for alternative ID {damageReducedAlternative.AlternativeID} has been initiated."));
                ReportMessage(this, beginComputeMessageArgs);

                foreach (ImpactAreaScenarioResults withProjectResults in alternative.FutureYearScenarioResults.ResultsList.Cast<ImpactAreaScenarioResults>())
                {
                    //TODO at this level, we have assumed that there are results for a given impact area in both with and without conditions 
                    ImpactAreaScenarioResults withoutProjectResults = withoutProjectAlternativeResults.FutureYearScenarioResults.GetResults(withProjectResults.ImpactAreaID);
                    ConsequenceDistributionResults withprojectDamageResults = withProjectResults.ConsequenceResults;
                    ConsequenceDistributionResults withoutProjectDamageResults = withoutProjectResults.ConsequenceResults;

                    List<ConsequenceDistributionResult> withoutProjectDamageResultsList = new();

                    foreach (ConsequenceDistributionResult withoutProjectDistributionResult in withoutProjectDamageResults.ConsequenceResultList)
                    {
                        withoutProjectDamageResultsList.Add(withoutProjectDistributionResult);
                    }

                    foreach (ConsequenceDistributionResult withProjectDamageResult in withprojectDamageResults.ConsequenceResultList)
                    {
                        ConsequenceDistributionResult withoutProjectDamageResult = withoutProjectDamageResults.GetConsequenceResult(withProjectDamageResult.DamageCategory, withProjectDamageResult.AssetCategory, withProjectDamageResult.RegionID);
                        withoutProjectDamageResultsList.Remove(withoutProjectDamageResult);

                        SingleEmpiricalDistributionOfConsequences damageReducedResult = IterateOnConsequenceDistributionResult(ConsequenceDistributionResult.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult), ConsequenceDistributionResult.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult), true);
                        damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                    }
                    if (withoutProjectDamageResultsList.Count > 0)
                    {
                        foreach (ConsequenceDistributionResult withoutProjectDamageResult in withoutProjectDamageResultsList)
                        {
                            ConsequenceDistributionResult withProjectDamageResult = withprojectDamageResults.GetConsequenceResult(withoutProjectDamageResult.DamageCategory, withoutProjectDamageResult.AssetCategory, withoutProjectDamageResult.RegionID);
                            SingleEmpiricalDistributionOfConsequences damageReducedResult = IterateOnConsequenceDistributionResult(ConsequenceDistributionResult.ConvertToSingleEmpiricalDistributionOfConsequences(withProjectDamageResult), ConsequenceDistributionResult.ConvertToSingleEmpiricalDistributionOfConsequences(withoutProjectDamageResult), false);
                            damageReducedAlternative.AddExistingConsequenceResultObject(damageReducedResult);
                        }
                    }
                }
                damageReducedAlternatives.Add(damageReducedAlternative);
            }
            _FutureYearEADResults = damageReducedAlternatives;

        }

        public void ReportProgress(object sender, ProgressReportEventArgs e)
        {
            ProgressReport?.Invoke(sender, e);
        }
    }
}
