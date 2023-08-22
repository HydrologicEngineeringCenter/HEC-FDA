using HEC.FDA.ViewModel.Editors;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    public class OccupancyTypeGroupEditable : NameValidatingVM
    {
        public List<OccupancyTypeEditable> Occtypes { get; set; }
        public int ID { get; set; }
        public List<OccupancyTypeEditable> ModifiedOcctypes
        {
            get
            {
                List<OccupancyTypeEditable> retval = new List<OccupancyTypeEditable>();
                foreach (OccupancyTypeEditable ot in Occtypes)
                {
                    if (ot.HasChanges)
                    {
                        retval.Add(ot);
                    }
                }
                return retval;
            }
        }

        public OccupancyTypeGroupEditable(int id, string name, List<OccupancyTypeEditable> occtypes)
        {
            ID = id;
            Name = name;
            Occtypes = occtypes;
        }

        public List<OccupancyType> CreateOcctypes()
        {
            List<OccupancyType> occtypes = new List<OccupancyType>();
            foreach (OccupancyTypeEditable ot in Occtypes)
            {
                occtypes.Add(ot.CreateOccupancyType());
            }
            return occtypes;
        }

    }
}
