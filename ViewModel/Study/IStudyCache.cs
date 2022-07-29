using System;
using System.Collections.Generic;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Study
{
    public interface IStudyCache
    {
        StudyPropertiesElement GetStudyPropertiesElement();
        List<ChildElement> GetChildElementsOfType(Type elementType);
        List<ChildElement> GetChildrenOfParent(ParentElement element);
        List<T> GetChildElementsOfType<T>() where T : ChildElement ;
        T GetParentElementOfType<T>() where T : ParentElement;
        ChildElement GetChildElementOfType(Type type, int ID);

        event FDACache.AddElementEventHandler AlternativeCompReportAdded;
        event FDACache.AddElementEventHandler AlternativeAdded;
        event FDACache.AddElementEventHandler RatingAdded;
        event FDACache.AddElementEventHandler TerrainAdded;
        event FDACache.AddElementEventHandler ImpactAreaAdded;
        event FDACache.AddElementEventHandler IndexPointsAdded;
        event FDACache.AddElementEventHandler WaterSurfaceElevationAdded;
        event FDACache.AddElementEventHandler FlowFrequencyAdded;
        event FDACache.AddElementEventHandler InflowOutflowAdded;
        event FDACache.AddElementEventHandler ExteriorInteriorAdded;
        event FDACache.AddElementEventHandler LeveeAdded;
        event FDACache.AddElementEventHandler StageDamageAdded;
        event FDACache.AddElementEventHandler StructureInventoryAdded;
        event FDACache.AddElementEventHandler IASElementAdded;
        event FDACache.AddElementEventHandler OccTypeElementAdded;

        event FDACache.AddElementEventHandler AlternativeCompReportRemoved;
        event FDACache.AddElementEventHandler AlternativeRemoved;
        event FDACache.AddElementEventHandler RatingRemoved;
        event FDACache.AddElementEventHandler TerrainRemoved;
        event FDACache.AddElementEventHandler ImpactAreaRemoved;
        event FDACache.AddElementEventHandler IndexPointsRemoved;
        event FDACache.AddElementEventHandler WaterSurfaceElevationRemoved;
        event FDACache.AddElementEventHandler FlowFrequencyRemoved;
        event FDACache.AddElementEventHandler InflowOutflowRemoved;
        event FDACache.AddElementEventHandler ExteriorInteriorRemoved;
        event FDACache.AddElementEventHandler LeveeRemoved;
        event FDACache.AddElementEventHandler StageDamageRemoved;
        event FDACache.AddElementEventHandler StructureInventoryRemoved;
        event FDACache.AddElementEventHandler IASElementRemoved;
        event FDACache.AddElementEventHandler OccTypeElementRemoved;

        event FDACache.UpdateElementEventHandler AlternativeCompReportUpdated;
        event FDACache.UpdateElementEventHandler AlternativeUpdated;
        event FDACache.UpdateElementEventHandler RatingUpdated;
        event FDACache.UpdateElementEventHandler TerrainUpdated;
        event FDACache.UpdateElementEventHandler ImpactAreaUpdated;
        event FDACache.UpdateElementEventHandler IndexPointsUpdated;
        event FDACache.UpdateElementEventHandler WaterSurfaceElevationUpdated;
        event FDACache.UpdateElementEventHandler FlowFrequencyUpdated;
        event FDACache.UpdateElementEventHandler InflowOutflowUpdated;
        event FDACache.UpdateElementEventHandler ExteriorInteriorUpdated;
        event FDACache.UpdateElementEventHandler LeveeUpdated;
        event FDACache.UpdateElementEventHandler StageDamageUpdated;
        event FDACache.UpdateElementEventHandler StructureInventoryUpdated;
        event FDACache.UpdateElementEventHandler IASElementUpdated;
        event FDACache.UpdateElementEventHandler OccTypeElementUpdated;

    }
}
