using Geospatial.GDALAssist;
using Geospatial.GDALAssist.Vectors;
using HEC.FDA.Model.interfaces;
using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using RasMapperLib.Utilities;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Base.Enumerations;

namespace HEC.FDA.Model.structures
{
    //TODO: Figure out how to set Occupany Type Set
    public class Inventory: Validation 
    {
        #region Fields
        private string _structureInventoryShapefile;
        private string _impactAreaShapefile;
        private StructureSelectionMapping _map;
        private Dictionary<string, OccupancyType> _occtypes;
        private string _impactAreaUniqueColumnHeader;
        private bool _updateGroundElevsFromTerrain;
        private string _terrainPath;
        private double _priceIndex;
        #endregion

        #region Properties
        public List<Structure> Structures { get; } = new List<Structure>();
        public List<int> ImpactAreas
        {
            get
            {
                List<int> impactAreas = new List<int>();
                foreach (var structure in Structures)
                {
                    if (!impactAreas.Contains(structure.ImpactAreaID))
                    {
                        impactAreas.Add(structure.ImpactAreaID);
                    }
                }
                return impactAreas;
            }
        }
        public List<string> DamageCategories
        {
            get
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
        }
        public float[] GroundElevations
        {
            get
            {
                float[] result = new float[Structures.Count];
                for (int i = 0; i < Structures.Count; i++)
                {
                    result[i] = (float)Structures[i].GroundElevation;
                }
                return result;
            }
        }
        #endregion

        #region Constructors
        public Inventory(string pointShapefilePath, string impactAreaShapefilePath, StructureSelectionMapping map, Dictionary<string, OccupancyType> occTypes,
            string impactAreaUniqueColumnHeader, bool updateGroundElevFromTerrain, string terrainPath, double priceIndex = 1)
        {
            _structureInventoryShapefile = pointShapefilePath;
            _impactAreaShapefile = impactAreaShapefilePath;
            _map = map;
            _occtypes = occTypes;
            _impactAreaUniqueColumnHeader = impactAreaUniqueColumnHeader;
            _updateGroundElevsFromTerrain = updateGroundElevFromTerrain;
            _terrainPath = terrainPath;
            _priceIndex = priceIndex;
            //TODO: Add some validation here
            //If we have a bad shapefile name, then we get a null ref exception in the below method
            LoadStructuresFromSourceFiles();
            AddRules();
        }
        public Inventory(string pointShapefilePath, string impactAreaShapefilePath, StructureSelectionMapping map, Dictionary<string, OccupancyType> occTypes,
        string impactAreaUniqueColumnHeader, bool updateGroundElevFromTerrain, string terrainPath, List<Structure> structures, double priceIndex = 1)
        {
            _structureInventoryShapefile = pointShapefilePath;
            _impactAreaShapefile = impactAreaShapefilePath;
            _map = map;
            _occtypes = occTypes;
            _impactAreaUniqueColumnHeader = impactAreaUniqueColumnHeader;
            _updateGroundElevsFromTerrain = updateGroundElevFromTerrain;
            _terrainPath = terrainPath;
            Structures = structures;
            _priceIndex = priceIndex;
            AddRules();

        }
        #endregion

        #region Methods
        private void AddRules()
        {
            foreach (Structure structure in Structures)
            {
                AddSinglePropertyRule("Structure " + structure.Fid, new Rule(() => { structure.Validate(); return !structure.HasErrors; }, $"Structure {structure.Fid} has the following errors: " + structure.GetErrors().ToString(), structure.ErrorLevel));
            }
            foreach (OccupancyType occupancyType in _occtypes)
            {
                AddSinglePropertyRule("Occupancy Type " + occupancyType.Name, new Rule(() => { occupancyType.Validate(); return !occupancyType.HasErrors; }, $"Occupancy Type {occupancyType.Name} has the following errors: " + occupancyType.GetErrors().ToString(), occupancyType.ErrorLevel));
            }
            AddSinglePropertyRule(nameof(_priceIndex), new Rule(() => _priceIndex >= 1, $"The price index must be greater than or equal to 1 but was entered as {_priceIndex}", ErrorLevel.Major));
        }
        public Polygon GetImpactAreaPolygon(string impactAreaName)
        {
            PolygonFeatureLayer impactAreas = new PolygonFeatureLayer("ImpactAreas", _impactAreaShapefile);
            for (int i = 0; i < impactAreas.FeatureCount(); i++)
            {
                var row = impactAreas.FeatureRow(i);
                string thisImpactAreaName = TryGetObj<string>(row[_impactAreaUniqueColumnHeader]);
                //TODO: this line does not appear to work correctly. THe condition is being evaluated as true despite the strings being vastly different. 
                if (thisImpactAreaName.Equals(impactAreaName))
                {
                    return impactAreas.Polygon(i);
                }
            }
            return null;
        }

        private T GetRowValueForColumn<T>(DataRow row, string mappingColumnName, T defaultValue) where T : struct
        {
            T retval = defaultValue;
            if (mappingColumnName != null && row.Table.Columns.Contains(mappingColumnName))
            {
                //column could have wrong data type, or be null, or dbnull
                retval = TryGet<T>(row[mappingColumnName], defaultValue);
            }
            return retval;
        }
        private string GetRowValueForColumn(DataRow row, string mappingColumnName, string defaultValue)
        {
            string retval = defaultValue;
            if (mappingColumnName != null && row.Table.Columns.Contains(mappingColumnName))
            {
                //column could have wrong data type, or be null, or dbnull
                retval = TryGetObj<string>(row[mappingColumnName], defaultValue);
            }
            return retval;
        }

        private void LoadStructuresFromSourceFiles()
        {
            PointFeatureLayer structureInventory = new PointFeatureLayer("Structure_Inventory", _structureInventoryShapefile);
            
            PolygonFeatureLayer impactAreaSet = new PolygonFeatureLayer("Impact_Area_Set", _impactAreaShapefile);

            float[] groundelevs = Array.Empty<float>();
            if (_updateGroundElevsFromTerrain)
            {
                groundelevs = GetGroundElevationFromTerrain(_structureInventoryShapefile, _terrainPath);
            }

            int defaultMissingValue = -999;
            PointMs pointMs = new PointMs(structureInventory.Points().Select(p => p.PointM()));
            for (int i = 0; i < structureInventory.FeatureCount(); i++)
            {
                //required parameters
                PointM point = pointMs[i];
                DataRow row = structureInventory.FeatureRow(i);

                int fid = GetRowValueForColumn<int>(row, _map.StructureIDCol, defaultMissingValue);               
                double val_struct = GetRowValueForColumn<double>(row,_map.StructureValueCol, defaultMissingValue);

                string st_damcat = GetRowValueForColumn(row, _map.DamageCatagory, "NA");
                string occtype = GetRowValueForColumn(row, _map.OccTypeCol, "NA");
                //semi-required. We'll either have ff_elev given to us, or both ground elev and found_ht
                double found_ht = GetRowValueForColumn<double>(row, _map.FoundationHeightCol, defaultMissingValue); //not gauranteed
                double ground_elv;
                if (_updateGroundElevsFromTerrain)
                {
                    ground_elv = groundelevs[i];
                }
                else
                {
                    ground_elv = GetRowValueForColumn<double>(row, _map.GroundElevCol, defaultMissingValue); //not gauranteed
                }
                double ff_elev = GetRowValueForColumn<double>(row, _map.FirstFloorElevCol, defaultMissingValue); // not gauranteed  
                if (_map.FirstFloorElevCol == null || row[_map.FirstFloorElevCol] == DBNull.Value)
                {
                    ff_elev = ground_elv + found_ht;
                }
                //optional parameters
                double val_cont = GetRowValueForColumn<double>(row, _map.ContentValueCol, 0);
                double val_vehic = GetRowValueForColumn<double>(row,_map.VehicleValueCol, 0);
                double val_other = GetRowValueForColumn<double>(row, _map.OtherValueCol, 0);
                string cbfips = GetRowValueForColumn(row, _map.CBFips, "NA");
                double beginningDamage = GetRowValueForColumn<double>(row, _map.BeginningDamageDepthCol, 0);
                int numStructures = GetRowValueForColumn<int>(row, _map.NumberOfStructuresCol, 1);
                int yearInService = GetRowValueForColumn<int>(row, _map.YearInConstructionCol, defaultMissingValue);
                //TODO: handle number 
                int impactAreaID = GetImpactAreaFID(point);
                Structures.Add(new Structure(fid, point, ff_elev, val_struct, st_damcat, occtype, impactAreaID, val_cont, 
                    val_vehic, val_other, cbfips, beginningDamage, ground_elv, found_ht, yearInService, numStructures));
            }
        }
       
        public static float[] GetGroundElevationFromTerrain(string pointShapefilePath, string TerrainPath)
        {
            PointFeatureLayer structureInventory = new PointFeatureLayer("Structure_Inventory", pointShapefilePath);
            PointMs pointMs = new PointMs(structureInventory.Points().Select(p => p.PointM()));
            TerrainLayer terrain = new TerrainLayer("Terrain", TerrainPath);
            return terrain.ComputePointElevations(pointMs);
        }
        private PointM ReprojectPoint(PointM point, Projection newProjection, Projection currentProjection)
        {

            Geospatial.Vectors.Point p = Converter.Convert(point);
            VectorExtensions.Reproject(p, currentProjection, newProjection);
            return Converter.ConvertPtM(p);
        }
        private Projection GetTerrainProjection(string Pointsfilename, string terrainFilename)
        {
            //Check extension of terrain file
            string extension = System.IO.Path.GetExtension(terrainFilename);
            // If HDF, create RASTerrainLayer, then get source files. Create a GDAL Raster from any source.
            if (extension == "hdf")
            {
                TerrainLayer terrain = new TerrainLayer("Terrain", terrainFilename);
                terrainFilename = terrain.get_RasterFilename(0);
            }
            GDALRaster raster = new GDALRaster(terrainFilename);
            return raster.GetProjection();
        }
        private Projection GetVectorProjection(string vectorPath)
        {
            VectorDataset vector = new VectorDataset(vectorPath);
            VectorLayer vectorLayer = vector.GetLayer(0);
            return vectorLayer.GetProjection();
        }
        public Inventory GetInventoryTrimmmedToPolygon(int impactAreaFID)
        {
            PolygonFeatureLayer impactAreaSet = new PolygonFeatureLayer("ImpactAreas", _impactAreaShapefile);
            List<Structure> filteredStructureList = new List<Structure>();

            foreach (Structure structure in Structures)
            {
                if (impactAreaSet[impactAreaFID].Contains(structure.Point))
                {
                    filteredStructureList.Add(structure);
                }
            }
            return new Inventory(_structureInventoryShapefile, _impactAreaShapefile, _map, _occtypes, _impactAreaUniqueColumnHeader, _updateGroundElevsFromTerrain, _terrainPath, filteredStructureList);
        }

        public Inventory GetInventoryTrimmedToDamageCategory(string damageCategory)
        {
            List<Structure> filteredStructureList = new List<Structure>();
            foreach (Structure structure in Structures)
            {
                if (structure.DamageCatagory == damageCategory)
                {
                    filteredStructureList.Add(structure);
                }
            }
            return new Inventory(_structureInventoryShapefile, _impactAreaShapefile, _map, _occtypes, _impactAreaUniqueColumnHeader, _updateGroundElevsFromTerrain, _terrainPath, filteredStructureList);
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
        public int GetImpactAreaFID(PointM point)
        {
            PolygonFeatureLayer impactAreaSet = new PolygonFeatureLayer("ImpactAreas", _impactAreaShapefile);
            List<Polygon> polygons = impactAreaSet.Polygons().ToList();
            for (int i = 0; i < polygons.Count; i++)
            {
                if (polygons[i].Contains(point))
                {
                    return i;
                }
            }
            return -9999;
        }

        public DeterministicInventory Sample(IProvideRandomNumbers randomProvider, bool computeIsDeterministic = false)
        {
            List<DeterministicStructure> inventorySample = new List<DeterministicStructure>();
            foreach (Structure structure in Structures)
            {
                if(_occtypes.ContainsKey(structure.OccTypeName))
                {
                    OccupancyType occupancyType = _occtypes[structure.OccTypeName];
                    inventorySample.Add(structure.Sample(randomProvider, occupancyType, computeIsDeterministic));           
                }
            }
            return new DeterministicInventory(inventorySample, _priceIndex);
        }

        internal List<string> StructureDetails()
        {
            string header = "StructureID,YearInService,DamageCategory,OccupancyType,X_Coordinate,Y_Coordinate,StructureValueInDatabase,StructureValueInflated,ContentValue,ContentValueInflated,OtherValue,OtherValueInflated,VehicleValue,VehicleValueInflated,TotalValue,TotalValueInflated,NumberOfStructures,FirstFloorElevation,GroundElevation,FoundationHeight,DepthBeginningDamage,";
            List<string> structureDetails = new List<string>() { header };
            foreach (Structure structure in Structures)
            {
                structureDetails.Add(structure.ProduceDetails(_priceIndex));
            }
            return structureDetails;
        }
        #endregion

        #region Utilities
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
        #endregion
    }
}
