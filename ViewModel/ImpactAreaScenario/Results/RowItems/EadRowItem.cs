using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario.Results.RowItems
{
    public class EadRowItem
    {

        public string ExpectedAnnualDamageMeasure { get; set; }
        public double DollarsInThousands { get; set; }

        public EadRowItem(string ead, double value)
        {
            ExpectedAnnualDamageMeasure = ead;
            DollarsInThousands = value;
        }

    }
}
