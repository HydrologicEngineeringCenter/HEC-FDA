using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.IO;

namespace Importer
{
    [Serializable]
    public class StrucGroupLookup : DbfFileFdLookupManager
    {
        public StrucGroupLookup() : base()
        {
            ;
        }
        public void Print()
        {
            Write($"\nPlan ID: {_IdPlan}");
            Write($"\tYear ID: {_IdYear}");
            Write($"\tStructure Group ID: {_IdDataFunc}");
            Write("\n");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\nPlan ID: {_IdPlan}");
            wr.WriteLine($"\tPlan Name: {GlobalVariables.mp_fdaStudy.GetPlanList().getName(_IdPlan)}");
            wr.WriteLine($"\tYear ID: {_IdYear}");
            wr.WriteLine($"\tYear Name: {GlobalVariables.mp_fdaStudy.GetYearList().getName(_IdYear)}");
            wr.WriteLine($"\tData Function ID: {_IdDataFunc}");
            wr.WriteLine($"\tStructure Group Name: {GlobalVariables.mp_fdaStudy.GetStructureModuleList().GetName(_IdDataFunc)}");
        }
        public void setNextObjectID(long id)
        {
            GlobalVariables.mp_fdaStudy.GetNextIdMgr().setNextObjIdRatData(id);
        }
        public long getNextObjectID()
        {
            return GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdRatData();
        }

    }
}
