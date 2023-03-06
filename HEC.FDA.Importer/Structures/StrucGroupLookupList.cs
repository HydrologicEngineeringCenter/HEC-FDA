using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.Linq;
using System.IO;

namespace Importer
{
    [Serializable]
    public class StrucGroupLookupList
    {
        #region Fields
        private StrucGroupLookup _funcLookup;
        private List<StrucGroupLookup> _funcLookupList = new List<StrucGroupLookup>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public StrucGroupLookupList()
        {
        }
        #endregion
        #region Voids
        public void Add(StrucGroupLookup theLookupFunction)
        {
            StrucGroupLookup aLookupFunction = ObjectCopier.Clone(theLookupFunction);
            _funcLookupList.Add(aLookupFunction);
            WriteLine($"Add StrucGroupLookup Function to List. IdPlan: {aLookupFunction._IdPlan}\tIdYear: {aLookupFunction._IdYear} \tId Data: {aLookupFunction._IdDataFunc}");
            if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aLookupFunction.Print();
        }

        public void setNextObjectID(long id)
        {
            GlobalVariables.mp_fdaStudy.GetNextIdMgr().setNextObjIdModule(id);
        }
        public void Print()
        {
            StrucGroupLookup aFunc;
            WriteLine($"Number of StrucGroupLookup Records {_funcLookupList.Count}");
            for (int i = 0; i < _funcLookupList.Count; i++)
            {
                aFunc = _funcLookupList.ElementAt(i);
                aFunc.Print();
            }
        }
        public void PrintToFile()
        {
            StrucGroupLookup aFunc;
            Study._StreamWriter.WriteLine($"Number of StrucGroupLookup Records {_funcLookupList.Count}");
            WriteLine($"Number of StrucGroupLookup Records {_funcLookupList.Count}");
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
