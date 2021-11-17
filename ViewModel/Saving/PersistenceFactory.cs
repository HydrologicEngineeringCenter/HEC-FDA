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
using ViewModel.AlternativeComparisonReport;
using ViewModel.Alternatives;
using ViewModel.ImpactAreaScenario;
using ViewModel.AggregatedStageDamage;
using ViewModel.Inventory;
using ViewModel.GeoTech;
using ViewModel.FlowTransforms;
using ViewModel.Utilities;

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
        

        public static IElementManager GetElementManager(ChildElement element)
        {
            IElementManager manager = null;
            if (element.GetType() == typeof(TerrainElement))
            {
                manager = new TerrainElementPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(RatingCurveElement))
            {
                manager = new RatingElementPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(ExteriorInteriorElement))
            {
                manager = new ExteriorInteriorPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(ImpactAreaElement))
            {
                manager = new ImpactAreaPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(WaterSurfaceElevationElement))
            {
                manager = new WaterSurfaceAreaPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(AnalyticalFrequencyElement))
            {
                manager = new FlowFrequencyPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(InflowOutflowElement))
            {
                manager = new InflowOutflowPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(LeveeFeatureElement))
            {
                manager = new LeveePersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(FailureFunctionElement))
            {
                manager = new FailureFunctionPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(InventoryElement))
            {
                manager = new StructureInventoryPersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(AggregatedStageDamageElement))
            {
                manager = new StageDamagePersistenceManager(StudyCacheForSaving);
            }
            else if (element.GetType() == typeof(IASElementSet))
            {
                manager = new IASPersistenceManager(StudyCacheForSaving);
            }
            else if(element.GetType() == typeof(AlternativeElement))
            {
                manager = new AlternativePersistenceManager(StudyCacheForSaving);
            }
            else if(element is AlternativeComparisonReportElement)
            {
                manager = new AlternativeComparisonReportPersistenceManager(StudyCacheForSaving);
            }
           
            return manager;
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
        public static AlternativeComparisonReportPersistenceManager GetAlternativeCompReportManager()
        {
            return new AlternativeComparisonReportPersistenceManager(StudyCacheForSaving);
        }
    }
}
