using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DbMemory
{
    class ReachDataTable
    {
        public void ProcessReachDataTable(DataTable dataTable)
        {
            DamageReachList theFuncList = GlobalVariables.mp_fdaStudy.GetDamageReachList();
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable and create new Plan function and Add
            for (int irec = 0; irec < dataTable.Rows.Count; irec++)
            {
                DamageReach theFunc = new DamageReach();

                for (int ifield = 0; ifield < dataTable.Columns.Count; ifield++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[ifield].ColumnName;
                    if (colName == "ID_IMPAREA")
                        theFunc.Id = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NM_IMPAREA")
                        theFunc.Name = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "DE_IMPAREA")
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
                    else if (colName == "ID_STREAM")
                        theFunc.StreamId = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STA_BEGIN")
                        theFunc.StationBegin = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "STA_INDEX")
                        theFunc.StationIndex = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "STA_END")
                        theFunc.StationEnd = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "IA_NORTH")
                        ;// theFunc.
                    else if (colName == "IA_EAST")
                        ;
                    else if (colName == "BANK")
                    {
                        StreamBank theBank = StreamBank.BOTH;
                        int theBankInt = (int)dataTable.Rows[irec][ifield];
                        switch (theBankInt)
                        {
                            case 0:
                                theBank = StreamBank.LEFT;
                                break;
                            case 1:
                                theBank = StreamBank.RIGHT;
                                break;
                            default:
                                theBank = StreamBank.BOTH;
                                break;
                        }
                        theFunc.BankStream = theBank;
                    }
                    else if (colName == "EL_MIN_TAB")
                        ;// theFunc.
                    else if (colName == "EL_MAX_TAB")
                        ;// theFunc.
                    else if (colName == "EL_INC_TAB")
                        ;// theFunc.
                }
                theFuncList.Add(theFunc);
            }
        }

    }
}
