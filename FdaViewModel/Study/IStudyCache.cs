using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Study
{
    public interface IStudyCache
    {
        List<ChildElement> GetChildElementsOfType(Type elementType);
        List<ChildElement> GetChildrenOfParent(ParentElement element);
        List<T> GetChildElementsOfType<T>() where T : ChildElement ;
        T GetParentElementOfType<T>() where T : ParentElement;
        ChildElement GetChildElementOfType(Type type, int ID);

        event FDACache.AddElementEventHandler PlanAdded;

        event FDACache.AddElementEventHandler RatingAdded;
        event FDACache.AddElementEventHandler TerrainAdded;
        event FDACache.AddElementEventHandler ImpactAreaAdded;
        event FDACache.AddElementEventHandler WaterSurfaceElevationAdded;
        event FDACache.AddElementEventHandler FlowFrequencyAdded;
        event FDACache.AddElementEventHandler InflowOutflowAdded;
        event FDACache.AddElementEventHandler ExteriorInteriorAdded;
        event FDACache.AddElementEventHandler LeveeAdded;
        event FDACache.AddElementEventHandler FailureFunctionAdded;
        event FDACache.AddElementEventHandler StageDamageAdded;
        event FDACache.AddElementEventHandler StructureInventoryAdded;
        event FDACache.AddElementEventHandler ConditionsElementAdded;
        event FDACache.AddElementEventHandler OccTypeElementAdded;

        event FDACache.AddElementEventHandler PlanRemoved;
        event FDACache.AddElementEventHandler RatingRemoved;
        event FDACache.AddElementEventHandler TerrainRemoved;
        event FDACache.AddElementEventHandler ImpactAreaRemoved;
        event FDACache.AddElementEventHandler WaterSurfaceElevationRemoved;
        event FDACache.AddElementEventHandler FlowFrequencyRemoved;
        event FDACache.AddElementEventHandler InflowOutflowRemoved;
        event FDACache.AddElementEventHandler ExteriorInteriorRemoved;
        event FDACache.AddElementEventHandler LeveeRemoved;
        event FDACache.AddElementEventHandler FailureFunctionRemoved;
        event FDACache.AddElementEventHandler StageDamageRemoved;
        event FDACache.AddElementEventHandler StructureInventoryRemoved;
        event FDACache.AddElementEventHandler ConditionsElementRemoved;
        event FDACache.AddElementEventHandler OccTypeElementRemoved;

        event FDACache.UpdateElementEventHandler PlanUpdated;
        event FDACache.UpdateElementEventHandler RatingUpdated;
        event FDACache.UpdateElementEventHandler TerrainUpdated;
        event FDACache.UpdateElementEventHandler ImpactAreaUpdated;
        event FDACache.UpdateElementEventHandler WaterSurfaceElevationUpdated;
        event FDACache.UpdateElementEventHandler FlowFrequencyUpdated;
        event FDACache.UpdateElementEventHandler InflowOutflowUpdated;
        event FDACache.UpdateElementEventHandler ExteriorInteriorUpdated;
        event FDACache.UpdateElementEventHandler LeveeUpdated;
        event FDACache.UpdateElementEventHandler FailureFunctionUpdated;
        event FDACache.UpdateElementEventHandler StageDamageUpdated;
        event FDACache.UpdateElementEventHandler StructureInventoryUpdated;
        event FDACache.UpdateElementEventHandler ConditionsElementUpdated;
        event FDACache.UpdateElementEventHandler OccTypeElementUpdated;

    }
}
