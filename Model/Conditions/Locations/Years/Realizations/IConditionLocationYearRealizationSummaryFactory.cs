using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Conditions.Locations.Years.Realizations
{
    public static class IConditionLocationYearRealizationSummaryFactory
    {

        public static IConditionLocationYearRealizationSummary Factory(IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> fxs, ISampledParameter<IParameterOrdinate> failureElevation, IReadOnlyDictionary<IMetric, double> metrics, int id)
        {

            return new ConditionLocationYearRealizationWithLateralStructure(fxs, failureElevation, metrics, id);

        }

        public static IConditionLocationYearRealizationSummary Factory(IReadOnlyDictionary<IParameterEnum, ISampledParameter<IFdaFunction>> fxs, IReadOnlyDictionary<IMetric, double> metrics, int id)
        {

            return new ConditionLocationYearRealizationNoLateralStructure(fxs, metrics, id);

        }
    }
}
