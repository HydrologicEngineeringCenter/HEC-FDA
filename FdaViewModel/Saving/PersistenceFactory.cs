using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Saving.PersistenceManagers;
using FdaViewModel.Watershed;

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


            return null;
        }

        public static RatingElementPersistenceManager GetRatingManager(Study.FDACache studyCache)
        {
            return new RatingElementPersistenceManager( studyCache);
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
    }
}
