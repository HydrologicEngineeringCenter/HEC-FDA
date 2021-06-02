using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Conditions.Locations.Years
{
    public class IConditionLocationYearSummaryFactory
    {

        public IConditionLocationYearSummary FactoryNoLateralStructure(ILocation location, int yr, IFrequencyFunction frequencyFx, IEnumerable<ITransformFunction> transformFxs, IEnumerable<IMetric> metrics, string label = "")
        {
            return new ConditionLocationYearNoLateralStructure( location,  yr,  frequencyFx,  transformFxs,  metrics,  label = "");
        }

        public IConditionLocationYearSummary FactoryWithLateralStructure(ILocation location, int yr, IFrequencyFunction frequencyFx, IEnumerable<ITransformFunction> transformFxs, ILateralStructure lateralStructure, IEnumerable<IMetric> metrics, string label = "")
        {
            return new ConditionLocationYearWithLateralStructure(location, yr, frequencyFx, transformFxs, lateralStructure, metrics, label = "");
        }

        

    }
}
