using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Inventory.OccupancyTypes
{
    public interface IOccupancyTypeGroupEditable
    {
        string Name { get; set; }
        /// <summary>
        /// Used to find the correct child element in the study cache in order to 
        /// remove it or update it when saving from the occtype editor.
        /// </summary>
        string OriginalName { get; set; }
        List<IOccupancyTypeEditable> Occtypes { get; set; }

        bool IsModified { get; set; }
        List<IOccupancyTypeEditable> ModifiedOcctypes { get; }
         int ID { get; }
        SaveAllReportGroupVM SaveAll();
        string PrintUnsuccessfullySavedOcctypes();
        string PrintSuccessfullySavedOcctypes();
    }
}
