using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Console;
using System.IO;

namespace DbMemory
{
    public class EadResultList
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2018
        #endregion
        #region Fields
        private EadResult _EadResult;
        private SortedList<string, EadResult> _EadResultListSort = new SortedList<string, EadResult>();
        #endregion
        #region Properties
        public long IdCurrent { get; set; }
        #endregion
        #region Constructors
        public EadResultList()
        {
        }
        #endregion
        #region Voids
        public void Add(EadResult theEadResult)
        {
            EadResult aEadResult = ObjectCopier.Clone(theEadResult);
            _EadResultListSort.Add(aEadResult.Name, aEadResult);
            WriteLine($"Add Ead Result to SortList.  {aEadResult.Name}");
            if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aEadResult.Print();
        }
        public void Print()
        {
            EadResult aEadResult;
            WriteLine($"\nNumber of Ead Results {_EadResultListSort.Count}");
            for (int i = 0; i < _EadResultListSort.Count; i++)
            {
                aEadResult = _EadResultListSort.ElementAt(i).Value;
                aEadResult.Print();
            }
        }
        public void PrintToFile()
        {
            EadResult aEadResult;
            WriteLine($"\nNumber of Ead Results {_EadResultListSort.Count}");
            for (int i = 0; i < _EadResultListSort.Count; i++)
            {
                aEadResult = _EadResultListSort.ElementAt(i).Value;
                aEadResult.PrintToFile();
            }
        }
        public void Export(StreamWriter wr, char delimt)
        {
            EadResult aEadResult;
            WriteLine("\n\nNo Export available for EAD Results!------------------------------------");
        }
        #endregion
        #region Functions
        public long getNextId()
        {
            IdCurrent = GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdEadData();
            return IdCurrent;
        }
        public long getCurrentId()
        {
            IdCurrent = GlobalVariables.mp_fdaStudy.GetNextIdMgr().getCurrentObjIdEadData();
            return IdCurrent;
        }
        public EadResult GetEadResult(string name)
        {
            int ix = _EadResultListSort.IndexOfKey(name);
            if (ix > -1)
            {
                _EadResult = _EadResultListSort.ElementAt(ix).Value;
                WriteLine($"Ead Result found for {name}.");
            }
            else
            {
                _EadResult = new EadResult();
                WriteLine($"Failure to find Ead Result for {name}.");
            }
            return _EadResult;
        }
        public string GetName(long theId)
        {
            string name = "";
            bool found = false;
            EadResult aFunc = null;

            for (int i = 0; i < _EadResultListSort.Count && !found; i++)
            {
                aFunc = _EadResultListSort.ElementAt(i).Value;
                if (theId == aFunc.Id)
                {
                    found = true;
                    name = aFunc.Name;
                }
            }
            return name;

            #endregion

        }
    }
}
