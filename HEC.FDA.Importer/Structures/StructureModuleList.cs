using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

namespace Importer
{
    public class StructureModuleList
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private StructureModule _Module;
        private SortedList<string, StructureModule> _ModuleListSort = new SortedList<string, StructureModule>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public StructureModuleList()
        {
        }
        #endregion
        #region Voids
        public void Add(StructureModule module)
        {
            _Module = ObjectCopier.Clone(module);

            _ModuleListSort.Add(this._Module.Name.Trim(), this._Module);
            WriteLine($"Add Structure Module to SortList.  {_Module.Name}");
            if (GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) this._Module.Print();
        }
        public void Print()
        {
            WriteLine($"Number of Structure Modules {_ModuleListSort.Count}");
            for (int i = 0; i < _ModuleListSort.Count; i++)
            {
                this._Module = _ModuleListSort.ElementAt(i).Value;
                this._Module.Print();
            }
        }
        public void PrintToFile()
        {
            WriteLine($"Number of Structure Modules {_ModuleListSort.Count}");
            for (int i = 0; i < _ModuleListSort.Count; i++)
            {
                this._Module = _ModuleListSort.ElementAt(i).Value;
                this._Module.PrintToFile();
            }
        }
        public void Export(StreamWriter wr, char delimt)
        {
            StructureModule aModule = new StructureModule();
            for (int i = 0; i < _ModuleListSort.Count; i++)
            {
                aModule = _ModuleListSort.ElementAt(i).Value;
                if (i == 0) aModule.ExportHeader(wr, delimt);
                aModule.Export(wr, delimt);
            }
        }
        #endregion
        #region Functions
        public long getId(string name)
        {
            long id = -1L;
            int ix = _ModuleListSort.IndexOfKey(name);
            if (ix > -1)
            {
                StructureModule theFunc = _ModuleListSort.ElementAt(ix).Value;
                id = theFunc.Id;
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
            for (int i = 0; i < _ModuleListSort.Count && !found; i++)
            {
                StructureModule theFunc = _ModuleListSort.ElementAt(i).Value;
                if (id == theFunc.Id)
                {
                    found = true;
                    name = theFunc.Name;
                }
            }
            return name;
        }

        public StructureModule Get(string nameModule)
        {
            int ix = _ModuleListSort.IndexOfKey(nameModule);
            this._Module = _ModuleListSort.ElementAt(ix).Value;
            WriteLine($"Did I find the {nameModule} Structure Module, name = {this._Module.Name}");
            return this._Module;
        }
        public string GetName(long theId)
        {
            string name = "";
            bool found = false;
            StructureModule aFunc = null;

            for (int i = 0; i < _ModuleListSort.Count && !found; i++)
            {
                aFunc = _ModuleListSort.ElementAt(i).Value;
                if (theId == aFunc.Id)
                {
                    found = true;
                    name = aFunc.Name;
                }
            }
            return name;

            #endregion
        }
    }
}
