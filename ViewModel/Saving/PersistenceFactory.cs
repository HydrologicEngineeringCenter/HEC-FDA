using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.AlternativeComparisonReport;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.IndexPoints;

namespace HEC.FDA.ViewModel.Saving
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
            else if(element.GetType() == typeof(StudyPropertiesElement))
            {
                manager = new StudyPropertiesPersistenceManager(StudyCacheForSaving);
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
            else if (element.GetType() == typeof(HydraulicElement))
            {
                manager = new HydraulicPersistenceManager(StudyCacheForSaving);
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
            else if(element is IndexPointsElement)
            {
                manager = new IndexPointsPersistenceManager(StudyCacheForSaving);
            }
           
            return manager;
        }

        public static IndexPointsPersistenceManager GetIndexPointsPersistenceManager()
        {
            return new IndexPointsPersistenceManager(StudyCacheForSaving);
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
        public static HydraulicPersistenceManager GetWaterSurfaceManager( )
        {
            return new HydraulicPersistenceManager(StudyCacheForSaving);
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
        public static StudyPropertiesPersistenceManager GetStudyPropertiesPersistenceManager()
        {
            return new StudyPropertiesPersistenceManager(StudyCacheForSaving);
        }
    }
}
