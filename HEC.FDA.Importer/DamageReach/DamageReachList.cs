using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

namespace Importer
{
    public class DamageReachList
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private DamageReach _Reach;
        private SortedList<string, DamageReach> _ReachListSort = new SortedList<string, DamageReach>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public DamageReachList()
        {
        }
        #endregion
        #region Voids
        public void Add(DamageReach theReach)
        {
            DamageReach aDamageReach = ObjectCopier.Clone(theReach);

            _ReachListSort.Add(aDamageReach.Name.Trim(), aDamageReach);
            WriteLine($"Add Damage Reach to SortList.  {aDamageReach.Name}");
            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aDamageReach.Print();
        }
        public void Print()
        {
            DamageReach aReach;
            WriteLine($"Number of Damage Reaches {_ReachListSort.Count}");
            for (int i = 0; i < _ReachListSort.Count; i++)
            {
                aReach = _ReachListSort.ElementAt(i).Value;
                aReach.Print();
            }
        }
        public void PrintToFile()
        {
            DamageReach aReach;
            WriteLine($"Number of Damage Reaches {_ReachListSort.Count}");
            for (int i = 0; i < _ReachListSort.Count; i++)
            {
                aReach = _ReachListSort.ElementAt(i).Value;
                aReach.PrintToFile();
            }
        }
        public void Export(StreamWriter wr, char delimt)
        {
            DamageReach aReach = new DamageReach();
            for (int i = 0; i < _ReachListSort.Count; i++)
            {
                aReach = _ReachListSort.ElementAt(i).Value;
                if (i == 0) aReach.ExportHeader(wr, delimt);
                aReach.Export(wr, delimt);
            }
        }
        #endregion
        #region Functions
        public long GetId(string name)
        {
            long id = -1L;
            int ix = _ReachListSort.IndexOfKey(name);
            if (ix > -1)
            {
                _Reach = _ReachListSort.ElementAt(ix).Value;
                id = _Reach.Id;
            }
            else
            {
                id = -1L;
            }
            return id;
        }
        public string getName(long id)
        {
            bool found = false;
            string name = "";
            for (int i = 0; i < _ReachListSort.Count && !found; i++)
            {
                _Reach = _ReachListSort.ElementAt(i).Value;
                if (id == _Reach.Id)
                {
                    found = true;
                    name = _Reach.Name;
                }
            }
            return name;
        }

        public DamageReach GetDamageReach(string nameReach)
        {
            int ix = _ReachListSort.IndexOfKey(nameReach);
            _Reach = _ReachListSort.ElementAt(ix).Value;
            WriteLine($"Did I find the {nameReach} Damage Reach, name = {_Reach.Name}");
            return _Reach;
        }
        #endregion
    }
}
