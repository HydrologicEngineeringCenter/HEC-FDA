using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DbMemory
{
    class EconGlobalParametersDataTable
    {
        public void ProcessEconGlobalParametersTable(DataTable dataTable)
        {
            //RatingFunctionList theFuncList = GlobalVariables.mp_fdaStudy.getStudyData();    //There is no NextObjId Data Object in DB

            /*
            double[,] valuesFunc = null;
            string theMemoFieldStr = "";
            int numRowsMemo = 0;
            int numColsMemo = 0;
            */

            //Process Each Record (Row) in _dataTable (Should be only one)
            for (int irec = 0; irec < dataTable.Rows.Count; irec++)
            {
                EconGlobalParameters theFunc = GlobalVariables.mp_fdaStudy.getEconGlobalParameters();

                for (int ifield = 0; ifield < dataTable.Columns.Count; ifield++)
                {
                    //Console.Write($"{_dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    string colName = dataTable.Columns[ifield].ColumnName;

                    if (colName == "NU_SIM")
                        theFunc.NumOfSimulations = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "MIN_NU_INT")
                        theFunc.NumOfStageDamageOrdsMinimum = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "MAX_NU_INT")
                        theFunc.NumOfStageDamageOrdsMaximum = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "TRACE_LEV")
                        theFunc.TraceLevelStageDamage = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FL_APTRACE")
                        theFunc.TraceStageDamageAppend = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FL_DETAILS")
                        theFunc.TraceStrucDetail = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FL_APDETAI")
                        theFunc.TraceStrucDetailAppend = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FL_STDDEV")
                        theFunc.TraceStdDev = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FL_APSD")
                        theFunc.TraceStdDevAppend = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "TRACE")
                        theFunc.Trace = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STD_TRACE")
                        theFunc.TraceSTD = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "SEPARATORS")
                        theFunc.Separators = (string)dataTable.Rows[irec][ifield];
                    else if (colName == "UPD_IMPORT")
                        theFunc.UpdateImport = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FL_ADDUSTR")
                        theFunc.FL_ADDUSTR = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_ALL")
                        theFunc.ExportAll = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_PLAN")
                        theFunc.ExportPlan = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_YEAR")
                        theFunc.ExportYear = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_STRM")
                        theFunc.ExportStream = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_RCH")
                        theFunc.ExportReach = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_CAT")
                        theFunc.ExportCategory = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_OCCTYP")
                        theFunc.ExportOccType = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_MODULE")
                        theFunc.ExportModule = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_STRUCT")
                        theFunc.ExportStructure = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_WSP")
                        theFunc.ExportWsp = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_FREQFN")
                        theFunc.ExportProbFunc = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_RATING")
                        theFunc.ExportRateFunc = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_LEVEE")
                        theFunc.ExportLevee = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EXP_STGDMG")
                        theFunc.ExportStageDamage = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "TRACEL_EAD")
                        theFunc.TraceLevelEad = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "MAXEADSIM")
                        theFunc.NumEadSimulationsMax = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "MINEADSIM")
                        theFunc.NumEadSimulationsMin = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "USENEWORDR")
                        theFunc.UseNewOrderStats = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "USEMATCH")
                        theFunc.UseMatchPoint = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "PROBMATLOW")
                        theFunc.ProbMatchLow = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "PROBMATHI")
                        theFunc.ProbMatchHigh = (double)dataTable.Rows[irec][ifield];
                    else if (colName == "FL_POPCMP")
                        theFunc.ComputePopulation = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FL_UP_IAID")
                        theFunc.UpdateIAID = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FL_STRCSD")
                        theFunc.STRCSD = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FL_STRCPD")
                        theFunc.STRCPD = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FL_TRUNCLV")
                        theFunc.TRUNCLV = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "FL_AUTOSN")
                        theFunc.AUTOSN = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_EADRES")
                        theFunc.StoEadResults = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_QFRES")
                        theFunc.StoQfResults = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_QSRES")
                        theFunc.StoQsResults = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_SDRES")
                        theFunc.StoSdResults = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_FDRES")
                        theFunc.StoFdResults = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_SFRES")
                        theFunc.StoSfResults = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_NODSS")
                        theFunc.StoNoDSS = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_LIMCL")
                        theFunc.StoLimcl = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_ORDER")
                        theFunc.StoOrder = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_INPFUN")
                        theFunc.StoInputFunc = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_EADIST")
                        theFunc.StoEadDistribution = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_EADCAT")
                        theFunc.StoEadCategory = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_AEPDIS")
                        theFunc.StoAepDistribution = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_EADINP")
                        theFunc.StoEadInput = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "STO_EVTDIS")
                        theFunc.StoEventDistribution = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "EADCALCLEV")
                        theFunc.EadCalcLevel = (int)dataTable.Rows[irec][ifield];
                    else if (colName == "NUMREPSTOR")
                        theFunc.NumRepitionsToStor = (int)dataTable.Rows[irec][ifield];
                }
            }
        }
    }
}
