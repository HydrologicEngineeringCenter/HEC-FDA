using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.IO;


namespace Importer
{
    [Serializable]
    public class EconPY
    {
        #region Properties
        public int IdPlan { get; set; }
        public int IdYear { get; set; }
        public int UseUserDefinedReaches { get; set; }
        public int UseRiskAnalysis { get; set; }
        #endregion
        public void Print()
        {
            WriteLine($"\n\tPlan ID: {this.IdPlan}");
            WriteLine($"\tPlan Name: {GlobalVariables.mp_fdaStudy.GetPlanList().getName(IdPlan)}");
            WriteLine($"\tYear ID: {this.IdYear}");
            WriteLine($"\tYear: {GlobalVariables.mp_fdaStudy.GetYearList().getName(IdYear)}");
            WriteLine($"\tUse User Defined Reaches: {UseUserDefinedReaches}");
            WriteLine($"\tUse Risk Analysis: {UseRiskAnalysis}");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\nPlan ID: {this.IdPlan}");
            wr.WriteLine($"\tPlan Name: {GlobalVariables.mp_fdaStudy.GetPlanList().getName(IdPlan)}");
            wr.WriteLine($"\tYear ID: {this.IdYear}");
            wr.WriteLine($"\tYear: {GlobalVariables.mp_fdaStudy.GetYearList().getName(IdYear)}");
            wr.WriteLine($"\tUse User Defined Reaches: {UseUserDefinedReaches}");
            wr.WriteLine($"\tUse Risk Analysis: {UseRiskAnalysis}");
        }
    }
}
