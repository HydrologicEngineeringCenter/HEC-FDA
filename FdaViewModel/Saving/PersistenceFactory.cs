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


        public static IPersistable GetElementManager(Utilities.ChildElement element, Study.FDACache studyCache)
        {

            if (element.GetType() == typeof(TerrainElement))
            {
                return new TerrainElementPersistenceManager(studyCache);
            }
            else if(element.GetType() == typeof(RatingCurveElement))
            {
                return new RatingElementPersistenceManager(studyCache);
            }
            else if (element.GetType() == typeof(RatingCurveElement))
            {
                return new RatingElementPersistenceManager(studyCache);
            }
            else if (element.GetType() == typeof(ExteriorInteriorElement))
            {
                return new ExteriorInteriorPersistenceManager(studyCache);
            }
            else if (element.GetType() == typeof(ImpactAreaElement))
            {
                return new ImpactAreaPersistenceManager(studyCache);
            }
            else if (element.GetType() == typeof(WaterSurfaceElevationElement))
            {
                return new WaterSurfaceAreaPersistenceManager(studyCache);
            }
            else if (element.GetType() == typeof(AnalyticalFrequencyElement))
            {
                return new FlowFrequencyPersistenceManager(studyCache);
            }
            else if (element.GetType() == typeof(FlowTransforms.InflowOutflowElement))
            {
                return new InflowOutflowPersistenceManager(studyCache);
            }
            else if (element.GetType() == typeof(GeoTech.LeveeFeatureElement))
            {
                return new LeveePersistenceManager(studyCache);
            }
            else if (element.GetType() == typeof(GeoTech.FailureFunctionElement))
            {
                return new FailureFunctionPersistenceManager(studyCache);
            }
            else if (element.GetType() == typeof(Inventory.InventoryElement))
            {
                return new StructureInventoryPersistenceManager(studyCache);
            }
            else if (element.GetType() == typeof(AggregatedStageDamage.AggregatedStageDamageElement))
            {
                return new StageDamagePersistenceManager(studyCache);
            }
            else if (element.GetType() == typeof(Conditions.ConditionsElement))
            {
                return new ConditionsPersistenceManager(studyCache);
            }
            return null;
        }

        public static RatingElementPersistenceManager GetRatingManager(Study.FDACache studyCache)
        {
            RatingElementPersistenceManager manager = new RatingElementPersistenceManager(studyCache);
            return manager;
        }
       
        public static TerrainElementPersistenceManager GetTerrainManager(Study.FDACache studyCache)
        {
            return new TerrainElementPersistenceManager(studyCache);
        }
        public static  ImpactAreaPersistenceManager GetImpactAreaManager(Study.FDACache studyCache)
        {
            return new ImpactAreaPersistenceManager(studyCache);
        }
        public static WaterSurfaceAreaPersistenceManager GetWaterSurfaceManager(Study.FDACache studyCache)
        {
            return new WaterSurfaceAreaPersistenceManager(studyCache);
        }
        public static FlowFrequencyPersistenceManager GetFlowFrequencyManager(Study.FDACache studyCache)
        {
            return new FlowFrequencyPersistenceManager(studyCache);
        }
        public static InflowOutflowPersistenceManager GetInflowOutflowManager(Study.FDACache studyCache)
        {
            return new InflowOutflowPersistenceManager(studyCache);
        }
        public static ExteriorInteriorPersistenceManager GetExteriorInteriorManager(Study.FDACache studyCache)
        {
            return new ExteriorInteriorPersistenceManager(studyCache);
        }
        public static LeveePersistenceManager GetLeveeManager(Study.FDACache studyCache)
        {
            return new LeveePersistenceManager(studyCache);
        }
        public static FailureFunctionPersistenceManager GetFailureFunctionManager(Study.FDACache studyCache)
        {
            return new FailureFunctionPersistenceManager(studyCache);
        }
        public static StageDamagePersistenceManager GetStageDamageManager(Study.FDACache studyCache)
        {
            return new StageDamagePersistenceManager(studyCache);
        }
        public static StructureInventoryPersistenceManager GetStructureInventoryManager(Study.FDACache studyCache)
        {
            return new StructureInventoryPersistenceManager(studyCache);
        }
        public static ConditionsPersistenceManager GetConditionsManager(Study.FDACache studyCache)
        {
            return new ConditionsPersistenceManager(studyCache);
        }
    }
}
