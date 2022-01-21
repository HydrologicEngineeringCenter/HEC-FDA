using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

namespace Importer
{
    public class AggregateDamageFunctionList
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private AggregateDamageFunction _AggDamgFunc;
        private SortedList<string, AggregateDamageFunction> _AggDamgFuncListSort = new SortedList<string, AggregateDamageFunction>();
        #endregion
        #region Properties
        public SortedList<string, AggregateDamageFunction> GetAggDamageFunctions
        {
            get { return _AggDamgFuncListSort; }
        }
        public long IdCurrent { get; set; }
        #endregion
        #region Constructors
        public AggregateDamageFunctionList()
        {
        }
        #endregion
        #region Voids
        public void Add(AggregateDamageFunction theAggDamgFunc, AsyncLogger logger)
        {
            AggregateDamageFunction aAggDamgFunc = ObjectCopier.Clone(theAggDamgFunc);
            _AggDamgFuncListSort.Add(aAggDamgFunc.Name, aAggDamgFunc);
            WriteLine($"Add Aggregate Damage Function to SortList.  {aAggDamgFunc.Name}");
            if(logger != null)
            {
                if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aAggDamgFunc.Print(logger);
            }
        }
        public void Print(AsyncLogger logger)
        {
            AggregateDamageFunction aAggDamgFunc;
            logger.Log($"Number of Aggregate Damage Functions {_AggDamgFuncListSort.Count}");
            for (int i = 0; i < _AggDamgFuncListSort.Count; i++)
            {
                aAggDamgFunc = _AggDamgFuncListSort.ElementAt(i).Value;
                aAggDamgFunc.Print(logger);
            }
        }
        public void PrintToFile()
        {
            AggregateDamageFunction aAggDamgFunc;
            WriteLine($"Number of Aggregate Damage Functions {_AggDamgFuncListSort.Count}");
            Study._StreamWriter.WriteLine($"Number of Aggregate Damage Functions {_AggDamgFuncListSort.Count}");
            for (int i = 0; i < _AggDamgFuncListSort.Count; i++)
            {
                aAggDamgFunc = _AggDamgFuncListSort.ElementAt(i).Value;
                aAggDamgFunc.PrintToFile();
            }
        }
        public void Export(StreamWriter wr, char delimt)
        {
            AggregateDamageFunction aAggDamgFunc;

            for (int i = 0; i < _AggDamgFuncListSort.Count; i++)
            {
                aAggDamgFunc = _AggDamgFuncListSort.ElementAt(i).Value;
                aAggDamgFunc.Export(wr, delimt);
            }
        }
        #endregion
        #region Functions
        public long getNextId()
        {
            IdCurrent = GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdAggDfData();
            return IdCurrent;
        }
        public long getCurrentId()
        {
            IdCurrent = GlobalVariables.mp_fdaStudy.GetNextIdMgr().getCurrentObjIdAggDfData();
            return IdCurrent;
        }

        public AggregateDamageFunction GetAggregateDamageFunction(string name)
        {
            int ix = _AggDamgFuncListSort.IndexOfKey(name);
            _AggDamgFunc = _AggDamgFuncListSort.ElementAt(ix).Value;
            WriteLine($"Did I find the {name} Aggregate Damage Function, name = {_AggDamgFunc.Name}");
            return _AggDamgFunc;
        }
        public string GetName(long theId)
        {
            string name = "";
            bool found = false;
            AggregateDamageFunction aFunc = null;

            for (int i = 0; i < _AggDamgFuncListSort.Count && !found; i++)
            {
                aFunc = _AggDamgFuncListSort.ElementAt(i).Value;
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
