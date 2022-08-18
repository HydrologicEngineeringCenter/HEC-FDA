using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public interface IOccupancyTypeGroupEditable
    {
        string Name { get; set; }
        List<IOccupancyTypeEditable> Occtypes { get; set; }
        List<IOccupancyTypeEditable> ModifiedOcctypes { get; }
         int ID { get; }
        List<IOccupancyType> CreateOcctypes();
    }
}
