using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

namespace Importer
{
    public class OccupancyTypeList
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private OccupancyType _OccupancyType;
        private SortedList<string, OccupancyType> _OcctypeListSort = new SortedList<string, OccupancyType>();
        #endregion
        #region Properties
        public List<OccupancyType> Occtypes
        {
            get
            {
                List<OccupancyType> occtypes = new List<OccupancyType>();
                OccupancyType aOcctype;
                for (int i = 0; i < _OcctypeListSort.Count; i++)
                {
                    occtypes.Add( _OcctypeListSort.ElementAt(i).Value);
                    
                }
                return occtypes;
            }
        }

        #endregion
        #region Constructors
        public OccupancyTypeList()
        {
        }
        #endregion
        #region Voids
        public void Add(OccupancyType theOcctype, AsyncLogger logger)
        {
            OccupancyType aOccType = ObjectCopier.Clone(theOcctype);
            _OcctypeListSort.Add(aOccType.Name.Trim(), aOccType);
            WriteLine($"Add Occupancy Type to SortList. {aOccType.Name}");
            if (logger != null && GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19)
            {
                aOccType.Print(logger);
            }
        }
        public void Print(AsyncLogger logger)
        {
            OccupancyType aOcctype;
            logger.Log($"Number of Occupancy Types ", _OcctypeListSort.Count.ToString());
            for (int i = 0; i < _OcctypeListSort.Count; i++)
            {
                aOcctype = _OcctypeListSort.ElementAt(i).Value;
                aOcctype.Print(logger);
            }
        }
        public void PrintToTable()
        {
            OccupancyType aOcctype;
            WriteLine($"Number of Occupancy Types {_OcctypeListSort.Count}");
            for (int i = 0; i < _OcctypeListSort.Count; i++)
            {
                aOcctype = _OcctypeListSort.ElementAt(i).Value;
                aOcctype.PrintToFile();
            }
        }
        public void Export(StreamWriter wr, char delimt)
        {
            OccupancyType aOccType = new OccupancyType();

            for (int i = 0; i < _OcctypeListSort.Count; i++)
            {
                aOccType = _OcctypeListSort.ElementAt(i).Value;
                if (i == 0) aOccType.ExportHeader(wr, delimt);
                aOccType.Export(wr, delimt);
            }
        }
        #endregion
        #region Functions
        public OccupancyType GetOccupancyType(string nameOcctype)
        {
            int ix = _OcctypeListSort.IndexOfKey(nameOcctype);
            _OccupancyType = _OcctypeListSort.ElementAt(ix).Value;
            WriteLine($"Did I find the {nameOcctype} Occupancy Type, name = {_OccupancyType.Name}");
            return _OccupancyType;
        }
        #endregion
    }
}
