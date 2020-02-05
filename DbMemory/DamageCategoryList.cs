using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

namespace DbMemory
{
    public class DamageCategoryList
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private DamageCategory _Category;
        private SortedList<string, DamageCategory> _CategoryListSort = new SortedList<string, DamageCategory>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public DamageCategoryList() 
        {
        }
        #endregion
        #region Voids
        public void Add(DamageCategory theCategory)
        {
            DamageCategory aDamageCategory = ObjectCopier.Clone(theCategory);
            _CategoryListSort.Add(aDamageCategory.Name.Trim(), aDamageCategory);
            WriteLine($"Add Damage Category to SortList.  {aDamageCategory.Name}");
            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aDamageCategory.Print();
        }
        public void Print()
        {
            DamageCategory aCategory;
            WriteLine($"Number of Damage Categories {_CategoryListSort.Count}");
            for (int i = 0; i < _CategoryListSort.Count; i++)
            {
                aCategory = _CategoryListSort.ElementAt(i).Value;
                aCategory.Print();
            }
        }
        public void PrintToFile()
        {
            DamageCategory aCategory;
            WriteLine($"Number of Damage Categories {_CategoryListSort.Count}");
            for (int i = 0; i < _CategoryListSort.Count; i++)
            {
                aCategory = _CategoryListSort.ElementAt(i).Value;
                aCategory.PrintToFile();
            }
        }
        public void Export(StreamWriter wr, char delimt)
        {
            DamageCategory aCategory = new DamageCategory();
            for (int i = 0; i < _CategoryListSort.Count; i++)
            {
                aCategory = _CategoryListSort.ElementAt(i).Value;
                if (i == 0) aCategory.ExportHeader(wr, delimt);
                aCategory.Export(wr, delimt);
            }
        }
        #endregion
        #region Functions
        public long GetId(string name)
        {
            long id = -1L;
            int ix = _CategoryListSort.IndexOfKey(name);
            if (ix > -1)
            {
                _Category = _CategoryListSort.ElementAt(ix).Value;
                id = _Category.Id;
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
            for(int i = 0; i < _CategoryListSort.Count && !found; i++)
            {
                _Category = _CategoryListSort.ElementAt(i).Value;
                if (id == _Category.Id)
                {
                    found = true;
                    name = _Category.Name;
                }
            }
            return name;
        }
        public DamageCategory GetDamageCategory(string nameCategory)
        {
            int ix = _CategoryListSort.IndexOfKey(nameCategory);
            _Category = _CategoryListSort.ElementAt(ix).Value;
            WriteLine($"Did I find the {nameCategory} Damage Category, name = {_Category.Name}");
            return _Category;
        }
        #endregion
    }
}
