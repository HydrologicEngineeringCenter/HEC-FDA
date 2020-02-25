using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static System.Console;

namespace Importer
{
    [Serializable]
    public class ProbLookup : DbfFileFdLookupManager
    {
        public ProbLookup() : base()
        {
            ;
        }
        public void Print()
        {
            Write($"\nPlan ID: {_IdPlan}");
            Write($"\tYear ID: {_IdYear}");
            Write($"\tStream ID: {_IdStream}");
            Write($"\tReach ID: {_IdReach}");
            Write($"\tProb Function ID: {_IdDataFunc}");
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
            wr.WriteLine($"\tReach ID: {_IdReach}");
            wr.WriteLine($"\tReach Name: {GlobalVariables.mp_fdaStudy.GetDamageReachList().getName(_IdReach)}");
            wr.WriteLine($"\tData Function ID: {_IdDataFunc}");
            wr.WriteLine($"\tProb Function Name: {GlobalVariables.mp_fdaStudy.GetProbabilityFuncList().GetName(_IdDataFunc)}");
        }
        public void setNextObjectID(long id)
        {
            GlobalVariables.mp_fdaStudy.GetNextIdMgr().setNextObjIdFreqData(id);
        }
        public long getNextObjectID()
        {
            return GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdFreqData();
        }


    }
}
