using HEC.FDA.Model.alternatives;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.Base.Implementations;
using System;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Compute
{
    public class ComputeAlternativeVM : ComputeWithProgressAndMessagesBase
    {


        public ComputeAlternativeVM(AlternativeElement altElem, Action<AlternativeResults> callback) : base()
        {
            ProgressLabel = StringConstants.ALTERNATIVE_PROGRESS_LABEL;

       
            Alternative alt = new Alternative();
            alt.ProgressReport += Alt_ProgressReport;
            MessageVM.InstanceHash.Add(alt.GetHashCode());

            //Task.Run(() =>
            //{
            //    AlternativeResults results = alt.AnnualizationCompute(randomProvider, discountRate, periodOfAnalysis, altElem.ID, firstResults, secondResults);
            //    callback?.Invoke(results);
            //});
            RunAnnualizationCompute(alt, altElem, callback);
        }

        public Task RunAnnualizationCompute(Alternative alt, AlternativeElement altElem, Action<AlternativeResults> callback)
        {
            IASElement[] iASElems = altElem.GetElementsFromID();
            IASElement firstElem = iASElems[0];
            IASElement secondElem = iASElems[1];

            ScenarioResults firstResults = firstElem.Results;
            ScenarioResults secondResults = secondElem.Results;
            int seed = 99;
            RandomProvider randomProvider = new RandomProvider(seed);
            StudyPropertiesElement studyProperties = StudyCache.GetStudyPropertiesElement();

            double discountRate = studyProperties.DiscountRate;
            int periodOfAnalysis = studyProperties.PeriodOfAnalysis;

            return Task.Run(() =>
            {
                AlternativeResults results = alt.AnnualizationCompute(randomProvider, discountRate, periodOfAnalysis, altElem.ID, firstResults, secondResults);
                callback?.Invoke(results);
            });
        }

        private void Alt_ProgressReport(object sender, MVVMFramework.Base.Events.ProgressReportEventArgs progress)
        {
            if (sender is Alternative)
            {
                Progress = progress.Progress;
            }
        }
    }
}
