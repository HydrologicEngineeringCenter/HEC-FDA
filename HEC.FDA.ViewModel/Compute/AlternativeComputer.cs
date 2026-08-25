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

        (int baseYear, int futureYear) = GetAnalysisYears(altElem, props);

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

    /// <summary>
    /// The base and future years the annualization compute will actually use. When the alternative has no
    /// future scenario the future year is derived from the study's period of analysis. Validation in the
    /// alternative editor and pre-compute checks call this so they cannot drift from what gets computed.
    /// </summary>
    public static (int BaseYear, int FutureYear) GetAnalysisYears(AlternativeElement altElem, StudyPropertiesElement props)
    {
        int baseYear = altElem.BaseScenario.Year;
        int futureYear = altElem.FutureScenario?.Year ?? (baseYear + props.PeriodOfAnalysis - 1);
        return (baseYear, futureYear);
    }
}
