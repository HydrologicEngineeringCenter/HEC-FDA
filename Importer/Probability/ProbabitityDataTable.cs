using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Collections;
using static System.Console;

using static Importer.ProbabilityFunction;

namespace Importer
{
    class ProbabitityDataTable
    {
        public void ProcessProbdataDataTable(DataTable dataTable)
        {
            ProbabilityFunctionList probFuncList = GlobalVariables.mp_fdaStudy.GetProbabilityFuncList();
            double[,] valuesProbFuncGraphical = null;
            double[,] valuesCalcPoints = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;


            //Process Each Record (Row) in _dataTable and create new prob function and Add
            for (int irow = 0; irow < dataTable.Rows.Count; irow++)
            {
                ProbabilityFunction probFunc = new ProbabilityFunction();

                probFunc.Id = (int)dataTable.Rows[irow][0];
                probFunc.Name = (string)dataTable.Rows[irow][1];

                //Need some information that is read after-the-fact
                probFunc.UncertTypeSpecification = (UncertaintyTypeSpecification)dataTable.Rows[irow][27];


                for (int icol = 0; icol < dataTable.Columns.Count; icol++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[icol].ColumnName;

                    if (colName == "ID_PROBFU")
                        probFunc.Id = (int)dataTable.Rows[irow][icol];
                    else if (colName == "NM_PROBFU")
                        probFunc.Name = (string)dataTable.Rows[irow][icol];
                    else if (colName == "DE_PROBFU")
                        probFunc.Description = (string)dataTable.Rows[irow][icol];
                    else if (colName == "DATE")
                        probFunc.CalculationDate = (string)dataTable.Rows[irow][icol];
                    //ID_PLAN, ID_YEAR, ID_STREAM, ID_IMPAREA, NU_REFER
                    else if (colName == "CALC_TYPE")
                        probFunc.ProbabilityFunctionTypeId = (FrequencyFunctionType)dataTable.Rows[irow][icol];
                    else if (colName == "DATA_TYPE")
                        probFunc.ProbabilityDataTypeId = (ProbabilityDataType)dataTable.Rows[irow][icol];
                    else if (colName == "REC_LENGTH")
                        probFunc.EquivalentLengthOfRecord = (int)dataTable.Rows[irow][icol];
                    else if (colName == "LOG_MEAN")
                        probFunc.MomentsLp3[0] = (double)dataTable.Rows[irow][icol];
                    else if (colName == "STD_DEV")
                        probFunc.MomentsLp3[1] = (double)dataTable.Rows[irow][icol];
                    else if (colName == "SKEW")
                        probFunc.MomentsLp3[2] = (double)dataTable.Rows[irow][icol];
                    else if (colName == "SOURCESTAT")
                        probFunc.SourceOfStatisticsId = (SourceOfStatistics)dataTable.Rows[irow][icol];
                    else if (colName == "Q_01")
                        probFunc.PointsSynthetic[2] = (double)dataTable.Rows[irow][icol];
                    else if (colName == "Q_10")
                        probFunc.PointsSynthetic[1] = (double)dataTable.Rows[irow][icol];
                    else if (colName == "Q_50")
                        probFunc.PointsSynthetic[0] = (double)dataTable.Rows[irow][icol];
                    else if (colName == "NUORDGRAPH")
                        probFunc.NumberOfGraphicalPoints = (int)dataTable.Rows[irow][icol];
                    else if (colName == "FU_GRAPH")
                    {
                        theMemoFieldStr = dataTable.Rows[irow][icol].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesProbFuncGraphical);

                        if (numRowsMemo > 0)
                            probFunc.ReallocateGraphicalWithCheckAndSave(numRowsMemo);

                        for (int jr = 0; jr < numRowsMemo; jr++)
                        {
                            probFunc.ExceedanceProbability[jr] = valuesProbFuncGraphical[jr, 0];

                            if (probFunc.ProbabilityDataTypeId == ProbabilityDataType.DISCHARGE_FREQUENCY)
                                probFunc.Discharge[jr] = valuesProbFuncGraphical[jr, 1];
                            else if (probFunc.ProbabilityDataTypeId == ProbabilityDataType.STAGE_FREQUENCY)
                                probFunc.Stage[jr] = valuesProbFuncGraphical[jr, 1];
                            if (probFunc.UncertTypeSpecification == UncertaintyTypeSpecification.NORMAL &&
                                2 < numColsMemo)
                            {
                                probFunc.StdDevNormalUserDef[jr] = valuesProbFuncGraphical[jr, 2];
                            }
                            else if (probFunc.UncertTypeSpecification == UncertaintyTypeSpecification.LOG_NORMAL &&
                                2 < numColsMemo)
                            {
                                probFunc.StdDevLogUserDef[jr] = valuesProbFuncGraphical[jr, 3];
                            }
                            else if (probFunc.UncertTypeSpecification == UncertaintyTypeSpecification.TRIANGULAR &&
                                5 < numColsMemo)
                            {
                                probFunc.StdDevLowerUserDef[jr] = valuesProbFuncGraphical[jr, 4];
                                probFunc.StdDevUpperUserDef[jr] = valuesProbFuncGraphical[jr, 5];
                            }
                        }
                        ;   //Process memo field
                    }
                    else if (colName == "FF_UNCERTY")
                        probFunc.UncertTypeSpecification = (UncertaintyTypeSpecification)dataTable.Rows[irow][icol];
                    else if (colName == "NUORD_TRAN")
                        probFunc.NumberOfTransFlowPoints = (int)dataTable.Rows[irow][icol];
                    else if (colName == "FU_TRANS")
                    {
                        theMemoFieldStr = dataTable.Rows[irow][icol].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;
                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesProbFuncGraphical);

                        if (numRowsMemo > 0)
                            probFunc.ReallocateTransformFlowWithCheckAndSave(numRowsMemo);

                        for (int jr = 0; jr < numRowsMemo; jr++)
                        {
                            probFunc.TransFlowInflow[jr] = valuesProbFuncGraphical[jr, 0];
                            probFunc.TransFlowOutflow[jr] = valuesProbFuncGraphical[jr, 1];
                            if (2 < numColsMemo)
                                probFunc.TransFlowStdDev[jr] = valuesProbFuncGraphical[jr, 2];
                            if (3 < numColsMemo)
                                probFunc.TransFlowLogStdDev[jr] = valuesProbFuncGraphical[jr, 3];
                            if (5 < numColsMemo)
                            {
                                probFunc.TransFlowLower[jr] = valuesProbFuncGraphical[jr, 4];
                                probFunc.TransFlowUpper[jr] = valuesProbFuncGraphical[jr, 5];
                            }
                        }
                    }
                    else if (colName == "PDF_TRANSQ")
                        probFunc.ErrorTypeTransformFlow = (ErrorType)dataTable.Rows[irow][icol];
                    else if (colName == "NUORD_CALC")
                    {
                        ;   //probFunc._NumCalcPoints = (int)_dataTable.Rows[irow][icol];   //probFunc.Nu  Number of calculation points
                        int nuCalc = (int)dataTable.Rows[irow][icol];   //probFunc.Nu  Number of calculation points
                        if (nuCalc > 0) probFunc._NumCalcPoints = nuCalc;   //Looks like not set in database
                    }
                    else if (colName == "FU_CALCTAB")
                    {
                        theMemoFieldStr = dataTable.Rows[irow][icol].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesCalcPoints);

                        if (numRowsMemo > 0)
                        {
                            probFunc.ReallocateCalculationPointsWithCheckAndSave(numRowsMemo);
                            probFunc._NumCalcPoints = numRowsMemo;
                        }

                        for (int jr = 0; jr < numRowsMemo; jr++)
                        {
                            probFunc.Calc95[jr] = valuesCalcPoints[jr, 0];
                            probFunc.Calc75[jr] = valuesCalcPoints[jr, 1];
                            probFunc.Calc50[jr] = valuesCalcPoints[jr, 2];
                            probFunc.Calc25[jr] = valuesCalcPoints[jr, 3];
                            probFunc.Calc05[jr] = valuesCalcPoints[jr, 4];
                        }
                    }
                }

                //GlobalVariables.mp_fdaStudy.GetProbabilityFuncList().Add(probFunc);
                probFuncList.Add(probFunc, null);
                //_MustFlushProbFunc = false;
                probFunc.Reset();
                probFunc.Deallocate();
            }
            //WriteLine("\n\n\n\n");
            //for (int j = 0; j < 100; j++) Write("-");
            //WriteLine("\nOutput After adding probability functions.");
            //probFuncList.Print();


        }

    }
}
