using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport.Results
{
    public class EADSummaryRowItem
    {
        public string WithoutProjAlternative { get; set; }
        public double WithoutProjEAD { get; set; }
        public string WithProjAlternative { get; set; }
        public double WithProjEAD { get; set; }
        public double EADDamageReduced { get; set; }
        public double Point75 { get; set; }
        public double Point5 { get; set; }
        public double Point25 { get; set; }

        public string QuartileDamages { get; set; }

        public EADSummaryRowItem(string withoutName, double withoutAAEQDamage, string withProjName, double withProjAAEQ, double AAEQReduced, double point75, double point5, double point25 )
        {
            WithoutProjAlternative = withoutName;
            WithoutProjEAD = withoutAAEQDamage;
            WithProjAlternative = withProjName;
            WithProjEAD = withProjAAEQ;
            EADDamageReduced = AAEQReduced;
            Point75 = point75;
            Point5 = point5;
            Point25 = point25;

            QuartileDamages = point75.ToString(StringConstants.DEFAULT_UNIT_FORMAT) + "\t\t" +
                point5.ToString(StringConstants.DEFAULT_UNIT_FORMAT) + "\t\t" +
                point25.ToString(StringConstants.DEFAULT_UNIT_FORMAT) ;
        }
    }
}
