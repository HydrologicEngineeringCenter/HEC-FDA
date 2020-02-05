using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DbMemory
{
    class YearDataTable
    {
        public void ProcessYearDataTable(DataTable dataTable)
        {
            YearList theFuncList = GlobalVariables.mp_fdaStudy.GetYearList();
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable and create new Year function and Add
            for (int irow = 0; irow < dataTable.Rows.Count; irow++)
            {
                Year theFunc = new Year();

                for (int icol = 0; icol < dataTable.Columns.Count; icol++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[icol].ColumnName;
                    if (colName == "ID_YEAR")
                        theFunc.Id = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NM_YEAR")
                        theFunc.Name = (string)dataTable.Rows[irow][icol];
                    else if (colName == "DE_YEAR")
                        theFunc.Description = (string)dataTable.Rows[irow][icol];
                    else if (colName == "DATE")
                        theFunc.CalculationDate = (string)dataTable.Rows[irow][icol];
                    else if (colName == "META_DATA")
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
