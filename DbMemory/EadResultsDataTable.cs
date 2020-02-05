using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DbMemory
{
    class EadResultsDataTable
    {
        public void ProcessEadResultsDataTable(DataTable dataTable)
        {
            EadResultList theFuncList = GlobalVariables.mp_fdaStudy.GetEadResultList();
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;

            int i = 0, j = 0;

            //Process Each Record (Row) in _dataTable and create new Ead Result and Add
            for (int irec = 0; irec < dataTable.Rows.Count; irec++)
            {
                EadResult theFunc = new EadResult();

                for (int ifield = 0; ifield < dataTable.Columns.Count; ifield++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[ifield].ColumnName;

                    if (colName == "ID_EADRES")
                        theFunc.Id = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NAME_EADRS")
                        theFunc.Name = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "DESC_EADRS")
                        theFunc.Description = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_PLAN")
                        theFunc.IdPlan = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_YEAR")
                        theFunc.IdYear = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_STREAM")
                        theFunc.IdStream = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "ID_IMPAREA")
                        theFunc.IdReach = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FDA_REF")
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
                    else if (colName == "IUNC")
                        theFunc.Iunc = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "PTARG")
                        theFunc.Ptarg = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "FDMTRG")
                        theFunc.Fdmtrg = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "NYEARS")
                        theFunc.Nyears = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "DRATE")
                        theFunc.Drate = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "ISTICS")
                        theFunc.Istics = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NSTICS")
                        theFunc.Nstics = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NCALC")
                        theFunc.Ncalc = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NFR")
                        theFunc.Nfr = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STARG")
                        theFunc.Starg = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "PARGMN")
                        theFunc.Pargmn = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "PARGMD")
                        theFunc.Pargmd = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "NEVENT")
                        theFunc.Nevent = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NEVSTP")
                        theFunc.Nevstp = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NRISK")
                        theFunc.Nrisk = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NCAT")
                        theFunc.Ncat = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NETBL")
                        theFunc.NetBl = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "SIMSTATTBL")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        //Study.PrintTable(numRowsMemo, numColsMemo, "SIMSTATTBL", valuesFunc);

                        double[] dmean = new double[numRowsMemo];
                        double[] dsdev = new double[numRowsMemo];
                        double[] dskew = new double[numRowsMemo];

                        for(i = 0; i < numRowsMemo; i++)
                        {
                            dmean[i] = valuesFunc[i, 0];
                            dsdev[i] = valuesFunc[i, 1];
                            dskew[i] = valuesFunc[i, 2];
                        }
                        theFunc.NumRows_SimStatTable = numRowsMemo;
                        theFunc.Dmean = dmean;
                        theFunc.Dsdev = dsdev;
                        theFunc.Dskew = dskew;

                        if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) theFunc.Print_SIMSTATTBL();
                    }
                    else if (colName == "EADCUM_TBL")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        double[] pcalc = new double[numRowsMemo];
                        int[] ntable = new int[numRowsMemo];
                        double[] cead = new double[numRowsMemo];

                        for(i = 0; i < numRowsMemo; i++)
                        {
                            pcalc[i] = valuesFunc[i, 0];
                            ntable[i] = (int)Math.Round(valuesFunc[i, 1]);
                            cead[i] = valuesFunc[i, 2];
                        }
                        //theFunc.Ncalc = numRowsMemo;
                        theFunc.NumRows_EadCum_Tbl = numRowsMemo;
                        theFunc.Pcalc = pcalc;
                        theFunc.Ntable = ntable;
                        theFunc.Cead = cead;

                        if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) theFunc.Print_EADCUM_TBL();
                    }
                    else if (colName == "SIMAVG_TBL")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        double[] prob = new double[numRowsMemo];
                        double[] qprob = new double[numRowsMemo];
                        double[] qtprob = new double[numRowsMemo];
                        double[] stprob = new double[numRowsMemo];
                        double[] dprob = new double[numRowsMemo * (numColsMemo - 4)];

                        for(i = 0; i < numRowsMemo; i++)
                        {
                            prob[i] = valuesFunc[i, 0];
                            qprob[i] = valuesFunc[i, 1];
                            qtprob[i] = valuesFunc[i, 2];
                            stprob[i] = valuesFunc[i, 3];

                            for (j = 4; j < numColsMemo; j++)
                            {
                                int m = i * (numColsMemo-4) + j - 4;
                                dprob[m] = valuesFunc[i, j];
                            }
                        }
                        theFunc.Ncat = numColsMemo - 5;
                        theFunc.Nfr = numRowsMemo;

                        theFunc.Prob = prob;
                        theFunc.Qprob = qprob;
                        theFunc.Qtprob = qtprob;
                        theFunc.Stprob = stprob;
                        theFunc.Dprob = dprob;

                        if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) theFunc.Print_SIMAVG_TBL();
                    }
                    else if (colName == "PPNONEXTAR")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        double[] pevent = new double[numRowsMemo];
                        double[] targev = new double[numRowsMemo];

                        for(i = 0; i < numRowsMemo; i++)
                        {
                            pevent[i] = valuesFunc[i, 0];
                            targev[i] = valuesFunc[i, 1];
                        }
                        //theFunc.Nevent = numRowsMemo; rdc critical;07Nov2018
                        theFunc.NumRows_Ppnonextar = numRowsMemo;
                        theFunc.NumCols_Ppnonextar = numColsMemo;
                        theFunc.Pevent = pevent;
                        theFunc.Targev = targev;

                        if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) theFunc.Print_PPNONEXTAR();
                    }
                    else if (colName == "PPNONEXTBL")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        double[] pevent = new double[numRowsMemo];
                        double[] sevent = new double[numRowsMemo * (numColsMemo - 1)];

                        //theFunc.Ncalc = numRowsMemo;rdc critical; Use Nevent?
                        for(i = 0; i < numRowsMemo; i++)
                        {
                            pevent[i] = valuesFunc[i, 0];

                            for(j = 1; j < numColsMemo; j++)
                            {
                                int m = i * (numColsMemo-1) + j-1;
                                sevent[m] = valuesFunc[i, j];
                            }
                        }
                        theFunc.NumRows_PPNONEXTBL = numRowsMemo;
                        theFunc.NumCols_PPNONEXTBL = numColsMemo;
                        theFunc.Pevent = pevent;
                        theFunc.Sevent = sevent;

                        if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) theFunc.Print_PPNONEXTBL(); 
                    }
                    else if (colName == "LTRISK_TBL")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        double[] targyr = new double[numRowsMemo];
                        double[] targrk = new double[numRowsMemo];

                        theFunc.Nrisk = numRowsMemo;
                        for(i = 0; i < numRowsMemo; i++)
                        {
                            targyr[i] = valuesFunc[i, 0];
                            targrk[i] = valuesFunc[i, 1];
                        }
                        theFunc.Targyr = targyr;
                        theFunc.Targrk = targrk;

                        if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) theFunc.Print_LTRISK_TBL();
                    }
                    else if (colName == "SIMEAD_TBL")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        int[] idCat = new int[numRowsMemo];
                        double[] ead = new double[numRowsMemo];

                        for(i = 0; i < numRowsMemo; i++)
                        {
                            idCat[i] = (int)Math.Round(valuesFunc[i, 0]);
                            ead[i] = valuesFunc[i, 1];
                        }
                        theFunc.NumRows_SIMEAD_TBL = numRowsMemo;
                        theFunc.IdCat = idCat;
                        theFunc.Ead = ead;

                        if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) theFunc.Print_SIMEAD_TBL();
                    }
                    else if (colName == "EADDISTTBL")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        double[] probCtead = new double[numRowsMemo];
                        double[] ctead = new double[numRowsMemo];

                        theFunc.NetBl = numRowsMemo;
                        for(i = 0; i < numRowsMemo; i++)
                        {
                            probCtead[i] = valuesFunc[i, 0];
                            ctead[i] = valuesFunc[i, 1];
                        }
                        theFunc.Ptead = probCtead;
                        theFunc.Ctead = ctead;

                        if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) theFunc.Print_EADDISTTB();
                    }
                    else if (colName == "EADDISXTBL")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        double[] eadClass = new double[numRowsMemo];
                        int[] nhitEad = new int[numRowsMemo];
                        double[] eadFreq = new double[numRowsMemo];
                        double[] eadFreqI = new double[numRowsMemo];

                        for(i = 0; i < numRowsMemo; i++)
                        {
                            eadClass[i] = valuesFunc[i, 0];
                            nhitEad[i] = (int)(valuesFunc[i, 1] + 0.01);
                            eadFreq[i] = valuesFunc[i, 2];
                            eadFreqI[i] = valuesFunc[i, 3];
                        }
                        theFunc.NumClassEad = numRowsMemo;
                        theFunc.EadClass = eadClass;
                        theFunc.NhitEad = nhitEad;
                        theFunc.EadFreq = eadFreq;
                        theFunc.EadFreqI = eadFreqI;

                        if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) theFunc.Print_EADDISXTBL();
                    }
                    else if (colName == "AEPDISXTBL")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        double[] aepClass = new double[numRowsMemo];
                        int[] nhitAep = new int[numRowsMemo];
                        double[] aepFreq = new double[numRowsMemo];
                        double[] aepFreqI = new double[numRowsMemo];

                        for (i = 0; i < numRowsMemo; i++)
                        {
                            aepClass[i] = valuesFunc[i, 0];
                            nhitAep[i] = (int)(valuesFunc[i, 1] + 0.01);
                            aepFreq[i] = valuesFunc[i, 2];
                            aepFreqI[i] = valuesFunc[i, 3];
                        }

                        theFunc.NumClassAep = numRowsMemo;
                        theFunc.AepClass = aepClass;
                        theFunc.NhitAep = nhitAep;
                        theFunc.AepFreq = aepFreq;
                        theFunc.AepFreqI = aepFreqI;

                        if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) theFunc.Print_AEPDISXTBL();
                    }
                    else if (colName == "AEPDISSTBL")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);

                        double[] aepStandard = new double[numRowsMemo];
                        double[] freqStandard = new double[numRowsMemo];
                        double[] freqStandardI = new double[numRowsMemo];

                        for (i = 0; i < numRowsMemo; i++)
                        {
                            aepStandard[i] = valuesFunc[i, 0];
                            freqStandard[i] = valuesFunc[i, 1];
                            freqStandardI[i] = valuesFunc[i, 2];
                        }

                        theFunc.NclassStd = numRowsMemo;
                        theFunc.AepStandard = aepStandard;
                        theFunc.FreqStandard = freqStandard;
                        theFunc.FreqStandardI = freqStandardI;

                        if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) theFunc.Print_AEPDISSTBL();
                    }
                    else if (colName == "VERDATEMTH")
                        theFunc.SetVersionDateMethod((string)dataTable.Rows[irec][ifield]);
                    else if (colName == "EADWOUNTBL")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);
                    }
                    else if (colName == "PROBDAMTBL")
                    {
                        theMemoFieldStr = dataTable.Rows[irec][ifield].ToString();
                        numRowsMemo = 0;
                        numColsMemo = 0;

                        MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRowsMemo, ref numColsMemo, ref valuesFunc);
                    }
                }
                theFuncList.Add(theFunc);
            }
        }
    }
}
