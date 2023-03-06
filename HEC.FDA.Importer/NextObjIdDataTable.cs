using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Importer
{
    class NextObjIdDataTable
    {
        public void ProcessNextObjIdTable(DataTable dataTable)
        {
            //RatingFunctionList theFuncList = GlobalVariables.mp_fdaStudy.getStudyData();    //There is no NextObjId Data Object in DB

            /*
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;
            */

            //Process Each Record (Row) in _dataTable (Should be only one)
            for (int irow = 0; irow < dataTable.Rows.Count; irow++)
            {
                NextIdMgr theFunc = GlobalVariables.mp_fdaStudy.GetNextIdMgr();

                for (int icol = 0; icol < dataTable.Columns.Count; icol++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[icol].ColumnName;

                    if (colName == "STY_NEXTID")
                        theFunc.NextObjectID = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDPLAN")
                        theFunc.NextObjIdPlan = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDYEAR")
                        theFunc.NextObjIdYear = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDSTREAM")
                        theFunc.NextObjIdStream = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDREACH")
                        theFunc.NextObjIdReach = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDDMGCAT")
                        theFunc.NextObjIdDmgCat = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDOCCTYP")
                        theFunc.NextObjIdOccType = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDMODULE")
                        theFunc.NextObjIdModule = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDSTRUCT")
                        theFunc.NextObjIdStruct = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDWSPDAT")
                        theFunc.NextObjIdWspData = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDFRQDAT")
                        theFunc.NextObjIdFreqData = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDRATDAT")
                        theFunc.NextObjIdRatData = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDLEVDAT")
                        theFunc.NextObjIdLevData = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDADFDAT")
                        theFunc.NextObjIdAggDfData = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NXIDEADDAT")
                        theFunc.NextObjIdEadData = (int)dataTable.Rows[irow][icol];
                }
            }
        }
    }
}
