using System;
using System.Collections.Generic;
using System.Text;

namespace OccupancyTypes
{
    public interface IOccupancyTypeGroup
    {
        List<IOccupancyType> OccupancyTypes { get; set; }
        List<IDamageCategory> DamageCategories { get; set; }
        IOccupancyType GetOcctypeByNameAndDamCat(string name, string damCatName);
        int GetOccTypeIndex(string name);
        List<IOccupancyType> GetOccypesByName(string name);
        string WriteToXml(string outputPath);
    }
}
