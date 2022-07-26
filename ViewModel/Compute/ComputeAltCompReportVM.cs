using compute;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.Utilities;
using metrics;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Compute
{
    public class ComputeAltCompReportVM:ComputeBase
    {
        private readonly List<AlternativeResults> AllResults = new List<AlternativeResults>();
        private void ComputeCompleted(AlternativeResults results)
        {
            lock (AllResults)
            {
                AllResults.Add(results);
            }
        }

        public ComputeAltCompReportVM(AlternativeElement withoutAlt, List<AlternativeElement> withProjAlts, Action<AlternativeComparisonReportResults> callback) : base()
        {
            ProgressLabel = StringConstants.ALTERNATIVE_PROGRESS_LABEL;

            List<AlternativeElement> allAlts = new List<AlternativeElement>(withProjAlts);
            allAlts.Add(withoutAlt);

            List<Task> tasks = new List<Task>();

            foreach (AlternativeElement elem in allAlts)
            {
               tasks.Add(Task.Run(() =>
               {
                   try
                   {
                       elem.ComputeAlternative(ComputeCompleted);
                   }
                   catch (Exception ex)
                   {
                       //todo:
                   }

               }
               ));


            }

            Task.Run(() =>
            {
                foreach (Task t in tasks)
                {
                    //todo: could cause infinite loop. maybe pass in milliseconds
                    t.Wait();
                }
                //here we have done all the alt computes. Find the without results from the list of results
                AlternativeResults withoutResult = null;
                foreach (AlternativeResults result in AllResults)
                {
                    if(result.AlternativeID == withoutAlt.ID)
                    {
                        withoutResult = result;
                        break;
                    }
                }
                AllResults.Remove(withoutResult);

                int seed = 99;
                RandomProvider randomProvider = new RandomProvider(seed);
                ConvergenceCriteria cc = new ConvergenceCriteria();

                AlternativeComparisonReportResults results = alternativeComparisonReport.AlternativeComparisonReport.ComputeAlternativeComparisonReport(randomProvider, cc, withoutResult, AllResults);
                callback?.Invoke(results);
            }
            );



            if (withoutProjResults == null)
            {
                //This should never happen.
                canComputeValidationResult.AddErrorMessage(withoutAlt.Name + " compute produced no results.");
            }
            foreach (AlternativeElement withProjElem in withProjAlts)
            {
                AlternativeResults withProjResults = withProjElem.ComputeAlternative();
                if (withProjResults == null)
                {
                    //This should never happen.
                    canComputeValidationResult.AddErrorMessage(withProjElem.Name + " compute produced no results.");
                }
                else
                {
                    withResults.Add(withProjResults);
                }
            }

            if (canComputeValidationResult.IsValid)
            {
                ComputeAltCompReport(withoutProjResults, withResults);
                if (_Results != null)
                {
                    AltCompReportResultsVM vm = new AltCompReportResultsVM(CreateResults());
                    string header = "Alternative Comparison Report Results: " + Name;
                    DynamicTabVM tab = new DynamicTabVM(header, vm, "AlternativeComparisonReportResults" + Name);
                    Navigate(tab, false, true);
                }
                else
                {
                    MessageBox.Show("There are no results to view.", "No Results", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("There are no results to view.", "No Results", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            ////todo:
            ////MessageHub.Register(firstResults);
            ////firstResults.ProgressReport += Sim_ProgressReport;
            ////sims.Add(sim);

            //Task.Run(() =>
            //{
            //    AlternativeResults results = Alternative.AnnualizationCompute(randomProvider, discountRate, periodOfAnalysis, id, firstResults, secondResults);
            //    callback?.Invoke(results);
            //});

        }


    }
}
