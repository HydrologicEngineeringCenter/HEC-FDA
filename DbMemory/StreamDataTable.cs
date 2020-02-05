using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DbMemory
{
    class StreamDataTable
    {
        public void ProcessStreamDataTable(DataTable dataTable)
        {
            StreamList theFuncList = GlobalVariables.mp_fdaStudy.GetStreamList();
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable and create new Plan function and Add
            for (int irow = 0; irow < dataTable.Rows.Count; irow++)
            {
                Stream theFunc = new Stream();

                for (int icol = 0; icol < dataTable.Columns.Count; icol++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[icol].ColumnName;
                    if (colName == "ID_STREAM")
                        theFunc.Id = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NM_STREAM")
                        theFunc.Name = (string)dataTable.Rows[irow][icol];
                    else if (colName == "DE_STREAM")
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
