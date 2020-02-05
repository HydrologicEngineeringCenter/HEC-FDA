using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DbMemory
{
    class PlanDataTable
    {
        public void ProcessPlanDataTable(DataTable dataTable)
        {
            PlanList theFuncList = GlobalVariables.mp_fdaStudy.GetPlanList();
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable and create new Plan function and Add
            for (int irow = 0; irow < dataTable.Rows.Count; irow++)
            {
                Plan theFunc = new Plan();

                for (int icol = 0; icol < dataTable.Columns.Count; icol++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[icol].ColumnName;
                    if (colName == "ID_PLAN")
                        theFunc.Id = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NM_PLAN")
                        theFunc.Name = (string)dataTable.Rows[irow][icol];
                    else if (colName == "DE_PLAN")
                        theFunc.Description = (string)dataTable.Rows[irow][icol];
                    else if (colName == "DATE")
                        theFunc.CalculationDate = (string)dataTable.Rows[irow][icol];
                    else if(colName == "META_DATA")
                    {
                        theMemoFieldStr = dataTable.Rows[irow][icol].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        theFunc.MetaData = theMemoFieldStr;
                    }
                }
                theFuncList.Add(theFunc);
            }
        }
    }
}
