using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.IO;

namespace DbMemory
{
    [Serializable]
    public class WspLookup : DbfFileFdLookupManager
    {
        public WspLookup() : base()
        {
            ;
        }
        public void Print()
        {
            Write($"\nPlan ID: {_IdPlan}");
            Write($"\tYear ID: {_IdYear}");
            Write($"\tStream ID: {_IdStream}");
            Write($"\tData Function ID: {_IdDataFunc}");
            Write("\n");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\nPlan ID: {_IdPlan}");
            wr.WriteLine($"\tPlan Name: {GlobalVariables.mp_fdaStudy.GetPlanList().getName(_IdPlan)}");
            wr.WriteLine($"\tYear ID: {_IdYear}");
            wr.WriteLine($"\tYear Name: {GlobalVariables.mp_fdaStudy.GetYearList().getName(_IdYear)}");
            wr.WriteLine($"\tStream ID: {_IdStream}");
            wr.WriteLine($"\tStream Name: {GlobalVariables.mp_fdaStudy.GetStreamList().getName(_IdStream)}");
            wr.WriteLine($"\tData Function ID: {_IdDataFunc}");
            wr.WriteLine($"\tWSP Function Name: {GlobalVariables.mp_fdaStudy.GetWspList().GetName(_IdDataFunc)}");
        }
        public void setNextObjectID(long id)
        {
            GlobalVariables.mp_fdaStudy.GetNextIdMgr().setNextObjIdWspData(id);
        }
        public long getNextObjectID()
        {
            return GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdWspData();
        }

    }
}
