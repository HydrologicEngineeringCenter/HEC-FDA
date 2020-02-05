using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using DbMemory;

namespace FdaCombinedMain
{
    class ProcessAllDbfFiles
    {
        public void ProcessAllTheDbfFiles(string theDataBaseDirectory)
        {

            bool procAllFlag = false;
            ProcessFlags procFlags = new ProcessFlags
            {
                ProcessStudy = false,
                ProcessNextId = false,
                ProcessEconParm = false,
                ProcessPlan = false,
                ProcessYear = false,
                ProcessStream = false,
                ProcessReach = false,
                ProcessCategory = false,
                ProcessOccType = false,
                ProcessStrucGroup = false,
                ProcessStructures = false,
                ProcessWsp = false,
                ProcessProb = false,
                ProcessRate = false,
                ProcessLevee = false,
                ProcessEconPY = false,
                ProcessStageDamage = false,
                ProcessEad = true,
                ProcessEquivAD = false,

                ProcessStrucGroupLook = false,
                ProcessWspLook = false,
                ProcessProbLook = false,
                ProcessRatLook = false,
                ProcessLevLook = false,
                ProcessAggDamgLook = false,
                ProcessEadLook = false
            };

            DbfFileManager dbfFileMgr = new DbfFileManager();
            //DbfProcessor dbfProc = new DbfProcessor();
            DataTable dataTable = null;


            if (procFlags.ProcessStudy || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "STUDY.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetStudyData().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetStudyData().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }

            if (procFlags.ProcessNextId || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "NEXTID.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetNextIdMgr().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetNextIdMgr().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessEconParm || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "ECONPARM.dbf");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.getEconGlobalParameters().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.getEconGlobalParameters().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessPlan || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "Plan.dbf");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetPlanList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetPlanList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);

                //Test Searching for plan object
                Plan thePlan = GlobalVariables.mp_fdaStudy.GetPlanList().GetPlan("Without");
            }
            if (procFlags.ProcessYear || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "Year.dbf");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetYearList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetYearList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessStream || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "Stream.dbf");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetStreamList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetStreamList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessReach || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "IMPAREA.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetDamageReachList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetDamageReachList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessCategory || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "CATEGORY.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetDamageCategoryList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetDamageCategoryList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessOccType || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "OCCTYPE.dbf");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetOccupancyTypeList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetOccupancyTypeList().PrintToTable();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessStrucGroup || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "SGRPDATA.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetStructureModuleList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetStructureModuleList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessStructures || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "STRUCT.dbf");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetStructureList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetStructureList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessEconPY || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "ECONPY.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetEconPyList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetEconPyList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessWsp == true || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "WSPDATA.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetWspList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetWspList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessProb || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "PROBDATA.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetProbabilityFuncList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetProbabilityFuncList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessRate || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "RATDATA.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetRatingFunctionList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetRatingFunctionList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessLevee || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "LEVDATA.dbf");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetLeveeList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetLeveeList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessStageDamage || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "SDMGDATA.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetAggDamgFuncList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetAggDamgFuncList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessEad || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "EADDATA.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetEadResultList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetEadResultList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessEquivAD || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "EQUIVBEN.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetEquivBenefitsList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetEquivBenefitsList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessStrucGroupLook || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "SGRPLOOK.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetStrucGroupLookupList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetStrucGroupLookupList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessWspLook || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "WSPLOOK.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetWspLookupList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetWspLookupList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessProbLook || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "PROBLOOK.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetProbLookupList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetProbLookupList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessRatLook || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "RATLOOK.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetRateLookupList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetRateLookupList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessLevLook || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "LEVLOOK.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetLeveeLookupList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetLeveeLookupList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessAggDamgLook || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "SDMGLOOK.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetAggDamgLookupList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetAggDamgLookupList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }
            if (procFlags.ProcessEadLook || procAllFlag)
            {
                dataTable = new DataTable();
                dbfFileMgr.SetTheDbfFile(theDataBaseDirectory + "\\" + "EADLOOK.DBF");
                dbfFileMgr.ProcessTheDbfFile(dataTable);
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) GlobalVariables.mp_fdaStudy.GetEadResultsLookupList().Print();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 0) GlobalVariables.mp_fdaStudy.GetEadResultsLookupList().PrintToFile();
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) dbfFileMgr.PrintDataTableVertical(dataTable);
            }

        }
    }
}
