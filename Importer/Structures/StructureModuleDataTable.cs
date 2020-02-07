using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Importer
{
    class StructureModuleDataTable
    {
        public void ProcessStructureModulesTable(DataTable dataTable)
        {
            StructureModuleList theFuncList = GlobalVariables.mp_fdaStudy.GetStructureModuleList();    //There is no NextObjId Data Object in DB

            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable (Should be only one)
            for (int irec = 0; irec < dataTable.Rows.Count; irec++)
            {
                StructureModule theFunc = new StructureModule();

                for (int ifield = 0; ifield < dataTable.Columns.Count; ifield++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[ifield].ColumnName;

                    if (colName == "ID_SGRP")
                        theFunc.Id = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NM_SGRP")
                        theFunc.Name = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "DE_SGRP")
                        theFunc.Description = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "FDA_REF")
                        theFunc.NumReferences = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "DATE")
                        theFunc.CalculationDate = (string)dataTable.Rows[irec][ifield];
                    else if(colName == "META_DATA")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
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
