using HEC.FDA.ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.ViewModel.TableWithPlot
{
    public class TableErrorsReport
    {
        public string Errors { get; }
        public string Name { get; }
        public IOccupancyTypeEditable OccType { get; }
     
        public TableErrorsReport(IOccupancyTypeEditable ot, string errors)
        {
            OccType = ot;
            Name = ot.Name;
            Errors = errors;
        }
    }
}
