using System.Collections.Generic;
using System.Data;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class SaveAllReportVM:BaseViewModel
    {
        public List<SaveAllReportGroupVM> Groups { get; }
        public SaveAllReportVM(List<SaveAllReportGroupVM> groups)
        {
            Groups = groups;
        }

        /// <summary>
        /// The user wanted to save the occtypes that have warnings.
        /// </summary>
        public void SaveOcctypes()
        {
            foreach(SaveAllReportGroupVM group in Groups)
            {
                group.SaveOcctypes();
            }
        }

        
    }
}
