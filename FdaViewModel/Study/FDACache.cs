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
        public AggregatedStageDamageOwnerElement StageDamageParent { get; set; }
        public OccupancyTypesOwnerElement OccTypeParent { get; set; }

        public ConditionsOwnerElement ConditionsParent { get; set; }
        public ConditionsTreeOwnerElement ConditionsTreeParent { get; set; }
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
            //FDACache cr = new FDACache();

            return new FDACache();
        }

        #region Remove Elements

        public void RemoveElement(ChildElement element)
        {
            //if (element.GetType() == typeof(TerrainElement))
            //{
            //    foreach (BaseFdaElement elem in TerrainElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(ImpactAreaElement))
            //{
            //    foreach (BaseFdaElement elem in ImpactAreaElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(WaterSurfaceElevationElement))
            //{
            //    foreach (BaseFdaElement elem in WaterSurfaceElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(AnalyticalFrequencyElement))
            //{
            //    foreach (BaseFdaElement elem in FlowFrequencyElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(InflowOutflowElement))
            //{
            //    foreach (BaseFdaElement elem in InflowOutflowElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(RatingCurveElement))
            //{
            //    foreach (BaseFdaElement elem in RatingCurveElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(ExteriorInteriorElement))
            //{
            //    foreach (BaseFdaElement elem in ExteriorInteriorElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(LeveeFeatureElement))
            //{
            //    foreach (BaseFdaElement elem in LeveeElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(FailureFunctionElement))
            //{
            //    foreach (BaseFdaElement elem in FailureFunctionElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            ////if (element.GetType() == typeof(Inventory.OccupancyTypes.OccupancyTypesElement))
            ////{
            ////    foreach (BaseFdaElement elem in o)
            ////    {
            ////        retVal.Add(elem);
            ////    }
            ////    return retVal;
            ////}
            //if (element.GetType() == typeof(InventoryElement))
            //{
            //    foreach (BaseFdaElement elem in StructureInventoryElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(AggregatedStageDamageElement))
            //{
            //    foreach (BaseFdaElement elem in StageDamageElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(ConditionsElement))
            //{
            //    foreach (BaseFdaElement elem in ConditionsElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}

        }


        public void RemoveRatingElement(RatingCurveElement elem)
        {
            foreach(RatingCurveElement element in RatingCurveElements)
            {
                if(element.Name.Equals(elem.Name))
                {
                    RatingCurveElements.Remove(element);
                    RatingRemoved?.Invoke(this, new Saving.ElementAddedEventArgs(element));
                    return;
                }
            }
        }
        public void RemoveInflowOutflowElement(InflowOutflowElement elem)
        {
            InflowOutflowElements.Remove(elem);
            InflowOutflowRemoved?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void RemoveExteriorInteriorElement(ExteriorInteriorElement elem)
        {
            ExteriorInteriorElements.Remove(elem);
            ExteriorInteriorRemoved?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void RemoveStageDamageElement(AggregatedStageDamageElement elem)
        {
            StageDamageElements.Remove(elem);
            StageDamageRemoved?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void RemoveFailureFunctionElement(FailureFunctionElement elem)
        {
            FailureFunctionElements.Remove(elem);
            FailureFunctionRemoved?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void RemoveFlowFrequencyElement(AnalyticalFrequencyElement elem)
        {
            FlowFrequencyElements.Remove(elem);
            FlowFrequencyRemoved?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void RemoveLeveeElement(LeveeFeatureElement elem)
        {
            LeveeElements.Remove(elem);
            LeveeRemoved?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void RemoveStructureInventoryElement(InventoryElement elem)
        {
            StructureInventoryElements.Remove(elem);
            StructureInventoryRemoved?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }

        public void RemoveWaterSurfaceElevationElement(WaterSurfaceElevationElement elem)
        {
            WaterSurfaceElements.Remove(elem);
            WaterSurfaceElevationRemoved?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void RemoveConditionsElement(ConditionsElement elem)
        {
            ConditionsElements.Remove(elem);
            ConditionsElementRemoved?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void RemoveImpactAreaElement(ImpactAreaElement elem)
        {
            ImpactAreaElements.Remove(elem);
            ImpactAreaRemoved?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void RemoveTerrainElement(TerrainElement elem)
        {
            TerrainElements.Remove(elem);
            TerrainRemoved?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void RemoveOccTypeElement(OccupancyTypesElement elem)
        {
            OccTypeElements.Remove(elem);
            OccTypeElementRemoved?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        #endregion

        #region add elements

        public void AddElement(ChildElement element)
        {
            //if (element.GetType() == typeof(TerrainElement))
            //{
            //    foreach (BaseFdaElement elem in TerrainElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(ImpactAreaElement))
            //{
            //    foreach (BaseFdaElement elem in ImpactAreaElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(WaterSurfaceElevationElement))
            //{
            //    foreach (BaseFdaElement elem in WaterSurfaceElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(AnalyticalFrequencyElement))
            //{
            //    foreach (BaseFdaElement elem in FlowFrequencyElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(InflowOutflowElement))
            //{
            //    foreach (BaseFdaElement elem in InflowOutflowElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(RatingCurveElement))
            //{
            //    foreach (BaseFdaElement elem in RatingCurveElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(ExteriorInteriorElement))
            //{
            //    foreach (BaseFdaElement elem in ExteriorInteriorElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(LeveeFeatureElement))
            //{
            //    foreach (BaseFdaElement elem in LeveeElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(FailureFunctionElement))
            //{
            //    foreach (BaseFdaElement elem in FailureFunctionElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            ////if (element.GetType() == typeof(Inventory.OccupancyTypes.OccupancyTypesElement))
            ////{
            ////    foreach (BaseFdaElement elem in o)
            ////    {
            ////        retVal.Add(elem);
            ////    }
            ////    return retVal;
            ////}
            //if (element.GetType() == typeof(InventoryElement))
            //{
            //    foreach (BaseFdaElement elem in StructureInventoryElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(AggregatedStageDamageElement))
            //{
            //    foreach (BaseFdaElement elem in StageDamageElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}
            //if (element.GetType() == typeof(ConditionsElement))
            //{
            //    foreach (BaseFdaElement elem in ConditionsElements)
            //    {
            //        retVal.Add(elem);
            //    }
            //    return retVal;
            //}

        }
        public void AddOccTypesElement(OccupancyTypesElement elem)
        {
            OccTypeElements.Add(elem);
            OccTypeElementAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void AddRatingElement(RatingCurveElement elem)
        {
            RatingCurveElements.Add(elem);
            RatingAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }

        public void AddTerrainElement(TerrainElement elem)
        {
            TerrainElements.Add(elem);
            TerrainAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }

        public void AddImpactAreaElement(ImpactAreaElement elem)
        {
            ImpactAreaElements.Add(elem);
            ImpactAreaAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }

        public void AddWaterSurfaceElevationElement(WaterSurfaceElevationElement elem)
        {
            WaterSurfaceElements.Add(elem);
            WaterSurfaceElevationAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }

        public void AddFlowFrequencyElement(AnalyticalFrequencyElement elem)
        {
            FlowFrequencyElements.Add(elem);
            FlowFrequencyAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void AddInflowOutflowElement(InflowOutflowElement elem)
        {
            InflowOutflowElements.Add(elem);
            InflowOutflowAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void AddExteriorInteriorElement(ExteriorInteriorElement elem)
        {
            ExteriorInteriorElements.Add(elem);
            ExteriorInteriorAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void AddLeveeElement(LeveeFeatureElement elem)
        {
            LeveeElements.Add(elem);
            LeveeAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void AddFailureFunctionElement(FailureFunctionElement elem)
        {
            FailureFunctionElements.Add(elem);
            FailureFunctionAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void AddStageDamageElement(AggregatedStageDamageElement elem)
        {
            StageDamageElements.Add(elem);
            StageDamageAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void AddStructureInventoryElement(InventoryElement elem)
        {
            StructureInventoryElements.Add(elem);
            StructureInventoryAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        public void AddConditionsElement(ConditionsElement elem)
        {
            ConditionsElements.Add(elem);
            ConditionsElementAdded?.Invoke(this, new Saving.ElementAddedEventArgs(elem));
        }
        #endregion


      

        #region UpdateElements


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

        //public void UpdateOccTypeElement(InventoryElement oldElement, InventoryElement newElement)
        //{
        //    int index = -1;
        //    for (int i = 0; i < StructureInventoryElements.Count; i++)
        //    {
        //        if (StructureInventoryElements[i].Name.Equals(oldElement.Name))
        //        {
        //            index = i;
        //            break;
        //        }
        //    }
        //    if (index != -1)
        //    {
        //        StructureInventoryElements.RemoveAt(index);
        //        StructureInventoryElements.Insert(index, newElement);
        //        StructureInventoryUpdated?.Invoke(this, new Saving.ElementUpdatedEventArgs(oldElement, newElement));
        //    }
        //}
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

            }
            if (parentType == typeof(FailureFunctionOwnerElement))
            {

            }
            if (parentType == typeof(OccupancyTypesOwnerElement))
            {
                return OccTypeParent as T;
            }
            if (parentType == typeof(StructureInventoryOwnerElement))
            {

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
                    retVal.Add(elem );
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

        
        #endregion

    }
}
