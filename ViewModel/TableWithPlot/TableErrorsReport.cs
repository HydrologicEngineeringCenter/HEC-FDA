using HEC.FDA.ViewModel.Inventory.OccupancyTypes;

namespace HEC.FDA.ViewModel.TableWithPlot
{
    public class TableErrorsReport
    {
        public string Errors { get; }
        public string Name { get; }
        public OccupancyTypeEditable OccType { get; }
     
        public TableErrorsReport(OccupancyTypeEditable ot, string errors)
        {
            OccType = ot;
            Name = ot.Name;
            Errors = errors;
        }
    }
}
