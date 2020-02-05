using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

namespace DbMemory
{
    public class ProbabilityFunctionList
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private ProbabilityFunction _ProbabilityFunction;
        private SortedList<string, ProbabilityFunction> _ProbabilityFunctionListSort = new SortedList<string, ProbabilityFunction>();
        #endregion
        #region Properties
        public long IdCurrent { get; set; }
        #endregion
        #region Constructors
        public ProbabilityFunctionList()
        {
        }
        #endregion
        #region Voids
        public void Add(ProbabilityFunction theProbFunc)
        {
            ProbabilityFunction aProbFunc = ObjectCopier.Clone(theProbFunc);
            _ProbabilityFunctionListSort.Add(aProbFunc.Name.Trim(), aProbFunc);
            WriteLine($"Add Probability Function to SortList.  {aProbFunc.Name}");
            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aProbFunc.Print();
        }
        public void Print()
        {
            ProbabilityFunction aProbFunc;
            WriteLine($"Number of Probability Functions {_ProbabilityFunctionListSort.Count}");
            for (int i = 0; i < _ProbabilityFunctionListSort.Count; i++)
            {
                aProbFunc = _ProbabilityFunctionListSort.ElementAt(i).Value;
                aProbFunc.Print();
            }
        }
        public void PrintToFile()
        {
            ProbabilityFunction aProbFunc;
            WriteLine($"Number of Probability Functions {_ProbabilityFunctionListSort.Count}");
            for (int i = 0; i < _ProbabilityFunctionListSort.Count; i++)
            {
                aProbFunc = _ProbabilityFunctionListSort.ElementAt(i).Value;
                aProbFunc.PrintToFile();
            }
        }
        public void Export(StreamWriter wr, char delimt)
        {
            ProbabilityFunction aProbFunc;

            for (int i = 0; i < _ProbabilityFunctionListSort.Count; i++)
            {
                aProbFunc = _ProbabilityFunctionListSort.ElementAt(i).Value;
                aProbFunc.Export(wr, delimt);
            }
        }
        #endregion
        #region Functions
        public long getNextId()
        {
            IdCurrent = GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdFreqData();
            return IdCurrent;
        }
        public long getCurrentId()
        {
            IdCurrent = GlobalVariables.mp_fdaStudy.GetNextIdMgr().getCurrentObjIdFreqData();
            return IdCurrent;
        }

        public ProbabilityFunction GetProbabilityFunction(string nameFreqFunc)
        {
            int ix = _ProbabilityFunctionListSort.IndexOfKey(nameFreqFunc);
            _ProbabilityFunction = _ProbabilityFunctionListSort.ElementAt(ix).Value;
            WriteLine($"Did I find the {nameFreqFunc} Probability Function, name = {_ProbabilityFunction.Name}");
            return _ProbabilityFunction;
        }
        public string GetName(long theId)
        {
            string name = "";
            bool found = false;
            ProbabilityFunction aFunc = null;

            for (int i = 0; i < _ProbabilityFunctionListSort.Count && !found; i++)
            {
                aFunc = _ProbabilityFunctionListSort.ElementAt(i).Value;
                if (theId == aFunc.Id)
                {
                    found = true;
                    name = aFunc.Name;
                }
            }
            return name;
        }

        #endregion
    }
}
