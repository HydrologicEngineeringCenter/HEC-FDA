using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Inventory.DamageCategory;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    internal class OccupancyTypeGroup : IOccupancyTypeGroup
    {
        public List<IOccupancyType> OccupancyTypes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<IDamageCategory> DamageCategories { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        public OccupancyTypeGroup(string selectedPath)
        {

        }

        public IOccupancyType GetOcctypeByNameAndDamCat(string name, string damCatName)
        {
            throw new NotImplementedException();
        }

        public int GetOccTypeIndex(string name)
        {
            throw new NotImplementedException();
        }

        public List<IOccupancyType> GetOccypesByName(string name)
        {
            throw new NotImplementedException();
        }

        public void LoadFromFile(string inputFile)
        {
            throw new NotImplementedException();
        }

        public string WriteToXml(string outputPath)
        {
            throw new NotImplementedException();
        }
    }
}
