using Functions;
using FunctionsView.ViewModel;
using HEC.Plotting.Core;
using HEC.Plotting.SciChart2D.Charts;
using HEC.Plotting.SciChart2D.Controller;
using HEC.Plotting.SciChart2D.DataModel;
using HEC.Plotting.SciChart2D.ViewModel;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModel.AggregatedStageDamage;
using ViewModel.Editors;
using ViewModel.FlowTransforms;
using ViewModel.FrequencyRelationships;
using ViewModel.GeoTech;
using ViewModel.ImpactArea;
using ViewModel.ImpactAreaScenario.Editor.ChartControls;
using ViewModel.StageTransforms;
using ViewModel.Utilities;

namespace ViewModel.ImpactAreaScenario.Editor
{
    public class SpecificIASEditorVM : BaseViewModel
    {
        private IASElement _currentElement;
        private bool _isInEditMode;
        private Chart2DController _controller;
        private AdditionalThresholdsVM _additionalThresholdsVM;
        private ChildElementComboItem _selectedStageDamageElement;
        private List<string> _damageCategories;
        private string _selectedDamageCategory;
        private ChildElementComboItem _selectedFrequencyRelationship;
        private bool _ratingRequired;

        public IASPlotControlVM PlotControlVM { get; } = new IASPlotControlVM();

        public bool RatingRequired
        {
            get { return _ratingRequired; }
            set { _ratingRequired = value; NotifyPropertyChanged(); }
        }

        public int Year { get; set; } = DateTime.Now.Year;


        public List<string> DamageCategories
        {
            get { return _damageCategories; }
            set { _damageCategories = value; NotifyPropertyChanged(); }
        }
        public List<AdditionalThresholdRowItem> Thresholds { get; set; }
        public ObservableCollection<ChildElementComboItem> ImpactAreaElements { get; set; }
        public ObservableCollection<ChildElementComboItem> FrequencyElements { get; set; }
        public ObservableCollection<ChildElementComboItem> InflowOutflowElements { get; set; }
        //public List<RatingCurveElement> RatingCurveElements { get; set; }
        public ObservableCollection<ChildElementComboItem> RatingCurveElements { get; set; }

        public ObservableCollection<ChildElementComboItem> LeveeFeatureElements { get; set; }
        public ObservableCollection<ChildElementComboItem> ExteriorInteriorElements { get; set; }
        public ObservableCollection<ChildElementComboItem> StageDamageElements { get; set; }


        private ChildElementComboItem _selectedImpactAreaElement;
        private ChildElementComboItem _selectedInflowOutflowElement;
        private ChildElementComboItem _selectedRatingCurveElement;
        private ChildElementComboItem _selectedLeveeElement;
        private ChildElementComboItem _selectedExteriorInteriorElement;

        public string SelectedDamageCategory
        {
            get { return _selectedDamageCategory; }
            set { _selectedDamageCategory = value; NotifyPropertyChanged(); }
        }
        //public ChildElementComboItem SelectedImpactAreaElement
        //{
        //    get { return _selectedImpactAreaElement; }
        //    set { _selectedImpactAreaElement = value; NotifyPropertyChanged(); }
        //}
        public ChildElementComboItem SelectedFrequencyElement
        {
            get { return _selectedFrequencyRelationship; }
            set { _selectedFrequencyRelationship = value; UpdateRatingRequired(); }
        }
        public ChildElementComboItem SelectedInflowOutflowElement
        {
            get { return _selectedInflowOutflowElement; }
            set { _selectedInflowOutflowElement = value; NotifyPropertyChanged(); }
        }
        public ChildElementComboItem SelectedRatingCurveElement
        {
            get { return _selectedRatingCurveElement; }
            set { _selectedRatingCurveElement = value; NotifyPropertyChanged(); }
        }
        public ChildElementComboItem SelectedLeveeFeatureElement
        {
            get { return _selectedLeveeElement; }
            set { _selectedLeveeElement = value; NotifyPropertyChanged(); }
        }
        public ChildElementComboItem SelectedExteriorInteriorElement
        {
            get { return _selectedExteriorInteriorElement; }
            set { _selectedExteriorInteriorElement = value; NotifyPropertyChanged(); }
        }
        public ChildElementComboItem SelectedStageDamageElement
        {
            get { return _selectedStageDamageElement; }
            set { _selectedStageDamageElement = value; StageDamageSelectionChanged(); }
        }

       
        public ObservableCollection<RecommendationRowItem> MessageRows { get; set; }

        /// <summary>
        /// This is the create new ctor
        /// </summary>
        public SpecificIASEditorVM(ImpactAreaRowItem rowItem)
        {            
            Initialize();
            Name = rowItem.Name;
            IndexLocationID = rowItem.ID;
        }

        //todo: this ctor probably needs some work
        public SpecificIASEditorVM(IASElement elem, string impactAreaName)
        {
            _currentElement = elem;
            _isInEditMode = true;
            Initialize();
            FillForm(elem);
            IndexLocationID = elem.ImpactAreaID;
            Name = impactAreaName;
        }

        private void Initialize()
        {
            MessageRows = new ObservableCollection<RecommendationRowItem>();
            _additionalThresholdsVM = new AdditionalThresholdsVM();
            _additionalThresholdsVM.RequestNavigation += Navigate;

            Thresholds = new List<AdditionalThresholdRowItem>();

            LoadElements();


            //StudyCache.ImpactAreaAdded += AddImpactAreaElement;
            //StudyCache.ImpactAreaRemoved += RemoveImpactAreaElement;
            //StudyCache.ImpactAreaUpdated += UpdateImpactAreaElement;

            StudyCache.FlowFrequencyAdded += AddFlowFreqElement;
            StudyCache.FlowFrequencyRemoved += RemoveFlowFreqElement;
            StudyCache.FlowFrequencyUpdated += UpdateFlowFreqElement;

            StudyCache.InflowOutflowAdded += AddInOutElement;
            StudyCache.InflowOutflowRemoved += RemoveInOutElement;
            StudyCache.InflowOutflowUpdated += UpdateInOutElement;

            StudyCache.RatingAdded += AddRatingElement;
            StudyCache.RatingRemoved += RemoveRatingElement;
            StudyCache.RatingUpdated += UpdateRatingElement;

            StudyCache.LeveeAdded += AddLeveeElement;
            StudyCache.LeveeRemoved += RemoveLeveeElement;
            StudyCache.LeveeUpdated += UpdateLeveeElement;

            StudyCache.ExteriorInteriorAdded += AddExtIntElement;
            StudyCache.ExteriorInteriorRemoved += RemoveExtIntElement;
            StudyCache.ExteriorInteriorUpdated += UpdateExtIntElement;

            StudyCache.StageDamageAdded += AddStageDamageElement;
            StudyCache.StageDamageRemoved += RemoveStageDamageElement;
            StudyCache.StageDamageUpdated += UpdateStageDamageElement;
        }


        #region Live Update Event Methods

        private void AddStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        {
            StageDamageElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), StageDamageElements);
        }
        private void UpdateStageDamageElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            updateElement(StageDamageElements, SelectedStageDamageElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }

        private void AddExtIntElement(object sender, Saving.ElementAddedEventArgs e)
        {
            ExteriorInteriorElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveExtIntElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), ExteriorInteriorElements);
        }
        private void UpdateExtIntElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            updateElement(ExteriorInteriorElements, SelectedExteriorInteriorElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }

        private void AddLeveeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            LeveeFeatureElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveLeveeElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), LeveeFeatureElements);
        }
        private void UpdateLeveeElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            updateElement(LeveeFeatureElements, SelectedLeveeFeatureElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }

        private void AddInOutElement(object sender, Saving.ElementAddedEventArgs e)
        {
            InflowOutflowElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveInOutElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), InflowOutflowElements);
        }
        private void UpdateInOutElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            updateElement(InflowOutflowElements, SelectedInflowOutflowElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }


        private void AddFlowFreqElement(object sender, Saving.ElementAddedEventArgs e)
        {
            FrequencyElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveFlowFreqElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), FrequencyElements);
        }
        private void UpdateFlowFreqElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            updateElement(FrequencyElements, SelectedFrequencyElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }

        private void AddImpactAreaElement(object sender, Saving.ElementAddedEventArgs e)
        {
            ImpactAreaElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveImpactAreaElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), ImpactAreaElements);
        }
        //private void UpdateImpactAreaElement(object sender, Saving.ElementUpdatedEventArgs e)
        //{
        //    updateElement(ImpactAreaElements, SelectedImpactAreaElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        //}

        private void AddRatingElement(object sender, Saving.ElementAddedEventArgs e)
        {
            RatingCurveElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }
        private void RemoveRatingElement(object sender, Saving.ElementAddedEventArgs e)
        {
            removeElement(((ChildElement)e.Element).GetElementID(), RatingCurveElements);
        }
        private void UpdateRatingElement(object sender, Saving.ElementUpdatedEventArgs e)
        {
            updateElement(RatingCurveElements, SelectedRatingCurveElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        }

        private void removeElement(int idToRemove, ObservableCollection<ChildElementComboItem> collection)
        {
            collection.Remove(collection.Where(elem => elem.ChildElement != null && elem.ChildElement.GetElementID() == idToRemove).Single());
        }

        private void updateElement(ObservableCollection<ChildElementComboItem> collection, ChildElementComboItem selectedItem,
             ChildElement oldElement, ChildElement newElement)
        {
            int idToUpdate = oldElement.GetElementID();

            ChildElementComboItem itemToUpdate = collection.Where(elem => elem.ChildElement != null && elem.ChildElement.GetElementID() == idToUpdate).SingleOrDefault();
            if (itemToUpdate != null)
            {
                int index = collection.IndexOf(itemToUpdate);

                //this was an attempt to update the selected item if that is the one we are swapping out. For some reason
                //this doesn't work. I was trying to find a way to pass the property into this method and was unsuccessful.
                //bool needToUpdateSelected = selectedItem.ChildElement != null && selectedItem.ChildElement.GetElementID() == idToUpdate;

                //if (index != -1)
                //{
                //    collection.RemoveAt(index);
                //    collection.Insert(index, new ChildElementComboItem(newElement));
                //    if (needToUpdateSelected)
                //    {
                //        propToUpdate (collection[index]);
                //    }
                //}
            }
        }

        #endregion

        private void FillForm(IASElement elem)
        {

            FillThresholds(elem);

            //all the available elements have been loaded into this editor. We now want to select
            //the correct element for each dropdown. If we can't find the correct element then the selected elem 
            //will be null.
            //SelectedImpactAreaElement = ImpactAreaElements.FirstOrDefault(imp => imp.ChildElement != null && imp.ChildElement.GetElementID() == elem.ImpactAreaID);
            SelectedFrequencyElement = FrequencyElements.FirstOrDefault(freq => freq.ChildElement != null && freq.ChildElement.GetElementID() == elem.FlowFreqID);
            SelectedInflowOutflowElement = InflowOutflowElements.FirstOrDefault(inf => inf.ChildElement != null && inf.ChildElement.GetElementID() == elem.InflowOutflowID);
            SelectedRatingCurveElement = RatingCurveElements.FirstOrDefault(rat => rat.ChildElement != null && rat.ChildElement.GetElementID() == elem.RatingID);
            SelectedLeveeFeatureElement = LeveeFeatureElements.FirstOrDefault(levee => levee.ChildElement != null && levee.ChildElement.GetElementID() == elem.LeveeFailureID);
            SelectedExteriorInteriorElement = ExteriorInteriorElements.FirstOrDefault(ext => ext.ChildElement != null && ext.ChildElement.GetElementID() == elem.ExtIntStageID);
            SelectedStageDamageElement = StageDamageElements.FirstOrDefault(stage => stage.ChildElement != null && stage.ChildElement.GetElementID() == elem.StageDamageID);

            //i don't want a selected value to ever be null. Even if there are no elements there should be a blank row option.
            //so if it is null, i will set it to the first option which is empty.
            if(SelectedFrequencyElement == null) SelectedFrequencyElement = FrequencyElements[0];
            if (SelectedInflowOutflowElement == null) SelectedInflowOutflowElement = InflowOutflowElements[0];
            if (SelectedRatingCurveElement == null) SelectedRatingCurveElement = RatingCurveElements[0];
            if (SelectedLeveeFeatureElement == null) SelectedLeveeFeatureElement = LeveeFeatureElements[0];
            if (SelectedExteriorInteriorElement == null) SelectedExteriorInteriorElement = ExteriorInteriorElements[0];
            if (SelectedStageDamageElement == null) SelectedStageDamageElement = StageDamageElements[0];

            //todo: plot something?


        }

        private void FillThresholds(IASElement elem)
        {
            //todo: maybe add a different ctor or a fill method to load the rows?
            _additionalThresholdsVM.Rows = new ObservableCollection<AdditionalThresholdRowItem>();
            foreach (AdditionalThresholdRowItem row in elem.Thresholds)
            {
                _additionalThresholdsVM.Rows.Add(row);
                Thresholds.Add(row);
            }
            
        }

        private void LoadElements()
        {
            //what happens if there are no elements for a combo?
            //I will always add an empty ChildElementComboItem and then select it by default.
            //this means that when asking for the selected combo item, it should never be null.
            List<ChildElement> childElems = new List<ChildElement>();

            //List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            //childElems.AddRange(impactAreaElements);
            //ImpactAreaElements = CreateComboItems(childElems);
            //SelectedImpactAreaElement = ImpactAreaElements.First();

            List<AnalyticalFrequencyElement> analyticalFrequencyElements = StudyCache.GetChildElementsOfType<AnalyticalFrequencyElement>();
            childElems.Clear();
            childElems.AddRange(analyticalFrequencyElements);
            FrequencyElements = CreateComboItems(childElems);
            SelectedFrequencyElement = FrequencyElements.First();

            List<InflowOutflowElement> inflowOutflowElements = StudyCache.GetChildElementsOfType<InflowOutflowElement>();
            childElems.Clear();
            childElems.AddRange(inflowOutflowElements);
            InflowOutflowElements = CreateComboItems(childElems);
            SelectedInflowOutflowElement = InflowOutflowElements.First();

            List<RatingCurveElement> ratingCurveElements = StudyCache.GetChildElementsOfType<RatingCurveElement>();
            childElems.Clear();
            childElems.AddRange(ratingCurveElements);
            RatingCurveElements = CreateComboItems(childElems);
            SelectedRatingCurveElement = RatingCurveElements.First();

            List<LeveeFeatureElement> leveeFeatureElements = StudyCache.GetChildElementsOfType<LeveeFeatureElement>();
            childElems.Clear();
            childElems.AddRange(leveeFeatureElements);
            LeveeFeatureElements = CreateComboItems(childElems);
            SelectedLeveeFeatureElement = LeveeFeatureElements.First();

            List<ExteriorInteriorElement> exteriorInteriorElements = StudyCache.GetChildElementsOfType<ExteriorInteriorElement>();
            childElems.Clear();
            childElems.AddRange(exteriorInteriorElements);
            ExteriorInteriorElements = CreateComboItems(childElems);
            SelectedExteriorInteriorElement = ExteriorInteriorElements.First();

            List<AggregatedStageDamageElement> aggregatedStageDamageElements = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            childElems.Clear();
            childElems.AddRange(aggregatedStageDamageElements);
            StageDamageElements = CreateComboItems(childElems);
            SelectedStageDamageElement = StageDamageElements.First();
        }

        private ObservableCollection<ChildElementComboItem> CreateComboItems(List<ChildElement> elems)
        {
            ObservableCollection<ChildElementComboItem> items = new ObservableCollection<ChildElementComboItem>();
            items.Add(new ChildElementComboItem(null));
            foreach (ChildElement elem in elems)
            {
                items.Add(new ChildElementComboItem(elem));
            }

            return items;
        }

        

        private void UpdateRatingRequired()
        {
            if (SelectedFrequencyElement != null && SelectedFrequencyElement.ChildElement != null)
            {
                //todo: we need to check to see if this is a flow freq.
                //if the flow freq is of type flow freq, then the rating curve is required.
                //this is task 5 in the clean document.
                RatingRequired = true;
            }
        }
        private void StageDamageSelectionChanged()
        {
            if (SelectedStageDamageElement != null && SelectedStageDamageElement.ChildElement != null)
            {
                List<string> damCats = new List<string>();

                AggregatedStageDamageElement elem = (AggregatedStageDamageElement)SelectedStageDamageElement.ChildElement;

                foreach (StageDamageCurve curve in elem.Curves)
                {
                    damCats.Add(curve.DamCat);
                }

                DamageCategories = damCats;
                SelectedDamageCategory = damCats[0];
            }
        }

        #region validation

        private FdaValidationResult IsFrequencyRelationshipValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (SelectedFrequencyElement.ChildElement == null)
            {
                vr.IsValid = false;
                vr.ErrorMessage = new StringBuilder("A Frequency Relationship is required.");
            }

            return vr;
        }

        private FdaValidationResult IsRatingCurveValid()
        {
            //todo: the rating curve is required is the frequency relationship is of type
            //flow-frequency. This will need to get added once we complete task 5 in the clean doc.
            FdaValidationResult vr = new FdaValidationResult();
            if (_ratingRequired && SelectedRatingCurveElement.ChildElement == null)
            {
                vr.IsValid = false;
                vr.ErrorMessage = new StringBuilder("A Rating Curve is required when using a flow-frequency relationship.");
            }

            return vr;
        }
        private FdaValidationResult IsStageDamageValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (SelectedStageDamageElement.ChildElement == null)
            {
                vr.IsValid = false;
                vr.ErrorMessage = new StringBuilder("A Stage Damage is required.");
            }

            return vr;
        }

        private FdaValidationResult IsThresholdsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (Thresholds.Count == 0)
            {
                vr.IsValid = false;
                vr.ErrorMessage = new StringBuilder("At least one threshold is required.");
            }

            return vr;
        }



        #endregion


        #region Check overlapping ranges

        private const string INFLOW_OUTFLOW = "Inflow-Outflow";
        private const string RATING = "Rating-Curve";
        private const string EXTERIOR_INTERIOR = "Exterior-Interior";
        private const string STAGE_DAMAGE = "Stage-Damage";
        private const string FLOW = "Flow";
        private const string STAGE = "Stage";

        private void CheckForOverlappingRanges()
        {
            //Note: the following axis determinations are based on this table:
            //       flow freq:  frequency,       inflow
            //inflow - outflow:  inflow,          outflow
            //          Rating:  Outflow,         Exterior Stage
            //       Ext - Int:  Exterior Stage,  Interior Stage
            //    Stage Damage:  Interior Stage,  Damage

            MessageRows.Clear();
            //assume that the big 3 exist by the time we get here.

            bool inflowOutflowSelected = SelectedInflowOutflowElement.ChildElement != null;
            bool extInteriorSelected = SelectedExteriorInteriorElement.ChildElement != null;

            if(inflowOutflowSelected && !extInteriorSelected)
            {
                //check in-out flows with flow freq
                CheckRangeValues(SelectedInflowOutflowElement, SelectedFrequencyElement, true, false, INFLOW_OUTFLOW, FLOW);
                //check outflows with rating flows
                CheckRangeValues(SelectedRatingCurveElement, SelectedInflowOutflowElement, true, false, RATING, FLOW);
                //check rating stages with stage-damage stages
                CheckRangeWithStageDamage((AggregatedStageDamageElement)SelectedStageDamageElement.ChildElement, SelectedRatingCurveElement);

            }
            else if(!inflowOutflowSelected && extInteriorSelected)
            {
                //check rating flows with flow-freq flows
                CheckRangeValues(SelectedRatingCurveElement, SelectedFrequencyElement,true, false, RATING, FLOW);

                //check rating stages with ext-int exterior stages
                CheckRangeValues(SelectedExteriorInteriorElement, SelectedRatingCurveElement, false, true, EXTERIOR_INTERIOR, STAGE);

                //check ext-int interior stages with stage-damage stages.
                CheckRangeWithStageDamage((AggregatedStageDamageElement)SelectedStageDamageElement.ChildElement, SelectedExteriorInteriorElement);

            }
            else if(inflowOutflowSelected && extInteriorSelected)
            {
                //check in-out flows with flow freq
                CheckRangeValues(SelectedInflowOutflowElement, SelectedFrequencyElement, true, false, INFLOW_OUTFLOW, FLOW);

                //check outflows with rating flows
                CheckRangeValues(SelectedRatingCurveElement, SelectedInflowOutflowElement, true, false, RATING, FLOW);

                //check rating stages with ext-int exterior stages
                CheckRangeValues(SelectedExteriorInteriorElement, SelectedRatingCurveElement, false, true, EXTERIOR_INTERIOR, STAGE);

                //check ext-int interior stages with stage-damage stages.
                CheckRangeWithStageDamage((AggregatedStageDamageElement)SelectedStageDamageElement.ChildElement, SelectedExteriorInteriorElement);

            }
            else
            {
                //check rating flows with flow-freq flows
                CheckRangeValues(SelectedRatingCurveElement, SelectedFrequencyElement, true, false, RATING, FLOW);

                //check rating stages with stage-damage stages
                CheckRangeWithStageDamage((AggregatedStageDamageElement)SelectedStageDamageElement.ChildElement, SelectedRatingCurveElement);
            }
        }

        /// <summary>
        /// Dealing with the stage damage element is a little more complicated. The stage damage element has a list of curves. We have to find 
        /// the correct curve based on what damage category is selected by the user.
        /// </summary>
        /// <param name="stageDamElem"></param>
        /// <param name="otherElem"></param>
        /// <param name="compareXAxis"></param>
        private void CheckRangeWithStageDamage(AggregatedStageDamageElement stageDamElem, ChildElementComboItem otherElem)
        {
            //this will always compare the x values of the stage-damage to the y values of the other element.
            double stageDamageMin = -1;
            double stageDamageMax = -1;
            double otherMin = -1;
            double otherMax = -1;

            IFdaFunction otherCurve = otherElem.ChildElement.Curve;
            StageDamageCurve selectedCurve = null;
            foreach (StageDamageCurve curve in stageDamElem.Curves)
            {
                if(curve.DamCat.Equals(SelectedDamageCategory))
                {
                    //I don't think it is possible to not find the correct curve since we are
                    //pulling the selected dam cat name from the curves and are just matching it back up here.
                    selectedCurve = curve;
                   
                    break;
                }
            }
         
                stageDamageMin = selectedCurve.Function.Coordinates.First().X.Value();
                stageDamageMax = selectedCurve.Function.Coordinates.Last().X.Value();

                otherMin = otherCurve.YSeries.Range.Min;
                otherMax = otherCurve.YSeries.Range.Max;           

            AddRecommendationForNonoverlappingRange(stageDamageMin, stageDamageMax, otherMin, otherMax, STAGE_DAMAGE, STAGE, 
                stageDamElem.Name, otherElem.ChildElement.Name);

        }

        /// <summary>
        /// This method finds the non-overlapping regions and creates a message object that gets displayed in the UI. 
        /// </summary>
        /// <param name="element1">This needs to be the curve that is associated with that node in the warnings tree.</param>
        /// <param name="element2"></param>
        /// <param name="compareXAxis"></param>
        /// <param name="headerBase"></param>
        /// <param name="axisLabel"></param>
        private void CheckRangeValues(ChildElementComboItem element1, ChildElementComboItem element2, bool compareXAxisOnElem1, bool compareXAxisOnElem2, string headerBase, string axisLabel)
        {
            ChildElement elem1 = element1.ChildElement;
            ChildElement elem2 = element2.ChildElement;
            string name1 = elem1.Name;
            string name2 = elem2.Name;
            IParameterRange range1 = null;
            IParameterRange range2 = null;
            if(compareXAxisOnElem1)
            {
                range1 = elem1.Curve.XSeries;
            }
            else
            {
                range1 = elem1.Curve.YSeries;
            }
            if(compareXAxisOnElem2)
            {
                range2 = elem2.Curve.XSeries;
            }
            else
            {
                range2 = elem2.Curve.YSeries;
            }

            double min1 = range1.Range.Min;
            double max1 = range1.Range.Max;

            double min2 = range2.Range.Min;
            double max2 = range2.Range.Max;


            AddRecommendationForNonoverlappingRange(min1, max1, min2, max2, headerBase, axisLabel, name1, name2);

        }

        private void AddRecommendationForNonoverlappingRange(double min1, double max1, double min2, double max2, string headerBase, string axisLabel, string name1, string name2)
        {
            RecommendationRowItem ri = new RecommendationRowItem(headerBase + ": " + name1);
            bool nonOverlapMin = false;
            bool nonOverlapMax = false;
            string minRange = "";
            string maxRange = "";

            //todo: apply some min and max rule.
            if (min1 != min2)
            {
                nonOverlapMin = true;
                //i want to display the lowest value first
                minRange = getRangeString(min1, min2);
            }
            if (max1 != max2)
            {
                nonOverlapMax = true;
                maxRange = getRangeString(max1, max2);
            }

            if(nonOverlapMin && nonOverlapMax)
            {
                ri.Messages.Add("Non-overlapping " + axisLabel + " with " + name2 + ": " + minRange + " and " + maxRange);
            }
            else if(nonOverlapMin)
            {
                ri.Messages.Add("Non-overlapping " + axisLabel + " with " + name2 + ": " + minRange);
            }
            else if(nonOverlapMax)
            {
                ri.Messages.Add("Non-overlapping " + axisLabel + " with " + name2 + ": " + maxRange);
            }

            if (ri.Messages.Count > 0)
            {
                MessageRows.Add(ri);
            }
        }

        private string getRangeString(double val1, double val2)
        {
            string retval;
            //i want to display the lowest value first
            bool val1IsLowest = false;
            if(val1<val2)
            {
                val1IsLowest = true;
            }

            //only dispaly 2 decimal places
            val1 = Math.Round(val1, 2);
            val2 = Math.Round(val2, 2);

            if(val1IsLowest)
            {
                retval = "[" + val1 + " - " + val2 + "]";
            }
            else
            {
                retval = "[" + val2 + " - " + val1 + "]";
            }
            return retval;
        }

        private void CheckRatingFlowsAgainstFreqRelationship()
        {
            IParameterRange ySeries = SelectedFrequencyElement.ChildElement.Curve.YSeries;
            double min = ySeries.Range.Min;
            double max = ySeries.Range.Max;
            string flowName = SelectedFrequencyElement.ChildElement.Name;

            IParameterRange ratingYSeries = SelectedRatingCurveElement.ChildElement.Curve.YSeries;
            double ratingMin = ratingYSeries.Range.Min;
            double ratingMax = ratingYSeries.Range.Max;
            string ratName = SelectedRatingCurveElement.ChildElement.Name;
            

            
            RecommendationRowItem ri = new RecommendationRowItem("Frequency Relationship");
            if(min != ratingMin)
            {
                ri.Messages.Add(flowName + " has a minimum flow of " + min + " while " + ratName + " has a minimum flow of " + ratingMin +
                    ". The non-overlapping range will not be used during the compute.");
            }


            if(ri.Messages.Count>0)
            {
                MessageRows.Add(ri);
            }
            
        }


        #endregion


        #region PlotCurves
        public FdaValidationResult IsValid()
        {
            FdaValidationResult vr = new FdaValidationResult();

            vr.AddValidationResult(IsFrequencyRelationshipValid());        
            vr.AddValidationResult( IsRatingCurveValid());
            vr.AddValidationResult( IsStageDamageValid());
            vr.AddValidationResult(IsThresholdsValid());
            //todo: actually run the compute and see if it was successful?

            if(!vr.IsValid)
            {
                vr.InsertNewLineMessage(0, "Errors in Impact Area: " + Name);
            }
            return vr;

        }
        private IFdaFunction getFrequencyRelationshipFunction()
        {
            //todo: this will just be getting the selected curve

            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                xValues.Add(i / 10.0);
                yValues.Add(i * 900);
            }
            ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.OutflowFrequency, coordinatesFunction);
            return fdaFunction;
        }

        private IFdaFunction getRatingCurveFunction()
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                xValues.Add(i * 1100);
                yValues.Add(i);
            }

            ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.Rating, coordinatesFunction);
            return fdaFunction;
        }

        private IFdaFunction getStageDamageFunction()
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                xValues.Add(i + 2);
                yValues.Add(i * 90);
            }

            ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.InteriorStageDamage, coordinatesFunction);
            return fdaFunction;
        }

        private IFdaFunction getDamageFrequencyFunction()
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                xValues.Add(i / 9.0);
                yValues.Add(i * 110);
            }

            ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
            IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.DamageFrequency, coordinatesFunction);
            return fdaFunction;
        }

        public IASElement GetElement()
        {
            //todo: if this is being called, do we assume everything is valid?
            //int impAreaID = SelectedImpactAreaElement.ChildElement != null ? SelectedImpactAreaElement.ChildElement.GetElementID() : -1;
            int flowFreqID = SelectedFrequencyElement.ChildElement != null ? SelectedFrequencyElement.ChildElement.GetElementID() : -1;
            int inflowOutID = SelectedInflowOutflowElement.ChildElement != null ? SelectedInflowOutflowElement.ChildElement.GetElementID() : -1;
            int ratingID = SelectedRatingCurveElement.ChildElement != null ? SelectedRatingCurveElement.ChildElement.GetElementID() : -1;
            int extIntID = SelectedExteriorInteriorElement.ChildElement != null ? SelectedExteriorInteriorElement.ChildElement.GetElementID() : -1;
            int latStructID = SelectedLeveeFeatureElement.ChildElement != null ? SelectedLeveeFeatureElement.ChildElement.GetElementID() : -1;
            int stageDamID = SelectedStageDamageElement.ChildElement != null ? SelectedStageDamageElement.ChildElement.GetElementID() : -1;

            List<AdditionalThresholdRowItem> thresholdRowItems = _additionalThresholdsVM.GetThresholds();

            IASElement elementToSave = new IASElement(IndexLocationID,
            flowFreqID, inflowOutID,
            ratingID, extIntID, latStructID, stageDamID, thresholdRowItems);
            return elementToSave;
        }
        public void Plot()
        {
            FdaValidationResult validationResult = IsValid();
            if (validationResult.IsValid)
            {
                CheckForOverlappingRanges();
                //get the current curves and set that data on the chart controls
                //this update call will set the current crosshair data on each one
                PlotControlVM.FrequencyRelationshipControl.UpdatePlotData(getFrequencyRelationshipFunction());
                PlotControlVM.RatingRelationshipControl.UpdatePlotData(getRatingCurveFunction());
                PlotControlVM.StageDamageControl.UpdatePlotData(getStageDamageFunction());
                PlotControlVM.DamageFrequencyControl.UpdatePlotData(getDamageFrequencyFunction());

                PlotControlVM.Plot();
            }
            else
            {
                MessageBox.Show(validationResult.ErrorMessage.ToString(), "Insufficient Data", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

                #endregion


        private Boolean ValidateIAS()
        {
            //todo: the rating curve is required is the frequency relationship is of type
            //flow-frequency. This will need to get added once we complete task 5 in the clean doc.
            //if (Description == null) { Description = ""; }

            //todo: is this the same as the CanPlot() or are there differences?
            //return IsValid();
            return true;
        }

        //public override void Save()
        //{
        //    bool isValid = ValidateIAS();

        //    if (isValid)
        //    {
        //        Thresholds = _additionalThresholdsVM.GetThresholds();

        //        int impAreaID = SelectedImpactAreaElement.ChildElement != null ? SelectedImpactAreaElement.ChildElement.GetElementID() : -1;
        //        int flowFreqID = SelectedFrequencyElement.ChildElement != null ? SelectedFrequencyElement.ChildElement.GetElementID() : -1;
        //        int inflowOutID = SelectedInflowOutflowElement.ChildElement != null ? SelectedInflowOutflowElement.ChildElement.GetElementID() : -1;
        //        int ratingID = SelectedRatingCurveElement.ChildElement != null ? SelectedRatingCurveElement.ChildElement.GetElementID() : -1;
        //        int extIntID = SelectedExteriorInteriorElement.ChildElement != null ? SelectedExteriorInteriorElement.ChildElement.GetElementID() : -1;
        //        int latStructID = SelectedLeveeFeatureElement.ChildElement != null ? SelectedLeveeFeatureElement.ChildElement.GetElementID() : -1;
        //        int stageDamID = SelectedStageDamageElement.ChildElement != null ? SelectedStageDamageElement.ChildElement.GetElementID() : -1;

        //        List<AdditionalThresholdRowItem> thresholdRowItems = _additionalThresholdsVM.GetThresholds();

        //        IASElement elementToSave = new IASElement(Name, Description, Year, impAreaID,
        //        flowFreqID, inflowOutID,
        //        ratingID, extIntID, latStructID, stageDamID, thresholdRowItems);
        //        CurrentElement = elementToSave;

        //        if (_isInEditMode)
        //        {
        //            Saving.PersistenceFactory.GetIASManager().SaveExisting(_currentElement, elementToSave);
        //        }
        //        else
        //        {
        //            Saving.PersistenceFactory.GetIASManager().SaveNew(elementToSave);
        //        }
        //    }

        //}

        public void AddThresholds()
        {
            string header = "Annual Exceedance Probabilities Thresholds";
            DynamicTabVM tab = new DynamicTabVM(header, _additionalThresholdsVM, "additionalThresholds");
            Navigate(tab, true, true);
            Thresholds = _additionalThresholdsVM.GetThresholds();


        }

    }
}
