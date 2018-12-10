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

        //void RemoveElement(ChildElement element);
        //void AddElement(ChildElement element);
        //void UpdateElement(ChildElement oldElement, ChildElement newElement);

        List<ChildElement> GetChildElementsOfType(Type elementType);
        List<ChildElement> GetChildrenOfParent(ParentElement element);
        List<T> GetChildElementsOfType<T>() where T : ChildElement ;
        T GetParentElementOfType<T>() where T : ParentElement;
        //Conditions.ConditionsOwnerElement ConditionsParent { get; }

        //right now there will be a bunch of errors surrounding the three events that get attached in the owner's ctors.
        //I can get around all this if I just set the lists of the owners to be the study cach lists. Then the only adding and removing
        //and updating will be done by the saving managers, and i wont have to add anything else to this interface? maybe not?
        //in BaseViewModel, change the FDACache to be an IStudyCache. 
        //I will also need to make these methods in the FDACache that check the type and call the appropriate add, remove, or update.
         //delegate void AddElementEventHandler(object sender, Saving.ElementAddedEventArgs args);
         //delegate void UpdateElementEventHandler(object sender, Saving.ElementUpdatedEventArgs args);


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
