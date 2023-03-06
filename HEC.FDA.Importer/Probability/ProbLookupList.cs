using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static System.Console;
using System.Data;
using System.Linq;

namespace Importer
{
    public class ProbLookupList
    {
        #region Fields
        private ProbLookup _funcLookup;
        private List<ProbLookup> _funcLookupList = new List<ProbLookup>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public ProbLookupList()
        {
        }
        #endregion
        #region Voids
        public void Add(ProbLookup theLookupFunction)
        {
            ProbLookup aLookupFunction = ObjectCopier.Clone(theLookupFunction);
            _funcLookupList.Add(aLookupFunction);
            WriteLine($"Add ProbLook Function to List. IdPlan: {aLookupFunction._IdPlan}\tIdYear: {aLookupFunction._IdYear}\tIdStream: {aLookupFunction._IdStream}\tIdReach: {aLookupFunction._IdReach}\tId Data: {aLookupFunction._IdDataFunc}");
            if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aLookupFunction.Print();
        }

        public void setNextObjectID(long id)
        {
            GlobalVariables.mp_fdaStudy.GetNextIdMgr().setNextObjIdFreqData(id);
        }
        public void Print()
        {
            ProbLookup aFunc;
            WriteLine($"Number of ProbLook Records {_funcLookupList.Count}");
            for (int i = 0; i < _funcLookupList.Count; i++)
            {
                aFunc = _funcLookupList.ElementAt(i);
                aFunc.Print();
            }
        }
        public void PrintToFile()
        {
            ProbLookup aFunc;
            Study._StreamWriter.WriteLine($"Number of ProbLookup Records {_funcLookupList.Count}");
            WriteLine($"Number of ProbLookup Records {_funcLookupList.Count}");
            for (int i = 0; i < _funcLookupList.Count; i++)
            {
                aFunc = _funcLookupList.ElementAt(i);
                aFunc.PrintToFile();
            }
        }
        #endregion
        #region Functions
        public long getNextObjectID()
        {
            return GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdFreqData();
        }
        #endregion

    }
}
