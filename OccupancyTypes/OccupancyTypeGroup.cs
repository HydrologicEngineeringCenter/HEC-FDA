using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace OccupancyTypes
{
    internal class OccupancyTypeGroup :IOccupancyTypeGroup
    {
        public List<IOccupancyType> OccupancyTypes { get; set; }
        public List<IDamageCategory> DamageCategories { get; set; }

       internal OccupancyTypeGroup(List<IOccupancyType> occtypes)
        {
            OccupancyTypes = occtypes;
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

        public string WriteToXml(string outputPath)
        {
            throw new NotImplementedException();
        }
    }
}
