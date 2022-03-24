using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class OcctypeErrorsReportVM:BaseViewModel
    {
        public List<SaveAllReportGroupVM> Groups { get; }
        public OcctypeErrorsReportVM(List<SaveAllReportGroupVM> groups)
        {
            Groups = groups;
        }
    }
}
