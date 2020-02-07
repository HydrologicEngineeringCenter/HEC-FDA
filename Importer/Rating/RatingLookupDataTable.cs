using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Importer
{
    class RatingLookupDataTable
    {
        public void ProcessRateLookDataTable(DataTable dataTable)
        {
            RateLookupList theFuncList = GlobalVariables.mp_fdaStudy.GetRateLookupList();

            //Process Each Record (Row) in _dataTable and create new Prob function and Add
            for (int irec = 0; irec < dataTable.Rows.Count; irec++)
            {
                RateLookup theFunc = new RateLookup();

                for (int ifield = 0; ifield < dataTable.Columns.Count; ifield++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[ifield].ColumnName;

                    if (colName == "ID_PLAN")
                        theFunc._IdPlan = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_YEAR")
                        theFunc._IdYear = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_STREAM")
                        theFunc._IdStream = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_IMPAREA")
                        theFunc._IdReach = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_RATING")
                        theFunc._IdDataFunc = (int)dataTable.Rows[irec][ifield];
                }
                theFuncList.Add(theFunc);
            }
        }

    }
}
