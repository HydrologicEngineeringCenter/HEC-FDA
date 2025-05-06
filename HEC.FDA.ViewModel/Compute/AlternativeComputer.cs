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

        // Group the base and future scenarios for readability.
        var baseScenario = altElem.BaseScenario;
        var futureScenario = altElem.FutureScenario;

        // Retrieve the results from each scenario's element.
        var firstResults = baseScenario.GetElement().Results;
        var secondResults = futureScenario.GetElement().Results;

        // Start the computation on a separate task.
        return Task.Run(() => Alternative.AnnualizationCompute(
            props.DiscountRate,
            props.PeriodOfAnalysis,
            altElem.ID,
            firstResults,
            secondResults,
            baseScenario.Year,
            futureScenario.Year,
            reporter
        ));
    }
}
