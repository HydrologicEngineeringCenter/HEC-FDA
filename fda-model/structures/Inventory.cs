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
using Geospatial.Terrain;
using HEC.MVVMFramework.Model.Messaging;

namespace HEC.FDA.Model.structures
{
    //TODO: Figure out how to set Occupany Type Set
    public class Inventory: Validation, IContainValidationGroups
    {
        #region Properties
        public List<Structure> Structures { get; } = new List<Structure>();
        public Dictionary<string, OccupancyType> OccTypes { get; set; }
        public double PriceIndex { get; set; }
        public List<ValidationGroup> ValidationGroups { get; } = new List<ValidationGroup>();
    #endregion

    #region Constructors
    public Inventory(string pointShapefilePath, string impactAreaShapefilePath, StructureSelectionMapping map, Dictionary<string, OccupancyType> occTypes, bool updateGroundElevFromTerrain, string terrainPath, double priceIndex = 1)
        {
            OccTypes = occTypes;
            PriceIndex = priceIndex;
            TerrainLayer terrainLayer = new TerrainLayer("ThisNameIsNotUsed", terrainPath);
            PointFeatureLayer structureFeatureLayer= new PointFeatureLayer("ThisNameIsNotUsed", pointShapefilePath);
            PolygonFeatureLayer impactAreaFeatureLayer = new PolygonFeatureLayer("ThisNameIsNotUsed", impactAreaShapefilePath);
            LoadStructuresFromSourceFiles(structureFeatureLayer, map, terrainLayer, updateGroundElevFromTerrain, impactAreaFeatureLayer);
            AddRules();
        }

        public Inventory(Dictionary<string, OccupancyType> occTypes, List<Structure> structures, double priceIndex = 1)
        {
            OccTypes = occTypes;
            Structures = structures;
            PriceIndex = priceIndex;
            AddRules();
        }
        #endregion

        #region Methods
        public float[] GetGroundElevations()
        {
            float[] result = new float[Structures.Count];
            for (int i = 0; i < Structures.Count; i++)
            {
                result[i] = (float)Structures[i].GroundElevation;
            }
            return result;
        }
        internal List<string> GetDamageCategories()
        {
            List<string> uniqueDamageCategories = new List<string>();
            foreach( Structure structure in Structures)
            {
                if (!uniqueDamageCategories.Contains(structure.DamageCatagory))
                {
                    uniqueDamageCategories.Add(structure.DamageCatagory);
                }
            }
            return uniqueDamageCategories;
        }

        private List<Polygon> LoadImpactAreasFromSourceFiles(PolygonFeatureLayer impactAreaSet, TerrainLayer terrain)
        {
            Projection terrainProjection = GetTerrainProjection(terrain);
            List<Polygon> polygons = impactAreaSet.Polygons().ToList();

            //Projections
            Projection impactAreaPrj = GetVectorProjection(impactAreaSet);
            if (impactAreaPrj.IsEqual(terrainProjection))
            {
                return polygons;
            }
            else
            {
                List<Polygon> ImpactAreas = new List<Polygon>();
                foreach (Polygon poly in polygons)
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
        private void AddValidationGroup()
        {
            ValidationGroup vg = new ValidationGroup("This inventory has the following errors:");
            foreach (OccupancyType ot in _occtypes.Values)
            {
                vg.ChildGroups.AddRange(ot.ValidationGroups);
            }
            ValidationGroups.Add(vg);
        }
        private void AddRules()
        {
            foreach (Structure structure in Structures)
            {
                AddSinglePropertyRule("Structure " + structure.Fid, new Rule(() => { structure.Validate(); return !structure.HasErrors; }, $"Structure {structure.Fid} has the following errors: " + structure.GetErrors().ToString(), structure.ErrorLevel));
            }
            foreach (OccupancyType occupancyType in OccTypes.Values)
            {
                AddSinglePropertyRule("Occupancy Type " + occupancyType.Name, new Rule(() => { occupancyType.Validate(); return !occupancyType.HasErrors; }, $"Occupancy Type {occupancyType.Name} has the following errors: " + occupancyType.GetErrors().ToString(), occupancyType.ErrorLevel));
            }
            AddSinglePropertyRule(nameof(PriceIndex), new Rule(() => PriceIndex >= 1, $"The price index must be greater than or equal to 1 but was entered as {PriceIndex}", ErrorLevel.Major));
        }
        private T GetRowValueForColumn<T>(System.Data.DataRow row, string mappingColumnName, T defaultValue) where T : struct
        {
            T retval = defaultValue;
            if (mappingColumnName != null && row.Table.Columns.Contains(mappingColumnName))
            {
                //column could have wrong data type, or be null, or dbnull
                retval = TryGet<T>(row[mappingColumnName], defaultValue);
            }
            return retval;
        }
        private string GetRowValueForColumn(System.Data.DataRow row, string mappingColumnName, string defaultValue)
        {
            string retval = defaultValue;
            if (mappingColumnName != null && row.Table.Columns.Contains(mappingColumnName))
            {
                //column could have wrong data type, or be null, or dbnull
                retval = TryGetObj<string>(row[mappingColumnName], defaultValue);
            }
            return retval;
        }

        private void LoadStructuresFromSourceFiles(PointFeatureLayer structureFeatureLayer, StructureSelectionMapping map, TerrainLayer terrainLayer, bool updateGroundElevFromTerrain, PolygonFeatureLayer ImpactAreaShapefilePath)
        {
            List<Polygon> impactAreas = LoadImpactAreasFromSourceFiles(ImpactAreaShapefilePath, terrainLayer);
            float[] groundelevs = Array.Empty<float>();
            int defaultMissingValue = utilities.IntegerConstants.DEFAULT_MISSING_VALUE;
            PointMs pointMs = new PointMs(structureFeatureLayer.Points().Select(p => p.PointM()));

            if (updateGroundElevFromTerrain)
            {
                groundelevs = GetGroundElevationFromTerrain(structureFeatureLayer, terrainLayer);
            }

            for (int i = 0; i < structureFeatureLayer.FeatureCount(); i++)
            {
                //required parameters
                PointM point = pointMs[i];
                System.Data.DataRow row = structureFeatureLayer.FeatureRow(i);
                int fid = GetRowValueForColumn<int>(row, map.StructureIDCol, defaultMissingValue);
                double val_struct = GetRowValueForColumn<double>(row, map.StructureValueCol, defaultMissingValue);
                string occtype = GetRowValueForColumn(row, map.OccTypeCol, "NA");
                string st_damcat = "NA";
                if (OccTypes.ContainsKey(occtype))
                {
                    st_damcat = OccTypes[occtype].DamageCategory;
                }
                //semi-required. We'll either have ff_elev given to us, or both ground elev and found_ht
                double found_ht = GetRowValueForColumn<double>(row, map.FoundationHeightCol, defaultMissingValue); //not gauranteed
                double ground_elv;
                if (updateGroundElevFromTerrain)
                {
                    ground_elv = groundelevs[i];
                }
                else
                {
                    ground_elv = GetRowValueForColumn<double>(row, map.GroundElevCol, defaultMissingValue); //not gauranteed
                }
                double ff_elev = GetRowValueForColumn<double>(row, map.FirstFloorElevCol, defaultMissingValue); // not gauranteed  
                if (ff_elev == defaultMissingValue)
                {
                    ff_elev = ground_elv + found_ht;
                }
                //optional parameters
                double val_cont = GetRowValueForColumn<double>(row, map.ContentValueCol, 0);
                double val_vehic = GetRowValueForColumn<double>(row, map.VehicleValueCol, 0);
                double val_other = GetRowValueForColumn<double>(row, map.OtherValueCol, 0);
                string cbfips = GetRowValueForColumn(row, map.CBFips, "NA");
                double beginningDamage = GetRowValueForColumn<double>(row, map.BeginningDamageDepthCol, 0);
                int numStructures = GetRowValueForColumn<int>(row, map.NumberOfStructuresCol, 1);
                int yearInService = GetRowValueForColumn<int>(row, map.YearInConstructionCol, defaultMissingValue);
                //TODO: handle number 
                int impactAreaID = GetImpactAreaFID(point, impactAreas);
                Structures.Add(new Structure(fid, point, ff_elev, val_struct, st_damcat, occtype, impactAreaID, val_cont,
                    val_vehic, val_other, cbfips, beginningDamage, ground_elv, found_ht, yearInService, numStructures));
            }
            Console.WriteLine("finished");
        }
        public static float[] GetGroundElevationFromTerrain(PointFeatureLayer pointLayer, TerrainLayer terrain)
        {
            Projection terrainProjection = GetTerrainProjection(terrain);
            Projection siProjection = GetVectorProjection(pointLayer);
            PointMs pointMs = new PointMs(pointLayer.Points().Select(p => p.PointM()));
            if (!terrainProjection.IsEqual(siProjection))
            {
                PointMs reprojPointMs = new PointMs();
                foreach (PointM pt in pointMs)
                {
                    reprojPointMs.Add(ReprojectPoint(pt, terrainProjection, siProjection));
                }
                pointMs = reprojPointMs;
            }
            return terrain.ComputePointElevations(pointMs);
        }
        #region Projection
        public static PointM ReprojectPoint(PointM point, Projection newProjection, Projection currentProjection)
        {

            Geospatial.Vectors.Point p = Converter.Convert(point);
            Geospatial.Vectors.Point newp = VectorExtensions.Reproject(p, currentProjection, newProjection);
            return Converter.ConvertPtM(newp);
        }
        public static Polygon ReprojectPolygon(Polygon polygon, Projection newProjection, Projection currentProjection)
        {
            Geospatial.Vectors.Polygon poly = Converter.Convert(polygon);
            Geospatial.Vectors.Polygon reprojPoly = VectorExtensions.Reproject(poly, currentProjection, newProjection);
            return Converter.Convert(reprojPoly);

        }
        public static Projection GetTerrainProjection(TerrainLayer terrain)
        {
            string terrainFilename = terrain.get_RasterFilename(0);
            GDALRaster raster = new GDALRaster(terrainFilename);
            return raster.GetProjection();
        }
        public static Projection GetVectorProjection(FeatureLayer featureLayer)
        {
            string siFilename = featureLayer.SourceFilename;
            VectorDataset vector = new VectorDataset(siFilename);
            VectorLayer vectorLayer = vector.GetLayer(0);
            return vectorLayer.GetProjection();
        }
        #endregion
        public Inventory GetInventoryTrimmedToImpactArea(int impactAreaFID)
        {
            List<Structure> filteredStructureList = new List<Structure>();

            foreach (Structure structure in Structures)
            {
                if (structure.ImpactAreaID == impactAreaFID)
                {
                    filteredStructureList.Add(structure);
                }
            }
            return new Inventory(OccTypes, filteredStructureList);
        }
        /// <summary>
        /// This method filters structures and the water surface profiles by damage category
        /// Structures and profiles need to be filtered jointly because they are related by index only
        /// This method preserves the relationship between a given structure and water at the structure 
        /// </summary>
        /// <param name="damageCategory"></param>
        /// <param name="wsesAtEachStructureByProfile"></param>
        /// <returns></returns>
        public (Inventory, List<float[]>) GetInventoryAndWaterTrimmedToDamageCategory(string damageCategory, List<float[]> wsesAtEachStructureByProfile)
        {
            //set up list for filtered structures 
            List<Structure> filteredStructureList = new List<Structure>();
            //set up lists for filtered WSEs - this list of list of floats will be converted back to list of float arrays below
            //the list of list is used because we are uncertain of the needed size a priori 
            List<List<float>> listedWSEsFiltered = new List<List<float>>();
            for (int j = 0; j < wsesAtEachStructureByProfile.Count; j++)
            {
                List<float> listOfStages = new List<float>();
                listedWSEsFiltered.Add(listOfStages);
            }
            for (int i = 0; i < Structures.Count; i++)
            {
                if (Structures[i].DamageCatagory == damageCategory)
                {
                    filteredStructureList.Add(Structures[i]);
                    //add WSEs for structure i for each profile j 
                    for (int j = 0; j < wsesAtEachStructureByProfile.Count; j++)
                    {
                        listedWSEsFiltered[j].Add(wsesAtEachStructureByProfile[j][i]);
                    }
                }
            }
            List<float[]> arrayedWSEsFiltered = new List<float[]>();
            foreach (List<float> wses in listedWSEsFiltered)
            {
                arrayedWSEsFiltered.Add(wses.ToArray());
            }
            return (new Inventory(OccTypes, filteredStructureList, PriceIndex), arrayedWSEsFiltered);
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
        public int GetImpactAreaFID(PointM point, List<Polygon> ImpactAreas)
        {
            for (int i = 0; i < ImpactAreas.Count; i++)
            {
                if (ImpactAreas[i].Contains(point))
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
                if (OccTypes.ContainsKey(structure.OccTypeName))
                {
                    OccupancyType occupancyType = OccTypes[structure.OccTypeName];
                    inventorySample.Add(structure.Sample(randomProvider, occupancyType, computeIsDeterministic));
                }
            }
            return new DeterministicInventory(inventorySample, PriceIndex);
        }
        internal List<string> StructureDetails()
        {
            string header = "StructureID,YearInService,DamageCategory,OccupancyType,X_Coordinate,Y_Coordinate,StructureValueInDatabase,StructureValueInflated,ContentValue,ContentValueInflated,OtherValue,OtherValueInflated,VehicleValue,VehicleValueInflated,TotalValue,TotalValueInflated,NumberOfStructures,FirstFloorElevation,GroundElevation,FoundationHeight,DepthBeginningDamage,";
            List<string> structureDetails = new List<string>() { header };
            foreach (Structure structure in Structures)
            {
                structureDetails.Add(structure.ProduceDetails(PriceIndex));
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
