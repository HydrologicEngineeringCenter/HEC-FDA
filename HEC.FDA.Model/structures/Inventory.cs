using Geospatial.GDALAssist;
using HEC.FDA.Model.interfaces;
using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Model.Messaging;
using HEC.FDA.Model.metrics;

namespace HEC.FDA.Model.structures
{
    //TODO: Figure out how to set Occupany Type Set
    public class Inventory: IDontImplementValidationButMyPropertiesDo
    {
        #region Properties
        public List<Structure> Structures { get; } = new List<Structure>();
        //The string key is the occupancy type name 
        public Dictionary<string, OccupancyType> OccTypes { get; set; }
        public double PriceIndex { get; set; }
        public bool HasErrors { get; set; }
        public ErrorLevel ErrorLevel { get; set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for building an Inventory without a terrain
        /// </summary>
        /// <param name="pointShapefilePath"></param>
        /// <param name="impactAreaShapefilePath"></param>
        /// <param name="map"></param>
        /// <param name="occTypes"></param>
        /// <param name="priceIndex"></param>
        /// <param name="projectionFilePath"></param>
        public Inventory(string pointShapefilePath, string impactAreaShapefilePath, StructureSelectionMapping map, Dictionary<string, OccupancyType> occTypes,
             double priceIndex = 1, string projectionFilePath = "")
        {
            OccTypes = occTypes;
            PriceIndex = priceIndex;
            Projection studyProjection = Projection.FromFile(projectionFilePath);//Projection.FromFile returns Null if the path is bad. We'll check for null before we reproject. 
            LoadStructuresFromSourceFiles(pointShapefilePath, map, null, false, impactAreaShapefilePath, studyProjection);
        }

        /// <summary>
        /// Constructor for building an inventory with a terrain
        /// </summary>
        /// <param name="pointShapefilePath"></param>
        /// <param name="impactAreaShapefilePath"></param>
        /// <param name="map"></param>
        /// <param name="occTypes"></param>
        /// <param name="terrainPath"></param>
        /// <param name="priceIndex"></param>
        public Inventory(string pointShapefilePath, string impactAreaShapefilePath, StructureSelectionMapping map, Dictionary<string, OccupancyType> occTypes,
    string terrainPath, double priceIndex = 1)
        {
            OccTypes = occTypes;
            PriceIndex = priceIndex;
            Projection studyProjection = RASHelper.GetProjectionFromTerrain(terrainPath);
            LoadStructuresFromSourceFiles(pointShapefilePath, map, terrainPath, true, impactAreaShapefilePath, studyProjection);

        }

        /// <summary>
        /// Builds a terrain from in-memory
        /// </summary>
        /// <param name="occTypes"></param>
        /// <param name="structures"></param>
        /// <param name="priceIndex"></param>
        public Inventory(Dictionary<string, OccupancyType> occTypes, List<Structure> structures, double priceIndex = 1)
        {
            OccTypes = occTypes;
            Structures = structures;
            PriceIndex = priceIndex;
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

        private void LoadStructuresFromSourceFiles(string pointShapefilePath, StructureSelectionMapping map, string terrrainFilePath, bool updateGroundElevFromTerrain,
            string ImpactAreaShapefilePath, Projection studyProjection)
        {
            PolygonFeatureLayer impactAreaFeatureLayer = new("ThisNameIsNotUsed", ImpactAreaShapefilePath);
            List<Polygon> impactAreas = RASHelper.LoadImpactAreasFromSourceFiles(impactAreaFeatureLayer, studyProjection);
            float[] groundelevs = Array.Empty<float>();
            int defaultMissingValue = utilities.IntegerGlobalConstants.DEFAULT_MISSING_VALUE;
            string defaultMissingStringValue = "EMPTY";
            PointFeatureLayer structureFeatureLayer = new("ThisNameIsNotUsed", pointShapefilePath);
            PointMs pointMs = new(structureFeatureLayer.Points().Select(p => p.PointM()));
            if (updateGroundElevFromTerrain)
            {
                groundelevs = RASHelper.SamplePointsFromRaster(pointShapefilePath, terrrainFilePath, studyProjection);
            }

            for (int i = 0; i < structureFeatureLayer.FeatureCount(); i++)
            {
                //required parameters
                PointM point = pointMs[i];
                System.Data.DataRow row = structureFeatureLayer.FeatureRow(i);
                string fid = RASHelper.GetRowValueForColumn(row, map.StructureIDCol, defaultMissingStringValue);
                double val_struct = RASHelper.GetRowValueForColumn<double>(row, map.StructureValueCol, defaultMissingValue);
                string occtype = RASHelper.GetRowValueForColumn(row, map.OccTypeCol, "NA");
                string st_damcat = "NA";
                if (OccTypes.ContainsKey(occtype))
                {
                    st_damcat = OccTypes[occtype].DamageCategory;
                    occtype = OccTypes[occtype].Name;
                }
                //semi-required. We'll either have ff_elev given to us, or both ground elev and found_ht
                double found_ht = RASHelper.GetRowValueForColumn<double>(row, map.FoundationHeightCol, defaultMissingValue); //not gauranteed
                double ground_elv;
                if (updateGroundElevFromTerrain)
                {
                    ground_elv = groundelevs[i];
                }
                else
                {
                    ground_elv = RASHelper.GetRowValueForColumn<double>(row, map.GroundElevCol, defaultMissingValue); //not gauranteed
                }
                double ff_elev = RASHelper.GetRowValueForColumn<double>(row, map.FirstFloorElevCol, defaultMissingValue); // not gauranteed  
                if (ff_elev == defaultMissingValue)
                {
                    ff_elev = ground_elv + found_ht;
                }
                //optional parameters
                double val_cont = RASHelper.GetRowValueForColumn<double>(row, map.ContentValueCol, 0);
                double val_vehic = RASHelper.GetRowValueForColumn<double>(row, map.VehicleValueCol, 0);
                double val_other = RASHelper.GetRowValueForColumn<double>(row, map.OtherValueCol, 0);
                string cbfips = RASHelper.GetRowValueForColumn(row, map.CBFips, "NA");
                double beginningDamage = RASHelper.GetRowValueForColumn<double>(row, map.BeginningDamageDepthCol, defaultMissingValue);
                if (beginningDamage == defaultMissingValue)
                {
                    if (found_ht != defaultMissingValue)
                    {
                        beginningDamage = -found_ht;
                    }
                }
                int numStructures = RASHelper.GetRowValueForColumn(row, map.NumberOfStructuresCol, 1);
                int yearInService = RASHelper.GetRowValueForColumn(row, map.YearInConstructionCol, defaultMissingValue);
                //TODO: handle number 
                int impactAreaID = GetImpactAreaFID(point, impactAreas);
                string notes = RASHelper.GetRowValueForColumn(row, map.NotesCol, "No Notes Provided");
                string description = RASHelper.GetRowValueForColumn(row, map.DescriptionCol, "No Description Provided");
                Structures.Add(new Structure(fid, point, ff_elev, val_struct, st_damcat, occtype, impactAreaID, val_cont,
                    val_vehic, val_other, cbfips, beginningDamage, ground_elv, found_ht, yearInService, numStructures, notes, description));
            }
        }

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
        internal List<string> StructureDetails(List<DeterministicOccupancyType> deterministicOccupancyTypes)
        {
            string header = Structure.ProduceDetailsHeader();
            List<string> structureDetails = new() { header };
            foreach (Structure structure in Structures)
            {
                structureDetails.Add(structure.ProduceDetails(deterministicOccupancyTypes, PriceIndex));
            }
            return structureDetails;
        }

        public List<DeterministicOccupancyType> SampleOccupancyTypes(IProvideRandomNumbers randomNumberProvider, bool computeIsDeterministic)
        {
            List<DeterministicOccupancyType> deterministicOccupancyTypes = new();
            foreach (OccupancyType occupancyType in OccTypes.Values)
            {
                DeterministicOccupancyType deterministicOccupancyType = occupancyType.Sample(randomNumberProvider, computeIsDeterministic);
                deterministicOccupancyTypes.Add(deterministicOccupancyType);
            }

            return deterministicOccupancyTypes;
        }

        // POC - not safe to invoke in parallel. Can guard with ThreadLocal<float[,]>
        private float[,] _invertedWSEL; // [struc, pf]
        private double[,] _strucParallelCollection;
        private double[,] _contentParallelCollection;
        private double[,] _otherParallelCollection;
        private double[,] _vehicleParallelCollection;
        private int[] _occTypeIndices;
        /// <summary>
        /// Begins the sixth loop of the Scenario Stage Damage Compute. 
        /// Scenario SD 
        /// Impact Area SD 
        /// Damage Catagory 
        /// Compute Chunk 
        /// Iteration
        /// Structure <--
        /// W.S.Profile
        /// </summary>
        /// <param name="wses"></param>
        /// <param name="analysisYear"></param>
        /// <param name="damageCategory"></param>
        /// <param name="deterministicOccupancyType"></param>
        /// <returns></returns>
        public List<ConsequenceResult> ComputeDamages(List<float[]> wses, int analysisYear, string damageCategory, List<DeterministicOccupancyType> deterministicOccupancyType)
        {

            List<ConsequenceResult> aggregateConsequenceResults = new();
            //assume each structure has a corresponding index to the depth

            int nPf = wses.Count;
            int nStruc = wses[0].Length;
            // NOT SAFE TO CALL THIS METHOD IN PARALLEL
            if (_invertedWSEL == null || _invertedWSEL.GetLength(0) != nStruc || _invertedWSEL.GetLength(1) != nPf)
            {
                _invertedWSEL = new float[nStruc, nPf];
            }

            for (int i = 0; i < nPf; i++)
            {
                var pf = wses[i];
                for (int j = 0; j < nStruc; j++)
                {
                    _invertedWSEL[j, i] = pf[j];
                }
            }

            if (_strucParallelCollection == null || _strucParallelCollection.GetLength(0) != nPf || _strucParallelCollection.GetLength(1) != nStruc)
            {
                _strucParallelCollection = new double[nPf, nStruc];
            }
            if (_contentParallelCollection == null || _contentParallelCollection.GetLength(0) != nPf || _contentParallelCollection.GetLength(1) != nStruc)
            {
                _contentParallelCollection = new double[nPf, nStruc];
            }
            if (_otherParallelCollection == null || _otherParallelCollection.GetLength(0) != nPf || _otherParallelCollection.GetLength(1) != nStruc)
            {
                _otherParallelCollection = new double[nPf, nStruc];
            }
            if (_vehicleParallelCollection == null || _vehicleParallelCollection.GetLength(0) != nPf || _vehicleParallelCollection.GetLength(1) != nStruc)
            {
                _vehicleParallelCollection = new double[nPf, nStruc];
            }

            if (_occTypeIndices == null || _occTypeIndices.Length != nStruc)
            {
                _occTypeIndices = new int[nStruc];
                for (int i = 0; i < nStruc; i++)
                {
                    var struc = Structures[i];
                    int occc = struc.FindOccTypeIndex(deterministicOccupancyType);
                    _occTypeIndices[i] = occc;
                }
            }

            Utility.Parallel.SmartFor(nStruc, (start, end) =>
            {
                for (int i = start; i <= end; i++)
                {
                    DeterministicOccupancyType dt = null;
                    var dtIdx = _occTypeIndices[i];
                    if (dtIdx == -1)
                    {
                        // Shouldnt create a new one, but repros prior bevehaior
                        dt = new DeterministicOccupancyType();
                    }
                    else
                    {
                        dt = deterministicOccupancyType[dtIdx];
                    }
                    for (int j = 0; j < nPf; j++)
                    {
                        float wse = _invertedWSEL[i, j];
                        if (wse != -9999)
                        {
                            var (structDamage, contDamage, vehicleDamage, otherDamage) = Structures[i].ComputeDamage(wse, dt, PriceIndex, analysisYear);
                            _strucParallelCollection[j, i] = (structDamage);
                            _contentParallelCollection[j, i] = (contDamage);
                            _otherParallelCollection[j, i] = (vehicleDamage);
                            _vehicleParallelCollection[j, i] = (otherDamage);
                        }
                    }
                }
            }, 256);
            return AggregateResults(wses, damageCategory, aggregateConsequenceResults, _strucParallelCollection, _contentParallelCollection, _otherParallelCollection, _vehicleParallelCollection);
        }

        private List<ConsequenceResult> AggregateResults(List<float[]> wses, string damageCategory, List<ConsequenceResult> aggregateConsequenceResults, double[,] structureParallelCollection,
            double[,] contentParallelCollection, double[,] otherParallelCollection, double[,] vehicleParallelCollection)
        {
            for (int j = 0; j < wses.Count; j++)
            {
                ConsequenceResult aggregateConsequenceResult = new(damageCategory);
                for (int i = 0; i < Structures.Count; i++)
                {
                    aggregateConsequenceResult.IncrementConsequence(structureParallelCollection[j, i], contentParallelCollection[j, i], otherParallelCollection[j, i], vehicleParallelCollection[j, i]);
                }
                aggregateConsequenceResults.Add(aggregateConsequenceResult);
            }
            return aggregateConsequenceResults;
        }
        #endregion

        public List<string> AreOcctypesValid()
        {
            List<string> errors = new();

            foreach (KeyValuePair<string, OccupancyType> entry in OccTypes)
            {
                if (entry.Value.ErrorLevel > ErrorLevel.Unassigned);
                {
                    errors.Add(entry.Value.GetErrorsFromProperties());
                }
            }
            return errors;
        }
        public void Validate()
        {
            HasErrors = false;
            ErrorLevel = ErrorLevel.Unassigned;
            foreach (OccupancyType occupancyType in OccTypes.Values)
            {
                occupancyType.Validate();
                if (occupancyType.HasErrors)
                {
                    if (occupancyType.ErrorLevel > ErrorLevel)
                    {
                        ErrorLevel = occupancyType.ErrorLevel;
                    }
                    HasErrors = true;
                }
            }
            foreach (Structure structure in Structures)
            {
                structure.Validate();
                if (structure.HasErrors)
                {
                    if (structure.ErrorLevel > ErrorLevel) { ErrorLevel = structure.ErrorLevel; }
                    HasErrors = true;
                }
            }
        }
        public string GetErrorsFromProperties()
        {

            string errors = "";
            foreach (OccupancyType occupancyType in OccTypes.Values)
            {
                errors += occupancyType.GetErrorMessages(ErrorLevel.Unassigned, "Occupancy Type" + occupancyType.Name) + Environment.NewLine;
            }
            foreach (Structure structure in Structures)
            {
                errors += structure.GetErrorMessages(ErrorLevel.Unassigned, "Structure" + structure.Fid) + Environment.NewLine;
            }

            return errors;
        }
        internal void ResetStructureWaterIndexTracking()
        {
            foreach (Structure structure in Structures)
            {
                structure.ResetIndexTracking();
            }
        }
    }
}
