using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DbMemory
{
    class CategoryDataTable
    {
        public void ProcessCategoryDataTable(DataTable dataTable)
        {
            DamageCategoryList theFuncList = GlobalVariables.mp_fdaStudy.GetDamageCategoryList();
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable and create new Category function and Add
            for (int irec = 0; irec < dataTable.Rows.Count; irec++)
            {
                DamageCategory theFunc = new DamageCategory();

                for (int ifield = 0; ifield < dataTable.Columns.Count; ifield++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[ifield].ColumnName;
                    if (colName == "ID_CATEGRY")
                        theFunc.Id = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NM_CATEGRY")
                        theFunc.Name = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "DE_CATEGRY")
                        theFunc.Description = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "DATE")
                        theFunc.CalculationDate = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "META_DATA")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        theFunc.MetaData = theMemoFieldStr;
                    }
                    else if (colName == "FACT_PRICE")
                        theFunc.CostFactor = (double)dataTable.Rows[irec][ifield];
                }
                theFuncList.Add(theFunc);
            }
        }
    }
}
