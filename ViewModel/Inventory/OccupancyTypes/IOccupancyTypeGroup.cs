using HEC.FDA.ViewModel.Inventory.DamageCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public interface IOccupancyTypeGroup
    {
        string Name { get; set; }
        int ID { get; set; }
        List<IOccupancyType> OccupancyTypes { get; set; }
        List<IDamageCategory> DamageCategories { get; set; }
        IOccupancyType GetOcctypeByNameAndDamCat(string name, string damCatName);
        int GetOccTypeIndex(string name);
        List<IOccupancyType> GetOccypesByName(string name);
        void LoadFromFile(string inputFile);
        string WriteToXml(string outputPath);

    }
}
