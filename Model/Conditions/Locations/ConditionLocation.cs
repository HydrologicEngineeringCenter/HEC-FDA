using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Functions;
using Utilities;

namespace Model.Conditions.Locations
{
    
    internal sealed class ConditionLocation: IConditionLocation
    {
        public IReadOnlyDictionary<int, IConditionLocationYearSummary> Years { get; }
        internal ConditionLocation(List<IConditionLocationYearSummary> years)
        {
            //TODO: Validation
            //different years, same location, discount rate 0 - 1
            Dictionary<int, IConditionLocationYearSummary> yrs = new Dictionary<int, IConditionLocationYearSummary>();
            foreach (var yr in years) yrs[yr.Year] = yr;
        }
        public IConditionLocationRealization ComputePreview()
        {
            Dictionary<int, IConditionLocationYearRealizationSummary> realizations = new Dictionary<int, IConditionLocationYearRealizationSummary>();
            foreach (var yr in Years) realizations[yr.Key] = yr.Value.ComputePreview();
            return new ConditionLocationRealization(realizations);
        }
    }
    
    
}
