using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;

namespace DbMemory
{
    public class StructureList
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private Structure _Structure;
        private SortedList<string, Structure> _StructureListSort = new SortedList<string, Structure>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public StructureList()
        {
        }
        #endregion
        #region Voids
        public void Add(Structure theStructure)
        {
            Structure aStruct = ObjectCopier.Clone(theStructure);

            _StructureListSort.Add(aStruct.Name.Trim(), aStruct);
            WriteLine($"Add Structure to SortList.  {aStruct.Name}");
            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aStruct.Print();
        }
        public void Print()
        {
            Structure aStructure;
            WriteLine($"Number of Structures {_StructureListSort.Count}");
            for (int i = 0; i < _StructureListSort.Count; i++)
            {
                aStructure = _StructureListSort.ElementAt(i).Value;
                aStructure.Print();
            }
        }
        public void PrintToFile()
        {
            Structure aStructure;
            WriteLine($"Number of Structures {_StructureListSort.Count}");
            for (int i = 0; i < _StructureListSort.Count; i++)
            {
                aStructure = _StructureListSort.ElementAt(i).Value;
                aStructure.PrintToFile();
            }
        }
        public void Export(StreamWriter wr, char delimt)
        {
            Structure aStructure = new Structure();
            for (int i = 0; i < _StructureListSort.Count; i++)
            {
                aStructure = _StructureListSort.ElementAt(i).Value;
                if (i == 0) aStructure.ExportHeader(wr, delimt);
                aStructure.Export(wr, delimt);
            }



            // Test Query
            #region TestQueryStructInventory
            /*
            //rdc temp;rdc critical;Test LINQ for filtering and sorting
            //----------------------------------------------------------------------------
            var queryAllStructures = from aStrucApt in _StructureListSort

                                         where aStrucApt.Value.CategoryName == "COMM" ||
                                         aStrucApt.Value.CategoryName == "AUTO" ||
                                         aStrucApt.Value.CategoryName == "RES"

                                         orderby aStrucApt.Value.CategoryName ascending , 
                                         aStrucApt.Value.DamageFunctionName ascending,
                                         aStrucApt.Value.Name ascending

                                         select aStrucApt;

            int icount = 0;
            wr.WriteLine("\n\nIndex\tStructure Name\tCategory\tOccupancy Type");
            foreach ( var aStrucApt in queryAllStructures)
            {
                icount++;
                string icountStr = Convert.ToString(icount);
                icountStr = icountStr.PadLeft(5, ' ');
                Console.WriteLine($"{ icountStr}" +"\t" +
                                  $"Structure Name {aStrucApt.Value.Name.PadRight(32,' ')}" +
                                  $"\tCategory: {aStrucApt.Value.CategoryName.PadRight(18,' ')}" +
                                  $"\tOccupancy Type: {aStrucApt.Value.DamageFunctionName}");
                wr.WriteLine(   $"{ icountStr}" + 
                                $"\t{aStrucApt.Value.Name.PadRight(32, ' ')}" + 
                                $"\t{aStrucApt.Value.CategoryName.PadRight(18, ' ')}" +
                                $"\t{aStrucApt.Value.DamageFunctionName}"
                    );
            }
            //Export all structures with ordering
            //----------------------------------------------------------------------------
            queryAllStructures = from aStrucApt in _StructureListSort
                                 orderby aStrucApt.Value.StreamName ascending,
                                 aStrucApt.Value.StationAtStructure,
                                 aStrucApt.Value.BankOfStream,
                                 aStrucApt.Value.CategoryName,
                                 aStrucApt.Value.DamageFunctionName,
                                 aStrucApt.Value.Name
                                 select aStrucApt;
            //Process all structures
            icount = 0;
            foreach(var aStrucApt in queryAllStructures)
            {
                icount++;
                if(icount == 1)
                {
                    aStrucApt.Value.ExportHeader(wr, delimt);
                } 
                aStrucApt.Value.Export(wr, delimt);
            }
            */
            #endregion
        }
        #endregion
        #region Functions
        public Structure GetStructure(string name)
        {
            int ixOfStruc = _StructureListSort.IndexOfKey(name);
            _Structure = _StructureListSort.ElementAt(ixOfStruc).Value;
            WriteLine($"Did I find the {name} Structure, name = {_Structure.Name}");
            return _Structure;
        }
        #endregion
    }
}
