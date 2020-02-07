using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Collections;
using System.IO;

namespace Importer
{
    public class PlanList
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private Plan _Plan;
        private SortedList<string, Plan> _PlanListSort = new SortedList<string, Plan>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public PlanList()
        {
        }
        #endregion
        #region Voids
        public void Add(Plan thePlan)
        {
            Plan aPlan = ObjectCopier.Clone(thePlan);
            _PlanListSort.Add(aPlan.Name.Trim(), aPlan);
            WriteLine($"Add Plan to SortList.  {aPlan.Name}");
            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aPlan.Print();
        }
        public void Print()
        {
            Plan aPlan;
            WriteLine($"Number of Plans {_PlanListSort.Count}");
            for (int i = 0; i < _PlanListSort.Count; i++)
            {
                aPlan = _PlanListSort.ElementAt(i).Value;
                if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aPlan.Print();
            }

            /*
            aPlan = planListSort.ElementAt(2).Value;
            WriteLine($"Try one element at 2, Name: {aPlan.M_name},\tDescription: {aPlan.M_description}");

            foreach (KeyValuePair<string, Plan> element in planListSort)
            {
                string name = element.Key;
                Plan thePlan = (Plan)element.Value;

                WriteLine($"Name: {name}");
                WriteLine($"Plan description {thePlan.M_description}");
            }
            */
        }
        public void PrintToFile()
        {
            Plan aPlan;
            WriteLine($"Number of Plans {_PlanListSort.Count}");
            for (int i = 0; i < _PlanListSort.Count; i++)
            {
                aPlan = _PlanListSort.ElementAt(i).Value;
                aPlan.PrintToFile();
            }
        }
        public void ExportPlans(StreamWriter wr, char delimt)
        {
            Plan aPlan = new Plan();
            for (int i = 0; i < _PlanListSort.Count; i++)
            {
                aPlan = _PlanListSort.ElementAt(i).Value;
                if (i == 0) aPlan.ExportHeader(wr, delimt);
                aPlan.Export(wr, delimt);
            }
        }
        #endregion
        #region Functions
        public Plan GetPlan(string namePlan)
        {
            //Print All the Plans
            WriteLine("\n\n\nList of Plans before trying to find one.");
            Print();

            int ixOfPlan = _PlanListSort.IndexOfKey(namePlan);
            //ixOfPlan = _PlanListSort.IndexOfKey("01FW");

            if (ixOfPlan < 0)
            {
                WriteLine($"Failure to find the plan name: {namePlan}");
            }
            else
            {
                _Plan = _PlanListSort.ElementAt(ixOfPlan).Value;
                WriteLine($"Successfully found the {namePlan} Plan, name = {_Plan.Name}");
            }
            return _Plan;
        }

        //TODO;20Nov2018;Starting to add code to getName getId for configuration items.
        public long GetId(string theName)
        {
            long Id = -1;
            Plan aPlan = null;

            int ix = _PlanListSort.IndexOfKey(theName);

            if (ix > -1)
            {
                aPlan = _PlanListSort.ElementAt(ix).Value;
                Id = aPlan.Id;
            }
            else
            {
                Id = -1;
            }
            return Id;
        }
        public string getName(long theId)
        {
            string name = "";
            bool found = false;
            Plan aPlan = null;

            for(int i = 0; i < _PlanListSort.Count && !found; i++)
            {
                aPlan = _PlanListSort.ElementAt(i).Value;
                if(theId == aPlan.Id)
                {
                    found = true;
                    name = aPlan.Name;
                }
            }
            return name;
        }
        #endregion
    }
}
