using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Linq;


namespace structures
{
    //TODO: Figure out how to set Occupany Type Set
    public class Inventory
    {
        private List<Structure> _structures;
        private OccupancyTypeSet _Occtypes;
        public List<Structure> Structures { get; set; }
        public List<string> ImpactAreas
        {
            get
            {
                List<string> impactAreas = new List<string>();
                foreach (var structure in Structures)
                {
                    if (!impactAreas.Contains(structure.ImpactAreaID.ToString()))
                    {
                        impactAreas.Add(structure.ImpactAreaID.ToString());
                    }
                }
                return impactAreas;
            }
        }

        /// <summary>
        /// Loops through entire inventory and reports back a list of all the unique damage catagories associated with the structures
        /// </summary>
        /// <returns></returns>
        public List<string> GetUniqueDamageCatagories()
        {
            List<string> damageCatagories = new List<string>();
            foreach (Structure structure in Structures)
            {
                if (damageCatagories.Contains(structure.DamageCatagory))
                {
                    continue;
                }
                else
                {
                    damageCatagories.Add(structure.DamageCatagory);
                }
            }
            return damageCatagories;
        }


        /// <summary>
        /// Constructor to create a SI from a shapefile. Gonna need to do this from database potentially as well
        /// </summary>
        /// <param name="pointShapefilePath"></param>
        public Inventory(string pointShapefilePath, string impactAreaShapefilePath)
        {
            PointFeatureLayer structureInventory = new PointFeatureLayer("Structure_Inventory", pointShapefilePath);
            PointMs pointMs = new PointMs(structureInventory.Points().Select(p => p.PointM()));
            _structures = new List<Structure>();
            for (int i = 0; i < structureInventory.FeatureCount(); i++)
            {

                PointM point = pointMs[i];
                var row = structureInventory.FeatureRow(i);
                int fid = (int)row["fd_id"];
                double found_ht = (double)row["found_ht"];
                double val_struct = (double)row["val_struct"];
                double val_cont = (double)row["val_cont"];
                double val_vehic = (double)row["val_vehic"];
                double val_other = (double)row["val_vehic"];
                string st_damcat = (string)row["st_damcat"];
                string occtype = (string)row["occtype"];
                string cbfips = (string)row["cbfips"];
                int impactAreaID = GetImpactAreaID(point, impactAreaShapefilePath);
                _structures.Add(new Structure(fid, point, found_ht, val_struct, val_cont, val_vehic, val_other, st_damcat, occtype, impactAreaID, cbfips));
            }
        }
        // Will need a constructor/load from Database ; 


        public Inventory(List<Structure> structures, OccupancyTypeSet occTypes)
        {
            _structures = structures;
            _Occtypes = occTypes;
        }

        public Inventory GetInventoryTrimmmedToPolygon(Polygon impactArea)
        {
            List<Structure> filteredStructureList = new List<Structure>();

            foreach (Structure structure in _structures)
            {
                if (impactArea.Contains(structure.Point))
                {
                    filteredStructureList.Add(structure);
                }
            }
            return new Inventory(filteredStructureList, _Occtypes);
        }

        public PointMs GetPointMs()
        {
            PointMs points = new PointMs();
            foreach (Structure structure in _structures)
            {
                points.Add(structure.Point);
            }
            return points;
        }

        private int GetImpactAreaID(PointM point, string polygonShapefilePath)
        {
            PolygonFeatureLayer polygonFeatureLayer = new PolygonFeatureLayer("impactAreas", polygonShapefilePath);
            List<Polygon> polygons = polygonFeatureLayer.Polygons().ToList();
            var polygonsList = polygons.ToList();
            for (int i = 0; i < polygonsList.Count; i++)
            {
                if (polygons[i].Contains(point))
                {
                    var row = polygonFeatureLayer.FeatureRow(i);
                    return (int)row["FID"];
                }
            }
            return -9999;
        }

        public DeterministicInventory Sample(int seed)
        {
            Random random = new Random(seed);

            List<DeterministicOccupancyType> _OcctypesSample = _Occtypes.Sample(random.Next());
            List<DeterministicStructure> inventorySample = new List<DeterministicStructure>();
            foreach (Structure structure in _structures)
            {
                foreach (DeterministicOccupancyType deterministicOccupancyType in _OcctypesSample)
                {
                    if (structure.DamageCatagory.Equals(deterministicOccupancyType.DamageCatagory))
                    {
                        if (structure.OccTypeName.Equals(deterministicOccupancyType.Name))
                        {
                            inventorySample.Add(structure.Sample(random.Next(), deterministicOccupancyType));
                            break;
                        }
                    }
                }
                //it is possible that if an occupancy type doesnt exist a structure wont get added...
            }
            return new DeterministicInventory(inventorySample);
        }
    }
}
