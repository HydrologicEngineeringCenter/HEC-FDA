using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

namespace Importer
{
    public class LeveeList
    {
        #region Notes
        // Created By: $username$
        // Created Date: $time$
        #endregion
        #region Fields
        private Levee _Levee;
        private SortedList<string, Levee> _LeveeListSort = new SortedList<string, Levee>();
        #endregion
        #region Properties
        public SortedList<string, Levee> Levees
        {
            get { return _LeveeListSort; }
        }
        public long IdCurrent { get; set; }
        #endregion
        #region Constructors
        public LeveeList()
        {
        }
        #endregion
        #region Voids
        public void Add(Levee theLevee)
        {
            Levee aLevee = ObjectCopier.Clone(theLevee);
            WriteLine($"Add Levee to SortList. Name: {aLevee.Name}");
            _LeveeListSort.Add(aLevee.Name.Trim(), aLevee);
            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aLevee.Print();
        }
        public void Print()
        {
            Levee aLevee;
            WriteLine($"Number of Levees {_LeveeListSort.Count}");
            for (int i = 0; i < _LeveeListSort.Count; i++)
            {
                aLevee = _LeveeListSort.ElementAt(i).Value;
                aLevee.Print();
            }
        }
        public void PrintToFile()
        {
            Levee aLevee;
            WriteLine($"Number of Levees {_LeveeListSort.Count}");
            for (int i = 0; i < _LeveeListSort.Count; i++)
            {
                aLevee = _LeveeListSort.ElementAt(i).Value;
                aLevee.PrintToFile();
            }
        }
        public void Export(StreamWriter wr, char delimt)
        {
            Levee aLevee;

            for (int i = 0; i < _LeveeListSort.Count; i++)
            {
                aLevee = _LeveeListSort.ElementAt(i).Value;
                aLevee.Export(wr, delimt);
            }
        }

        #endregion
        #region Functions
        public long getNextId()
        {
            IdCurrent = GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdLevData();
            return IdCurrent;
        }
        public long getCurrentId()
        {
            IdCurrent = GlobalVariables.mp_fdaStudy.GetNextIdMgr().getCurrentObjIdLevData();
            return IdCurrent;
        }

        public Levee GetLevee(string nameLevee)
        {
            int ix = _LeveeListSort.IndexOfKey(nameLevee);
            _Levee = _LeveeListSort.ElementAt(ix).Value;
            WriteLine($"Did I find the {nameLevee} Levee, name = {_Levee.Name}");
            return _Levee;
        }
        public string GetName(long theId)
        {
            string name = "";
            bool found = false;
            Levee aFunc = null;

            for (int i = 0; i < _LeveeListSort.Count && !found; i++)
            {
                aFunc = _LeveeListSort.ElementAt(i).Value;
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
