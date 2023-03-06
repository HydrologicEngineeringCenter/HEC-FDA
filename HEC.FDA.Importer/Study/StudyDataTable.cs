using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Data;

namespace Importer
{
    class StudyDataTable
    {
        public void ProcessStudyDataTable(DataTable dataTable)
        {
            //RatingFunctionList theFuncList = GlobalVariables.mp_fdaStudy.getStudyData();    //There is no Study Data Object in DB

            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable (Should be only one)
            for (int irow = 0; irow < dataTable.Rows.Count; irow++)
            {
                StudyData theFunc = GlobalVariables.mp_fdaStudy.GetStudyData();

                for (int icol = 0; icol < dataTable.Columns.Count; icol++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[icol].ColumnName;

                    if (colName == "STY_DBVER")
                        theFunc.VersionDatabase = (string)dataTable.Rows[irow][icol];
                    else if (colName == "STY_NAME")
                        theFunc.StudyName = (string)dataTable.Rows[irow][icol];
                    else if (colName == "STY_DESC")
                        theFunc.StudyDescription = (string)dataTable.Rows[irow][icol];
                    else if (colName == "STY_NOTES")
                    {
                        theMemoFieldStr = dataTable.Rows[irow][icol].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        theFunc.StudyNotes = theMemoFieldStr;
                    }
                    else if (colName == "STY_SUNITS")
                        theFunc.StudySystemUnits = (int)dataTable.Rows[irow][icol];
                    else if (colName == "STY_MUNITS")
                        theFunc.StudyMonetaryUnits = (string)dataTable.Rows[irow][icol];
                    else if (colName == "STY_PLIFE")
                        theFunc.StudyProjectLife = (int)dataTable.Rows[irow][icol];
                    else if (colName == "STY_DRATE")
                        theFunc.StudyDiscountRate = (double)dataTable.Rows[irow][icol];
                    else if (colName == "STY_PYEAR")
                        theFunc.StudyPriceYear = (int)dataTable.Rows[irow][icol];
                    else if (colName == "STY_PNDX")
                        theFunc.StudyPriceFactor = (double)dataTable.Rows[irow][icol];
                    else if (colName == "STY_BPYEAR")
                        theFunc.StudyBaseYear = (int)dataTable.Rows[irow][icol];
                    else if (colName == "STY_BPNDX")
                        theFunc.StudyBasePriceFactor = (double)dataTable.Rows[irow][icol];
                    else if (colName == "STY_WOPLAN")
                        theFunc.StudyIdWithoutPlan = (int)dataTable.Rows[irow][icol];
                }
            }
        }
    }
}
