using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static System.Console;
using System.Data;
using System.Linq;

namespace Importer
{
    [Serializable]
    public class RateLookupList
    {
        #region Fields
        private RateLookup _funcLookup;
        private List<RateLookup> _funcLookupList = new List<RateLookup>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public RateLookupList()
        {
        }
        #endregion
        #region Voids
        public void Add(RateLookup theLookupFunction)
        {
            RateLookup aLookupFunction = ObjectCopier.Clone(theLookupFunction);
            _funcLookupList.Add(aLookupFunction);
            WriteLine($"Add RateLook Function to List. IdPlan: {aLookupFunction._IdPlan}\tIdYear: {aLookupFunction._IdYear}\tIdStream: {aLookupFunction._IdStream}\tIdReach: {aLookupFunction._IdReach}\tId Data: {aLookupFunction._IdDataFunc}");
            if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aLookupFunction.Print();
        }

        public void setNextObjectID(long id)
        {
            GlobalVariables.mp_fdaStudy.GetNextIdMgr().setNextObjIdRatData(id);
        }
        public void Print()
        {
            RateLookup aFunc;
            WriteLine($"Number of RateLook Records {_funcLookupList.Count}");
            for (int i = 0; i < _funcLookupList.Count; i++)
            {
                aFunc = _funcLookupList.ElementAt(i);
                aFunc.Print();
            }
        }
        public void PrintToFile()
        {
            RateLookup aFunc;
            Study._StreamWriter.WriteLine($"Number of RateLook Records {_funcLookupList.Count}");
           WriteLine($"Number of RateLook Records {_funcLookupList.Count}");
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
            return GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdRatData();
        }
        #endregion


    }
}
