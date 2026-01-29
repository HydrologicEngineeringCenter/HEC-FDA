using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.AlternativeComparisonReport;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.Hydraulics.SteadyHDF;
using HEC.FDA.ViewModel.Hydraulics.UnsteadyHDF;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.LifeLoss;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.ViewModel.Study
{
    /// <summary>
    /// The cache is used to hold all the elements in fda in memory. It gets loaded with elements
    /// by the persistence managers when the study opens and then gets modified by the managers. The 
    /// managers are the only ones that can modify the data, but everything else can get the data.
    /// </summary>
    public class FDACache : IStudyCache
    {
        public delegate void AddElementEventHandler(object sender, ElementAddedEventArgs args);
        public delegate void UpdateElementEventHandler(object sender, ElementUpdatedEventArgs args);

        public event AddElementEventHandler RatingAdded;
        public event AddElementEventHandler TerrainAdded;
        public event AddElementEventHandler ImpactAreaAdded;
        public event AddElementEventHandler IndexPointsAdded;
        public event AddElementEventHandler WaterSurfaceElevationAdded;
        public event AddElementEventHandler FlowFrequencyAdded;
        public event AddElementEventHandler InflowOutflowAdded;
        public event AddElementEventHandler ExteriorInteriorAdded;
        public event AddElementEventHandler LeveeAdded;
        public event AddElementEventHandler StageDamageAdded;
        public event AddElementEventHandler StructureInventoryAdded;
        public event AddElementEventHandler StageLifeLossAdded;
        public event AddElementEventHandler IASElementAdded;
        public event AddElementEventHandler AlternativeAdded;
        public event AddElementEventHandler OccTypeElementAdded;
        public event AddElementEventHandler AlternativeCompReportAdded;

        public event AddElementEventHandler RatingRemoved;
        public event AddElementEventHandler TerrainRemoved;
        public event AddElementEventHandler ImpactAreaRemoved;
        public event AddElementEventHandler IndexPointsRemoved;
        public event AddElementEventHandler WaterSurfaceElevationRemoved;
        public event AddElementEventHandler FlowFrequencyRemoved;
        public event AddElementEventHandler InflowOutflowRemoved;
        public event AddElementEventHandler ExteriorInteriorRemoved;
        public event AddElementEventHandler LeveeRemoved;
        public event AddElementEventHandler StageDamageRemoved;
        public event AddElementEventHandler StructureInventoryRemoved;
        public event AddElementEventHandler StageLifeLossRemoved;
        public event AddElementEventHandler IASElementRemoved;
        public event AddElementEventHandler AlternativeRemoved;
        public event AddElementEventHandler OccTypeElementRemoved;
        public event AddElementEventHandler AlternativeCompReportRemoved;

        public event UpdateElementEventHandler RatingUpdated;
        public event UpdateElementEventHandler TerrainUpdated;
        public event UpdateElementEventHandler ImpactAreaUpdated;
        public event UpdateElementEventHandler IndexPointsUpdated;
        public event UpdateElementEventHandler WaterSurfaceElevationUpdated;
        public event UpdateElementEventHandler FlowFrequencyUpdated;
        public event UpdateElementEventHandler InflowOutflowUpdated;
        public event UpdateElementEventHandler ExteriorInteriorUpdated;
        public event UpdateElementEventHandler LeveeUpdated;
        public event UpdateElementEventHandler StageDamageUpdated;
        public event UpdateElementEventHandler StructureInventoryUpdated;
        public event UpdateElementEventHandler StageLifeLossUpdated;
        public event UpdateElementEventHandler IASElementUpdated;
        public event UpdateElementEventHandler OccTypeElementUpdated;
        public event UpdateElementEventHandler AlternativeUpdated;
        public event UpdateElementEventHandler AlternativeCompReportUpdated;

        #region Properties
        public List<StageDischargeElement> RatingCurveElements { get; } = new List<StageDischargeElement>();
        public List<OccupancyTypesElement> OccTypeElements { get; } = new List<OccupancyTypesElement>();
        public List<TerrainElement> TerrainElements { get; } = new List<TerrainElement>();
        public List<ImpactAreaElement> ImpactAreaElements { get; } = new List<ImpactAreaElement>();
        public List<IndexPointsElement> IndexPointsChildElements { get; } = new List<IndexPointsElement>();
        public List<HydraulicElement> WaterSurfaceElements { get; } = new List<HydraulicElement>();
        public List<FrequencyElement> FlowFrequencyElements { get; } = new List<FrequencyElement>();
        public List<InflowOutflowElement> InflowOutflowElements { get; } = new List<InflowOutflowElement>();
        public List<ExteriorInteriorElement> ExteriorInteriorElements { get; } = new List<ExteriorInteriorElement>();
        public List<LateralStructureElement> LeveeElements { get; } = new List<LateralStructureElement>();
        public List<AggregatedStageDamageElement> StageDamageElements { get; } = new List<AggregatedStageDamageElement>();
        public List<InventoryElement> StructureInventoryElements { get; } = new List<InventoryElement>();
        public List<StageLifeLossElement> StageLifeLossElements { get; } = new List<StageLifeLossElement>();
        public List<IASElement> IASElementSets { get; } = new List<IASElement>();
        public List<AlternativeElement> AlternativeElements { get; } = new List<AlternativeElement>();
        public List<AlternativeComparisonReportElement> AlternativeCompReports { get; } = new List<AlternativeComparisonReportElement>();
        public StudyPropertiesElement StudyPropertiesElement { get; set; }

        #region ParentElements
        public TerrainOwnerElement TerrainParent { get; set; }
        public ImpactAreaOwnerElement ImpactAreaParent { get; set; }
        public InflowOutflowOwnerElement InflowOutflowParent { get; set; }
        public RatingCurveOwnerElement RatingCurveParent { get; set; }
        public ExteriorInteriorOwnerElement ExteriorInteriorParent { get; set; }
        public LeveeFeatureOwnerElement LeveeFeatureParent { get; set; }
        public AggregatedStageDamageOwnerElement StageDamageParent { get; set; }
        public OccupancyTypesOwnerElement OccTypeParent { get; set; }
        public StructureInventoryOwnerElement StructureInventoryParent { get; set; }
        public LifeLossOwnerElement LifeLossParent { get; set; }
        public IASOwnerElement IASParent { get; set; }
        public AlternativeOwnerElement AlternativeParent { get; set; }
        public AlternativeComparisonReportOwnerElement AlternativeComparisonReportParent { get; set; }
        #endregion

        #endregion
        public FDACache()
        {
        }

        #region Remove Elements

        /// <summary>
        /// Removes the child element from the appropriate list of elements based on its type. 
        /// This will fire a removed event of the appropriate type.        
        /// /// </summary>
        /// <param name="elem">The element to remove</param>
        public void RemoveElement(ChildElement elem)
        {
            ElementAddedEventArgs elementAddedEventArgs = new ElementAddedEventArgs(elem);

            if (elem.GetType() == typeof(TerrainElement))
            {
                RemoveElementFromList(TerrainElements, elem);
                TerrainRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(ImpactAreaElement))
            {
                RemoveElementFromList(ImpactAreaElements, elem);
                ImpactAreaRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(IndexPointsElement))
            {
                RemoveElementFromList(IndexPointsChildElements, elem);
                IndexPointsRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(HydraulicElement))
            {
                RemoveElementFromList(WaterSurfaceElements, elem);
                WaterSurfaceElevationRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(FrequencyElement))
            {
                RemoveElementFromList(FlowFrequencyElements, elem);
                FlowFrequencyRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(InflowOutflowElement))
            {
                RemoveElementFromList(InflowOutflowElements, elem);
                InflowOutflowRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(StageDischargeElement))
            {
                RemoveElementFromList(RatingCurveElements, elem);
                RatingRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(ExteriorInteriorElement))
            {
                RemoveElementFromList(ExteriorInteriorElements, elem);
                ExteriorInteriorRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(LateralStructureElement))
            {
                RemoveElementFromList(LeveeElements, elem);
                LeveeRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(OccupancyTypesElement))
            {
                RemoveElementFromList(OccTypeElements, elem);
                OccTypeElementRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(InventoryElement))
            {
                RemoveElementFromList(StructureInventoryElements, elem);
                StructureInventoryRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(AggregatedStageDamageElement))
            {
                RemoveElementFromList(StageDamageElements, elem);
                StageDamageRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(StageLifeLossElement))
            {
                RemoveElementFromList(StageLifeLossElements, elem);
                StageLifeLossRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(IASElement))
            {
                RemoveElementFromList(IASElementSets, elem);
                IASElementRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem.GetType() == typeof(AlternativeElement))
            {
                RemoveElementFromList(AlternativeElements, elem);
                AlternativeRemoved?.Invoke(this, elementAddedEventArgs);
            }
            else if (elem is AlternativeComparisonReportElement)
            {
                RemoveElementFromList(AlternativeCompReports, elem);
                AlternativeCompReportRemoved?.Invoke(this, elementAddedEventArgs);
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
            if (elem is TerrainElement)
            {
                TerrainElements.Add((TerrainElement)elem);
                TerrainAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is IndexPointsElement)
            {
                IndexPointsChildElements.Add((IndexPointsElement)elem);
                IndexPointsAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is ImpactAreaElement)
            {
                ImpactAreaElements.Add((ImpactAreaElement)elem);
                ImpactAreaAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is HydraulicElement)
            {
                WaterSurfaceElements.Add((HydraulicElement)elem);
                WaterSurfaceElevationAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is FrequencyElement)
            {
                FlowFrequencyElements.Add((FrequencyElement)elem);
                FlowFrequencyAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is InflowOutflowElement)
            {
                InflowOutflowElements.Add((InflowOutflowElement)elem);
                InflowOutflowAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is StageDischargeElement)
            {
                RatingCurveElements.Add((StageDischargeElement)elem);
                RatingAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is ExteriorInteriorElement)
            {
                ExteriorInteriorElements.Add((ExteriorInteriorElement)elem);
                ExteriorInteriorAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is LateralStructureElement)
            {
                LeveeElements.Add((LateralStructureElement)elem);
                LeveeAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is OccupancyTypesElement)
            {
                OccTypeElements.Add((OccupancyTypesElement)elem);
                OccTypeElementAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is InventoryElement)
            {
                StructureInventoryElements.Add((InventoryElement)elem);
                StructureInventoryAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is AggregatedStageDamageElement)
            {
                StageDamageElements.Add((AggregatedStageDamageElement)elem);
                StageDamageAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is StageLifeLossElement)
            {
                StageLifeLossElements.Add((StageLifeLossElement)elem);
                StageLifeLossAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is IASElement)
            {
                IASElementSets.Add((IASElement)elem);
                IASElementAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem.GetType() == typeof(AlternativeElement))
            {
                AlternativeElements.Add((AlternativeElement)elem);
                AlternativeAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is AlternativeComparisonReportElement)
            {
                AlternativeCompReports.Add((AlternativeComparisonReportElement)elem);
                AlternativeCompReportAdded?.Invoke(this, new ElementAddedEventArgs(elem));
            }
            else if (elem is StudyPropertiesElement element)
            {
                StudyPropertiesElement = element;
            }
        }
        #endregion


        #region UpdateElements

        public void UpdateElement(ChildElement newElement)
        {
            if (newElement is TerrainElement)
            {
                UpdateTerrain((TerrainElement)newElement);
            }
            else if (newElement is StageDischargeElement)
            {
                UpdateRatingCurve((StageDischargeElement)newElement);
            }
            else if (newElement is ImpactAreaElement)
            {
                UpdateImpactAreaElement((ImpactAreaElement)newElement);
            }
            else if (newElement is IndexPointsElement)
            {
                UpdateIndexPointsElement((IndexPointsElement)newElement);
            }
            else if (newElement is HydraulicElement)
            {
                UpdateWaterSurfaceElevationElement((HydraulicElement)newElement);
            }
            else if (newElement is FrequencyElement)
            {
                UpdateFlowFrequencyElement((FrequencyElement)newElement);
            }
            else if (newElement is InflowOutflowElement)
            {
                UpdateInflowOutflowElement((InflowOutflowElement)newElement);
            }
            else if (newElement is ExteriorInteriorElement)
            {
                UpdateExteriorInteriorElement((ExteriorInteriorElement)newElement);
            }
            else if (newElement is LateralStructureElement)
            {
                UpdateLeveeElement((LateralStructureElement)newElement);
            }
            else if (newElement is AggregatedStageDamageElement)
            {
                UpdateStageDamageElement((AggregatedStageDamageElement)newElement);
            }
            else if (newElement is StageLifeLossElement)
            {
                UpdateStageLifeLossElement((StageLifeLossElement)newElement);
            }
            else if (newElement is IASElement)
            {
                UpdateIASElement((IASElement)newElement);
            }
            else if (newElement.GetType().Equals(typeof(AlternativeElement)))
            {
                UpdateAlternativeElement((AlternativeElement)newElement);
            }
            else if (newElement.GetType().Equals(typeof(InventoryElement)))
            {
                UpdateStructureInventoryElement((InventoryElement)newElement);
            }
            else if (newElement is AlternativeComparisonReportElement)
            {
                UpdateAlternativeCompReportElement((AlternativeComparisonReportElement)newElement);
            }
            else if (newElement is StudyPropertiesElement)
            {
                StudyPropertiesElement = (StudyPropertiesElement)newElement;
            }
            else if (newElement.GetType().Equals(typeof(OccupancyTypesElement)))
            {
                UpdateOccTypeElement((OccupancyTypesElement)newElement);
            }
        }

        public void UpdateTerrain(TerrainElement newElement)
        {
            int index = TerrainElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                TerrainElements[index] = newElement;
                TerrainUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }
        public void UpdateRatingCurve(StageDischargeElement newElement)
        {
            int index = RatingCurveElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                RatingCurveElements[index] = newElement;
                RatingUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }
        public void UpdateImpactAreaElement(ImpactAreaElement newElement)
        {
            int index = ImpactAreaElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                ImpactAreaElements[index] = newElement;
                ImpactAreaUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }

        public void UpdateIndexPointsElement(IndexPointsElement newElement)
        {
            int index = IndexPointsChildElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                IndexPointsChildElements[index] = newElement;
                IndexPointsUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }

        public void UpdateWaterSurfaceElevationElement(HydraulicElement newElement)
        {
            int index = WaterSurfaceElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                WaterSurfaceElements[index] = newElement;
                WaterSurfaceElevationUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }
        public void UpdateFlowFrequencyElement(FrequencyElement newElement)
        {
            int index = FlowFrequencyElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                FlowFrequencyElements[index] = newElement;
                FlowFrequencyUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }
        public void UpdateInflowOutflowElement(InflowOutflowElement newElement)
        {
            int index = InflowOutflowElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                InflowOutflowElements[index] = newElement;
                InflowOutflowUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }
        public void UpdateExteriorInteriorElement(ExteriorInteriorElement newElement)
        {
            int index = ExteriorInteriorElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                ExteriorInteriorElements[index] = newElement;
                ExteriorInteriorUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }
        public void UpdateLeveeElement(LateralStructureElement newElement)
        {
            int index = LeveeElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                LeveeElements[index] = newElement;
                LeveeUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }

        public void UpdateStageDamageElement(AggregatedStageDamageElement newElement)
        {
            int index = StageDamageElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                StageDamageElements[index] = newElement;
                StageDamageUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }

        public void UpdateStageLifeLossElement(StageLifeLossElement newElement)
        {
            int index = StageLifeLossElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                StageLifeLossElements[index] = newElement;
                StageLifeLossUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }

        public void UpdateIASElement(IASElement newElement)
        {
            int index = IASElementSets.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                IASElementSets[index] = newElement;
                IASElementUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }
        public void UpdateAlternativeElement(AlternativeElement newElement)
        {
            int index = AlternativeElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                AlternativeElements[index] = newElement;
                AlternativeUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }

        public void UpdateAlternativeCompReportElement(AlternativeComparisonReportElement newElement)
        {
            int index = AlternativeCompReports.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                AlternativeCompReports[index] = newElement;
                AlternativeCompReportUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }

        public void UpdateStructureInventoryElement(InventoryElement newElement)
        {
            int index = StructureInventoryElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                StructureInventoryElements[index] = newElement;
                StructureInventoryUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }

        public void UpdateOccTypeElement(OccupancyTypesElement newElement)
        {
            int index = OccTypeElements.FindIndex(elem => elem.ID == newElement.ID);
            if (index != -1)
            {
                OccTypeElements[index] = newElement;
                OccTypeElementUpdated?.Invoke(this, new ElementUpdatedEventArgs(newElement));
            }
        }
        #endregion

        #region Rename


        public List<ChildElement> GetChildrenOfParent(ParentElement element)
        {
            List<ChildElement> retVal = new List<ChildElement>();
            if (element is TerrainOwnerElement)
            {
                retVal.AddRange(TerrainElements);
            }
            else if (element is ImpactAreaOwnerElement)
            {
                retVal.AddRange(ImpactAreaElements);
            }
            else if (element is IndexPointsOwnerElement)
            {
                retVal.AddRange(IndexPointsChildElements);
            }
            else if (element is GriddedDataOwnerElement)
            {
                retVal.AddRange(WaterSurfaceElements);
            }
            else if (element is UnsteadyHDFOwnerElement)
            {
                retVal.AddRange(WaterSurfaceElements);
            }
            else if (element is SteadyHDFOwnerElement)
            {
                retVal.AddRange(WaterSurfaceElements);
            }
            else if (element is InflowOutflowOwnerElement)
            {
                retVal.AddRange(InflowOutflowElements);
            }
            else if (element is RatingCurveOwnerElement)
            {
                retVal.AddRange(RatingCurveElements);
            }
            else if (element is ExteriorInteriorOwnerElement)
            {
                retVal.AddRange(ExteriorInteriorElements);
            }
            else if (element is LeveeFeatureOwnerElement)
            {
                retVal.AddRange(LeveeElements);
            }
            else if (element is StructureInventoryOwnerElement)
            {
                retVal.AddRange(StructureInventoryElements);
            }
            else if (element is AggregatedStageDamageOwnerElement)
            {
                retVal.AddRange(StageDamageElements);
            }
            else if (element is LifeLossOwnerElement)
            {
                retVal.AddRange(StageLifeLossElements);
            }
            else if (element is IASOwnerElement)
            {
                retVal.AddRange(IASElementSets);
            }
            else if (element.GetType() == typeof(AlternativeOwnerElement))
            {
                foreach (ChildElement elem in AlternativeElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            return retVal;

        }

        public T GetParentElementOfType<T>() where T : ParentElement
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
            if (parentType == typeof(HydraulicsOwnerElement))
            {
                //todo: is this needed? Doesn't look like it.
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
            if (parentType == typeof(LifeLossOwnerElement))
            {
                return LifeLossParent as T;
            }
            if (parentType == typeof(IASOwnerElement))
            {
                return IASParent as T;
            }
            return null;
        }

        public List<ChildElement> GetChildElementsOfType(Type childElementType)
        {
            List<ChildElement> retVal = new List<ChildElement>();
            if (childElementType == typeof(TerrainElement))
            {
                retVal.AddRange(TerrainElements);
            }
            else if (childElementType == typeof(ImpactAreaElement))
            {
                retVal.AddRange(ImpactAreaElements);
            }
            else if (childElementType == typeof(IndexPointsElement))
            {
                retVal.AddRange(IndexPointsChildElements);
            }
            else if (childElementType == typeof(HydraulicElement))
            {
                retVal.AddRange(WaterSurfaceElements);
            }
            else if (childElementType == typeof(FrequencyElement))
            {
                retVal.AddRange(FlowFrequencyElements);
            }
            else if (childElementType == typeof(InflowOutflowElement))
            {
                retVal.AddRange(InflowOutflowElements);
            }
            else if (childElementType.IsAssignableFrom(typeof(StageDischargeElement)))
            {
                retVal.AddRange(RatingCurveElements);
            }
            else if (childElementType == typeof(ExteriorInteriorElement))
            {
                retVal.AddRange(ExteriorInteriorElements);
            }
            else if (childElementType == typeof(LateralStructureElement))
            {
                retVal.AddRange(LeveeElements);
            }
            else if (childElementType == typeof(OccupancyTypesElement))
            {
                retVal.AddRange(OccTypeElements);
            }
            else if (childElementType == typeof(InventoryElement))
            {
                retVal.AddRange(StructureInventoryElements);
            }
            else if (childElementType == typeof(AggregatedStageDamageElement))
            {
                retVal.AddRange(StageDamageElements);
            }
            else if (childElementType == typeof(StageLifeLossElement))
            {
                retVal.AddRange(StageLifeLossElements);
            }
            else if (childElementType == typeof(IASElement))
            {
                retVal.AddRange(IASElementSets);
            }
            if (childElementType == typeof(AlternativeElement))
            {
                foreach (ChildElement elem in AlternativeElements)
                {
                    retVal.Add(elem);
                }
                return retVal;
            }
            else if (childElementType == typeof(AlternativeComparisonReportElement))
            {
                retVal.AddRange(AlternativeCompReports);
            }
            else if (childElementType == typeof(StudyPropertiesElement))
            {
                retVal.Add(StudyPropertiesElement);
            }
            return retVal;
        }
        public List<T> GetChildElementsOfType<T>() where T : ChildElement
        {
            List<T> retVal = new List<T>();
            var childElementType = typeof(T);
            List<ChildElement> children = GetChildElementsOfType(typeof(T));
            foreach (ChildElement ele in children)
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
            ChildElement childElem = null;
            if (childElementType == typeof(ImpactAreaElement))
            {
                childElem = ImpactAreaElements.Where(elem => elem.ID == ID).FirstOrDefault();
            }
            else if (childElementType == typeof(IndexPointsElement))
            {
                childElem = IndexPointsChildElements.Where(elem => elem.ID == ID).FirstOrDefault();
            }
            else if (childElementType == typeof(FrequencyElement))
            {
                childElem = FlowFrequencyElements.Where(elem => elem.ID == ID).FirstOrDefault();
            }
            else if (childElementType == typeof(InflowOutflowElement))
            {
                childElem = InflowOutflowElements.Where(elem => elem.ID == ID).FirstOrDefault();
            }
            else if (childElementType.IsAssignableFrom(typeof(StageDischargeElement)))
            {
                childElem = RatingCurveElements.Where(elem => elem.ID == ID).FirstOrDefault();
            }
            else if (childElementType == typeof(ExteriorInteriorElement))
            {
                childElem = ExteriorInteriorElements.Where(elem => elem.ID == ID).FirstOrDefault();
            }
            else if (childElementType == typeof(LateralStructureElement))
            {
                childElem = LeveeElements.Where(elem => elem.ID == ID).FirstOrDefault();
            }
            else if (childElementType == typeof(AggregatedStageDamageElement))
            {
                childElem = StageDamageElements.Where(elem => elem.ID == ID).FirstOrDefault();
            }
            else if (childElementType == typeof(StageLifeLossElement))
            {
                childElem = StageLifeLossElements.Where(elem => elem.ID == ID).FirstOrDefault();
            }
            else if (childElementType == typeof(IASElement))
            {
                childElem = IASElementSets.Where(elem => elem.ID == ID).FirstOrDefault();
            }
            if (childElementType == typeof(AlternativeElement))
            {
                foreach (ChildElement elem in AlternativeElements)
                {
                    if (elem.ID == ID)
                    {
                        return elem;
                    }
                }
            }
            else if (childElementType == typeof(AlternativeComparisonReportElement))
            {
                childElem = AlternativeCompReports.Where(elem => elem.ID == ID).FirstOrDefault();
            }
            return childElem;
        }

        #endregion

        public StudyPropertiesElement GetStudyPropertiesElement()
        {
            if (StudyPropertiesElement != null)
            {
                return StudyPropertiesElement;
            }
            else
            {
                throw new MemberAccessException("A study properties element does not exist in the cache.");
            }
        }
    }
}
