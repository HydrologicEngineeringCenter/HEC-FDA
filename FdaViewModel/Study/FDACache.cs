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

namespace FdaViewModel.Study
{
    public class FDACache
    {
        public delegate void AddElementEventHandler(object sender, Saving.ElementAddedEventArgs args);

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

        private FDACache()
        {

        }

        public static FDACache Create()
        {
            //FDACache cr = new FDACache();

                return new FDACache();

        }

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
        }

        #region load elements
        private void LoadRatings()
        {
            List<Utilities.ChildElement> ratings = Saving.PersistenceFactory.GetRatingManager(this).Load();

            foreach (RatingCurveElement elem in ratings)
            {
                AddRatingElement(elem);
            }
        }

        private void LoadTerrains()
        {
            List<Utilities.ChildElement> terrains = Saving.PersistenceFactory.GetTerrainManager(this).Load();

            foreach (TerrainElement elem in terrains)
            {
                AddTerrainElement(elem);
            }
        }
        private void LoadImpactAreas()
        {
            List<Utilities.ChildElement> impAreas = Saving.PersistenceFactory.GetImpactAreaManager(this).Load();

            foreach (ImpactAreaElement elem in impAreas)
            {
                AddImpactAreaElement(elem);
            }
        }

        private void LoadWaterSurfaceElevations()
        {
            List<Utilities.ChildElement> waterSurfaceElevs = Saving.PersistenceFactory.GetWaterSurfaceManager(this).Load();

            foreach (WaterSurfaceElevationElement elem in waterSurfaceElevs)
            {
                AddWaterSurfaceElevationElement(elem);
            }
        }
        private void LoadFlowFrequencies()
        {
            List<Utilities.ChildElement> flowFreqs = Saving.PersistenceFactory.GetFlowFrequencyManager(this).Load();

            foreach (AnalyticalFrequencyElement elem in flowFreqs)
            {
                AddFlowFrequencyElement(elem);
            }
        }
        private void LoadInflowOutflows()
        {
            List<Utilities.ChildElement> inflowOutflows = Saving.PersistenceFactory.GetInflowOutflowManager(this).Load();

            foreach (InflowOutflowElement elem in inflowOutflows)
            {
                AddInflowOutflowElement(elem);
            }
        }

        private void LoadExteriorInteriors()
        {
            List<Utilities.ChildElement> exteriorInteriors = Saving.PersistenceFactory.GetExteriorInteriorManager(this).Load();

            foreach (ExteriorInteriorElement elem in exteriorInteriors)
            {
                AddExteriorInteriorElement(elem);
            }
        }
        private void LoadLevees()
        {
            List<Utilities.ChildElement> levees = Saving.PersistenceFactory.GetLeveeManager(this).Load();

            foreach (LeveeFeatureElement elem in levees)
            {
                AddLeveeElement(elem);
            }
        }

        private void LoadFailureFunctions()
        {
            List<Utilities.ChildElement> failures = Saving.PersistenceFactory.GetFailureFunctionManager(this).Load();

            foreach (FailureFunctionElement elem in failures)
            {
                AddFailureFunctionElement(elem);
            }
        }
        private void LoadStageDamages()
        {
            List<Utilities.ChildElement> stageDamages = Saving.PersistenceFactory.GetStageDamageManager(this).Load();

            foreach (AggregatedStageDamageElement elem in stageDamages)
            {
                AddStageDamageElement(elem);
            }
        }
        private void LoadStructureInventories()
        {
            List<Utilities.ChildElement> structures = Saving.PersistenceFactory.GetStructureInventoryManager(this).Load();

            foreach (InventoryElement elem in structures)
            {
                AddStructureInventoryElement(elem);
            }
        }
        #endregion


        #region add elements

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
        #endregion


        #region Rename

        public List<BaseFdaElement> GetSiblings(BaseFdaElement element)
        {
            List<BaseFdaElement> retVal = new List<BaseFdaElement>();

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
                if (element.GetType() == typeof(RatingCurveElement))
                {
                    foreach (BaseFdaElement elem in RatingCurveElements)
                    {
                        retVal.Add(elem);
                    }
                    return retVal;
                }

            }
            return new List<BaseFdaElement>();
        }


        /// <summary>
        /// This is used to add rules that the name cannot be the same as a sibling. 
        /// This method is to be called for editors. It will exclude the original name
        /// from the list of banned words.
        /// </summary>
        /// <param name="editorVM"></param>
        /// <param name="element"></param>
        public void AddSiblingRules(BaseViewModel editorVM, ChildElement element)
        {         
            string originalName = element.Name;
           
            List<string> existingElements = new List<string>();
            foreach (BaseFdaElement elem in GetSiblings(element))
            {
                if (elem.Name.Equals(originalName))
                {
                    continue;
                }
                else
                {
                    existingElements.Add(elem.Name);
                }
            }

            foreach (string existingName in existingElements)
            {
                editorVM.AddRule(nameof(editorVM.Name), () =>
                {
                    return editorVM.Name != existingName;
                }, "This name is already used. Names must be unique.");
            }

        }

        /// <summary>
        /// This is used to add rules that the name cannot be the same as a sibling. 
        /// This method is to be called for importers. 
        /// </summary>
        /// <param name="editorVM"></param>
        /// <param name="element"></param>
        public void AddSiblingRules(BaseViewModel editorVM, ParentElement element)
        {
            List<string> existingElements = new List<string>();
            foreach (BaseFdaElement elem in GetSiblings(element))
            {
                    existingElements.Add(elem.Name);
            }

            foreach (string existingName in existingElements)
            {
                editorVM.AddRule(nameof(editorVM.Name), () =>
                {
                    return editorVM.Name != existingName;
                }, "This name is already used. Names must be unique.");
            }

        }


        public void RenameElement(BaseFdaElement element)
        {
            if(element.GetType() == typeof(StageTransforms.RatingCurveElement))
            {
                //foreach(rat
            }
        }


        #endregion

    }
}
