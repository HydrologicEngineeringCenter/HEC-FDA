using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.Linq;

namespace Importer
{
    public class EadResultsLookupList
    {
        #region Fields
        private EadResultsLookup _funcLookup;
        private List<EadResultsLookup> _funcLookupList = new List<EadResultsLookup>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public EadResultsLookupList()
        {
        }
        #endregion
        #region Voids
        public void Add(EadResultsLookup theLookupFunction)
        {
            EadResultsLookup aLookupFunction = ObjectCopier.Clone(theLookupFunction);
            _funcLookupList.Add(aLookupFunction);
            WriteLine($"Add EadResultsLookup Function to List. IdPlan: {aLookupFunction._IdPlan}\tIdYear: {aLookupFunction._IdYear}\tIdStream: {aLookupFunction._IdStream}\tIdReach: {aLookupFunction._IdReach}\tId Data: {aLookupFunction._IdDataFunc}");
            if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aLookupFunction.Print();
        }

        public void setNextObjectID(long id)
        {
            GlobalVariables.mp_fdaStudy.GetNextIdMgr().setNextObjIdEadData(id);
        }
        public void Print()
        {
            EadResultsLookup aFunc;
            WriteLine($"Number of EAD Lookup Records {_funcLookupList.Count}");
            for (int i = 0; i < _funcLookupList.Count; i++)
            {
                aFunc = _funcLookupList.ElementAt(i);
                aFunc.Print();
            }
        }
        public void PrintToFile()
        {
            EadResultsLookup aFunc;
            Study._StreamWriter.WriteLine($"Number of EadResultsLookup Records {_funcLookupList.Count}");
            WriteLine($"Number of EadResultsLookup Records {_funcLookupList.Count}");
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
            return GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdEadData();
        }
        #endregion
    }
}
