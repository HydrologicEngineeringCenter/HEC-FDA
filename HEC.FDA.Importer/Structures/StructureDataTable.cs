using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Importer
{
    class StructureDataTable
    {
        public void ProcessStructureDataTable(DataTable dataTable)
        {
            StructureList theFuncList = GlobalVariables.mp_fdaStudy.GetStructureList();
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable and create new Structure  and Add
            for (int irec = 0; irec < dataTable.Rows.Count; irec++)
            {
                Structure theFunc = new Structure();

                for (int ifield = 0; ifield < dataTable.Columns.Count; ifield++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[ifield].ColumnName;

                    if (colName == "ID_STRUCT")
                        theFunc.Id = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NM_STRUCT")
                        theFunc.Name = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "DE_STRUCT")
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
                    else if (colName == "YEAR_SRVC")
                        theFunc.YearInService = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_CATEGRY")
                        theFunc.CategoryId = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_STREAM")
                        theFunc.StreamId = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "BANK")
                    {
                        int theBankInt = (int)dataTable.Rows[irec][ifield];
                        StreamBank theBank = StreamBank.LEFT;
                        if (theBankInt == 1)
                            theBank = StreamBank.RIGHT;
                        theFunc.BankOfStream = theBank;
                    }
                    else if (colName == "STA_STRUC")
                        theFunc.StationAtStructure = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_IA_CALC")
                        theFunc.CalculatedReachId = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_IMPAREA")
                        theFunc.SidReachId = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_SGRP")
                        theFunc.StructureModuleId = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FL_USE_FF")
                    {
                        int usesFfElevInt = (int)dataTable.Rows[irec][ifield];
                        if (usesFfElevInt == 0)
                            theFunc.UsesFirstFloorElev = false;
                        else
                            theFunc.UsesFirstFloorElev = true;
                    }
                    else if (colName == "EL_1FLOOR")
                        theFunc.ElevationsStructure[(int)Structure.ElevationValue.FIRST_FLOOR] = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "EL_GROUND")
                        theFunc.ElevationsStructure[(int)Structure.ElevationValue.GROUND] = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "HT_FOUND")
                        theFunc.ElevationsStructure[(int)Structure.ElevationValue.DELTAG] = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "DP_ZERODMG")
                        theFunc.ElevationsStructure[(int)Structure.ElevationValue.DELTAZ] = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "AUTODIFF")
                        theFunc.ElevationsStructure[(int)Structure.ElevationValue.AUTODIFF] = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_OCCTYPE")
                        theFunc.DamageFunctionId = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "VAL_STRUC")
                        theFunc.ValueOfStructure = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "VAL_CONT")
                        theFunc.ValueOfContent = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "VAL_OTHER")
                        theFunc.ValueOfOther = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "VAL_CAR")
                        theFunc.ValueOfCar = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "STREET")
                        theFunc.StreetName = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "CITY")
                        theFunc.CityName = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_STATE")
                        theFunc.StateCode = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "ZIP")
                        theFunc.ZipCode = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "STR_NORTH")
                        theFunc.NorthingCoordinate = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "STR_EAST")
                        theFunc.EastingCoordinate = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "ST_ZONE")
                        theFunc.ZoneCoordinate = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "NUM_STRUCT")
                        theFunc.NumberOfStructures = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "IMAGE")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        theFunc.ImageFilename = theMemoFieldStr;
                    }
                    else if (colName == "NUM_CARS")
                        theFunc.NumberOfCars = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "PARCEL_NUM")
                        theFunc.ParcelNumber = (string)dataTable.Rows[irec][ifield];
                }
                theFuncList.Add(theFunc);
            }
        }

    }
}
