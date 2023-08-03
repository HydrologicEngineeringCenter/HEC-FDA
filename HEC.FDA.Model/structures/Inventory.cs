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
using Utilities;
using HEC.FDA.Model.metrics;
using System.Threading.Tasks;

namespace HEC.FDA.Model.structures
{
    //TODO: Figure out how to set Occupany Type Set
    public class Inventory : Validation, IContainValidationGroups
    {
        #region Properties
        public List<Structure> Structures { get; } = new List<Structure>();
        //The string key is the occupancy type name 
        public Dictionary<string, OccupancyType> OccTypes { get; set; }
        public double PriceIndex { get; set; }
        public List<ValidationGroup> ValidationGroups { get; } = new List<ValidationGroup>();
        #endregion

        #region Constructors
        public Inventory(string pointShapefilePath, string impactAreaShapefilePath, StructureSelectionMapping map, Dictionary<string, OccupancyType> occTypes, bool updateGroundElevFromTerrain,
            string terrainPath, double priceIndex = 1, string projectionFilePath = "")
        {
            OccTypes = occTypes;
            PriceIndex = priceIndex;
            //Projection.FromFile returns Null if the path is bad. We'll check for null before we reproject. 
            Projection studyProjection = Projection.FromFile(projectionFilePath);
            TerrainLayer terrainLayer = new("ThisNameIsNotUsed", terrainPath);
            PointFeatureLayer structureFeatureLayer = new("ThisNameIsNotUsed", pointShapefilePath);
            PolygonFeatureLayer impactAreaFeatureLayer = new("ThisNameIsNotUsed", impactAreaShapefilePath);

            LoadStructuresFromSourceFiles(structureFeatureLayer, map, terrainLayer, updateGroundElevFromTerrain, impactAreaFeatureLayer, studyProjection);
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
            List<string> uniqueDamageCategories = new();
            foreach (Structure structure in Structures)
            {
                if (!uniqueDamageCategories.Contains(structure.DamageCatagory))
                {
                    uniqueDamageCategories.Add(structure.DamageCatagory);
                }
            }
            return uniqueDamageCategories;
        }

        private static List<Polygon> LoadImpactAreasFromSourceFiles(PolygonFeatureLayer impactAreaSet, Projection studyProjection)
        {
            List<Polygon> polygons = impactAreaSet.Polygons().ToList();
            Projection impactAreaPrj = GetVectorProjection(impactAreaSet);
            if (studyProjection.IsNull())
            {
                return polygons;
            }
            if (impactAreaPrj.IsEqual(studyProjection) )
            {
                return polygons;
            }
            else
            {
                return ReprojectPolygons(studyProjection, polygons, impactAreaPrj);
            }
        }

        private static List<Polygon> ReprojectPolygons(Projection studyProjection, List<Polygon> polygons, Projection impactAreaPrj)
        {
            List<Polygon> ImpactAreas = new();
            foreach (Polygon poly in polygons)
            {
                Polygon newPoly = ReprojectPolygon(poly, impactAreaPrj, studyProjection);
                ImpactAreas.Add(newPoly);
            }
            return ImpactAreas;
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
        private static T GetRowValueForColumn<T>(System.Data.DataRow row, string mappingColumnName, T defaultValue) where T : struct
        {
            T retval = defaultValue;
            if (mappingColumnName != null && row.Table.Columns.Contains(mappingColumnName))
            {
                //column could have wrong data type, or be null, or dbnull
                retval = TryGet<T>(row[mappingColumnName], defaultValue);
            }
            return retval;
        }
        private static string GetRowValueForColumn(System.Data.DataRow row, string mappingColumnName, string defaultValue)
        {
            string retval = defaultValue;
            if (mappingColumnName != null && row.Table.Columns.Contains(mappingColumnName))
            {
                //column could have wrong data type, or be null, or dbnull
                retval = TryGetObj<string>(row[mappingColumnName], defaultValue);
            }
            return retval;
        }

        private void LoadStructuresFromSourceFiles(PointFeatureLayer structureFeatureLayer, StructureSelectionMapping map, TerrainLayer terrainLayer, bool updateGroundElevFromTerrain,
            PolygonFeatureLayer ImpactAreaShapefilePath, Projection studyProjection)
        {
            List<Polygon> impactAreas = LoadImpactAreasFromSourceFiles(ImpactAreaShapefilePath, studyProjection);
            float[] groundelevs = Array.Empty<float>();
            int defaultMissingValue = utilities.IntegerConstants.DEFAULT_MISSING_VALUE;
            PointMs pointMs = new(structureFeatureLayer.Points().Select(p => p.PointM()));

            if (updateGroundElevFromTerrain)
            {
                groundelevs = GetGroundElevationFromRASTerrain(structureFeatureLayer, terrainLayer, studyProjection);
            }

            for (int i = 0; i < structureFeatureLayer.FeatureCount(); i++)
            {
                //required parameters
                PointM point = pointMs[i];
                System.Data.DataRow row = structureFeatureLayer.FeatureRow(i);
                int fid = GetRowValueForColumn(row, map.StructureIDCol, defaultMissingValue);
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
                int numStructures = GetRowValueForColumn(row, map.NumberOfStructuresCol, 1);
                int yearInService = GetRowValueForColumn(row, map.YearInConstructionCol, defaultMissingValue);
                //TODO: handle number 
                int impactAreaID = GetImpactAreaFID(point, impactAreas);
                Structures.Add(new Structure(fid, point, ff_elev, val_struct, st_damcat, occtype, impactAreaID, val_cont,
                    val_vehic, val_other, cbfips, beginningDamage, ground_elv, found_ht, yearInService, numStructures));
            }
            Console.WriteLine("finished");
        }
        public static float[] GetGroundElevationFromRASTerrain(PointFeatureLayer pointLayer, TerrainLayer terrain, Projection studyProjection)
        {
            Projection siProjection = GetVectorProjection(pointLayer);
            PointMs pointMs = new(pointLayer.Points().Select(p => p.PointM()));
            if (!studyProjection.IsNull())
            {
                if (!studyProjection.IsEqual(siProjection))
                {
                    pointMs = ReprojectPoints(studyProjection, siProjection, pointMs);
                }
            }
            return terrain.ComputePointElevations(pointMs);
        }

        public static PointMs ReprojectPoints(Projection studyProjection, Projection siProjection, PointMs pointMs)
        {
            PointMs reprojPointMs = new();
            foreach (PointM pt in pointMs)
            {
                reprojPointMs.Add(ReprojectPoint(pt, studyProjection, siProjection));
            }
            return reprojPointMs;
         
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
        public static Projection GetVectorProjection(FeatureLayer featureLayer)
        {
            string siFilename = featureLayer.SourceFilename;
            VectorDataset vector = new(siFilename);
            VectorLayer vectorLayer = vector.GetLayer(0);
            Projection projection = vectorLayer.GetProjection();
            return projection;
        }
        #endregion
        public Inventory GetInventoryTrimmedToImpactArea(int impactAreaFID)
        {
            List<Structure> filteredStructureList = new();

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
            List<Structure> filteredStructureList = new();
            //set up lists for filtered WSEs - this list of list of floats will be converted back to list of float arrays below
            //the list of list is used because we are uncertain of the needed size a priori 
            List<List<float>> listedWSEsFiltered = new();
            for (int j = 0; j < wsesAtEachStructureByProfile.Count; j++)
            {
                List<float> listOfStages = new();
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
            List<float[]> arrayedWSEsFiltered = new();
            foreach (List<float> wses in listedWSEsFiltered)
            {
                arrayedWSEsFiltered.Add(wses.ToArray());
            }
            return (new Inventory(OccTypes, filteredStructureList, PriceIndex), arrayedWSEsFiltered);
        }
        public PointMs GetPointMs()
        {
            PointMs points = new();
            foreach (Structure structure in Structures)
            {
                points.Add(structure.Point);
            }
            return points;
        }
        public static int GetImpactAreaFID(PointM point, List<Polygon> ImpactAreas)
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
        internal List<string> StructureDetails()
        {
            string header = "StructureID,YearInService,DamageCategory,OccupancyType,X_Coordinate,Y_Coordinate,StructureValueInDatabase,StructureValueInflated,ContentValue,ContentValueInflated,OtherValue,OtherValueInflated,VehicleValue,VehicleValueInflated,TotalValue,TotalValueInflated,NumberOfStructures,FirstFloorElevation,GroundElevation,FoundationHeight,DepthBeginningDamage,";
            List<string> structureDetails = new() { header };
            foreach (Structure structure in Structures)
            {
                structureDetails.Add(structure.ProduceDetails(PriceIndex));
            }
            return structureDetails;
        }
        
        public List<DeterministicOccupancyType> SampleOccupancyTypes(IProvideRandomNumbers randomNumberProvider)
        {
            List<DeterministicOccupancyType> deterministicOccupancyTypes = new();
            foreach(OccupancyType occupancyType in OccTypes.Values)
            {
                DeterministicOccupancyType deterministicOccupancyType = occupancyType.Sample(randomNumberProvider);
                deterministicOccupancyTypes.Add(deterministicOccupancyType);
            }

            return deterministicOccupancyTypes;
        }


        public ConsequenceResult ComputeDamages(float[] wses, int analysisYear, string damageCategory, List<DeterministicOccupancyType> deterministicOccupancyType)
        {
            ConsequenceResult aggregateConsequenceResult = new ConsequenceResult(damageCategory);
            //assume each structure has a corresponding index to the depth
            int iterationsPerComputeChunk = 1;
            if (Structures.Count >= 100)
            {
                iterationsPerComputeChunk = 100;
            }
            double computeChunks = System.Math.Floor(Structures.Count / Convert.ToDouble(iterationsPerComputeChunk));
            int remainderIterations = Structures.Count % iterationsPerComputeChunk;

            double[] structureParallelCollectionArray = new double[iterationsPerComputeChunk];
            double[] contentParallelCollectionArray = new double[iterationsPerComputeChunk];
            double[] otherParallelCollectionArray = new double[iterationsPerComputeChunk];
            double[] vehicleParallelCollectionArray = new double[iterationsPerComputeChunk];

            double[] structureParallelRemainderArray = new double[remainderIterations];
            double[] contentParallelRemainderArray = new double[remainderIterations];
            double[] otherParallelRemainderArray = new double[remainderIterations];
            double[] vehicleParallelRemainderArray = new double[remainderIterations];
            int startingPosition = 0;
            for (int j = 0; j < computeChunks; j++)
            {
                Parallel.For(0, iterationsPerComputeChunk, i =>
                {
                    float wse = wses[startingPosition + i];
                    if (wse != -9999)
                    {
                        ConsequenceResult consequenceResult = Structures[startingPosition + i].ComputeDamage(wse, deterministicOccupancyType, PriceIndex, analysisYear);
                        structureParallelCollectionArray[i] = consequenceResult.StructureDamage;
                        contentParallelCollectionArray[i] = consequenceResult.ContentDamage;
                        otherParallelCollectionArray[i] = consequenceResult.OtherDamage;
                        vehicleParallelCollectionArray[i] = consequenceResult.VehicleDamage;
                    }
                });
                startingPosition += iterationsPerComputeChunk;
                for (int i = 0; i < structureParallelCollectionArray.Length; i++)
                {
                    aggregateConsequenceResult.IncrementConsequence(structureParallelCollectionArray[i], contentParallelCollectionArray[i], otherParallelCollectionArray[i], vehicleParallelCollectionArray[i]);
                }
                Array.Clear(structureParallelCollectionArray);
                Array.Clear(contentParallelCollectionArray);
                Array.Clear(otherParallelCollectionArray);
                Array.Clear(vehicleParallelCollectionArray);
            }
            if (remainderIterations > 0)
            {
                Parallel.For(0, remainderIterations, i =>
                {
                    float wse = wses[startingPosition + i];
                    if (wse != -9999)
                    {
                        ConsequenceResult consequenceResult = Structures[startingPosition + i].ComputeDamage(wse, deterministicOccupancyType, PriceIndex, analysisYear);
                        structureParallelRemainderArray[i] = consequenceResult.StructureDamage;
                        contentParallelRemainderArray[i] = consequenceResult.ContentDamage;
                        otherParallelRemainderArray[i] = consequenceResult.OtherDamage;
                        vehicleParallelRemainderArray[i] = consequenceResult.VehicleDamage;
                    }
                });
                for (int i = 0; i < structureParallelCollectionArray.Length; i++)
                {
                    aggregateConsequenceResult.IncrementConsequence(structureParallelRemainderArray[i], contentParallelRemainderArray[i], otherParallelRemainderArray[i], vehicleParallelRemainderArray[i]);
                }
            }
            return aggregateConsequenceResult;
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
                T retn = value as T;
                if (retn != null)
                    return retn;
                else
                    return defaultValue;
            }
        }


        #endregion
    }
}
