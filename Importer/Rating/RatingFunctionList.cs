using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;

namespace Importer
{
    public class RatingFunctionList
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private RatingFunction _RatingFunction;
        private SortedList<string, RatingFunction> _RatingFunctionListSort = new SortedList<string, RatingFunction>();
        #endregion
        #region Properties
        public long IdCurrent { get; set; }
        #endregion
        #region Constructors
        public RatingFunctionList()
        {
        }
        #endregion
        #region Voids
        public void Add(RatingFunction theFunc)
        {
            RatingFunction aRatingFunc = Importer.ObjectCopier.Clone(theFunc);
            _RatingFunctionListSort.Add(aRatingFunc.Name.Trim(), aRatingFunc);
            WriteLine($"Add Rating Function to SortList.  {aRatingFunc.Name}");
            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aRatingFunc.Print();
        }
        public void Print()
        {
            RatingFunction aFunc;
            Write("\n\n\n\n");
            //for (int j = 0; j < 100; j++) Write("-");
            WriteLine($"\nNumber of Rating Functions {_RatingFunctionListSort.Count}");
            for (int i = 0; i < _RatingFunctionListSort.Count; i++)
            {
                aFunc = _RatingFunctionListSort.ElementAt(i).Value;
                aFunc.Print();
            }
        }
        public void PrintToFile()
        {
            RatingFunction aFunc;
            //Write("\n\n\n\n");
            //for (int j = 0; j < 100; j++) Write("-");
            WriteLine($"\nNumber of Rating Functions {_RatingFunctionListSort.Count}");
            for (int i = 0; i < _RatingFunctionListSort.Count; i++)
            {
                aFunc = _RatingFunctionListSort.ElementAt(i).Value;
                aFunc.PrintToFile();
            }
        }
        public void Export(StreamWriter wr, char delimt)
        {
            RatingFunction aFunc;

            for (int i = 0; i < _RatingFunctionListSort.Count; i++)
            {
                aFunc = _RatingFunctionListSort.ElementAt(i).Value;
                aFunc.Export(wr, delimt);
            }
        }
        #endregion
        #region Functions
        public long getNextId()
        {
            IdCurrent = GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdRatData();
            return IdCurrent;
        }
        public long getCurrentId()
        {
            IdCurrent = GlobalVariables.mp_fdaStudy.GetNextIdMgr().getCurrentObjIdRatData();
            return IdCurrent;
        }

        public RatingFunction GetRatingFunction(string nameRateFunc)
        {
            int ix = _RatingFunctionListSort.IndexOfKey(nameRateFunc);
            _RatingFunction = _RatingFunctionListSort.ElementAt(ix).Value;
            WriteLine($"Did I find the {nameRateFunc} Rating Function, name = {_RatingFunction.Name}");
            return _RatingFunction;
        }
        public string GetName(long theId)
        {
            string name = "";
            bool found = false;
            RatingFunction aFunc = null;

            for (int i = 0; i < _RatingFunctionListSort.Count && !found; i++)
            {
                aFunc = _RatingFunctionListSort.ElementAt(i).Value;
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
