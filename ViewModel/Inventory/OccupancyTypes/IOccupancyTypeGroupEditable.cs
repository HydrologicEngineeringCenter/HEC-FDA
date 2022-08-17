using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public interface IOccupancyTypeGroupEditable
    {
        string Name { get; set; }
        List<IOccupancyTypeEditable> Occtypes { get; set; }
        bool IsModified { get; set; }
        List<IOccupancyTypeEditable> ModifiedOcctypes { get; }
         int ID { get; }
        SaveAllReportGroupVM SaveAll();
        List<IOccupancyType> CreateOcctypes();
    }
}
