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
        private int _TotalProgressCount;
        private int _CurrentProgressCount;

        public ComputeAltCompReportVM(AlternativeElement withoutAlt, List<AlternativeElement> withProjAlts, Action<AlternativeComparisonReportResults> callback) : base()
        {
            _TotalProgressCount = withProjAlts.Count + 2;
            ProgressLabel = StringConstants.ALTERNATIVE_PROGRESS_LABEL;

            List<AlternativeElement> allAlts = new List<AlternativeElement>(withProjAlts);
            allAlts.Add(withoutAlt);

            List<Task> tasks = CreateAlternativeComputeTasks(allAlts);

            Task.Run(() =>
            {
                foreach (Task t in tasks)
                {
                    //todo: could cause infinite loop. maybe pass in milliseconds
                    t.Wait();
                }
                //here we have done all the alt computes. Find the without results from the list of results
                AlternativeResults withoutResult = FindWithoutProjectResult(withoutAlt.ID);
                AllResults.Remove(withoutResult);

                int seed = 99;
                RandomProvider randomProvider = new RandomProvider(seed);
                ConvergenceCriteria cc = new ConvergenceCriteria();

                AlternativeComparisonReportResults results = alternativeComparisonReport.AlternativeComparisonReport.ComputeAlternativeComparisonReport(randomProvider, cc, withoutResult, AllResults);
                Progress = 100;
                callback?.Invoke(results);
            }
            );

        }



        private void ComputeCompleted(AlternativeResults results)
        {
            lock (AllResults)
            {
                AllResults.Add(results);
                _CurrentProgressCount++;
                double progress = (_CurrentProgressCount / _TotalProgressCount) * 100;
                Progress = (int)progress;
            }
        }

        private List<Task> CreateAlternativeComputeTasks(List<AlternativeElement> allAlts)
        {
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
            return tasks;
        }

        private AlternativeResults FindWithoutProjectResult(int withoutAltID)
        {
            AlternativeResults withoutResult = null;
            foreach (AlternativeResults result in AllResults)
            {
                if (result.AlternativeID == withoutAltID)
                {
                    withoutResult = result;
                    break;
                }
            }
            return withoutResult;
        }

       

    }
}
