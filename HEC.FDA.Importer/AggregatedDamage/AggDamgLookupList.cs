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
    public class AggDamgLookupList
    {
        #region Fields
        private AggDamgLookup _funcLookup;
        private List<AggDamgLookup> _funcLookupList = new List<AggDamgLookup>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public AggDamgLookupList()
        {
        }
        #endregion
        #region Voids
        public void Add(AggDamgLookup theLookupFunction)
        {
            AggDamgLookup aLookupFunction = ObjectCopier.Clone(theLookupFunction);
            _funcLookupList.Add(aLookupFunction);
            WriteLine($"Add AggDamgLookup to List. IdPlan: {aLookupFunction._IdPlan}\tIdYear: {aLookupFunction._IdYear}\tIdStream: {aLookupFunction._IdStream}\tIdReach: {aLookupFunction._IdReach}" +
                $"\tId Category: {aLookupFunction._IdCategory} \tId Data: {aLookupFunction._IdDataFunc}");
            if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aLookupFunction.Print();
        }

        public void setNextObjectID(long id)
        {
            GlobalVariables.mp_fdaStudy.GetNextIdMgr().setNextObjIdAggDfData(id);
        }
        public void Print()
        {
            AggDamgLookup aFunc;
            WriteLine($"Number of AggDamg Lookup Records {_funcLookupList.Count}");
            for (int i = 0; i < _funcLookupList.Count; i++)
            {
                aFunc = _funcLookupList.ElementAt(i);
                aFunc.Print();
            }
        }
        public void PrintToFile()
        {
            AggDamgLookup aFunc;
            Study._StreamWriter.WriteLine($"Number of AggDamg Lookup Records {_funcLookupList.Count}");
            WriteLine($"Number of AggDamg Lookup Records {_funcLookupList.Count}");
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
            return GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdAggDfData();
        }
        #endregion
    }
}
