using fda_model.structures;
using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Linq;


namespace structures
{
    //TODO: Figure out how to set Occupany Type Set
    public class Inventory
    {
        private List<OccupancyType> _Occtypes;
        private List<string> _damageCategories;
        private List<int> _impactAreaIDs; 
        public List<Structure> Structures { get; }
        public List<int> ImpactAreas
        {
            get { return _impactAreaIDs; }
        }
        public List<string> DamageCategories
        {
            get { return _damageCategories; }
        }

        public static T TryGet<T>(object value, T defaultValue = default)
            where T : struct
        {
            if (value == null)
                return defaultValue;
            else if (value == DBNull.Value)
                return defaultValue;
            else
            {
                var retn = value as T?;
                if (retn.HasValue)
                    return retn.Value;
                else
                    return defaultValue;
            }
        }
        public static T TryGetObj<T>(object value, T defaultValue = default)
            where T : class
        {
            if (value == null)
                return defaultValue;
            else if (value == DBNull.Value)
                return defaultValue;
            else
            {
                var retn = value as T;
                if (retn != null)
                    return retn;
                else
                    return defaultValue;
            }
        }


        public Inventory(string pointShapefilePath, string impactAreaShapefilePath, StructureInventoryColumnMap map, List<OccupancyType> occTypes)
        {
            PointFeatureLayer structureInventory = new PointFeatureLayer("Structure_Inventory", pointShapefilePath);
            PointMs pointMs = new PointMs(structureInventory.Points().Select(p => p.PointM()));
            Structures = new List<Structure>();
            try
            {
                for (int i = 0; i < structureInventory.FeatureCount(); i++)
                {
                    PointM point = pointMs[i];
                    var row = structureInventory.FeatureRow(i);
                    int fid = TryGet<int>(row[map.StructureID]);
                    double found_ht = TryGet<double>(row[map.FoundationHeight]);
                    double ground_elv = TryGet<double>(row[map.GroundElev]);
                    double val_struct = TryGet<double>(row[map.StructureValue]);
                    double val_cont = TryGet<double>(row[map.ContentValue]);
                    double val_vehic = TryGet<double>(row[map.VehicalValue]);
                    double val_other = TryGet<double>(row[map.OtherValue]);
                    string st_damcat = TryGetObj<string>(row[map.DamageCatagory]);
                    string occtype = TryGetObj<string>(row[map.OccupancyType]);
                    string cbfips = TryGetObj<string>(row[map.CBFips]);
                    double ff_elev = TryGet<double>(row[map.FirstFloorElev]);
                    if (row[map.FirstFloorElev] == DBNull.Value)
                    {
                        ff_elev = ground_elv + found_ht;
                    }
                    int impactAreaID = GetImpactAreaID(point, impactAreaShapefilePath);
                    Structures.Add(new Structure(fid, point, found_ht, val_struct, val_cont, val_vehic, val_other, st_damcat, occtype, impactAreaID, cbfips));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            _Occtypes = occTypes;
            GetUniqueImpactAreas();
            GetUniqueDamageCatagories();
        }

        /// <summary>
        /// Updates a point shapefile with ground elevations sampled off a RAS terrain at their location. Requires a RAS HDF terrain, and modifies the file in place. 
        /// </summary>
        /// <param name="pointShapefilePath"></param>
        /// <param name="TerrainPath"></param>
        /// <param name="GroundElevColumnHeader"></param>
        public static void SaveGroundElevationFromTerrainToShapefile(string pointShapefilePath, string TerrainPath, string GroundElevColumnHeader = "ground_elv" )
        {
            PointFeatureLayer structureInventory = new PointFeatureLayer("Structure_Inventory", pointShapefilePath);
            PointMs pointMs = new PointMs(structureInventory.Points().Select(p => p.PointM()));
            TerrainLayer terrain = new TerrainLayer("Terrain", TerrainPath);
            var groundElevs = terrain.ComputePointElevations(pointMs);

            for (int i = 0; i < structureInventory.FeatureCount(); i++)
            {
                var row = structureInventory.FeatureRow(i);
                row[GroundElevColumnHeader] = groundElevs[i];
            }
            structureInventory.Save();
        }
        public Inventory(List<Structure> structures, List<OccupancyType> occTypes)
        {
            Structures = structures;
            _Occtypes = occTypes;
            GetUniqueImpactAreas();
            GetUniqueDamageCatagories();
        }
        private void GetUniqueImpactAreas()
        {
            List<int> impactAreas = new List<int>();
            foreach (var structure in Structures)
            {
                if (!impactAreas.Contains(structure.ImpactAreaID))
                {
                    impactAreas.Add(structure.ImpactAreaID);
                }
            }
            _impactAreaIDs = impactAreas;
        }
        /// <summary>
        /// Loops through entire inventory and reports back a list of all the unique damage catagories associated with the structures
        /// </summary>
        /// <returns></returns>
        internal void GetUniqueDamageCatagories()
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
            _damageCategories = damageCatagories;
        }
        public Inventory GetInventoryTrimmmedToPolygon(Polygon impactArea)
        {
            List<Structure> filteredStructureList = new List<Structure>();

            foreach (Structure structure in Structures)
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
            foreach (Structure structure in Structures)
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

            List<DeterministicStructure> inventorySample = new List<DeterministicStructure>();
            foreach (Structure structure in Structures)
            {
                foreach (OccupancyType occupancyType in _Occtypes)
                {
                    if (structure.DamageCatagory.Equals(occupancyType.DamageCategory))
                    {
                        if (structure.OccTypeName.Equals(occupancyType.Name))
                        {
                            inventorySample.Add(structure.Sample(random.Next(), occupancyType));
                            break;
                        }
                    }
                }
                //it is possible that if an occupancy type doesnt exist a structure wont get added...
            }
            return new DeterministicInventory(inventorySample, _impactAreaIDs, _damageCategories);
        }
    }
}
