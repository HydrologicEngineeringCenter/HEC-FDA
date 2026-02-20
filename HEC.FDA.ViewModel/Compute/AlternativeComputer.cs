using HEC.FDA.Model.alternatives;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.Study;
using System.Threading.Tasks;
using Utility.Progress;

namespace HEC.FDA.ViewModel.Compute;
public static class AlternativeComputer
{
    public static Task<AlternativeResults> RunAnnualizationCompute(AlternativeElement altElem, StudyPropertiesElement props, ProgressReporter reporter = null)
    {
        reporter ??= ProgressReporter.None();

        var baseScenario = altElem.BaseScenario;
        var futureScenario = altElem.FutureScenario;

        var firstResults = baseScenario.GetElement().Results;
        var secondResults = futureScenario?.GetElement().Results;

        int baseYear = baseScenario.Year;
        int futureYear = futureScenario?.Year ?? (baseYear + props.PeriodOfAnalysis - 1);

        return Task.Run(() => Alternative.AnnualizationCompute(
            props.DiscountRate,
            props.PeriodOfAnalysis,
            altElem.ID,
            firstResults,
            secondResults,
            baseYear,
            futureYear,
            reporter
        ));
    }
}
