using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.AlternativeComparisonReport.Results
{
    public class AAEQSummaryRowItem
    {

        public double WithoutProjAlternative { get; set; }
        public double WithoutProjAAEQ { get; set; }
        public double WithProjAlternative { get; set; }
        public double WithProjAAEQ { get; set; }
        public double AAEQDamageReduced { get; set; }
        public double Point75 { get; set; }
        public double Point5 { get; set; }
        public double Point25 { get; set; }
        public AAEQSummaryRowItem()
        {
            WithoutProjAlternative = 1;
            WithoutProjAAEQ = 2;
            WithProjAlternative = 3;
            WithProjAAEQ = 4;
            AAEQDamageReduced = 5;
            Point75 = 6;
            Point5 = 7;
            Point25 = 8;
        }


    }
}
