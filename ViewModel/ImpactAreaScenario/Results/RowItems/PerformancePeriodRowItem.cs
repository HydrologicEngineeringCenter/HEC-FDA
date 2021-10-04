using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class PerformancePeriodRowItem
    {
        public int Years { get; set; }
        public int LongTermRisk { get; set; }

        public PerformancePeriodRowItem(int years, int longTermRisk)
        {
            Years = years;
            LongTermRisk = longTermRisk;
        }
    }
}
