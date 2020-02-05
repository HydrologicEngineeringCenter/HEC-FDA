using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DbMemory
{
    class LeveeDataTable
    {
        public void ProcessProbdataDataTable(DataTable dataTable)
        {
            LeveeList theFuncList = GlobalVariables.mp_fdaStudy.GetLeveeList();
            double[,] valuesIntExt = null;
            double[,] valuesGeoTech = null;
            //double[,] valuesWave = null;
            double[,] valuesFunc = null;


            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;


            //Process Each Record (Row) in dataTable and create new levee data and Add
            for (int irec = 0; irec < dataTable.Rows.Count; irec++)
            {
                Levee theFunc = new Levee();

                //Need some information that is read after-the-fact
                //probFunc.UncertTypeSpecification = (UncertaintyTypeSpecification)dataTable.Rows[irec][27];

                for (int ifield = 0; ifield < dataTable.Columns.Count; ifield++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[ifield].ColumnName;

                    if (colName == "ID_LEVEE")
                        theFunc.Id = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NM_LEVEE")
                        theFunc.Name = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "DE_LEVEE")
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

                    else if (colName == "ID_PLAN")
                        theFunc.IdPlan = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_YEAR")
                        theFunc.IdYear = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_STREAM")
                        theFunc.IdStream = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_IMPAREA")
                        theFunc.IdReach = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NUM_REF")
                        theFunc.NumRefs = (int)dataTable.Rows[irec][ifield];

                    else if (colName == "EL_TOP")
                        theFunc.ElevationTopOfLevee = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "NUORD_GEO")
                        theFunc.NumOrdsGeoTech = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NUORD_EXIN")
                        theFunc.NumOrdsIntExt = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NUORD_WAVE")
                        theFunc.NumOrdsWave = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "PDF_WAVE")
                    {
                        int theWavePdf = (int)dataTable.Rows[irec][ifield];
                        //TODO
                    }
                    else if (colName == "NUORD_SHAP")
                        theFunc.NumOrdsShape = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NUORD_INT")
                        theFunc.NumOrdsInt = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FU_GEOTECH")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesGeoTech);

                        if (numRowsMemo > 0)
                        {
                            double[] stage = new double[numRowsMemo];
                            double[] prob = new double[numRowsMemo];
                            for (int j = 0; j < numRowsMemo; j++)
                            {
                                stage[j] = valuesGeoTech[j, 0];
                                prob[j] = valuesGeoTech[j, 1];
                            }
                            theFunc.SetGeoTech(numRowsMemo, stage, prob);
                        }
                    }
                    else if (colName == "FU_EXTINT")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesIntExt);

                        if (numRowsMemo > 0)
                        {
                            double[] stageExt = new double[numRowsMemo];
                            double[] stageInt = new double[numRowsMemo];
                            for (int j = 0; j < numRowsMemo; j++)
                            {
                                stageExt[j] = valuesIntExt[j, 0];
                                stageInt[j] = valuesIntExt[j, 1];
                            }
                            theFunc.SetIntExt(numRowsMemo, stageExt, stageInt);
                        }
                    }
                    else if (colName == "FU_WAVE")
                        ;// theFunc.xxxx = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FU_SHAPE")
                        ;// theFunc.xxxx = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FU_INT")
                        ;// theFunc.xxxx = (int)dataTable.Rows[irec][ifield];
                }
                theFuncList.Add(theFunc);
            }
        }
    }
}
