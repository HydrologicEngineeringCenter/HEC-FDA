using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Saving.PersistenceManagers;
using FdaViewModel.Watershed;
using FdaViewModel.StageTransforms;
using FdaViewModel.ImpactArea;
using FdaViewModel.WaterSurfaceElevation;
using FdaViewModel.FrequencyRelationships;

namespace FdaViewModel.Saving
{
    public class PersistenceFactory
    {
        public static Study.FDACache StudyCacheForSaving { get; set; }
        public PersistenceFactory(Study.FDACache cache)
        {
            StudyCacheForSaving = cache;
        }

        public static IPersistable GetElementManager(Utilities.ChildElement element)
        {

            if (element.GetType() == typeof(TerrainElement))
            {
                return new TerrainElementPersistenceManager(StudyCacheForSaving);
            }
            else if(element.GetType() == typeof(RatingCurveElement))
            {
                return new RatingElementPersistenceManager(StudyCacheForSaving);
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
            else if (element.GetType() == typeof(Conditions.ConditionsElement))
            {
                return new ConditionsPersistenceManager(StudyCacheForSaving);
            }
            return null;
        }

        public static RatingElementPersistenceManager GetRatingManager()
        {
            RatingElementPersistenceManager manager = new RatingElementPersistenceManager(StudyCacheForSaving);
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
        public static ConditionsPersistenceManager GetConditionsManager( )
        {
            return new ConditionsPersistenceManager(StudyCacheForSaving);
        }
    }
}
