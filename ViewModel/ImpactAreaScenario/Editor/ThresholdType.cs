using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario.Editor
{
    public class ThresholdType
    {

        public string DisplayName { get; set; }

        public IMetricEnum Metric {get;set;}

        public ThresholdType(IMetricEnum metric, string displayName)
        {
            Metric = metric;
            DisplayName = displayName;
        }


    }
}
