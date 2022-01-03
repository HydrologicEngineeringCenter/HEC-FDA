using System.Collections.Generic;
using System.Text;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
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

        /// <summary>
        /// This indicates if the occtype group has ever been saved before. If false, then
        /// this is a brand new occtype group. It needs to be saved new and not just an update on 
        /// an existing one in the database.
        /// </summary>
       // public bool HasBeenSaved { get; set; }

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
        public SaveAllReportGroupVM  SaveAll()
        {
            //if this group has not saved, then we need to do that first
            //if(!HasBeenSaved)
            //{
            //    //in order to get here, no occtype has been saved with in the group
            //    //a group id needs to be gotten, and then set on each occtype in the occtype list.
            //    List<FdaLogging.LogItem> errors = Saving.PersistenceFactory.GetOccTypeManager().SaveNew(this);

            //}
            SaveAllReportGroupVM saveAllGroup = new SaveAllReportGroupVM(Name);
            //_savedSuccessful.Clear();
            ////_savedUnsuccessful.Clear();
            foreach(IOccupancyTypeEditable otEditable in ModifiedOcctypes)
            {
                bool success = otEditable.SaveWithReturnValue();
                if(!success)
                {
                   // _savedUnsuccessful.Add(otEditable);
                    saveAllGroup.UnsuccessfulList.Add(otEditable.Name);
                }
                else
                {
                    //_savedSuccessful.Add(otEditable);
                    saveAllGroup.SuccessfulList.Add(otEditable.Name);
                }
            }

            return saveAllGroup;
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
