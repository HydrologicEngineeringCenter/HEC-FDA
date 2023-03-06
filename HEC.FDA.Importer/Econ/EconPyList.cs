using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Console;
using System.IO;

namespace Importer
{
    public class EconPYList
    {
        #region Fields
        private EconPY _EconPY;
        private List<EconPY> _EconPyList = new List<EconPY>();
        #endregion
        #region Properties

        #endregion
        #region Constructors
        public EconPYList()
        { }
        #endregion
        #region Voids
        public void Add(EconPY theEconPY)
        {
            EconPY aEconPY = ObjectCopier.Clone(theEconPY);
            _EconPyList.Add(aEconPY);
            WriteLine($"Add EconPY to SortList.  IdPlan {aEconPY.IdPlan} \tIdYear: {aEconPY.IdYear}");
            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aEconPY.Print();
        }
        public void Print()
        {
            EconPY aEconPY;
            WriteLine($"\nNumber of Economic Play/Year Parameters: {_EconPyList.Count}");
            for(int i = 0; i < _EconPyList.Count;i++)
            {
                aEconPY = _EconPyList.ElementAt(i);
                aEconPY.Print();
            }
        }
        public void PrintToFile()
        {
            EconPY aEconPY;
            WriteLine($"\nNumber of Economic Play/Year Parameters: {_EconPyList.Count}");
            for (int i = 0; i < _EconPyList.Count; i++)
            {
                aEconPY = _EconPyList.ElementAt(i);
                aEconPY.PrintToFile();
            }
        }
        #endregion
        #region Functions
        public EconPY GetEconPY(long idPlan, long idYear)
        {
            EconPY aEconPY = new EconPY();
            bool found = false;

            for(int i = 0; i < _EconPyList.Count && !found; i++)
            {
                aEconPY = _EconPyList.ElementAt(i);
                if(aEconPY.IdPlan == idPlan && aEconPY.IdYear == idYear)
                {
                    found = true;
                }
            }
            return aEconPY;
        }
        #endregion
    }
}
