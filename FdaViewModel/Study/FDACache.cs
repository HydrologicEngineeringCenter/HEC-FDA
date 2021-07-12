using FdaViewModel.StageTransforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Watershed;
using FdaViewModel.ImpactArea;
using FdaViewModel.WaterSurfaceElevation;
using FdaViewModel.Utilities;
using FdaViewModel.FrequencyRelationships;
using FdaViewModel.FlowTransforms;
using FdaViewModel.GeoTech;
using FdaViewModel.AggregatedStageDamage;
using FdaViewModel.Inventory;
using FdaViewModel.Conditions;
using System.Collections.ObjectModel;
using FdaViewModel.Inventory.OccupancyTypes;
using FdaViewModel.Saving;
using FdaViewModel.AlternativeComparisonReport;
using FdaViewModel.Alternatives;
//using FdaViewModel.Inventory.OccupancyTypes;

namespace FdaViewModel.Study
{
    /// <summary>
    /// The cache is used to hold all the elements in fda in memory. It gets loaded with elements
    /// by the persistence managers when the study opens and then gets modified by the managers. The 
    /// managers are the only ones that can modify the data, but everything else can get the data.
    /// </summary>
    public class FDACache: IStudyCache
    {
        public delegate void AddElementEventHandler(object sender, Saving.ElementAddedEventArgs args);
        public delegate void UpdateElementEventHandler(object sender, Saving.ElementUpdatedEventArgs args);


        //private static FDACache _Instance;

        public event AddElementEventHandler RatingAdded;
        public event AddElementEventHandler TerrainAdded;
        public event AddElementEventHandler ImpactAreaAdded;
        public event AddElementEventHandler WaterSurfaceElevationAdded;
        public event AddElementEventHandler FlowFrequencyAdded;
        public event AddElementEventHandler InflowOutflowAdded;
        public event AddElementEventHandler ExteriorInteriorAdded;
        public event AddElementEventHandler LeveeAdded;
        public event AddElementEventHandler FailureFunctionAdded;
        public event AddElementEventHandler StageDamageAdded;
        public event AddElementEventHandler StructureInventoryAdded;
        public event AddElementEventHandler ConditionsElementAdded;
        public event AddElementEventHandler OccTypeElementAdded;


        public event AddElementEventHandler RatingRemoved;
        public event AddElementEventHandler TerrainRemoved;
        public event AddElementEventHandler ImpactAreaRemoved;
        public event AddElementEventHandler WaterSurfaceElevationRemoved;
        public event AddElementEventHandler FlowFrequencyRemoved;
        public event AddElementEventHandler InflowOutflowRemoved;
        public event AddElementEventHandler ExteriorInteriorRemoved;
        public event AddElementEventHandler LeveeRemoved;
        public event AddElementEventHandler FailureFunctionRemoved;
        public event AddElementEventHandler StageDamageRemoved;
        public event AddElementEventHandler StructureInventoryRemoved;
        public event AddElementEventHandler ConditionsElementRemoved;
        public event AddElementEventHandler OccTypeElementRemoved;


        public event UpdateElementEventHandler RatingUpdated;
        public event UpdateElementEventHandler TerrainUpdated;
        public event UpdateElementEventHandler ImpactAreaUpdated;
        public event UpdateElementEventHandler WaterSurfaceElevationUpdated;
        public event UpdateElementEventHandler FlowFrequencyUpdated;
        public event UpdateElementEventHandler InflowOutflowUpdated;
        public event UpdateElementEventHandler ExteriorInteriorUpdated;
        public event UpdateElementEventHandler LeveeUpdated;
        public event UpdateElementEventHandler FailureFunctionUpdated;
        public event UpdateElementEventHandler StageDamageUpdated;
        public event UpdateElementEventHandler StructureInventoryUpdated;
        public event UpdateElementEventHandler ConditionsElementUpdated;
        public event UpdateElementEventHandler OccTypeElementUpdated;
        public event AddElementEventHandler PlanAdded;
        public event AddElementEventHandler PlanRemoved;
        public event UpdateElementEventHandler PlanUpdated;

        private List<RatingCurveElement> _Ratings = new List<RatingCurveElement>();
        private List<OccupancyTypesElement> _OccTypes = new List<OccupancyTypesElement>();
        private List<TerrainElement> _Terrains = new List<TerrainElement>();
        private List<ImpactAreaElement> _ImpactAreas = new List<ImpactAreaElement>();
        private List<WaterSurfaceElevationElement> _WaterSurfaceElevations = new List<WaterSurfaceElevationElement>();
        private List<AnalyticalFrequencyElement> _FlowFrequencies = new List<AnalyticalFrequencyElement>();
        private List<InflowOutflowElement> _InflowOutflows = new List<InflowOutflowElement>();
        private List<ExteriorInteriorElement> _ExteriorInteriors = new List<ExteriorInteriorElement>();
        private List<LeveeFeatureElement> _Levees = new List<LeveeFeatureElement>();
        private List<FailureFunctionElement> _Failures = new List<FailureFunctionElement>();
        private List<AggregatedStageDamageElement> _StageDamages = new List<AggregatedStageDamageElement>();
        private List<InventoryElement> _Structures = new List<InventoryElement>();
        private List<ConditionsElement> _Conditions = new List<ConditionsElement>();

        #region Properties
        public List<RatingCurveElement> RatingCurveElements { get { return _Ratings; }  }      
        public List<OccupancyTypesElement> OccTypeElements { get { return _OccTypes; } }
        public List<TerrainElement> TerrainElements { get { return _Terrains; } }
        public List<ImpactAreaElement> ImpactAreaElements { get { return _ImpactAreas; } }
        public List<WaterSurfaceElevationElement> WaterSurfaceElements { get { return _WaterSurfaceElevations; } }
        public List<AnalyticalFrequencyElement> FlowFrequencyElements { get { return _FlowFrequencies; } }
        public List<InflowOutflowElement> InflowOutflowElements { get { return _InflowOutflows; } }
        public List<ExteriorInteriorElement> ExteriorInteriorElements { get { return _ExteriorInteriors; } }
        public List<LeveeFeatureElement> LeveeElements { get { return _Levees; } }
        public List<FailureFunctionElement> FailureFunctionElements { get { return _Failures; } }
        public List<AggregatedStageDamageElement> StageDamageElements { get { return _StageDamages; } }
        public List<InventoryElement> StructureInventoryElements { get { return _Structures; } }
        public List<ConditionsElement> ConditionsElements { get { return _Conditions; } }

        #region ParentElements
        public TerrainOwnerElement TerrainParent { get; set; }
        public ImpactAreaOwnerElement ImpactAreaParent { get; set; }
        public AnalyticalFrequencyOwnerElement FlowFrequencyParent { get; set; }
        public InflowOutflowOwnerElement InflowOutflowParent { get; set; }
        public RatingCurveOwnerElement RatingCurveParent { get; set; }
        public ExteriorInteriorOwnerElement ExteriorInteriorParent { get; set; }
        public LeveeFeatureOwnerElement LeveeFeatureParent { get; set; }
        public AggregatedStageDamageOwnerElement StageDamageParent { get; set; }

        public OccupancyTypesOwnerElement OccTypeParent { get; set; }
        public StructureInventoryOwnerElement StructureInventoryParent { get; set; }
        public ConditionsOwnerElement ConditionsParent { get; set; }
        public ConditionsTreeOwnerElement ConditionsTreeParent { get; set; }
        public AltervativeOwnerElement PlansParent { get; set; }
        public AlternativeComparisonReportOwnerElement AlternativeComparisonReportParent { get; set; }
        #endregion
        #endregion
        private FDACache()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static FDACache Create()
        {
            return new FDACache();
        }

        #region Remove Elements

        /// <summary>
        /// Removes the child element from the appropriate list of elements based on its type. 
        /// This will fire a removed event of the appropriate type.        
        /// /// </summary>
        /// <param name="elem">The element to remove</param>
        public void RemoveElement(ChildElement elem, int id)
        {
            Saving.ElementAddedEventArgs elementAddedEventArgs = new Saving.ElementAddedEventArgs(elem);
            elementAddedEventArgs.ID = id;

            if (elem.GetType() == typeof(TerrainElement))
            {
                RemoveElementFromList(TerrainElements, elem);
                //TerrainElements.Remove((TerrainElement)elem);
                TerrainRemoved?.Invoke(this, elementAddedEventArgs);
            }
            if (elem.GetType() == typeof(ImpactAreaElement))
            {
                RemoveElementFromList(ImpactAreaElements, elem);
                //ImpactAreaElements.Remove((ImpactAreaElement)elem);
                ImpactAreaRemoved?.Invoke(this, elementAddedEventArgs);
            }
            if (elem.GetType() == typeof(WaterSurfaceElevationElement))
            {
                RemoveElementFromList(WaterSurfaceElements, elem);
                //WaterSurfaceElements.Remove((WaterSurfaceElevationElement)elem);
                WaterSurfaceElevationRemoved?.Invoke(this, elementAddedEventArgs);
            }
            if (elem.GetType() == typeof(AnalyticalFrequencyElement))
            {
                RemoveElementFromList(FlowFrequencyElements, elem);
                //FlowFrequencyElements.Remove((AnalyticalFrequencyElement)elem);
                FlowFrequencyRemoved?.Invoke(this, elementAddedEventArgs);
            }
            if (elem.GetType() == typeof(InflowOutflowElement))
            {
                RemoveElementFromList(InflowOutflowElements, elem);
                //InflowOutflowElements.Remove((InflowOutflowElement)elem);
                InflowOutflowRemoved?.Invoke(this, elementAddedEventArgs);
            }
            if (elem.GetType() == typeof(RatingCurveElement))
            {
                RemoveElementFromList(RatingCurveElements, elem);
                //RatingCurveElements.Remove((RatingCurveElement)elem);
                RatingRemoved?.Invoke(this, elementAddedEventArgs);
            }
            if (elem.GetType() == typeof(ExteriorInteriorElement))
            {
                RemoveElementFromList(ExteriorInteriorElements, elem);
                //ExteriorInteriorElements.Remove((ExteriorInteriorElement)elem);
                ExteriorInteriorRemoved?.Invoke(this, elementAddedEventArgs);
            }
            if (elem.GetType() == typeof(LeveeFeatureElement))
            {
                RemoveElementFromList(LeveeElements, elem);
                //LeveeElements.Remove((LeveeFeatureElement)elem);
                LeveeRemoved?.Invoke(this, elementAddedEventArgs);
            }
            if (elem.GetType() == typeof(FailureFunctionElement))
            {
                RemoveElementFromList(FailureFunctionElements, elem);
                //FailureFunctionElements.Remove((FailureFunctionElement)elem);
                FailureFunctionRemoved?.Invoke(this, elementAddedEventArgs);
            }
           
            if (elem.GetType() == typeof(OccupancyTypesElement))
            {
                RemoveElementFromList(OccTypeElements, elem);
                //OccTypeElements.Remove((OccupancyTypesElement)elem);
                OccTypeElementRemoved?.Invoke(this, elementAddedEventArgs);
            }
            if (elem.GetType() == typeof(InventoryElement))
            {
                RemoveElementFromList(StructureInventoryElements, elem);
                //StructureInventoryElements.Remove((InventoryElement)elem);
                StructureInventoryRemoved?.Invoke(this, elementAddedEventArgs);
            }
            if (elem.GetType() == typeof(AggregatedStageDamageElement))
            {
                RemoveElementFromList(StageDamageElements, elem);
                //StageDamageElements.Remove((AggregatedStageDamageElement)elem);
                StageDamageRemoved?.Invoke(this, elementAddedEventArgs);
            }
            if (elem.GetType() == typeof(ConditionsElement))
            {
                RemoveElementFromList(ConditionsElements, elem);
                //ConditionsElements.Remove((ConditionsElement)elem);
                ConditionsElementRemoved?.Invoke(this, elementAddedEventArgs);
            }
        }

        private void RemoveElementFromList<T>(List<T> list, ChildElement elem) where T : ChildElement
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Name.Equals(elem.Name))
                {
                    list.RemoveAt(i);
                    return;
                }
            }
        }
        
        #endregion

        #region add elements

        /// <summary>
        /// Adds the child element to the appropriate list of elements based on its type. 
        /// This will fire an added event of the appropriate type.
        /// </summary>
        /// <param name="elem">The child element</param>
        public void AddElement(ChildElement elem)
        {
            if (elem.GetType() == typeof(TerrainElement))
            {
                TerrainElements.Add((TerrainElement)elem);
                TerrainAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
            }
            else if (elem.GetType() == typeof(ImpactAreaElement))
            {
                ImpactAreaElements.Add((ImpactAreaElement)elem);
                ImpactAreaAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
            }
            else if (elem.GetType() == typeof(WaterSurfaceElevationElement))
            {
                WaterSurfaceElements.Add((WaterSurfaceElevationElement)elem);
                WaterSurfaceElevationAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
            }
            else if (elem.GetType() == typeof(AnalyticalFrequencyElement))
            {
                FlowFrequencyElements.Add((AnalyticalFrequencyElement)elem);
                FlowFrequencyAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
            }
            else if (elem.GetType() == typeof(InflowOutflowElement))
            {
                InflowOutflowElements.Add((InflowOutflowElement)elem);
                InflowOutflowAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
            }
            else if (elem.GetType() == typeof(RatingCurveElement))
            {
                RatingCurveElements.Add((RatingCurveElement)elem);
                RatingAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
            }
            else if (elem.GetType() == typeof(ExteriorInteriorElement))
            {
                ExteriorInteriorElements.Add((ExteriorInteriorElement)elem);
                ExteriorInteriorAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
            }
            else if (elem.GetType() == typeof(LeveeFeatureElement))
            {
                LeveeElements.Add((LeveeFeatureElement)elem);
                LeveeAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
            }
            else if (elem.GetType() == typeof(FailureFunctionElement))
            {
                FailureFunctionElements.Add((FailureFunctionElement)elem);
                FailureFunctionAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
            }
            else if (elem.GetType() == typeof(OccupancyTypesElement))
            {
                OccTypeElements.Add((OccupancyTypesElement)elem);
                OccTypeElementAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
            }
            else if (elem.GetType() == typeof(InventoryElement))
            {
                StructureInventoryElements.Add((InventoryElement)elem);
                StructureInventoryAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
            }
            else if (elem.GetType() == typeof(AggregatedStageDamageElement))
            {
                StageDamageElements.Add((AggregatedStageDamageElement)elem);
                StageDamageAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
            }
            else if (elem.GetType() == typeof(ConditionsElement))
            {
                ConditionsElements.Add((ConditionsElement)elem);
                ConditionsElementAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
            }

        }
  
        #endregion


      

        #region UpdateElements

        public void UpdateElement(ChildElement oldElement, ChildElement newElement, int id)
        {
         
            if (oldElement.GetType().Equals(typeof(TerrainElement)))
            {
                UpdateTerrain((TerrainElement) oldElement, (TerrainElement)newElement);
            }
            else if(oldElement.GetType().Equals(typeof(RatingCurveElement)))
            {
                UpdateRatingCurve((RatingCurveElement)oldElement, (RatingCurveElement)newElement);
            }
            else if (oldElement.GetType().Equals(typeof(ImpactAreaElement)))
            {
                UpdateImpactAreaElement((ImpactAreaElement)oldElement, (ImpactAreaElement)newElement);
            }
            else if (oldElement.GetType().Equals(typeof(WaterSurfaceElevationElement)))
            {
                UpdateWaterSurfaceElevationElement((WaterSurfaceElevationElement)oldElement, (WaterSurfaceElevationElement)newElement);
            }
            else if (oldElement.GetType().Equals(typeof(AnalyticalFrequencyElement)))
            {
                UpdateFlowFrequencyElement((AnalyticalFrequencyElement)oldElement, (AnalyticalFrequencyElement)newElement);
            }
            else if (oldElement.GetType().Equals(typeof(InflowOutflowElement)))
            {
                UpdateInflowOutflowElement((InflowOutflowElement)oldElement, (InflowOutflowElement)newElement);
            }
            else if (oldElement.GetType().Equals(typeof(ExteriorInteriorElement)))
            {
                UpdateExteriorInteriorElement((ExteriorInteriorElement)oldElement, (ExteriorInteriorElement)newElement);
            }
            else if (oldElement.GetType().Equals(typeof(LeveeFeatureElement)))
            {
                UpdateLeveeElement((LeveeFeatureElement)oldElement, (LeveeFeatureElement)newElement);
            }
            else if (oldElement.GetType().Equals(typeof(FailureFunctionElement)))
            {
                UpdateFailureFunctionElement((FailureFunctionElement)oldElement, (FailureFunctionElement)newElement);
            }
            else if (oldElement.GetType().Equals(typeof(AggregatedStageDamageElement)))
            {
                UpdateStageDamageElement((AggregatedStageDamageElement)oldElement, (AggregatedStageDamageElement)newElement);
            }
            else if (oldElement.GetType().Equals(typeof(ConditionsElement)))
            {
                UpdateConditionsElement((ConditionsElement)oldElement, (ConditionsElement)newElement);
            }
            else if (oldElement.GetType().Equals(typeof(InventoryElement)))
            {
                UpdateStructureInventoryElement((InventoryElement)oldElement, (InventoryElement)newElement);
            }
        }

        /// <summary>
        /// If an occtype group's list of occtypes changes then we need to update the owner element
        /// so that it has the current 
        /// </summary>
        /// <param name="ID"></param>
        public void UpdateOccTypeGroup(int ID)
        {
            //find the element
            foreach(OccupancyTypesElement ot in _OccTypes )
            {
                if(ot.ID == ID)
                {
                    //call the update event
                    //RatingCurveElements.RemoveAt(index);
                    //RatingCurveElements.Insert(index, newElement);
                    //RatingUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
                }
            }

        }

        public void UpdateTerrain(TerrainElement oldElement, TerrainElement newElement)
        {
            int index = -1;
            for (int i = 0; i < TerrainElements.Count; i++)
            {
                if (TerrainElements[i].Name.Equals(oldElement.Name))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                TerrainElements.RemoveAt(index);
                TerrainElements.Insert(index, newElement);
                TerrainUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
            }
        }
        public void UpdateRatingCurve(RatingCurveElement oldElement, RatingCurveElement newElement)
        {
            int index = -1;
            for(int i = 0;i<RatingCurveElements.Count;i++)
            {

                if(RatingCurveElements[i].Name.Equals(oldElement.Name))
                {
                    index = i;
                    break;
                }
            }
            if(index != -1)
            {
                RatingCurveElements.RemoveAt(index);
                RatingCurveElements.Insert(index, newElement);
                RatingUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
            }
        }
        public void UpdateImpactAreaElement(ImpactAreaElement oldElement, ImpactAreaElement newElement)
        {
            int index = -1;
            for (int i = 0; i < ImpactAreaElements.Count; i++)
            {
                if (ImpactAreaElements[i].Name.Equals(oldElement.Name))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                ImpactAreaElements.RemoveAt(index);
                ImpactAreaElements.Insert(index, newElement);
                ImpactAreaUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
            }
        }
        public void UpdateWaterSurfaceElevationElement(WaterSurfaceElevationElement oldElement, WaterSurfaceElevationElement newElement)
        {
            int index = -1;
            for (int i = 0; i < WaterSurfaceElements.Count; i++)
            {
                if (WaterSurfaceElements[i].Name.Equals(oldElement.Name))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                WaterSurfaceElements.RemoveAt(index);
                WaterSurfaceElements.Insert(index, newElement);
                WaterSurfaceElevationUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
            }
        }
        public void UpdateFlowFrequencyElement(AnalyticalFrequencyElement oldElement, AnalyticalFrequencyElement newElement)
        {
            int index = -1;
            for (int i = 0; i < FlowFrequencyElements.Count; i++)
            {
                if (FlowFrequencyElements[i].Name.Equals(oldElement.Name))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                FlowFrequencyElements.RemoveAt(index);
                FlowFrequencyElements.Insert(index, newElement);
                FlowFrequencyUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
            }
        }
        public void UpdateInflowOutflowElement(InflowOutflowElement oldElement, InflowOutflowElement newElement)
        {
            int index = -1;
            for (int i = 0; i < InflowOutflowElements.Count; i++)
            {
                if (InflowOutflowElements[i].Name.Equals(oldElement.Name))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                InflowOutflowElements.RemoveAt(index);
                InflowOutflowElements.Insert(index, newElement);
                InflowOutflowUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
            }
        }
        public void UpdateExteriorInteriorElement(ExteriorInteriorElement oldElement, ExteriorInteriorElement newElement)
        {
            int index = -1;
            for (int i = 0; i < ExteriorInteriorElements.Count; i++)
            {
                if (ExteriorInteriorElements[i].Name.Equals(oldElement.Name))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                ExteriorInteriorElements.RemoveAt(index);
                ExteriorInteriorElements.Insert(index, newElement);
                ExteriorInteriorUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
            }
        }
        public void UpdateLeveeElement(LeveeFeatureElement oldElement, LeveeFeatureElement newElement)
        {
            int index = -1;
            for (int i = 0; i < LeveeElements.Count; i++)
            {
                if (LeveeElements[i].Name.Equals(oldElement.Name))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                LeveeElements.RemoveAt(index);
                LeveeElements.Insert(index, newElement);
                LeveeUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
            }
        }
        public void UpdateFailureFunctionElement(FailureFunctionElement oldElement, FailureFunctionElement newElement)
        {
            int index = -1;
            for (int i = 0; i < FailureFunctionElements.Count; i++)
            {
                if (FailureFunctionElements[i].Name.Equals(oldElement.Name))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                FailureFunctionElements.RemoveAt(index);
                FailureFunctionElements.Insert(index, newElement);
                FailureFunctionUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
            }
        }
        public void UpdateStageDamageElement(AggregatedStageDamageElement oldElement, AggregatedStageDamageElement newElement)
        {
            int index = -1;
            for (int i = 0; i < StageDamageElements.Count; i++)
            {
                if (StageDamageElements[i].Name.Equals(oldElement.Name))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                StageDamageElements.RemoveAt(index);
                StageDamageElements.Insert(index, newElement);
                StageDamageUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
            }
        }
        public void UpdateConditionsElement(ConditionsElement oldElement, ConditionsElement newElement)
        {
            int index = -1;
            for (int i = 0; i < ConditionsElements.Count; i++)
            {
                if (ConditionsElements[i].Name.Equals(oldElement.Name))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                ConditionsElements.RemoveAt(index);
                ConditionsElements.Insert(index, newElement);
                ConditionsElementUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
            }
        }
        public void UpdateStructureInventoryElement(InventoryElement oldElement, InventoryElement newElement)
        {
            int index = -1;
            for (int i = 0; i < StructureInventoryElements.Count; i++)
            {
                if (StructureInventoryElements[i].Name.Equals(oldElement.Name))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                StructureInventoryElements.RemoveAt(index);
                StructureInventoryElements.Insert(index, newElement);
                StructureInventoryUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
            }
        }

        public void UpdateOccTypeElement(OccupancyTypesElement element)
        {
            int index = -1;
            for (int i = 0; i < OccTypeElements.Count; i++)
            {
                if (OccTypeElements[i].ID == element.ID)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                OccTypeElements.RemoveAt(index);
                OccTypeElements.Insert(index, element);
                //the old element parameter doesn't matter in this case. I only need the new one.
                OccTypeElementUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(element, element));
            }
        }
        #endregion

        #region Rename
        /// <summary>
        /// Gets all the child siblings. Either a child element or a parent element should be passed in. 
        /// If it is a child element then the child's siblings will be returned including itself.
        /// If a parent is passed in then all of its children will be returned.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        //public List<ChildElement> GetSiblingsOfChild(ChildElement element)
        //{
        //    List<ChildElement> retVal = new List<ChildElement>();         
        //    retVal = GetChildElementsOfType<ChildElement>();          
        //    return retVal;
        //}

        public List<ChildElement> GetChildrenOfParent(ParentElement element)
        {
            List<ChildElement> retVal = new List<ChildElement>();
            if (element.GetType() == typeof(TerrainOwnerElement))
            {
                foreach (ChildElement elem in TerrainElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (element.GetType() == typeof(ImpactAreaOwnerElement))
            {
                foreach (ChildElement elem in ImpactAreaElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (element.GetType() == typeof(WaterSurfaceElevationOwnerElement))
            {
                foreach (ChildElement elem in WaterSurfaceElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (element.GetType() == typeof(AnalyticalFrequencyOwnerElement))
            {
                foreach (ChildElement elem in FlowFrequencyElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (element.GetType() == typeof(InflowOutflowOwnerElement))
            {
                foreach (ChildElement elem in InflowOutflowElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (element.GetType() == typeof(RatingCurveOwnerElement))
            {
                foreach (ChildElement elem in RatingCurveElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (element.GetType() == typeof(ExteriorInteriorOwnerElement))
            {
                foreach (ChildElement elem in ExteriorInteriorElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (element.GetType() == typeof(LeveeFeatureOwnerElement))
            {
                foreach (ChildElement elem in LeveeElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (element.GetType() == typeof(FailureFunctionOwnerElement))
            {
                foreach (ChildElement elem in FailureFunctionElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            //if (element.GetType() == typeof(occtype))
            //{
            //    foreach (BaseFdaElement elem in ImpactAreaElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            if (element.GetType() == typeof(StructureInventoryOwnerElement))
            {
                foreach (ChildElement elem in StructureInventoryElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (element.GetType() == typeof(AggregatedStageDamageOwnerElement))
            {
                foreach (ChildElement elem in StageDamageElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (element.GetType() == typeof(ConditionsOwnerElement))
            {
                foreach (ChildElement elem in ConditionsElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            return retVal;

        }

        public T GetParentElementOfType<T>() where T:ParentElement
        {
            var parentType = typeof(T);
            if (parentType == typeof(TerrainOwnerElement))
            {
                return TerrainParent as T;

            }
            if (parentType == typeof(ImpactAreaOwnerElement))
            {
                return ImpactAreaParent as T;
            }
            if (parentType == typeof(WaterSurfaceElevationOwnerElement))
            {

            }
            if (parentType == typeof(AnalyticalFrequencyOwnerElement))
            {
                return FlowFrequencyParent as T;
            }
            if (parentType == typeof(InflowOutflowOwnerElement))
            {
                return InflowOutflowParent as T;
            }
            if (parentType == typeof(RatingCurveOwnerElement))
            {
                return RatingCurveParent as T;
            }
            if (parentType == typeof(ExteriorInteriorOwnerElement))
            {
                return ExteriorInteriorParent as T;
            }
            if (parentType == typeof(LeveeFeatureOwnerElement))
            {
                return LeveeFeatureParent as T;
            }
            //if (parentType == typeof(FailureFunctionOwnerElement))
            //{
                
            //}
            if (parentType == typeof(OccupancyTypesOwnerElement))
            {
                return OccTypeParent as T;
            }
            if (parentType == typeof(StructureInventoryOwnerElement))
            {
                return StructureInventoryParent as T;
            }
            if (parentType == typeof(AggregatedStageDamageOwnerElement))
            {
                return StageDamageParent as T;
            }
            if (parentType == typeof(ConditionsOwnerElement))
            {
                return ConditionsParent as T;
            }
            return null;
        }

        public List<ChildElement> GetChildElementsOfType(Type childElementType)
        {
            List<ChildElement> retVal = new List<ChildElement>();
            if (childElementType == typeof(TerrainElement))
            {
                foreach (ChildElement elem in TerrainElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (childElementType == typeof(ImpactAreaElement))
            {
                foreach (ChildElement elem in ImpactAreaElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (childElementType == typeof(WaterSurfaceElevationElement))
            {
                foreach (ChildElement elem in WaterSurfaceElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (childElementType == typeof(AnalyticalFrequencyElement))
            {
                foreach (ChildElement elem in FlowFrequencyElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (childElementType == typeof(InflowOutflowElement))
            {
                foreach (ChildElement elem in InflowOutflowElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (childElementType.IsAssignableFrom(typeof(RatingCurveElement)))
            {
                foreach (ChildElement elem in RatingCurveElements)
                {
                    retVal.Add(elem );
                }
                return retVal;
            }
            if (childElementType == typeof(ExteriorInteriorElement))
            {
                foreach (ChildElement elem in ExteriorInteriorElements)
                {
                    retVal.Add(elem );
                }
                return retVal;
            }
            if (childElementType == typeof(LeveeFeatureElement))
            {
                foreach (ChildElement elem in LeveeElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (childElementType == typeof(FailureFunctionElement))
            {
                foreach (ChildElement elem in FailureFunctionElements)
                {
                    retVal.Add(elem );
                }
                return retVal;
            }
            if (childElementType == typeof(OccupancyTypesElement))
            {
                foreach (ChildElement elem in OccTypeElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            if (childElementType == typeof(InventoryElement))
            {
                foreach (ChildElement elem in StructureInventoryElements)
                {
                    retVal.Add(elem );
                }
                return retVal;
            }
            if (childElementType == typeof(AggregatedStageDamageElement))
            {
                foreach (ChildElement elem in StageDamageElements)
                {
                    retVal.Add(elem );
                }
                return retVal;
            }
            if (childElementType == typeof(ConditionsElement))
            {
                foreach (ChildElement elem in ConditionsElements)
                {
                    retVal.Add(elem );
                }
                return retVal;
            }
            return retVal;
        }
        public List<T> GetChildElementsOfType<T>() where T : ChildElement
        {
            List<T> retVal = new List<T>();
            var childElementType = typeof(T);
            //typeof(TerrainElement).
            List<ChildElement> children = GetChildElementsOfType(typeof(T));
           foreach(ChildElement ele in children)
            {
                retVal.Add(ele as T);
            }
            return retVal;
        }

        /// <summary>
        /// This is used for the conditions element to get the children that it
        /// needs in order to do a compute.
        /// </summary>
        /// <param name="childElementType"></param>
        /// <param name="ID">The id associated with that element in the database.</param>
        /// <returns></returns>
        public ChildElement GetChildElementOfType(Type childElementType, int ID)
        {
            if (childElementType == typeof(ImpactAreaElement))
            {
                foreach (ChildElement elem in ImpactAreaElements)
                {
                    if(elem.GetElementID() == ID)
                    {
                        return elem;
                    }
                }
            }
            
            if (childElementType == typeof(AnalyticalFrequencyElement))
            {
                foreach (ChildElement elem in FlowFrequencyElements)
                {
                    if (elem.GetElementID() == ID)
                    {
                        return elem;
                    }
                }
            }
            if (childElementType == typeof(InflowOutflowElement))
            {
                foreach (ChildElement elem in InflowOutflowElements)
                {
                    if (elem.GetElementID() == ID)
                    {
                        return elem;
                    }
                }
            }
            if (childElementType.IsAssignableFrom(typeof(RatingCurveElement)))
            {
                foreach (ChildElement elem in RatingCurveElements)
                {
                    if (elem.GetElementID() == ID)
                    {
                        return elem;
                    }
                }
            }
            if (childElementType == typeof(ExteriorInteriorElement))
            {
                foreach (ChildElement elem in ExteriorInteriorElements)
                {
                    if (elem.GetElementID() == ID)
                    {
                        return elem;
                    }
                }
            }
            if (childElementType == typeof(LeveeFeatureElement))
            {
                foreach (ChildElement elem in LeveeElements)
                {
                    if (elem.GetElementID() == ID)
                    {
                        return elem;
                    }
                }
            }
            
           
            
            if (childElementType == typeof(AggregatedStageDamageElement))
            {
                foreach (ChildElement elem in StageDamageElements)
                {
                    if (elem.GetElementID() == ID)
                    {
                        return elem;
                    }
                }
            }
        
            return null;
        }

        #endregion


    }
}
