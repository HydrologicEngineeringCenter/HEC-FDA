using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Alternatives.Results
{
    public class ImpactAreaRowItem
    {
        public string ImpactArea { get; set; }
        public double EAD { get; set; }

        public ImpactAreaRowItem(string impactArea, double ead)
        {
            EAD = ead;
            ImpactArea = impactArea;
        }
    }
}
