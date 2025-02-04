using HEC.FDA.Model.alternatives;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Compute
{
    /// <summary>
    /// This guy should be the compute window for computing a alternative. 
    /// </summary>
    public class ComputeAlternativeVM : BaseViewModel
    {
        public ComputeAlternativeVM() : base()
        {
        }

        public static Task RunAnnualizationCompute(AlternativeElement altElem, Action<AlternativeResults> callback)
        {
            IASElement firstElem = altElem.BaseScenario.GetElement();
            IASElement secondElem = altElem.FutureScenario.GetElement();

            ScenarioResults firstResults = firstElem.Results;
            ScenarioResults secondResults = secondElem.Results;
            StudyPropertiesElement studyProperties = StudyCache.GetStudyPropertiesElement();

            double discountRate = studyProperties.DiscountRate;
            int periodOfAnalysis = studyProperties.PeriodOfAnalysis;

            int baseYear = altElem.BaseScenario.Year;
            int futureYear = altElem.FutureScenario.Year;
            return Task.Run(() =>
            {
                AlternativeResults results = Alternative.AnnualizationCompute(discountRate, periodOfAnalysis, altElem.ID, 
                    firstResults, secondResults,baseYear, futureYear);
                callback?.Invoke(results);
            });
        }
    }
}
