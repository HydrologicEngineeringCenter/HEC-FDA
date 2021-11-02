using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Saving.PersistenceManagers;
using ViewModel.Watershed;
using ViewModel.StageTransforms;
using ViewModel.ImpactArea;
using ViewModel.WaterSurfaceElevation;
using ViewModel.FrequencyRelationships;

namespace ViewModel.Saving
{
    /// <summary>
    /// Used to get persistence managers for each element. Holds the cache that contains all the elements in memory
    /// </summary>
    public class PersistenceFactory
    {

        /// <summary>
        /// Cache that holds all the elements in memory
        /// </summary>
        public static Study.FDACache StudyCacheForSaving { get; set; }
        /// <summary>
        /// Factory used to create persistence managers for elements in FDA. 
        /// </summary>
        /// <param name="cache">The cache holds all the elements in memory</param>
        private PersistenceFactory()
        {
            
        }
        

        public static IElementManager GetElementManager(Utilities.ChildElement element)
        {

            if (element.GetType() == typeof(TerrainElement))
            {
                return new TerrainElementPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(RatingCurveElement))
            {
                return new RatingElementPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(ExteriorInteriorElement))
            {
                return new ExteriorInteriorPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(ImpactAreaElement))
            {
                return new ImpactAreaPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(WaterSurfaceElevationElement))
            {
                return new WaterSurfaceAreaPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(AnalyticalFrequencyElement))
            {
                return new FlowFrequencyPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(FlowTransforms.InflowOutflowElement))
            {
                return new InflowOutflowPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(GeoTech.LeveeFeatureElement))
            {
                return new LeveePersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(GeoTech.FailureFunctionElement))
            {
                return new FailureFunctionPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(Inventory.InventoryElement))
            {
                return new StructureInventoryPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(AggregatedStageDamage.AggregatedStageDamageElement))
            {
                return new StageDamagePersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(ImpactAreaScenario.IASElementSet))
            {
                return new IASPersistenceManager(StudyCacheForSaving);
            }
           
            return null;
        }

        public static RatingElementPersistenceManager GetRatingManager()
        {
            RatingElementPersistenceManager manager = new RatingElementPersistenceManager(StudyCacheForSaving);
            return manager;
        }
        public static OccTypePersistenceManager GetOccTypeManager()
        {
            OccTypePersistenceManager manager = new OccTypePersistenceManager(StudyCacheForSaving);
            return manager;
        }
        public static StudyPropertiesPersistenceManager GetStudyPropertiesManager()
        {
            StudyPropertiesPersistenceManager manager = new StudyPropertiesPersistenceManager(StudyCacheForSaving);
            return manager;
        }
        public static TerrainElementPersistenceManager GetTerrainManager()
        {
            return new TerrainElementPersistenceManager(StudyCacheForSaving);
        }
        public static  ImpactAreaPersistenceManager GetImpactAreaManager( )
        {
            return new ImpactAreaPersistenceManager(StudyCacheForSaving);
        }
        public static WaterSurfaceAreaPersistenceManager GetWaterSurfaceManager( )
        {
            return new WaterSurfaceAreaPersistenceManager(StudyCacheForSaving);
        }
        public static FlowFrequencyPersistenceManager GetFlowFrequencyManager( )
        {
            return new FlowFrequencyPersistenceManager(StudyCacheForSaving);
        }
        public static InflowOutflowPersistenceManager GetInflowOutflowManager( )
        {
            return new InflowOutflowPersistenceManager(StudyCacheForSaving);
        }
        public static ExteriorInteriorPersistenceManager GetExteriorInteriorManager( )
        {
            return new ExteriorInteriorPersistenceManager(StudyCacheForSaving);
        }
        public static LeveePersistenceManager GetLeveeManager( )
        {
            return new LeveePersistenceManager(StudyCacheForSaving);
        }
        public static FailureFunctionPersistenceManager GetFailureFunctionManager( )
        {
            return new FailureFunctionPersistenceManager(StudyCacheForSaving);
        }
        public static StageDamagePersistenceManager GetStageDamageManager( )
        {
            return new StageDamagePersistenceManager(StudyCacheForSaving);
        }
        public static StructureInventoryPersistenceManager GetStructureInventoryManager( )
        {
            return new StructureInventoryPersistenceManager(StudyCacheForSaving);
        }
        public static IASPersistenceManager GetIASManager( )
        {
            return new IASPersistenceManager(StudyCacheForSaving);
        }
        public static AlternativePersistenceManager GetAlternativeManager()
        {
            return new AlternativePersistenceManager(StudyCacheForSaving);
        }
    }
}
