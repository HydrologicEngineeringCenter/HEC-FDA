using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Data;

namespace DbMemory
{
    class WspDataTable
    {
        public void ProcessWspDataTable(DataTable dataTable)
        {
            WaterSurfaceProfileList theFuncList = GlobalVariables.mp_fdaStudy.GetWspList();

            double[,] valuesFunc = null;
            double[,] wspData = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            //Process Each Record (Row) in _dataTable and create new Water Surface Profile function
            // and Add it to the list
            for (int irec = 0; irec < dataTable.Rows.Count; irec++)
            {
                WaterSurfaceProfile theFunc = new WaterSurfaceProfile();

                WspSectionData wspSectionData = null;

                for (int ifield = 0; ifield < dataTable.Columns.Count; ifield++)
                {
                    string colName = dataTable.Columns[ifield].ColumnName;

                    if (colName == "ID_WSP")
                        theFunc.Id = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NM_WSP")
                        theFunc.Name = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "DE_WSP")
                        theFunc.Description = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "DATE")
                        theFunc.CalculationDate = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "META_DATA")
                        theFunc.MetaData = dataTable.Rows[irec][ifield].ToString();
                    else if (colName == "ID_PLAN")
                        theFunc.IdPlan = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_STREAM")
                        theFunc.IdStream = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FDA_REF")
                        theFunc.NumRefs = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "DATA_TYPE")
                    {
                        int theType = (int)dataTable.Rows[irec][ifield];
                        if (theType == 1)
                            theFunc.WspDataTypeId = WspDataType.DISCHARGE_FREQUENCY;
                        else
                            theFunc.WspDataTypeId = WspDataType.STAGE_FREQUENCY;
                    }
                    else if (colName == "NU_XSECT")
                        theFunc.NumberOfCrossSections = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NU_PROFILE")
                        theFunc.NumberOfProfiles = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NOTE_WSP")
                        theFunc.Notes = dataTable.Rows[irec][ifield].ToString();
                    else if (colName == "PROB_WSP")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;
                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);
                        double[] probs = new double[numRowsMemo];
                        for (int j = 0; j < numRowsMemo; j++)
                            probs[j] = valuesFunc[j, 0];
                        theFunc.SetProbabilities(numRowsMemo, probs);

                        //Allocate data for cross-section data since we now know the size
                        wspSectionData = new WspSectionData(theFunc.NumberOfProfiles);

                        ;

                        if (theFunc.WspDataTypeId == WspDataType.DISCHARGE_FREQUENCY)
                            wspData = new double[theFunc.NumberOfCrossSections, 2 * theFunc.NumberOfProfiles + 2];
                        else
                            wspData = new double[theFunc.NumberOfCrossSections, theFunc.NumberOfProfiles + 2];
                    }
                    else if (colName == "WSP_STATN")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;
                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);
                        for (int j = 0; j < numRowsMemo; j++)
                        {
                            wspData[j, 0] = valuesFunc[j, 0];
                        }
                    }
                    else if (colName == "WSP_INVERT")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;
                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);
                        for (int j = 0; j < numRowsMemo; j++)
                            wspData[j, 1] = valuesFunc[j, 0];
                    }
                    else if (colName == "WSP_STAGES")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;
                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);
                        for (int j = 0; j < numRowsMemo; j++)
                        {
                            for (int l = 0; l < numColsMemo; l++)
                                wspData[j, 2 + l] = valuesFunc[j, l];
                        }
                    }
                    else if (colName == "WSP_DISCH" && theFunc.WspDataTypeId == WspDataType.DISCHARGE_FREQUENCY)
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;
                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);
                        for (int jrow = 0; jrow < numRowsMemo; jrow++)
                        {
                            for (int jcol = 0; jcol < numColsMemo; jcol++)
                                wspData[jrow, 2 + numColsMemo + jcol] = valuesFunc[jrow, jcol];
                        }
                    }
                }
                theFunc.AddWspSectionData(theFunc.NumberOfCrossSections, theFunc.NumberOfProfiles, theFunc.WspDataTypeId, wspData);
                theFuncList.Add(theFunc);
            }
        }
    }
}
