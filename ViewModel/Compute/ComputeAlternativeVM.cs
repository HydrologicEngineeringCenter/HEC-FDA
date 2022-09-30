using alternatives;
using compute;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using metrics;
using System;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Compute
{
    public class ComputeAlternativeVM : ComputeBase
    {


        public ComputeAlternativeVM(IASElement[] iASElems, int id, AlternativeElement altElem, Action<AlternativeResults> callback) : base()
        {
            ProgressLabel = StringConstants.ALTERNATIVE_PROGRESS_LABEL;

            IASElement firstElem = iASElems[0];
            IASElement secondElem = iASElems[1];

            ScenarioResults firstResults = firstElem.Results;
            ScenarioResults secondResults = secondElem.Results;

            int seed = 99;
            RandomProvider randomProvider = new RandomProvider(seed);
            StudyPropertiesElement studyProperties = StudyCache.GetStudyPropertiesElement();

            double discountRate = studyProperties.DiscountRate;
            int periodOfAnalysis = studyProperties.PeriodOfAnalysis;

            //todo: register something with message hub?

            Task.Run(() =>
            {
                AlternativeResults results = new Alternative().AnnualizationCompute(randomProvider, discountRate, periodOfAnalysis, id, firstResults, secondResults);
                callback?.Invoke(results);
            });

        }

    }
}
