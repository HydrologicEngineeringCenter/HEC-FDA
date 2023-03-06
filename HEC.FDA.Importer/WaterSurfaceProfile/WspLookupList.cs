using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.IO;
using System.Data;
using System.Linq;


namespace Importer
{
    public class WspLookupList
    {
        #region Fields
        private WspLookup _WspLookup;
        private List<WspLookup> _WspLookupList = new List<WspLookup>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public WspLookupList()
        {
        }
        #endregion
        #region Voids
        public void Add(WspLookup theLookupFunction)
        {
            WspLookup aLookupFunction = ObjectCopier.Clone(theLookupFunction);
            _WspLookupList.Add(aLookupFunction);
            WriteLine($"Add WspLook Function to List. IdPlan: {aLookupFunction._IdPlan}\tIdYear: {aLookupFunction._IdYear}\tIdStream: {aLookupFunction._IdStream}\tId Data: {aLookupFunction._IdDataFunc}");
            if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aLookupFunction.Print();
        }

        public void setNextObjectID(long id)
        {
            GlobalVariables.mp_fdaStudy.GetNextIdMgr().setNextObjIdWspData(id);
        }
        public void Print()
        {
            WspLookup aFunc;
            WriteLine($"Number of WspLook Records {_WspLookupList.Count}");
            for (int i = 0; i < _WspLookupList.Count; i++)
            {
                aFunc = _WspLookupList.ElementAt(i);
                aFunc.Print();
            }
        }
        public void PrintToFile()
        {
            WspLookup aFunc;
            Study._StreamWriter.WriteLine($"Number of WspLook Records {_WspLookupList.Count}");
            WriteLine($"Number of WspLook Records {_WspLookupList.Count}");
            for (int i = 0; i < _WspLookupList.Count; i++)
            {
                aFunc = _WspLookupList.ElementAt(i);
                aFunc.PrintToFile();
            }

        }
        #endregion
        #region Functions
        public long getNextObjectID()
        {
            return GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdWspData();
        }
        #endregion
        
    }
}
