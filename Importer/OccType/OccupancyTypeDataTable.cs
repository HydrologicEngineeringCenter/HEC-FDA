using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Importer
{
    class OccupancyTypeDataTable
    {
        public void ProcessOccupancyTypeDataTable(DataTable dataTable)
        {
            OccupancyTypeList theFuncList = GlobalVariables.mp_fdaStudy.GetOccupancyTypeList();
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable and create new Occupancy Type function and Add
            for (int irec = 0; irec < dataTable.Rows.Count; irec++)
            {
                OccupancyType theFunc = new OccupancyType();

                for (int ifield = 0; ifield < dataTable.Columns.Count; ifield++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[ifield].ColumnName;

                    if (colName == "ID_OCCTYPE")
                        theFunc.Id = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NM_OCCTYPE")
                        theFunc.Name = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "DE_OCCTYPE")
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
                    else if (colName == "ID_CATEGRY")
                        theFunc.CategoryId = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "RATIO_CONT")
                        theFunc.RatioContent = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "RATIO_OTHR")
                        theFunc.RatioOther = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "RATIO_CAR")
                        theFunc.RatioCar = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "VAL_CAR")
                        ;
                    else if (colName == "HT_CAR")
                        ;
                    else if (colName == "NU_CARS")
                        ;
                    else if (colName == "DF_DIRECT$")
                    {
                        int usesDollarInt = (int)dataTable.Rows[irec][ifield];
                        if (usesDollarInt == 0)
                            theFunc.UsesDollar = false;
                        else
                            theFunc.UsesDollar = true;
                    }
                    else if (colName == "NUORD_STR")
                        theFunc._SingleDamageFunction[0].SetNumRows((int)dataTable.Rows[irec][ifield]);
                    else if (colName == "FL_STRPCT")
                    {
                        ;   //Not used, all functions must have the same type of damage (percent or dollar)
                    }
                    else if (colName == "FL_STRDIST")
                    {
                        ErrorType errType = ErrorType.NONE;
                        errType = (ErrorType)((int)dataTable.Rows[irec][ifield]);
                        theFunc._SingleDamageFunction[0].SetType(errType);
                    }
                    else if (colName == "NUORD_CON")
                        theFunc._SingleDamageFunction[1].SetNumRows((int)dataTable.Rows[irec][ifield]);
                    else if (colName == "FL_CONPCT")
                        ;
                    else if (colName == "FL_CONDIST")
                    {
                        ErrorType errType = ErrorType.NONE;
                        errType = (ErrorType)((int)dataTable.Rows[irec][ifield]);
                        theFunc._SingleDamageFunction[1].SetType(errType);
                    }
                    else if (colName == "NUORD_OTR")
                        theFunc._SingleDamageFunction[3].SetNumRows((int)dataTable.Rows[irec][ifield]);
                    else if (colName == "FL_OTRPCT")
                        ;
                    else if (colName == "FL_OTRDIST")
                    {
                        ErrorType errType = ErrorType.NONE;
                        errType = (ErrorType)((int)dataTable.Rows[irec][ifield]);
                        theFunc._SingleDamageFunction[3].SetType(errType);
                    }
                    else if (colName == "NUORD_CAR")
                        theFunc._SingleDamageFunction[3].SetNumRows((int)dataTable.Rows[irec][ifield]);
                    else if (colName == "FL_CARPCT")
                        ;
                    else if (colName == "FL_CARDIST")
                    {
                        ErrorType errType = ErrorType.NONE;
                        errType = (ErrorType)((int)dataTable.Rows[irec][ifield]);
                        theFunc._SingleDamageFunction[3].SetType(errType);
                    }
                    else if (colName == "FU_STRUC")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        if (numRowsMemo > 0)
                            theFunc._SingleDamageFunction[0].ReallocateWithoutSave(numRowsMemo);


                        double[] stage = new double[numRowsMemo];
                        double[] damage = new double[numRowsMemo];
                        double[] stdDev = new double[numRowsMemo];
                        double[] stdDevLog = new double[numRowsMemo];
                        double[] stdDevLow = new double[numRowsMemo];
                        double[] stdDevHigh = new double[numRowsMemo];

                        for (int jr = 0; jr < numRowsMemo; jr++)
                        {
                            stage[jr] = valuesFunc[jr, 0];
                            damage[jr] = valuesFunc[jr, 1];

                            stdDev[jr] = stdDevLog[jr] = stdDevLow[jr] = stdDevHigh[jr] = Study.badNumber;

                            if (numColsMemo > 2) stdDev[jr] = valuesFunc[jr, 2];
                            if (numColsMemo > 3) stdDevLog[jr] = valuesFunc[jr, 3];
                            if (numColsMemo > 4) stdDevLow[jr] = valuesFunc[jr, 4];
                            if (numColsMemo > 5) stdDevHigh[jr] = valuesFunc[jr, 5];
                        }
                        theFunc._SingleDamageFunction[0].SetNumRows(numRowsMemo);
                        theFunc._SingleDamageFunction[0].SetDepth(stage);
                        theFunc._SingleDamageFunction[0].SetDamage(damage);
                        if(theFunc._SingleDamageFunction[0].GetTypeError() == ErrorType.NORMAL)
                            theFunc._SingleDamageFunction[0].SetStdDev(stdDev);
                        if (theFunc._SingleDamageFunction[0].GetTypeError() == ErrorType.LOGNORMAL)
                            theFunc._SingleDamageFunction[0].SetStdDev(stdDevLog);
                        if (theFunc._SingleDamageFunction[0].GetTypeError() == ErrorType.TRIANGULAR)
                        {
                            theFunc._SingleDamageFunction[0].SetTriangularLower(stdDevLow);
                            theFunc._SingleDamageFunction[0].SetTriangularUpper(stdDevHigh);
                        }
                    }
                    else if (colName == "FU_CONT")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        if (numRowsMemo > 0)
                            theFunc._SingleDamageFunction[1].ReallocateWithoutSave(numRowsMemo);


                        double[] stage = new double[numRowsMemo];
                        double[] damage = new double[numRowsMemo];
                        double[] stdDev = new double[numRowsMemo];
                        double[] stdDevLog = new double[numRowsMemo];
                        double[] stdDevLow = new double[numRowsMemo];
                        double[] stdDevHigh = new double[numRowsMemo];

                        for (int jr = 0; jr < numRowsMemo; jr++)
                        {
                            stage[jr] = valuesFunc[jr, 0];
                            damage[jr] = valuesFunc[jr, 1];

                            stdDev[jr] = stdDevLog[jr] = stdDevLow[jr] = stdDevHigh[jr] = Study.badNumber;

                            if (numColsMemo > 2) stdDev[jr] = valuesFunc[jr, 2];
                            if (numColsMemo > 3) stdDevLog[jr] = valuesFunc[jr, 3];
                            if (numColsMemo > 4) stdDevLow[jr] = valuesFunc[jr, 4];
                            if (numColsMemo > 5) stdDevHigh[jr] = valuesFunc[jr, 5];
                        }
                        theFunc._SingleDamageFunction[1].SetNumRows(numRowsMemo);
                        theFunc._SingleDamageFunction[1].SetDepth(stage);
                        theFunc._SingleDamageFunction[1].SetDamage(damage);
                        if (theFunc._SingleDamageFunction[1].GetTypeError() == ErrorType.NORMAL)
                            theFunc._SingleDamageFunction[1].SetStdDev(stdDev);
                        if (theFunc._SingleDamageFunction[1].GetTypeError() == ErrorType.LOGNORMAL)
                            theFunc._SingleDamageFunction[1].SetStdDev(stdDevLog);
                        if (theFunc._SingleDamageFunction[1].GetTypeError() == ErrorType.TRIANGULAR)
                        {
                            theFunc._SingleDamageFunction[1].SetTriangularLower(stdDevLow);
                            theFunc._SingleDamageFunction[1].SetTriangularUpper(stdDevHigh);
                        }
                    }
                    else if (colName == "FU_OTHER")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        if (numRowsMemo > 0)
                            theFunc._SingleDamageFunction[2].ReallocateWithoutSave(numRowsMemo);


                        double[] stage = new double[numRowsMemo];
                        double[] damage = new double[numRowsMemo];
                        double[] stdDev = new double[numRowsMemo];
                        double[] stdDevLog = new double[numRowsMemo];
                        double[] stdDevLow = new double[numRowsMemo];
                        double[] stdDevHigh = new double[numRowsMemo];

                        for (int jr = 0; jr < numRowsMemo; jr++)
                        {
                            stage[jr] = valuesFunc[jr, 0];
                            damage[jr] = valuesFunc[jr, 1];

                            stdDev[jr] = stdDevLog[jr] = stdDevLow[jr] = stdDevHigh[jr] = Study.badNumber;

                            if (numColsMemo > 2) stdDev[jr] = valuesFunc[jr, 2];
                            if (numColsMemo > 3) stdDevLog[jr] = valuesFunc[jr, 3];
                            if (numColsMemo > 4) stdDevLow[jr] = valuesFunc[jr, 4];
                            if (numColsMemo > 5) stdDevHigh[jr] = valuesFunc[jr, 5];
                        }
                        theFunc._SingleDamageFunction[2].SetNumRows(numRowsMemo);
                        theFunc._SingleDamageFunction[2].SetDepth(stage);
                        theFunc._SingleDamageFunction[2].SetDamage(damage);
                        if (theFunc._SingleDamageFunction[2].GetTypeError() == ErrorType.NORMAL)
                            theFunc._SingleDamageFunction[2].SetStdDev(stdDev);
                        if (theFunc._SingleDamageFunction[2].GetTypeError() == ErrorType.LOGNORMAL)
                            theFunc._SingleDamageFunction[2].SetStdDev(stdDevLog);
                        if (theFunc._SingleDamageFunction[2].GetTypeError() == ErrorType.TRIANGULAR)
                        {
                            theFunc._SingleDamageFunction[2].SetTriangularLower(stdDevLow);
                            theFunc._SingleDamageFunction[2].SetTriangularUpper(stdDevHigh);
                        }
                    }
                    else if (colName == "FU_CAR")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        if (numRowsMemo > 0)
                            theFunc._SingleDamageFunction[3].ReallocateWithoutSave(numRowsMemo);


                        double[] stage = new double[numRowsMemo];
                        double[] damage = new double[numRowsMemo];
                        double[] stdDev = new double[numRowsMemo];
                        double[] stdDevLog = new double[numRowsMemo];
                        double[] stdDevLow = new double[numRowsMemo];
                        double[] stdDevHigh = new double[numRowsMemo];

                        for (int jr = 0; jr < numRowsMemo; jr++)
                        {
                            stage[jr] = valuesFunc[jr, 0];
                            damage[jr] = valuesFunc[jr, 1];

                            stdDev[jr] = stdDevLog[jr] = stdDevLow[jr] = stdDevHigh[jr] = Study.badNumber;

                            if (numColsMemo > 2) stdDev[jr] = valuesFunc[jr, 2];
                            if (numColsMemo > 3) stdDevLog[jr] = valuesFunc[jr, 3];
                            if (numColsMemo > 4) stdDevLow[jr] = valuesFunc[jr, 4];
                            if (numColsMemo > 5) stdDevHigh[jr] = valuesFunc[jr, 5];
                        }
                        theFunc._SingleDamageFunction[3].SetNumRows(numRowsMemo);
                        theFunc._SingleDamageFunction[3].SetDepth(stage);
                        theFunc._SingleDamageFunction[3].SetDamage(damage);
                        if (theFunc._SingleDamageFunction[3].GetTypeError() == ErrorType.NORMAL)
                            theFunc._SingleDamageFunction[3].SetStdDev(stdDev);
                        if (theFunc._SingleDamageFunction[3].GetTypeError() == ErrorType.LOGNORMAL)
                            theFunc._SingleDamageFunction[3].SetStdDev(stdDevLog);
                        if (theFunc._SingleDamageFunction[3].GetTypeError() == ErrorType.TRIANGULAR)
                        {
                            theFunc._SingleDamageFunction[3].SetTriangularLower(stdDevLow);
                            theFunc._SingleDamageFunction[3].SetTriangularUpper(stdDevHigh);
                        }
                    }

                    //First Floor Errors
                    else if (colName == "FL_FFDIST")
                    {
                        ErrorType errType = ErrorType.NONE;
                        errType = (ErrorType)((int)dataTable.Rows[irec][ifield]);
                        theFunc._ErrorDistribution[0].SetErrorType(errType);
                    }
                    else if (colName == "SD_EL_FF" && theFunc._ErrorDistribution[0].GetErrorType() == ErrorType.NORMAL)
                        theFunc._ErrorDistribution[0].SetStdDev((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "ERLO_FF" && theFunc._ErrorDistribution[0].GetErrorType() == ErrorType.TRIANGULAR)
                        theFunc._ErrorDistribution[0].SetStdDev((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "ERHI_FF" && theFunc._ErrorDistribution[0].GetErrorType() == ErrorType.TRIANGULAR)
                        theFunc._ErrorDistribution[0].SetUpper((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "LOGSD_FF" && theFunc._ErrorDistribution[0].GetErrorType() == ErrorType.LOGNORMAL)
                        theFunc._ErrorDistribution[0].SetStdDev((double)dataTable.Rows[irec][ifield]);

                    //Structure Value Errors
                    else if (colName == "FL_SVALDIS")
                    {
                        ErrorType errType = ErrorType.NONE;
                        errType = (ErrorType)((int)dataTable.Rows[irec][ifield]);
                        theFunc._ErrorDistribution[1].SetErrorType(errType);
                    }
                    else if (colName == "SD_SVAL" && theFunc._ErrorDistribution[1].GetErrorType() == ErrorType.NORMAL)
                        theFunc._ErrorDistribution[1].SetStdDev((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "ERLO_SVAL" && theFunc._ErrorDistribution[1].GetErrorType() == ErrorType.TRIANGULAR)
                        theFunc._ErrorDistribution[1].SetStdDev((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "ERHI_SVAL" && theFunc._ErrorDistribution[1].GetErrorType() == ErrorType.TRIANGULAR)
                        theFunc._ErrorDistribution[1].SetUpper((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "LOGSD_SVAL" && theFunc._ErrorDistribution[1].GetErrorType() == ErrorType.LOGNORMAL)
                        theFunc._ErrorDistribution[1].SetStdDev((double)dataTable.Rows[irec][ifield]);

                    //Content Value Errors
                    else if (colName == "FL_CVALDIS")
                    {
                        ErrorType errType = ErrorType.NONE;
                        errType = (ErrorType)((int)dataTable.Rows[irec][ifield]);
                        theFunc._ErrorDistribution[2].SetErrorType(errType);
                    }
                    else if (colName == "SD_CVAL" && theFunc._ErrorDistribution[2].GetErrorType() == ErrorType.NORMAL)
                        theFunc._ErrorDistribution[2].SetStdDev((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "ERLO_CVAL" && theFunc._ErrorDistribution[2].GetErrorType() == ErrorType.TRIANGULAR)
                        theFunc._ErrorDistribution[2].SetStdDev((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "ERHI_CVAL" && theFunc._ErrorDistribution[2].GetErrorType() == ErrorType.TRIANGULAR)
                        theFunc._ErrorDistribution[2].SetUpper((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "LOGSD_CVAL" && theFunc._ErrorDistribution[2].GetErrorType() == ErrorType.LOGNORMAL)
                        theFunc._ErrorDistribution[2].SetStdDev((double)dataTable.Rows[irec][ifield]);

                    //Other Value Errors
                    else if (colName == "FL_OVALDIS")
                    {
                        ErrorType errType = ErrorType.NONE;
                        errType = (ErrorType)((int)dataTable.Rows[irec][ifield]);
                        theFunc._ErrorDistribution[3].SetErrorType(errType);
                    }
                    else if (colName == "SD_OVAL" && theFunc._ErrorDistribution[3].GetErrorType() == ErrorType.NORMAL)
                        theFunc._ErrorDistribution[3].SetStdDev((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "ERLO_OVAL" && theFunc._ErrorDistribution[3].GetErrorType() == ErrorType.TRIANGULAR)
                        theFunc._ErrorDistribution[3].SetStdDev((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "ERHI_OVAL" && theFunc._ErrorDistribution[3].GetErrorType() == ErrorType.TRIANGULAR)
                        theFunc._ErrorDistribution[3].SetUpper((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "LOGSD_OVAL" && theFunc._ErrorDistribution[3].GetErrorType() == ErrorType.LOGNORMAL)
                        theFunc._ErrorDistribution[3].SetStdDev((double)dataTable.Rows[irec][ifield]);

                    //Car Value Errors
                    else if (colName == "FL_CRVDIS")
                    {
                        ErrorType errType = ErrorType.NONE;
                        errType = (ErrorType)((int)dataTable.Rows[irec][ifield]);
                        theFunc._ErrorDistribution[4].SetErrorType(errType);
                    }
                    else if (colName == "SD_CRVAL" && theFunc._ErrorDistribution[4].GetErrorType() == ErrorType.NORMAL)
                        theFunc._ErrorDistribution[4].SetStdDev((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "ERLO_CRVAL" && theFunc._ErrorDistribution[4].GetErrorType() == ErrorType.TRIANGULAR)
                        theFunc._ErrorDistribution[4].SetStdDev((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "ERHI_CRVAL" && theFunc._ErrorDistribution[4].GetErrorType() == ErrorType.TRIANGULAR)
                        theFunc._ErrorDistribution[4].SetUpper((double)dataTable.Rows[irec][ifield]);
                    else if (colName == "LSD_CRVAL" && theFunc._ErrorDistribution[4].GetErrorType() == ErrorType.LOGNORMAL)
                        theFunc._ErrorDistribution[4].SetStdDev((double)dataTable.Rows[irec][ifield]);
                }
                theFuncList.Add(theFunc, null);
            }
        }

    }
}
