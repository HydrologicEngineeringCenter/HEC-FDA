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

namespace FdaViewModel.Study
{
    public class FDACache
    {
        public delegate void AddElementEventHandler(object sender, Saving.ElementAddedEventArgs args);
        public delegate void UpdateElementEventHandler(object sender, Saving.ElementUpdatedEventArgs args);


        //private static FDACache _Instance;

        public AddElementEventHandler RatingAdded;
        public AddElementEventHandler TerrainAdded;
        public AddElementEventHandler ImpactAreaAdded;
        public AddElementEventHandler WaterSurfaceElevationAdded;
        public AddElementEventHandler FlowFrequencyAdded;
        public AddElementEventHandler InflowOutflowAdded;
        public AddElementEventHandler ExteriorInteriorAdded;
        public AddElementEventHandler LeveeAdded;
        public AddElementEventHandler FailureFunctionAdded;
        public AddElementEventHandler StageDamageAdded;
        public AddElementEventHandler StructureInventoryAdded;
        public AddElementEventHandler ConditionsElementAdded;

        public AddElementEventHandler RatingRemoved;
        public AddElementEventHandler TerrainRemoved;
        public AddElementEventHandler ImpactAreaRemoved;
        public AddElementEventHandler WaterSurfaceElevationRemoved;
        public AddElementEventHandler FlowFrequencyRemoved;
        public AddElementEventHandler InflowOutflowRemoved;
        public AddElementEventHandler ExteriorInteriorRemoved;
        public AddElementEventHandler LeveeRemoved;
        public AddElementEventHandler FailureFunctionRemoved;
        public AddElementEventHandler StageDamageRemoved;
        public AddElementEventHandler StructureInventoryRemoved;
        public AddElementEventHandler ConditionsElementRemoved;

        public UpdateElementEventHandler RatingUpdated;
        public UpdateElementEventHandler TerrainUpdated;
        public UpdateElementEventHandler ImpactAreaUpdated;
        public UpdateElementEventHandler WaterSurfaceElevationUpdated;
        public UpdateElementEventHandler FlowFrequencyUpdated;
        public UpdateElementEventHandler InflowOutflowUpdated;
        public UpdateElementEventHandler ExteriorInteriorUpdated;
        public UpdateElementEventHandler LeveeUpdated;
        public UpdateElementEventHandler FailureFunctionUpdated;
        public UpdateElementEventHandler StageDamageUpdated;
        public UpdateElementEventHandler StructureInventoryUpdated;
        public UpdateElementEventHandler ConditionsElementUpdated;

        private List<RatingCurveElement> _Ratings = new List<RatingCurveElement>();
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


        public List<RatingCurveElement> RatingCurveElements { get { return _Ratings; }  }
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
        public ConditionsOwnerElement ConditionsParent { get; set; }
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

        //public static FDACache Instance
        //{
        //    get
        //    {
        //        if(_Instance == null)
        //        {
        //            _Instance = new FDACache();
                    
        //        }
        //        return _Instance;
        //    }
        //}

        public void LoadFDACache()
        {
            LoadRatings();
            LoadTerrains();
            LoadImpactAreas();
            LoadWaterSurfaceElevations();
            LoadFlowFrequencies();
            LoadInflowOutflows();
            LoadExteriorInteriors();
            LoadLevees();
            LoadFailureFunctions();
            LoadStageDamages();
            LoadStructureInventories();
            LoadConditions();
        }

        #region load elements
        private void LoadRatings()
        {
            List<Utilities.ChildElement> ratings = Saving.PersistenceFactory.GetRatingManager().Load();

            foreach (RatingCurveElement elem in ratings)
            {
                AddRatingElement(elem);
            }
        }

        private void LoadTerrains()
        {
            List<Utilities.ChildElement> terrains = Saving.PersistenceFactory.GetTerrainManager().Load();

            foreach (TerrainElement elem in terrains)
            {
                AddTerrainElement(elem);
            }
        }
        private void LoadImpactAreas()
        {
            List<Utilities.ChildElement> impAreas = Saving.PersistenceFactory.GetImpactAreaManager().Load();

            foreach (ImpactAreaElement elem in impAreas)
            {
                AddImpactAreaElement(elem);
            }
        }

        private void LoadWaterSurfaceElevations()
        {
            List<Utilities.ChildElement> waterSurfaceElevs = Saving.PersistenceFactory.GetWaterSurfaceManager().Load();

            foreach (WaterSurfaceElevationElement elem in waterSurfaceElevs)
            {
                AddWaterSurfaceElevationElement(elem);
            }
        }
        private void LoadFlowFrequencies()
        {
            List<Utilities.ChildElement> flowFreqs = Saving.PersistenceFactory.GetFlowFrequencyManager().Load();

            foreach (AnalyticalFrequencyElement elem in flowFreqs)
            {
                AddFlowFrequencyElement(elem);
            }
        }
        private void LoadInflowOutflows()
        {
            List<Utilities.ChildElement> inflowOutflows = Saving.PersistenceFactory.GetInflowOutflowManager().Load();

            foreach (InflowOutflowElement elem in inflowOutflows)
            {
                AddInflowOutflowElement(elem);
            }
        }

        private void LoadExteriorInteriors()
        {
            List<Utilities.ChildElement> exteriorInteriors = Saving.PersistenceFactory.GetExteriorInteriorManager().Load();

            foreach (ExteriorInteriorElement elem in exteriorInteriors)
            {
                AddExteriorInteriorElement(elem);
            }
        }
        private void LoadLevees()
        {
            List<Utilities.ChildElement> levees = Saving.PersistenceFactory.GetLeveeManager().Load();

            foreach (LeveeFeatureElement elem in levees)
            {
                AddLeveeElement(elem);
            }
        }

        private void LoadFailureFunctions()
        {
            List<Utilities.ChildElement> failures = Saving.PersistenceFactory.GetFailureFunctionManager().Load();

            foreach (FailureFunctionElement elem in failures)
            {
                AddFailureFunctionElement(elem);
            }
        }
        private void LoadStageDamages()
        {
            List<Utilities.ChildElement> stageDamages = Saving.PersistenceFactory.GetStageDamageManager().Load();

            foreach (AggregatedStageDamageElement elem in stageDamages)
            {
                AddStageDamageElement(elem);
            }
        }
        private void LoadStructureInventories()
        {
            List<Utilities.ChildElement> structures = Saving.PersistenceFactory.GetStructureInventoryManager().Load();

            foreach (InventoryElement elem in structures)
            {
                AddStructureInventoryElement(elem);
            }
        }
        private void LoadConditions()
        {
            List<Utilities.ChildElement> conditions = Saving.PersistenceFactory.GetConditionsManager().Load();

            foreach (ConditionsElement elem in conditions)
            {
                AddConditionsElement(elem);
            }
        }
        #endregion

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

        public void UpdateElement(ChildElement element)
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
        #endregion

        #region Rename

        public List<BaseFdaElement> GetSiblings(BaseFdaElement element)
        {
            List<BaseFdaElement> retVal = new List<BaseFdaElement>();
            ////////////////////////////
            //        child elements
            ////////////////////////////
            if(element.GetType().BaseType == typeof(Utilities.ChildElement))
            {
                if (element.GetType() == typeof(TerrainElement))
                {
                    foreach(BaseFdaElement elem in TerrainElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(ImpactAreaElement))
                {
                    foreach (BaseFdaElement elem in ImpactAreaElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(WaterSurfaceElevationElement))
                {
                    foreach (BaseFdaElement elem in WaterSurfaceElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(AnalyticalFrequencyElement))
                {
                    foreach (BaseFdaElement elem in FlowFrequencyElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(InflowOutflowElement))
                {
                    foreach (BaseFdaElement elem in InflowOutflowElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(RatingCurveElement))
                {
                    foreach (BaseFdaElement elem in RatingCurveElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(ExteriorInteriorElement))
                {
                    foreach (BaseFdaElement elem in ExteriorInteriorElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(LeveeFeatureElement))
                {
                    foreach (BaseFdaElement elem in LeveeElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(FailureFunctionElement))
                {
                    foreach (BaseFdaElement elem in FailureFunctionElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                //if (element.GetType() == typeof(Inventory.OccupancyTypes.OccupancyTypesElement))
                //{
                //    foreach (BaseFdaElement elem in o)
                //    {
                //        retVal.Add(elem);
                //    }
                //    return retVal;
                //}
                if (element.GetType() == typeof(InventoryElement))
                {
                    foreach (BaseFdaElement elem in StructureInventoryElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(AggregatedStageDamageElement))
                {
                    foreach (BaseFdaElement elem in StageDamageElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(ConditionsElement))
                {
                    foreach (BaseFdaElement elem in ConditionsElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }

            }
            ////////////////////////////
            //        parent elements
            ////////////////////////////
            else if (element.GetType().BaseType == typeof(Utilities.ParentElement))
            {
                if (element.GetType() == typeof(TerrainOwnerElement))
                {
                    foreach (BaseFdaElement elem in TerrainElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(ImpactAreaOwnerElement))
                {
                    foreach (BaseFdaElement elem in ImpactAreaElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(WaterSurfaceElevationOwnerElement))
                {
                    foreach (BaseFdaElement elem in WaterSurfaceElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(AnalyticalFrequencyOwnerElement))
                {
                    foreach (BaseFdaElement elem in FlowFrequencyElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(InflowOutflowOwnerElement))
                {
                    foreach (BaseFdaElement elem in InflowOutflowElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(RatingCurveOwnerElement))
                {
                    foreach (BaseFdaElement elem in RatingCurveElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(ExteriorInteriorOwnerElement))
                {
                    foreach (BaseFdaElement elem in ExteriorInteriorElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(LeveeFeatureOwnerElement))
                {
                    foreach (BaseFdaElement elem in LeveeElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(FailureFunctionOwnerElement))
                {
                    foreach (BaseFdaElement elem in FailureFunctionElements)
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
                    foreach (BaseFdaElement elem in StructureInventoryElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(AggregatedStageDamageOwnerElement))
                {
                    foreach (BaseFdaElement elem in StageDamageElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (element.GetType() == typeof(ConditionsOwnerElement))
                {
                    foreach (BaseFdaElement elem in ConditionsElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
            }
            return new List<BaseFdaElement>();
        }

        public List<BaseFdaElement> GetElementsOfType(Type T)
        {
            List<BaseFdaElement> retVal = new List<BaseFdaElement>();
            ////////////////////////////
            //        child elements
            ////////////////////////////
            if (T.BaseType == typeof(Utilities.ChildElement))
            {
                if (T == typeof(TerrainElement))
                {
                    foreach (BaseFdaElement elem in TerrainElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(ImpactAreaElement))
                {
                    foreach (BaseFdaElement elem in ImpactAreaElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(WaterSurfaceElevationElement))
                {
                    foreach (BaseFdaElement elem in WaterSurfaceElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(AnalyticalFrequencyElement))
                {
                    foreach (BaseFdaElement elem in FlowFrequencyElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(InflowOutflowElement))
                {
                    foreach (BaseFdaElement elem in InflowOutflowElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(RatingCurveElement))
                {
                    foreach (BaseFdaElement elem in RatingCurveElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(ExteriorInteriorElement))
                {
                    foreach (BaseFdaElement elem in ExteriorInteriorElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(LeveeFeatureElement))
                {
                    foreach (BaseFdaElement elem in LeveeElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(FailureFunctionElement))
                {
                    foreach (BaseFdaElement elem in FailureFunctionElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                //if (element.GetType() == typeof(Inventory.OccupancyTypes.OccupancyTypesElement))
                //{
                //    foreach (BaseFdaElement elem in o)
                //    {
                //        retVal.Add(elem);
                //    }
                //    return retVal;
                //}
                if (T == typeof(InventoryElement))
                {
                    foreach (BaseFdaElement elem in StructureInventoryElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(AggregatedStageDamageElement))
                {
                    foreach (BaseFdaElement elem in StageDamageElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(ConditionsElement))
                {
                    foreach (BaseFdaElement elem in ConditionsElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }

            }
            ////////////////////////////
            //        parent elements
            ////////////////////////////
            else if (T.BaseType == typeof(Utilities.ParentElement))
            {
                if (T == typeof(TerrainOwnerElement))
                {
                    foreach (BaseFdaElement elem in TerrainElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(ImpactAreaOwnerElement))
                {
                    foreach (BaseFdaElement elem in ImpactAreaElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(WaterSurfaceElevationOwnerElement))
                {
                    foreach (BaseFdaElement elem in WaterSurfaceElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(AnalyticalFrequencyOwnerElement))
                {
                    foreach (BaseFdaElement elem in FlowFrequencyElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(InflowOutflowOwnerElement))
                {
                    foreach (BaseFdaElement elem in InflowOutflowElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(RatingCurveOwnerElement))
                {
                    foreach (BaseFdaElement elem in RatingCurveElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(ExteriorInteriorOwnerElement))
                {
                    foreach (BaseFdaElement elem in ExteriorInteriorElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(LeveeFeatureOwnerElement))
                {
                    foreach (BaseFdaElement elem in LeveeElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(FailureFunctionOwnerElement))
                {
                    foreach (BaseFdaElement elem in FailureFunctionElements)
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
                if (T == typeof(StructureInventoryOwnerElement))
                {
                    foreach (BaseFdaElement elem in StructureInventoryElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(AggregatedStageDamageOwnerElement))
                {
                    foreach (BaseFdaElement elem in StageDamageElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
                if (T == typeof(ConditionsOwnerElement))
                {
                    foreach (BaseFdaElement elem in ConditionsElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }
            }
            return new List<BaseFdaElement>();
        }

        ///// <summary>
        ///// This is used to add rules that the name cannot be the same as a sibling. 
        ///// This method is to be called for editors. It will exclude the original name
        ///// from the list of banned words.
        ///// </summary>
        ///// <param name="editorVM"></param>
        ///// <param name="element"></param>
        //public void AddSiblingRules(BaseViewModel editorVM, BaseFdaElement element)
        //{
        //    //child elements need to exclude thier own name from the list of banned words
        //    bool isChild = false;
        //    if (element.GetType().IsSubclassOf(typeof(ChildElement)))
        //    {
        //        isChild = true;
        //    }

        //    List<string> existingElements = new List<string>();
        //    List<BaseFdaElement> siblings = GetSiblings(element);

        //    string originalName = element.Name;

        //    foreach (BaseFdaElement elem in siblings)
        //    {
        //        if (isChild && elem.Name.Equals(originalName))
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            existingElements.Add(elem.Name);
        //        }
        //    }

        //    foreach (string existingName in existingElements)
        //    {
        //        editorVM.AddRule(nameof(editorVM.Name), () =>
        //        {
        //            return editorVM.Name != existingName;
        //        }, "This name is already used. Names must be unique.");
        //    }

        //}

        ///// <summary>
        ///// This is used to add rules that the name cannot be the same as a sibling. 
        ///// This method is to be called for importers. 
        ///// </summary>
        ///// <param name="editorVM"></param>
        ///// <param name="element"></param>
        //public void AddSiblingRules(BaseViewModel editorVM, ParentElement element)
        //{
        //    List<string> existingElements = new List<string>();
        //    foreach (BaseFdaElement elem in GetSiblings(element))
        //    {
        //            existingElements.Add(elem.Name);
        //    }

        //    foreach (string existingName in existingElements)
        //    {
        //        editorVM.AddRule(nameof(editorVM.Name), () =>
        //        {
        //            return editorVM.Name != existingName;
        //        }, "This name is already used. Names must be unique.");
        //    }

        //}


        //public void RenameElement(BaseFdaElement element)
        //{
        //    if(element.GetType() == typeof(StageTransforms.RatingCurveElement))
        //    {
        //        //foreach(rat
        //    }
        //}


        #endregion

    }
}
