using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Data;

namespace Importer
{
    public class EconPYTable
    {
        public void ProcessEconPYDataTable(DataTable dataTable)
        {
            EconPYList theFuncList = GlobalVariables.mp_fdaStudy.GetEconPyList();
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable and create new EconPY function and Add
            for (int irow = 0; irow < dataTable.Rows.Count; irow++)
            {
                EconPY theFunc = new EconPY();

                for (int icol = 0; icol < dataTable.Columns.Count; icol++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[icol].ColumnName;

                    if (colName == "ID_PLAN")
                        theFunc.IdPlan = (int)dataTable.Rows[irow][icol];
                    else if (colName == "ID_YEAR")
                        theFunc.IdYear = (int)dataTable.Rows[irow][icol];
                    else if (colName == "FL_USR_IA")
                        theFunc.UseUserDefinedReaches = (int)dataTable.Rows[irow][icol];
                    else if (colName == "FL_RISKANL")
                        theFunc.UseRiskAnalysis = (int)dataTable.Rows[irow][icol];
                }
                theFuncList.Add(theFunc);
            }
        }
    }
}
