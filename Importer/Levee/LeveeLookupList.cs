using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static System.Console;
using System.Linq;

namespace Importer
{
    [Serializable]
    public class LeveeLookupList
    {
        #region Fields
        private LeveeLookup _funcLookup;
        private List<LeveeLookup> _funcLookupList = new List<LeveeLookup>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public LeveeLookupList()
        {
        }
        #endregion
        #region Voids
        public void Add(LeveeLookup theLookupFunction)
        {
            LeveeLookup aLookupFunction = ObjectCopier.Clone(theLookupFunction);
            _funcLookupList.Add(aLookupFunction);
            WriteLine($"Add LevLook Function to List. IdPlan: {aLookupFunction._IdPlan}\tIdYear: {aLookupFunction._IdYear}\tIdStream: {aLookupFunction._IdStream}\tIdReach: {aLookupFunction._IdReach}\tId Data: {aLookupFunction._IdDataFunc}");
            if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aLookupFunction.Print();
        }

        public void setNextObjectID(long id)
        {
            GlobalVariables.mp_fdaStudy.GetNextIdMgr().setNextObjIdLevData(id);
        }
        public void Print()
        {
            LeveeLookup aFunc;
            WriteLine($"Number of LeveeLook Records {_funcLookupList.Count}");
            for (int i = 0; i < _funcLookupList.Count; i++)
            {
                aFunc = _funcLookupList.ElementAt(i);
                aFunc.Print();
            }
        }
        public void PrintToFile()
        {
            LeveeLookup aFunc;
            Study._StreamWriter.WriteLine($"Number of LeveeLook Records {_funcLookupList.Count}");
            WriteLine($"Number of LeveeLook Records {_funcLookupList.Count}");
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
            return GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdLevData();
        }
        #endregion


    }
}
