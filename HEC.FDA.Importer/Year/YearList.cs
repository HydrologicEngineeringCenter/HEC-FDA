using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

namespace Importer
{
    public class YearList
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private Year _Year;
        private SortedList<string, Year> _YearListSort = new SortedList<string, Year>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public YearList()
        {
        }
        #endregion
        #region Voids
        public void Add(Year theYear)
        {
            Year aYear = ObjectCopier.Clone(theYear);
            _YearListSort.Add(aYear.Name.Trim(), aYear);
            WriteLine($"Add Year to SortList. {aYear.Name}");
            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aYear.Print();
        }
        public void Print()
        {
            Year aYear;
            WriteLine($"Number of Years {_YearListSort.Count}");
            for (int i = 0; i < _YearListSort.Count; i++)
            {
                aYear = _YearListSort.ElementAt(i).Value;
                aYear.Print();
            }
        }
        public void PrintToFile()
        {
            Year aYear;
            WriteLine($"Number of Years {_YearListSort.Count}");
            for (int i = 0; i < _YearListSort.Count; i++)
            {
                aYear = _YearListSort.ElementAt(i).Value;
                aYear.PrintToFile();
            }
        }
        public void Export(StreamWriter wr, char delimt)
        {
            Year aYear = new Year();
            for (int i = 0; i < _YearListSort.Count; i++)
            {
                aYear = _YearListSort.ElementAt(i).Value;
                if (i == 0) aYear.ExportHeader(wr, delimt);
                aYear.Export(wr, delimt);
            }
        }
        #endregion
        #region Functions
        public long GetId(string name)
        {
            long id = -1L;
            int ix = _YearListSort.IndexOfKey(name);
            if (ix > -1)
            {
                _Year = _YearListSort.ElementAt(ix).Value;
                id = _Year.Id;
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
            for (int i = 0; i < _YearListSort.Count && !found; i++)
            {
                _Year = _YearListSort.ElementAt(i).Value;
                if (id == _Year.Id)
                {
                    found = true;
                    name = _Year.Name;
                }
            }
            return name;
        }

        public Year GetYear(string nameYear)
        {
            int ixOfYear = _YearListSort.IndexOfKey(nameYear);
            _Year = _YearListSort.ElementAt(ixOfYear).Value;
            WriteLine($"Did I find the {nameYear} Year, name = {_Year.Name}");
            return _Year;
        }
        #endregion
    }
}
