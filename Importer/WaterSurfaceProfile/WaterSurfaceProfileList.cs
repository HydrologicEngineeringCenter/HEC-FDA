using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

namespace Importer
{
    public class WaterSurfaceProfileList
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private WaterSurfaceProfile _WaterSurfaceProfile;
        private SortedList<string, WaterSurfaceProfile> _WaterSurfaceProfileListSort = new SortedList<string, WaterSurfaceProfile>();
        #endregion
        #region Properties
        public long IdCurrent { get; set; }
        #endregion
        #region Constructors
        public WaterSurfaceProfileList()
        {
        }
        #endregion
        #region Voids
        public void Add(WaterSurfaceProfile theFunc)
        {
            WaterSurfaceProfile aWsp = ObjectCopier.Clone(theFunc);
            _WaterSurfaceProfileListSort.Add(aWsp.Name.Trim(), aWsp);
            WriteLine($"Add Water Surface Profile to SortList.  {aWsp.Name}");
            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aWsp.Print();
        }
        public void Print()
        {
            WaterSurfaceProfile aWsp;
            WriteLine($"Number of Water Surface Profiles {_WaterSurfaceProfileListSort.Count}");
            for (int i = 0; i < _WaterSurfaceProfileListSort.Count; i++)
            {
                aWsp = _WaterSurfaceProfileListSort.ElementAt(i).Value;
                aWsp.Print();
            }
        }
        public void PrintToFile()
        {
            WaterSurfaceProfile aWsp;
            WriteLine($"Number of Water Surface Profiles {_WaterSurfaceProfileListSort.Count}");
            for (int i = 0; i < _WaterSurfaceProfileListSort.Count; i++)
            {
                aWsp = _WaterSurfaceProfileListSort.ElementAt(i).Value;
                aWsp.PrintToFile();
            }
        }
        public void Export(StreamWriter wr, char delimt)
        {
            WaterSurfaceProfile aWsp;

            for (int i = 0; i < _WaterSurfaceProfileListSort.Count; i++)
            {
                aWsp = _WaterSurfaceProfileListSort.ElementAt(i).Value;
                aWsp.Export(wr, delimt);
            }
        }
        #endregion
        #region Functions
        public long getNextId()
        {
            IdCurrent = GlobalVariables.mp_fdaStudy.GetNextIdMgr().getNextObjIdWspData();
            return IdCurrent;
        }
        public long getCurrentId()
        {
            IdCurrent = GlobalVariables.mp_fdaStudy.GetNextIdMgr().getCurrentObjIdWspData();
            return IdCurrent;
        }

        public WaterSurfaceProfile GetWaterSurfaceProfile(string name)
        {
            int ix = _WaterSurfaceProfileListSort.IndexOfKey(name);
            _WaterSurfaceProfile = _WaterSurfaceProfileListSort.ElementAt(ix).Value;
            WriteLine($"Did I find the {name} Water Surface Profile, name = {_WaterSurfaceProfile.Name}");
            return _WaterSurfaceProfile;
        }
        public string GetName(long theId)
        {
            string name = "";
            bool found = false;
            WaterSurfaceProfile aFunc = null;

            for (int i = 0; i < _WaterSurfaceProfileListSort.Count && !found; i++)
            {
                aFunc = _WaterSurfaceProfileListSort.ElementAt(i).Value;
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
