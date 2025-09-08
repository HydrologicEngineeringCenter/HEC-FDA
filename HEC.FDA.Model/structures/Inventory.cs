using System;
using System.Collections.Generic;
using System.Linq;
using Geospatial.GDALAssist;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.Spatial;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Events;
using HEC.MVVMFramework.Base.Implementations;
using HEC.MVVMFramework.Model.Messaging;
using RasMapperLib;
using Statistics;
using Utility.Logging;

namespace HEC.FDA.Model.structures
{
    //TODO: Figure out how to set Occupany Type Set
    public class Inventory : PropertyValidationHelper, IDontImplementValidationButMyPropertiesDo
    {
        #region Properties
        public List<Structure> Structures { get; } = new List<Structure>();
        //The string key is the occupancy type name 
        public Dictionary<string, OccupancyType> OccTypes { get; set; }
        public double PriceIndex { get; set; }
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
            OperationResult operationResult = StructureFactory.LoadStructuresFromSourceFiles(pointShapefilePath, map, null, false, impactAreaShapefilePath, studyProjection, OccTypes, out List<Structure> structures);
            if (operationResult.Result)
            {
                Structures = structures;
            }
            else
            {
                throw new Exception("Error loading structures from shapefiles" + operationResult.GetConcatenatedMessages());
            }
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
            OperationResult operationResult = StructureFactory.LoadStructuresFromSourceFiles(pointShapefilePath, map, terrainPath, true, impactAreaShapefilePath, studyProjection, OccTypes, out List<Structure> structures);
            if (operationResult.Result)
            {
                Structures = structures;
            }
            else
            {
                throw new Exception("Error loading structures from shapefiles: " + operationResult.GetConcatenatedMessages());
            }
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
            return new Inventory(OccTypes, filteredStructureList, PriceIndex);
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

        internal List<string> StructureDetails(List<DeterministicOccupancyType> deterministicOccupancyTypes, Dictionary<int, string> impactAreaNames)
        {
            string header = Structure.ProduceDetailsHeader();
            List<string> structureDetails = new() { header };
            foreach (Structure structure in Structures)
            {
                structureDetails.Add(structure.ProduceDetails(deterministicOccupancyTypes, PriceIndex, impactAreaNames));
            }
            return structureDetails;
        }

        public void GenerateRandomNumbers(ConvergenceCriteria convergenceCriteria)
        {
            //generate slightly more random numbers than max iterations because it is possible that we keep iterating beyond max 
            //before re-checking for convergence 
            int quantityOfRandomNumbers = Convert.ToInt32(convergenceCriteria.MaxIterations * 2);
            foreach (OccupancyType occupancyType in OccTypes.Values)
            {
                occupancyType.GenerateRandomNumbers(quantityOfRandomNumbers);
            }
        }

        public List<DeterministicOccupancyType> SampleOccupancyTypes(long iteration, bool computeIsDeterministic)
        {
            List<DeterministicOccupancyType> deterministicOccupancyTypes = new();
            foreach (OccupancyType occupancyType in OccTypes.Values)
            {
                DeterministicOccupancyType deterministicOccupancyType = occupancyType.Sample(iteration, computeIsDeterministic);
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

        public event MessageReportedEventHandler MessageReport;

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
                    dt = deterministicOccupancyType[dtIdx];
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
        public void Validate() 
        {
            HasErrors = false;
            ErrorLevel = ErrorLevel.Unassigned;
            foreach (OccupancyType occupancyType in OccTypes.Values)
            {
                occupancyType.Validate();
                if(occupancyType.HasErrors)
                {
                    if(ErrorLevel < occupancyType.ErrorLevel)
                    {
                        ErrorLevel = occupancyType.ErrorLevel;
                    }
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
            if (Structures.Count == 0)
            {
                HasErrors = true;
                ErrorLevel = ErrorLevel.Minor;
            }
        }
        public string GetErrorsFromProperties()
        {
            string errors = "";
            foreach (OccupancyType occupancyType in OccTypes.Values)
            {
                errors += occupancyType.GetErrorsFromProperties();
            }
            foreach (Structure structure in Structures)
            {
                errors += structure.GetErrorMessages(ErrorLevel.Unassigned, "Structure" + structure.Fid);
            }
            return errors;
        }
        public string GetErrorsFromProperties(int impactAreaID)
        {

            string errors = "";
            foreach (OccupancyType occupancyType in OccTypes.Values)
            {
                errors += occupancyType.GetErrorsFromProperties();
            }
            foreach (Structure structure in Structures)
            {
                errors += structure.GetErrorMessages(ErrorLevel.Unassigned, "Structure" + structure.Fid);
            }
            if (Structures.Count == 0)
            {
                errors += $"There are no structures found in the inventory that lie within impact area {impactAreaID}" + Environment.NewLine;
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

        public void ReportMessage(object sender, MessageEventArgs e)
        {
            MessageHub.Register(this);
            MessageReport?.Invoke(sender, e);
            MessageHub.Unregister(this);
        }
    }
}
