using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    class OccupancyTypeGroupEditable : BaseViewModel, IOccupancyTypeGroupEditable
    {
       // private string _Name;
        public List<IOccupancyTypeEditable> Occtypes { get; set; }
        public List<IOccupancyTypeEditable> ModifiedOcctypes
        {
            get
            {
                List<IOccupancyTypeEditable> retval = new List<IOccupancyTypeEditable>();
                foreach (IOccupancyTypeEditable ot in Occtypes)
                {
                    if (ot.IsModified)
                    {
                        retval.Add(ot);
                    }
                }
                return retval;
            }
        }
        //public string Name 
        //{
        //    get { return _Name; }
        //    set { _Name = value; IsModified = true; NotifyPropertyChanged(); } 
        //}

        /// <summary>
        /// Used to find the correct child element in the study cache in order to 
        /// remove it or update it when saving from the occtype editor.
        /// </summary>
        public string OriginalName { get; set; }

        public int ID { get; }

        //we don't actually care if the list of occtypes has changed. We only care if the name has changed
        //because then we need to update the parent table
        public bool IsModified { get; set; }
        public OccupancyTypeGroupEditable(int id, string name, List<IOccupancyTypeEditable> occtypes)
        {
            ID = id;
            Name = name;
            Occtypes = occtypes;
            OriginalName = name;
        }

         private List<IOccupancyTypeEditable> _savedSuccessful = new List<IOccupancyTypeEditable>();
         private List<IOccupancyTypeEditable> _savedUnsuccessful = new List<IOccupancyTypeEditable>();

        //group id to list of occtype name
        //Dictionary<int, List<string>> _savedSuccessful = new Dictionary<int, List<string>>();
       // Dictionary<int, List<string>> _savedUnsuccessful = new Dictionary<int, List<string>>();
        public string PrintUnsuccessfullySavedOcctypes()
        {
            if(_savedUnsuccessful.Count == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder().AppendLine(Name+ ":");
            foreach(IOccupancyTypeEditable ot in _savedUnsuccessful)
            {
                sb.Append("\t-").AppendLine(ot.Name);
            }

            //for(int i = 0;i<150;i++)
            //{
            //    sb.Append("\t-").AppendLine("test dummy name");
            //}

            return sb.ToString();
        }
        public string PrintSuccessfullySavedOcctypes()
        {
            if (_savedSuccessful.Count == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder().AppendLine(Name + ":");
            foreach (IOccupancyTypeEditable ot in _savedSuccessful)
            {
                sb.Append("\t-").AppendLine(ot.Name);
            }
            return sb.ToString();
        }
        public void SaveAll()
        {
            _savedSuccessful.Clear();
            _savedUnsuccessful.Clear();
            foreach(IOccupancyTypeEditable otEditable in ModifiedOcctypes)
            {
                bool success = otEditable.SaveWithReturnValue();
                if(!success)
                {
                    _savedUnsuccessful.Add(otEditable);
                }
                else
                {
                    _savedSuccessful.Add(otEditable);
                }
            }
        }

        //private void AddOcctypeToSuccessfullSaveDictionary(IOccupancyTypeEditable ot)
        //{
        //    if (_savedSuccessful.ContainsKey(ot.GroupID))
        //    {
        //        _savedSuccessful[ot.GroupID].Add(ot.Name);
        //    }
        //    else
        //    {
        //        _savedSuccessful.Add(ot.GroupID, new List<string>() { ot.Name });
        //    }
        //}
        //private void AddOcctypeToUnsuccessfullSaveDictionary(IOccupancyTypeEditable ot)
        //{
        //    if(_savedUnsuccessful.ContainsKey(ot.GroupID))
        //    {
        //        _savedUnsuccessful[ot.GroupID].Add(ot.Name);
        //    }
        //    else
        //    {
        //        _savedUnsuccessful.Add(ot.GroupID, new List<string>() { ot.Name });
        //    }
        //}

    }
}
