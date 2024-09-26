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
        public SortedList<string, RatingFunction> RatingFunctions
        {
            get { return _RatingFunctionListSort; }
        }
        public long IdCurrent { get; set; }
        #endregion
        #region Constructors
        public RatingFunctionList()
        {
        }
        #endregion
        #region Voids
        public void Add(RatingFunction theFunc, AsyncLogger logger)
        {
            RatingFunction aRatingFunc = RatingFunction.Clone(theFunc);
            _RatingFunctionListSort.Add(aRatingFunc.Name.Trim(), aRatingFunc);
            WriteLine($"Add Rating Function to SortList.  {aRatingFunc.Name}");
            if(logger != null)
            {
                if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19)
                {
                    aRatingFunc.Print(logger);
                }
            }
        }
        public void Print(AsyncLogger logger)
        {

            RatingFunction aFunc;
            Write("\n\n\n\n");
            //for (int j = 0; j < 100; j++) Write("-");
            logger.Log($"\nNumber of Rating Functions ", _RatingFunctionListSort.Count.ToString());
            for (int i = 0; i < _RatingFunctionListSort.Count; i++)
            {
                aFunc = _RatingFunctionListSort.ElementAt(i).Value;
                aFunc.Print(logger);
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
