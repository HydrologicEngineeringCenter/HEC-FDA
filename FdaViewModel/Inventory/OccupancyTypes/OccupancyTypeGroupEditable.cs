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

    }
}
