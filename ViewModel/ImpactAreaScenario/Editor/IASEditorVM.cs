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
    public class IASEditorVM : BaseEditorVM
    {
        private IASElementSet _currentElement;
        private bool _isInEditMode;
        private Chart2DController _controller;
        private AdditionalThresholdsVM _additionalThresholdsVM;


        public int Year { get; set; } = DateTime.Now.Year;


       
        public List<AdditionalThresholdRowItem> Thresholds { get; set; }
        
        //public List<SpecificIASEditorVM> SpecificIASEditors { get; set; }
        public ObservableCollection<ChildElementComboItem> ImpactAreaElements { get; set; }
        private List<string> _ImpactAreaNames; 
        public List<string> ImpactAreaNames
        {
            get { return _ImpactAreaNames; }
            set { _ImpactAreaNames = value; NotifyPropertyChanged(); }
        }
        private string _SelectedImpactAreaName;
        public string SelectedImpactAreaName
        {
            get { return _SelectedImpactAreaName; }
            set { _SelectedImpactAreaName = value; SelectedImpactAreaNameChanged(); NotifyPropertyChanged(); }
        }
        private Dictionary<string, SpecificIASEditorVM> _ImpactAreaEditorDictionary;

        private SpecificIASEditorVM _SelectedEditorVM;
        public SpecificIASEditorVM SelectedEditorVM
        {
            get { return _SelectedEditorVM; }
            set { _SelectedEditorVM = value; NotifyPropertyChanged(); }
        }
        /// <summary>
        /// This is the create new ctor
        /// </summary>
        public IASEditorVM(EditorActionManager manager):base(manager)
        {
            Initialize();
            //SpecificIASEditors = new List<SpecificIASEditorVM>();
            

        }

        private void SelectedImpactAreaNameChanged()
        {
            if(_ImpactAreaEditorDictionary.ContainsKey(SelectedImpactAreaName))
            {
                SelectedEditorVM = _ImpactAreaEditorDictionary[SelectedImpactAreaName];
            }
        }

        private void CreateEmptySpecificIASEditors()
        {
            _ImpactAreaEditorDictionary = new Dictionary<string, SpecificIASEditorVM>();
            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            List<string> impactAreaNames = new List<string>();
            if (impactAreaElements.Count > 0)
            {
                ObservableCollection<ImpactAreaRowItem> impactAreaRows = impactAreaElements[0].ImpactAreaRows;
                foreach (ImpactAreaRowItem row in impactAreaRows)
                {
                    impactAreaNames.Add(row.Name);
                    _ImpactAreaEditorDictionary.Add(row.Name, new SpecificIASEditorVM(row.Name));
                }
                //todo: an exception gets thrown in the code behind if we don't start with an editor vm loaded in.
                //what do we do if no impact areas?
                SelectedImpactAreaName = impactAreaNames[0];
            }

            ImpactAreaNames = impactAreaNames;
        }

        //todo: this ctor probably needs some work
        public IASEditorVM(IASElementSet elem, EditorActionManager manager) : base(elem, manager)
        {
            _currentElement = elem;
            _isInEditMode = true;
            Initialize();
            FillForm(elem);
        }

        private void Initialize()
        {

            CreateEmptySpecificIASEditors();
            //hook up the navigate event for the additional thresholds dialog
            _additionalThresholdsVM = new AdditionalThresholdsVM();
            _additionalThresholdsVM.RequestNavigation += Navigate;

            Thresholds = new List<AdditionalThresholdRowItem>();

            //LoadElements();

            //StudyCache.ImpactAreaAdded += AddImpactAreaElement;
            //StudyCache.ImpactAreaRemoved += RemoveImpactAreaElement;
            //StudyCache.ImpactAreaUpdated += UpdateImpactAreaElement;

            //StudyCache.FlowFrequencyAdded += AddFlowFreqElement;
            //StudyCache.FlowFrequencyRemoved += RemoveFlowFreqElement;
            //StudyCache.FlowFrequencyUpdated += UpdateFlowFreqElement;

            //StudyCache.InflowOutflowAdded += AddInOutElement;
            //StudyCache.InflowOutflowRemoved += RemoveInOutElement;
            //StudyCache.InflowOutflowUpdated += UpdateInOutElement;

            //StudyCache.RatingAdded += AddRatingElement;
            //StudyCache.RatingRemoved += RemoveRatingElement;
            //StudyCache.RatingUpdated += UpdateRatingElement;

            //StudyCache.LeveeAdded += AddLeveeElement;
            //StudyCache.LeveeRemoved += RemoveLeveeElement;
            //StudyCache.LeveeUpdated += UpdateLeveeElement;

            //StudyCache.ExteriorInteriorAdded += AddExtIntElement;
            //StudyCache.ExteriorInteriorRemoved += RemoveExtIntElement;
            //StudyCache.ExteriorInteriorUpdated += UpdateExtIntElement;

            //StudyCache.StageDamageAdded += AddStageDamageElement;
            //StudyCache.StageDamageRemoved += RemoveStageDamageElement;
            //StudyCache.StageDamageUpdated += UpdateStageDamageElement;
        }


        #region Live Update Event Methods

        //private void AddStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    StageDamageElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        //}
        //private void RemoveStageDamageElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    removeElement(((ChildElement)e.Element).GetElementID(), StageDamageElements);
        //}
        //private void UpdateStageDamageElement(object sender, Saving.ElementUpdatedEventArgs e)
        //{
        //    updateElement(StageDamageElements, SelectedStageDamageElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        //}

        //private void AddExtIntElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    ExteriorInteriorElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        //}
        //private void RemoveExtIntElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    removeElement(((ChildElement)e.Element).GetElementID(), ExteriorInteriorElements);
        //}
        //private void UpdateExtIntElement(object sender, Saving.ElementUpdatedEventArgs e)
        //{
        //    updateElement(ExteriorInteriorElements, SelectedExteriorInteriorElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        //}

        //private void AddLeveeElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    LeveeFeatureElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        //}
        //private void RemoveLeveeElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    removeElement(((ChildElement)e.Element).GetElementID(), LeveeFeatureElements);
        //}
        //private void UpdateLeveeElement(object sender, Saving.ElementUpdatedEventArgs e)
        //{
        //    updateElement(LeveeFeatureElements, SelectedLeveeFeatureElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        //}

        //private void AddInOutElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    InflowOutflowElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        //}
        //private void RemoveInOutElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    removeElement(((ChildElement)e.Element).GetElementID(), InflowOutflowElements);
        //}
        //private void UpdateInOutElement(object sender, Saving.ElementUpdatedEventArgs e)
        //{
        //    updateElement(InflowOutflowElements, SelectedInflowOutflowElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        //}


        //private void AddFlowFreqElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    FrequencyElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        //}
        //private void RemoveFlowFreqElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    removeElement(((ChildElement)e.Element).GetElementID(), FrequencyElements);
        //}
        //private void UpdateFlowFreqElement(object sender, Saving.ElementUpdatedEventArgs e)
        //{
        //    updateElement(FrequencyElements, SelectedFrequencyElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        //}

        //private void AddImpactAreaElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    ImpactAreaElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        //}
        //private void RemoveImpactAreaElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    removeElement(((ChildElement)e.Element).GetElementID(), ImpactAreaElements);
        //}
        //private void UpdateImpactAreaElement(object sender, Saving.ElementUpdatedEventArgs e)
        //{
        //    updateElement(ImpactAreaElements, SelectedImpactAreaElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        //}

        //private void AddRatingElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    RatingCurveElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        //}
        //private void RemoveRatingElement(object sender, Saving.ElementAddedEventArgs e)
        //{
        //    removeElement(((ChildElement)e.Element).GetElementID(), RatingCurveElements);
        //}
        //private void UpdateRatingElement(object sender, Saving.ElementUpdatedEventArgs e)
        //{
        //    updateElement(RatingCurveElements, SelectedRatingCurveElement, (ChildElement)e.OldElement, (ChildElement)e.NewElement);
        //}

        //private void removeElement(int idToRemove, ObservableCollection<ChildElementComboItem> collection )
        //{
        //    collection.Remove(collection.Where(elem => elem.ChildElement != null && elem.ChildElement.GetElementID() == idToRemove).Single());
        //}

        //private void updateElement(ObservableCollection<ChildElementComboItem> collection, ChildElementComboItem selectedItem,
        //     ChildElement oldElement, ChildElement newElement)
        //{
        //    int idToUpdate = oldElement.GetElementID();

        //    ChildElementComboItem itemToUpdate = collection.Where(elem => elem.ChildElement != null && elem.ChildElement.GetElementID() == idToUpdate).SingleOrDefault();
        //    if (itemToUpdate != null)
        //    {
        //        int index = collection.IndexOf(itemToUpdate);

        //        //this was an attempt to update the selected item if that is the one we are swapping out. For some reason
        //        //this doesn't work. I was trying to find a way to pass the property into this method and was unsuccessful.
        //        //bool needToUpdateSelected = selectedItem.ChildElement != null && selectedItem.ChildElement.GetElementID() == idToUpdate;

        //        //if (index != -1)
        //        //{
        //        //    collection.RemoveAt(index);
        //        //    collection.Insert(index, new ChildElementComboItem(newElement));
        //        //    if (needToUpdateSelected)
        //        //    {
        //        //        propToUpdate (collection[index]);
        //        //    }
        //        //}
        //    }
        //}

        #endregion

        private void FillForm(IASElementSet elem)
        {
            Name = elem.Name;
            Description = elem.Description;
            Year = elem.AnalysisYear;
            //SelectedImpactAreaElement = elem.ImpactAreaID;
            FillThresholds(elem);

        }
        private void FillThresholds(IASElementSet elem)
        {
            //todo: maybe add a different ctor or a fill method to load the rows?
            _additionalThresholdsVM.Rows = new ObservableCollection<AdditionalThresholdRowItem>();
            foreach (AdditionalThresholdRowItem row in elem.Thresholds)
            {
                _additionalThresholdsVM.Rows.Add(row);
            }
        }

        //private void LoadElements()
        //{
        //    //what happens if there are no elements for a combo?
        //    //I will always add an empty ChildElementComboItem and then select it by default.
        //    //this means that when asking for the selected combo item, it should never be null.
        //    List<ChildElement> childElems = new List<ChildElement>();

        //    List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
        //    childElems.AddRange(impactAreaElements);
        //    ImpactAreaElements = CreateComboItems(childElems);
        //    SelectedImpactAreaElement = ImpactAreaElements.First();

        //    List<AnalyticalFrequencyElement> analyticalFrequencyElements = StudyCache.GetChildElementsOfType<AnalyticalFrequencyElement>();
        //    childElems.Clear();
        //    childElems.AddRange(analyticalFrequencyElements);
        //    FrequencyElements = CreateComboItems(childElems);
        //    SelectedFrequencyElement = FrequencyElements.First();

        //    List<InflowOutflowElement> inflowOutflowElements = StudyCache.GetChildElementsOfType<InflowOutflowElement>();
        //    childElems.Clear();
        //    childElems.AddRange(inflowOutflowElements);
        //    InflowOutflowElements = CreateComboItems(childElems);
        //    SelectedInflowOutflowElement = InflowOutflowElements.First();

        //    List<RatingCurveElement> ratingCurveElements = StudyCache.GetChildElementsOfType<RatingCurveElement>();
        //    childElems.Clear();
        //    childElems.AddRange(ratingCurveElements);
        //    RatingCurveElements = CreateComboItems(childElems);
        //    SelectedRatingCurveElement = RatingCurveElements.First();

        //    List<LeveeFeatureElement> leveeFeatureElements = StudyCache.GetChildElementsOfType<LeveeFeatureElement>();
        //    childElems.Clear();
        //    childElems.AddRange(leveeFeatureElements);
        //    LeveeFeatureElements = CreateComboItems(childElems);
        //    SelectedLeveeFeatureElement = LeveeFeatureElements.First();

        //    List<ExteriorInteriorElement> exteriorInteriorElements = StudyCache.GetChildElementsOfType<ExteriorInteriorElement>();
        //    childElems.Clear();
        //    childElems.AddRange(exteriorInteriorElements);
        //    ExteriorInteriorElements = CreateComboItems(childElems);
        //    SelectedExteriorInteriorElement = ExteriorInteriorElements.First();

        //    List<AggregatedStageDamageElement> aggregatedStageDamageElements = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
        //    childElems.Clear();
        //    childElems.AddRange(aggregatedStageDamageElements);
        //    StageDamageElements = CreateComboItems(childElems);
        //    SelectedStageDamageElement = StageDamageElements.First();
        //}

        //private ObservableCollection<ChildElementComboItem> CreateComboItems(List<ChildElement> elems)
        //{
        //    ObservableCollection<ChildElementComboItem> items = new ObservableCollection<ChildElementComboItem>();
        //    items.Add(new ChildElementComboItem(null));
        //    foreach(ChildElement elem in elems)
        //    {
        //        items.Add(new ChildElementComboItem(elem));
        //    }

        //    return items;
        //}

        public void AddThresholds()
        {
            //AdditionalThresholdsVM vm = new AdditionalThresholdsVM();
            string header = "Annual Exceedance Probabilities Thresholds";
            DynamicTabVM tab = new DynamicTabVM(header, _additionalThresholdsVM, "additionalThresholds");
            Navigate(tab,true, true);
            Thresholds = _additionalThresholdsVM.GetThresholds();
            

        }

        //private void UpdateRatingRequired()
        //{
        //    if (SelectedFrequencyElement != null && SelectedFrequencyElement.ChildElement != null )
        //    {
        //        //todo: we need to check to see if this is a flow freq.
        //        //if the flow freq is of type flow freq, then the rating curve is required.
        //        //this is task 5 in the clean document.
        //        RatingRequired = true;
        //    }
        //}
        //private void StageDamageSelectionChanged()
        //{
        //    if (SelectedStageDamageElement != null && SelectedStageDamageElement.ChildElement != null)
        //    {
        //        List<string> damCats = new List<string>();
        //        damCats.Add("todo: add dam cats");
        //        DamageCategories = damCats;
        //        SelectedDamageCategory = damCats[0];
        //    }
        //}

        #region validation

        //private ValidationResult IsFrequencyRelationshipValid()
        //{
        //    ValidationResult vr = new ValidationResult();
        //    if(SelectedFrequencyElement.ChildElement == null)
        //    {
        //        vr.IsValid = false;
        //        vr.ErrorMessage = "A Frequency Relationship is required.";
        //    }

        //    return vr;
        //}

        //private ValidationResult IsRatingCurveValid()
        //{
        //    ValidationResult vr = new ValidationResult();
        //    if (_ratingRequired && SelectedRatingCurveElement.ChildElement == null)
        //    {
        //        vr.IsValid = false;
        //        vr.ErrorMessage = "A Rating Curve is required when using a flow-frequency relationship.";
        //    }

        //    return vr;
        //}
        //private ValidationResult IsStageDamageValid()
        //{
        //    ValidationResult vr = new ValidationResult();
        //    if (SelectedStageDamageElement.ChildElement == null)
        //    {
        //        vr.IsValid = false;
        //        vr.ErrorMessage = "A Stage Damage is required.";
        //    }

        //    return vr;
        //}

        private FdaValidationResult IsYearValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (Year < 1900 && Year > 3000)
            {
                vr.IsValid = false;
                vr.ErrorMessage = new StringBuilder( "A year is required and must be greater than 1900 and less than 3000");
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



        #region PlotCurves
      
        //private IFdaFunction getFrequencyRelationshipFunction()
        //{
        //    //todo: this will just be getting the selected curve

        //    List<double> xValues = new List<double>();
        //    List<double> yValues = new List<double>();

        //    for (int i = 0; i < 10; i++)
        //    {
        //        xValues.Add(i / 10.0);
        //        yValues.Add(i * 9);
        //    }
        //    ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
        //    IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.OutflowFrequency, coordinatesFunction);
        //    return fdaFunction;
        //}

        //private IFdaFunction getRatingCurveFunction()
        //{
        //    List<double> xValues = new List<double>();
        //    List<double> yValues = new List<double>();

        //    for (int i = 0; i < 10; i++)
        //    {
        //        xValues.Add(i);
        //        yValues.Add(i * 11);
        //    }

        //    ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
        //    IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.Rating, coordinatesFunction);
        //    return fdaFunction;
        //}

        //private IFdaFunction getStageDamageFunction()
        //{
        //    List<double> xValues = new List<double>();
        //    List<double> yValues = new List<double>();

        //    for (int i = 0; i < 10; i++)
        //    {
        //        xValues.Add(i + 2);
        //        yValues.Add(i * 90);
        //    }

        //    ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
        //    IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.InteriorStageDamage, coordinatesFunction);
        //    return fdaFunction;
        //}

        //private IFdaFunction getDamageFrequencyFunction()
        //{
        //    List<double> xValues = new List<double>();
        //    List<double> yValues = new List<double>();

        //    for (int i = 0; i < 10; i++)
        //    {
        //        xValues.Add(i / 9.0);
        //        yValues.Add(i * 110);
        //    }

        //    ICoordinatesFunction coordinatesFunction = ICoordinatesFunctionsFactory.Factory(xValues, yValues, InterpolationEnum.Linear);
        //    IFdaFunction fdaFunction = IFdaFunctionFactory.Factory(IParameterEnum.DamageFrequency, coordinatesFunction);
        //    return fdaFunction;
        //}

        //public void Plot()
        //{
        //    //CanPlot() is being called by the view before calling this method.
        //    //if (CanPlot())
        //    {
        //        //get the current curves and set that data on the chart controls
        //        //this update call will set the current crosshair data on each one
        //        FrequencyRelationshipControl.UpdatePlotData(getFrequencyRelationshipFunction());
        //        RatingRelationshipControl.UpdatePlotData(getRatingCurveFunction());
        //        StageDamageControl.UpdatePlotData(getStageDamageFunction());
        //        DamageFrequencyControl.UpdatePlotData(getDamageFrequencyFunction());

        //        //link the crosshair data to eachother
        //        CrosshairData freqRelationshipCrosshairData = FrequencyRelationshipControl.currentCrosshairData;
        //        CrosshairData ratingCrosshairData = RatingRelationshipControl.currentCrosshairData;
        //        freqRelationshipCrosshairData.Next = new SharedAxisCrosshairData(ratingCrosshairData, Axis.Y, Axis.Y);
        //        ratingCrosshairData.Previous = new SharedAxisCrosshairData(freqRelationshipCrosshairData, Axis.Y, Axis.Y);

        //        //CrosshairData stageDamageCrosshairData = StageDamageControl.currentCrosshairData;
        //        //ratingCrosshairData.Next = new SharedAxisCrosshairData(stageDamageCrosshairData, Axis.X, Axis.X);
        //        //stageDamageCrosshairData.Previous = new SharedAxisCrosshairData(ratingCrosshairData, Axis.X, Axis.X);

        //        //CrosshairData damageFreqCrosshairData = DamageFrequencyControl.currentCrosshairData;
        //        //stageDamageCrosshairData.Next = new SharedAxisCrosshairData(damageFreqCrosshairData, Axis.Y, Axis.Y);
        //        //damageFreqCrosshairData.Previous = new SharedAxisCrosshairData(stageDamageCrosshairData, Axis.Y, Axis.Y);

        //        FrequencyRelationshipControl.Plot();
        //        RatingRelationshipControl.Plot();
        //        StageDamageControl.Plot();
        //        DamageFrequencyControl.Plot();

   
        //    }
        //}

        #endregion


        private Boolean ValidateIAS()
        {

            //if (Description == null) { Description = ""; }

            //StringBuilder errorMsg = new StringBuilder();
            FdaValidationResult vr = new FdaValidationResult();

            vr.AddValidationResult( IsYearValid());
            vr.AddValidationResult( IsThresholdsValid());

            //todo: actually run the compute and see if it was successful? - this might be done on each individual ias.
            foreach (KeyValuePair<string, SpecificIASEditorVM>  entry in _ImpactAreaEditorDictionary)
            {
                string name = entry.Key;
                SpecificIASEditorVM vm = entry.Value;

                vr.AddValidationResult(vm.IsValid());
            }


            if (!vr.IsValid)
            {
                MessageBox.Show(vr.ErrorMessage.ToString(), "Insufficient Data", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else
            {
                //if no msg's then we can plot
                return true;
            }

        }
        private IFdaFunction getFrequencyRelationshipFunction()
        {
            //todo: this will just be getting the selected curve

            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                xValues.Add(i / 10.0);
                yValues.Add(i * 9);
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
                xValues.Add(i);
                yValues.Add(i * 11);
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

        public void Plot()
        {
            ////CanPlot() is being called by the view before calling this method.
            ////if (CanPlot())
            //{
            //    //get the current curves and set that data on the chart controls
            //    //this update call will set the current crosshair data on each one
            //    FrequencyRelationshipControl.UpdatePlotData(getFrequencyRelationshipFunction());
            //    RatingRelationshipControl.UpdatePlotData(getRatingCurveFunction());
            //    StageDamageControl.UpdatePlotData(getStageDamageFunction());
            //    DamageFrequencyControl.UpdatePlotData(getDamageFrequencyFunction());

            //    //link the crosshair data to eachother
            //    CrosshairData freqRelationshipCrosshairData = FrequencyRelationshipControl.currentCrosshairData;
            //    CrosshairData ratingCrosshairData = RatingRelationshipControl.currentCrosshairData;
            //    freqRelationshipCrosshairData.Next = new SharedAxisCrosshairData(ratingCrosshairData, Axis.Y, Axis.Y);
            //    ratingCrosshairData.Previous = new SharedAxisCrosshairData(freqRelationshipCrosshairData, Axis.Y, Axis.Y);

            //    CrosshairData stageDamageCrosshairData = StageDamageControl.currentCrosshairData;
            //    ratingCrosshairData.Next = new SharedAxisCrosshairData(stageDamageCrosshairData, Axis.X, Axis.X);
            //    stageDamageCrosshairData.Previous = new SharedAxisCrosshairData(ratingCrosshairData, Axis.X, Axis.X);

            //    CrosshairData damageFreqCrosshairData = DamageFrequencyControl.currentCrosshairData;
            //    stageDamageCrosshairData.Next = new SharedAxisCrosshairData(damageFreqCrosshairData, Axis.Y, Axis.Y);
            //    damageFreqCrosshairData.Previous = new SharedAxisCrosshairData(stageDamageCrosshairData, Axis.Y, Axis.Y);

            //    FrequencyRelationshipControl.Plot();
            //    RatingRelationshipControl.Plot();
            //    StageDamageControl.Plot();
            //    DamageFrequencyControl.Plot();
            //}
        }




        public override void Save()
        {
            bool isValid = ValidateIAS();

            if (isValid)
            {

                //get the list of specific IAS elements.
                List<IASElement> elementsToSave = new List<IASElement>();
                foreach (KeyValuePair<string, SpecificIASEditorVM> entry in _ImpactAreaEditorDictionary)
                {
                    string name = entry.Key;
                    SpecificIASEditorVM vm = entry.Value;

                    elementsToSave.Add( vm.GetElement());
                }

                //Thresholds = _additionalThresholdsVM.GetThresholds();

                //int impAreaID = SelectedImpactAreaElement.ChildElement != null ? SelectedImpactAreaElement.ChildElement. GetElementID() : -1;
                //int flowFreqID = SelectedFrequencyElement.ChildElement != null ? SelectedFrequencyElement.ChildElement.GetElementID() : -1;
                //int inflowOutID = SelectedInflowOutflowElement.ChildElement != null ? SelectedInflowOutflowElement.ChildElement.GetElementID() : -1;
                //int ratingID = SelectedRatingCurveElement.ChildElement != null ? SelectedRatingCurveElement.ChildElement.GetElementID() : -1;
                //int extIntID = SelectedExteriorInteriorElement.ChildElement != null ? SelectedExteriorInteriorElement.ChildElement.GetElementID() : -1;
                //int latStructID = SelectedLeveeFeatureElement.ChildElement != null ? SelectedLeveeFeatureElement.ChildElement.GetElementID() : -1;
                //int stageDamID = SelectedStageDamageElement.ChildElement != null ? SelectedStageDamageElement.ChildElement.GetElementID() : -1;

                //List<AdditionalThresholdRowItem> thresholdRowItems = _additionalThresholdsVM.GetThresholds();

                //IASElement elementToSave = new IASElement(Name, Description, Year, impAreaID,
                //flowFreqID, inflowOutID,
                //ratingID, extIntID, latStructID, stageDamID, thresholdRowItems);
                //CurrentElement = elementToSave;

                //if (_isInEditMode)
                //{
                //    Saving.PersistenceFactory.GetIASManager().SaveExisting(_currentElement, elementToSave);
                //}
                //else
                //{
                //    Saving.PersistenceFactory.GetIASManager().SaveNew(elementToSave);
                //}
            }
        }


    }
}
