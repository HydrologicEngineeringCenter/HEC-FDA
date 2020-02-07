using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Importer
{
    class StageDamageDataTable
    {
        public void ProcessStageDamageDataTable(DataTable dataTable)
        {
            AggregateDamageFunctionList theFuncList = GlobalVariables.mp_fdaStudy.GetAggDamgFuncList();
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable and create new Stage-Damage function and Add
            for (int irec = 0; irec < dataTable.Rows.Count; irec++)
            {
                AggregateDamageFunction theFunc = new AggregateDamageFunction();

                for (int ifield = 0; ifield < dataTable.Columns.Count; ifield++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[ifield].ColumnName;

                    if (colName == "ID_STGDAMG")
                        theFunc.Id = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NM_STGDAMG")
                        theFunc.Name = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "DE_STGDAMG")
                        theFunc.Description = (string)dataTable.Rows[irec][ifield];

                    else if (colName == "ID_PLAN")
                        theFunc.IdPlan = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_YEAR")
                        theFunc.IdYear = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_STREAM")
                        theFunc.IdStream = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_IMPAREA")
                        theFunc.IdReach = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_CATEGRY")
                        theFunc.IdCategory = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NUM_REF")
                        theFunc.NumRefs = (int)dataTable.Rows[irec][ifield];

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
                    else if (colName == "NORD_STRUC")
                        theFunc.GetSingleDamageFunction(StructureValueType.STRUCTURE).SetNumRows((int)dataTable.Rows[irec][ifield]);
                    else if (colName == "PDF_STRUC")
                    {
                        ErrorType errType = (ErrorType)(int)dataTable.Rows[irec][ifield];
                        theFunc.GetSingleDamageFunction(StructureValueType.STRUCTURE).SetType(errType);
                    }
                    else if (colName == "NORD_CONT")
                        theFunc.GetSingleDamageFunction(StructureValueType.CONTENT).SetNumRows((int)dataTable.Rows[irec][ifield]);
                    else if (colName == "PDF_CONT")
                    {
                        ErrorType errType = (ErrorType)(int)dataTable.Rows[irec][ifield];
                        theFunc.GetSingleDamageFunction(StructureValueType.CONTENT).SetType(errType);
                    }
                    else if (colName == "NUORD_OTHR")
                        theFunc.GetSingleDamageFunction(StructureValueType.OTHER).SetNumRows((int)dataTable.Rows[irec][ifield]);
                    else if (colName == "PDF_OTHR")
                    {
                        ErrorType errType = (ErrorType)(int)dataTable.Rows[irec][ifield];
                        theFunc.GetSingleDamageFunction(StructureValueType.OTHER).SetType(errType);
                    }
                    else if (colName == "NORD_CAR")
                        theFunc.GetSingleDamageFunction(StructureValueType.CAR).SetNumRows((int)dataTable.Rows[irec][ifield]);
                    else if (colName == "PDF_CAR")
                    {
                        ErrorType errType = (ErrorType)(int)dataTable.Rows[irec][ifield];
                        theFunc.GetSingleDamageFunction(StructureValueType.CAR).SetType(errType);
                    }
                    else if (colName == "NORD_TOTL")
                        theFunc.GetSingleDamageFunction(StructureValueType.TOTAL).SetNumRows((int)dataTable.Rows[irec][ifield]);
                    else if (colName == "PDF_TOTL")
                    {
                        ErrorType errType = (ErrorType)(int)dataTable.Rows[irec][ifield];
                        theFunc.GetSingleDamageFunction(StructureValueType.TOTAL).SetType(errType);
                    }
                    else if (colName == "FU_STRUC")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        if (numRowsMemo > 0)
                            theFunc.GetSingleDamageFunction(StructureValueType.STRUCTURE).ReallocateWithoutSave(numRowsMemo);

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
                        theFunc.GetSingleDamageFunction(StructureValueType.STRUCTURE).SetNumRows(numRowsMemo);
                        theFunc.GetSingleDamageFunction(StructureValueType.STRUCTURE).SetDepth(stage);
                        theFunc.GetSingleDamageFunction(StructureValueType.STRUCTURE).SetDamage(damage);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.STRUCTURE).GetTypeError() == ErrorType.NORMAL)
                            theFunc.GetSingleDamageFunction(StructureValueType.STRUCTURE).SetStdDev(stdDev);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.STRUCTURE).GetTypeError() == ErrorType.LOGNORMAL)
                            theFunc.GetSingleDamageFunction(StructureValueType.STRUCTURE).SetStdDev(stdDevLog);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.STRUCTURE).GetTypeError() == ErrorType.TRIANGULAR)
                        {
                            theFunc.GetSingleDamageFunction(StructureValueType.STRUCTURE).SetTriangularLower(stdDevLow);
                            theFunc.GetSingleDamageFunction(StructureValueType.STRUCTURE).SetTriangularUpper(stdDevHigh);
                        }
                    }
                    else if (colName == "FU_CONT")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        if (numRowsMemo > 0)
                            theFunc.GetSingleDamageFunction(StructureValueType.CONTENT).ReallocateWithoutSave(numRowsMemo);

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
                        theFunc.GetSingleDamageFunction(StructureValueType.CONTENT).SetNumRows(numRowsMemo);
                        theFunc.GetSingleDamageFunction(StructureValueType.CONTENT).SetDepth(stage);
                        theFunc.GetSingleDamageFunction(StructureValueType.CONTENT).SetDamage(damage);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.CONTENT).GetTypeError() == ErrorType.NORMAL)
                            theFunc.GetSingleDamageFunction(StructureValueType.CONTENT).SetStdDev(stdDev);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.CONTENT).GetTypeError() == ErrorType.LOGNORMAL)
                            theFunc.GetSingleDamageFunction(StructureValueType.CONTENT).SetStdDev(stdDevLog);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.CONTENT).GetTypeError() == ErrorType.TRIANGULAR)
                        {
                            theFunc.GetSingleDamageFunction(StructureValueType.CONTENT).SetTriangularLower(stdDevLow);
                            theFunc.GetSingleDamageFunction(StructureValueType.CONTENT).SetTriangularUpper(stdDevHigh);
                        }
                    }
                    else if (colName == "FU_OTHR")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        if (numRowsMemo > 0)
                            theFunc.GetSingleDamageFunction(StructureValueType.OTHER).ReallocateWithoutSave(numRowsMemo);

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
                        theFunc.GetSingleDamageFunction(StructureValueType.OTHER).SetNumRows(numRowsMemo);
                        theFunc.GetSingleDamageFunction(StructureValueType.OTHER).SetDepth(stage);
                        theFunc.GetSingleDamageFunction(StructureValueType.OTHER).SetDamage(damage);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.OTHER).GetTypeError() == ErrorType.NORMAL)
                            theFunc.GetSingleDamageFunction(StructureValueType.OTHER).SetStdDev(stdDev);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.OTHER).GetTypeError() == ErrorType.LOGNORMAL)
                            theFunc.GetSingleDamageFunction(StructureValueType.OTHER).SetStdDev(stdDevLog);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.OTHER).GetTypeError() == ErrorType.TRIANGULAR)
                        {
                            theFunc.GetSingleDamageFunction(StructureValueType.OTHER).SetTriangularLower(stdDevLow);
                            theFunc.GetSingleDamageFunction(StructureValueType.OTHER).SetTriangularUpper(stdDevHigh);
                        }
                    }
                    else if (colName == "FU_CAR")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        if (numRowsMemo > 0)
                            theFunc.GetSingleDamageFunction(StructureValueType.CAR).ReallocateWithoutSave(numRowsMemo);

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
                        theFunc.GetSingleDamageFunction(StructureValueType.CAR).SetNumRows(numRowsMemo);
                        theFunc.GetSingleDamageFunction(StructureValueType.CAR).SetDepth(stage);
                        theFunc.GetSingleDamageFunction(StructureValueType.CAR).SetDamage(damage);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.CAR).GetTypeError() == ErrorType.NORMAL)
                            theFunc.GetSingleDamageFunction(StructureValueType.CAR).SetStdDev(stdDev);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.CAR).GetTypeError() == ErrorType.LOGNORMAL)
                            theFunc.GetSingleDamageFunction(StructureValueType.CAR).SetStdDev(stdDevLog);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.CAR).GetTypeError() == ErrorType.TRIANGULAR)
                        {
                            theFunc.GetSingleDamageFunction(StructureValueType.CAR).SetTriangularLower(stdDevLow);
                            theFunc.GetSingleDamageFunction(StructureValueType.CAR).SetTriangularUpper(stdDevHigh);
                        }
                    }
                    else if (colName == "FU_TOTL")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        if (numRowsMemo > 0)
                            theFunc.GetSingleDamageFunction(StructureValueType.TOTAL).ReallocateWithoutSave(numRowsMemo);

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
                        theFunc.GetSingleDamageFunction(StructureValueType.TOTAL).SetNumRows(numRowsMemo);
                        theFunc.GetSingleDamageFunction(StructureValueType.TOTAL).SetDepth(stage);
                        theFunc.GetSingleDamageFunction(StructureValueType.TOTAL).SetDamage(damage);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.TOTAL).GetTypeError() == ErrorType.NORMAL)
                            theFunc.GetSingleDamageFunction(StructureValueType.TOTAL).SetStdDev(stdDev);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.TOTAL).GetTypeError() == ErrorType.LOGNORMAL)
                            theFunc.GetSingleDamageFunction(StructureValueType.TOTAL).SetStdDev(stdDevLog);
                        if (theFunc.GetSingleDamageFunction(StructureValueType.TOTAL).GetTypeError() == ErrorType.TRIANGULAR)
                        {
                            theFunc.GetSingleDamageFunction(StructureValueType.TOTAL).SetTriangularLower(stdDevLow);
                            theFunc.GetSingleDamageFunction(StructureValueType.TOTAL).SetTriangularUpper(stdDevHigh);
                        }
                    }
                }
                theFuncList.Add(theFunc);
            }
        }
    }
}
