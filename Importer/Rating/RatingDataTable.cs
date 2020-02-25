using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Data;

namespace Importer
{
    class RatingDataTable
    {
        public void ProcessRatingDataTable(DataTable dataTable)
        {
            RatingFunctionList theFuncList = GlobalVariables.mp_fdaStudy.GetRatingFunctionList();
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable and create new prob function and Add
            for (int irow = 0; irow < dataTable.Rows.Count; irow++)
            {
                RatingFunction theFunc = new RatingFunction();

                for (int icol = 0; icol < dataTable.Columns.Count; icol++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[icol].ColumnName;

                    if (colName == "ID_RATING")
                        theFunc.Id = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NM_RATING")
                        theFunc.Name = (string)dataTable.Rows[irow][icol];
                    else if (colName == "DE_RATING")
                        theFunc.Description = (string)dataTable.Rows[irow][icol];
                    else if (colName == "DATE")
                        theFunc.CalculationDate = (string)dataTable.Rows[irow][icol];
                    //ID_PLAN, ID_YEAR, ID_STREAM, ID_IMPAREA, NU_REFER
                    else if (colName == "NUORD_RATE")
                        theFunc.NumberOfPoints = (int)dataTable.Rows[irow][icol];
                    else if (colName == "PDF_RATING")
                        theFunc.ErrorTypesId = (ErrorType)dataTable.Rows[irow][icol];
                    else if (colName == "USE_CALC")
                    {
                        int uge = (int)dataTable.Rows[irow][icol];
                        if (uge == 0) theFunc.UsesGlobalError = false;
                        else theFunc.UsesGlobalError = true;
                    }
                    else if (colName == "EL_CALC")
                        theFunc.BaseStage = (double)dataTable.Rows[irow][icol];
                    else if (colName == "SD_CALC")
                        theFunc.GlobalStdDev = (double)dataTable.Rows[irow][icol];
                    else if (colName == "LOGSD_CALC")
                        theFunc.GlobalStdDevLog = (double)dataTable.Rows[irow][icol];
                    else if (colName == "ERLO_CALC")
                        theFunc.GlobalStdDevLow = (double)dataTable.Rows[irow][icol];
                    else if (colName == "ERHI_CALC")
                        theFunc.GlobalStdDevHigh = (double)dataTable.Rows[irow][icol];
                    else if (colName == "FU_RATING")
                    {
                        theMemoFieldStr = dataTable.Rows[irow][icol].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        if (numRowsMemo > 0)
                            theFunc.ReallocateRatingWithCheckAndSave(numRowsMemo);  //rdc check; Need to clear ordinates?

                        double[] stage = new double[numRowsMemo];
                        double[] discharge = new double[numRowsMemo];
                        double[] stdDev = new double[numRowsMemo];
                        double[] stdDevLog = new double[numRowsMemo];
                        double[] stdDevLow = new double[numRowsMemo];
                        double[] stdDevHigh = new double[numRowsMemo];

                        for (int jr = 0; jr < numRowsMemo; jr++)
                        {
                            stage[jr] = valuesFunc[jr, 0];
                            discharge[jr] = valuesFunc[jr, 1];

                            stdDev[jr] = stdDevLog[jr] = stdDevLow[jr] = stdDevHigh[jr] = Study.badNumber;

                            if (numColsMemo > 2) stdDev[jr] = valuesFunc[jr, 2];
                            if (numColsMemo > 3) stdDevLog[jr] = valuesFunc[jr, 3];
                            if (numColsMemo > 4) stdDevLow[jr] = valuesFunc[jr, 4];
                            if (numColsMemo > 5) stdDevHigh[jr] = valuesFunc[jr, 5];
                        }
                        theFunc.SetStage(numRowsMemo, stage);
                        theFunc.SetDischarge(numRowsMemo, discharge);
                        theFunc.SetStdDev(numRowsMemo, stdDev);
                        theFunc.SetStdDevLog(numRowsMemo, stdDevLog);
                        theFunc.SetStdDevLow(numRowsMemo, stdDevLow);
                        theFunc.SetStdDevHigh(numRowsMemo, stdDevHigh);
                    }

                }
                theFuncList.Add(theFunc);
            }
        }
    }
}
