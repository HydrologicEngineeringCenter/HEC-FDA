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
        private List<ImpactAreaRowItem> _ImpactAreaNames; 


        public int Year { get; set; } = DateTime.Now.Year;

        
        //public List<SpecificIASEditorVM> SpecificIASEditors { get; set; }
        public ObservableCollection<ChildElementComboItem> ImpactAreaElements { get; set; }
        public List<ImpactAreaRowItem> ImpactAreas
        {
            get { return _ImpactAreaNames; }
            set { _ImpactAreaNames = value; NotifyPropertyChanged(); }
        }
        private ImpactAreaRowItem _SelectedImpactArea;
        public ImpactAreaRowItem SelectedImpactArea
        {
            get { return _SelectedImpactArea; }
            set { _SelectedImpactArea = value; SelectedImpactAreaNameChanged(); NotifyPropertyChanged(); }
        }
        private Dictionary<ImpactAreaRowItem, SpecificIASEditorVM> _ImpactAreaEditorDictionary;

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
            CreateEmptySpecificIASEditors();
        }



        //todo: this ctor probably needs some work
        public IASEditorVM(IASElementSet elem, EditorActionManager manager) : base(elem, manager)
        {
            _currentElement = elem;
            _isInEditMode = true;
            FillForm(elem);
        }

        private void CreateEmptySpecificIASEditors()
        {
            ImpactAreas = new List<ImpactAreaRowItem>();
            _ImpactAreaEditorDictionary = new Dictionary<ImpactAreaRowItem, SpecificIASEditorVM>();

            ObservableCollection<ImpactAreaRowItem> impactAreaRows = GetImpactAreaRowItems();
            foreach (ImpactAreaRowItem row in impactAreaRows)
            {
                ImpactAreas.Add(row);
                SpecificIASEditorVM specificIASEditorVM = new SpecificIASEditorVM(row);
                specificIASEditorVM.RequestNavigation += Navigate;
                _ImpactAreaEditorDictionary.Add(row, specificIASEditorVM);
            }
            //todo: an exception gets thrown in the code behind if we don't start with an editor vm loaded in.
            //what do we do if no impact areas?
            SelectedImpactArea = ImpactAreas[0];


        }

        private ObservableCollection<ImpactAreaRowItem> GetImpactAreaRowItems()
        {
            ObservableCollection<ImpactAreaRowItem> impactAreaRows = new ObservableCollection<ImpactAreaRowItem>();
            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impactAreaElements.Count > 0)
            {
                //todo: deal with this "[0]"
                //this should probably return a list not an obs collection
                impactAreaRows = impactAreaElements[0].ImpactAreaRows;
            }
            return impactAreaRows;
        }

        

        private void SelectedImpactAreaNameChanged()
        {
            if(_ImpactAreaEditorDictionary.ContainsKey(SelectedImpactArea))
            {
                SelectedEditorVM = _ImpactAreaEditorDictionary[SelectedImpactArea];
            }
        }

        #region Live Update Event Methods

        

        #endregion

        private void FillForm(IASElementSet elem)
        {
            Name = elem.Name;
            Description = elem.Description;
            Year = elem.AnalysisYear;
            //SelectedImpactAreaElement = elem.ImpactAreaID;

            ImpactAreas = new List<ImpactAreaRowItem>();
            _ImpactAreaEditorDictionary = new Dictionary<ImpactAreaRowItem, SpecificIASEditorVM>();

            //this is the list of current impact area rows in the study. They might not match the items
            //that were saved in the db. The user might have deleted the old impact area set and brought in 
            //a new one. I think we should only display saved items that still match up.
            ObservableCollection<ImpactAreaRowItem> impactAreaRows = GetImpactAreaRowItems();

            //this is the list that was saved
            List<IASElement> specificIASElements = elem.SpecificIASElements;
            foreach (ImpactAreaRowItem row in impactAreaRows)
            {
                ImpactAreas.Add(row);
                //try to find the saved ias with this row's id.
                IASElement foundElement = specificIASElements.FirstOrDefault(ias => ias.ImpactAreaID == row.ID);
                if (foundElement != null)
                {
                    SpecificIASEditorVM specificIASEditorVM = new SpecificIASEditorVM(foundElement, row.Name);
                    specificIASEditorVM.RequestNavigation += Navigate;
                    _ImpactAreaEditorDictionary.Add(row, specificIASEditorVM);

                }
                else
                {
                    SpecificIASEditorVM specificIASEditorVM = new SpecificIASEditorVM(row);
                    specificIASEditorVM.RequestNavigation += Navigate;
                    _ImpactAreaEditorDictionary.Add(row, specificIASEditorVM);

                }
            }
            //todo: an exception gets thrown in the code behind if we don't start with an editor vm loaded in.
            //what do we do if no impact areas?
            SelectedImpactArea = ImpactAreas[0];

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

            //todo: actually run the compute and see if it was successful? - this might be done on each individual ias.
            foreach (KeyValuePair<ImpactAreaRowItem, SpecificIASEditorVM>  entry in _ImpactAreaEditorDictionary)
            {
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
                foreach (KeyValuePair<ImpactAreaRowItem, SpecificIASEditorVM> entry in _ImpactAreaEditorDictionary)
                {
                    //string name = entry.Key;
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

                IASElementSet elemToSave = new IASElementSet(Name, Description, Year, elementsToSave);

                if (_isInEditMode)
                {
                    Saving.PersistenceFactory.GetIASManager().SaveExisting(_currentElement, elemToSave);
                }
                else
                {
                    Saving.PersistenceFactory.GetIASManager().SaveNew(elemToSave);
                    _isInEditMode = true;
                }
                _currentElement = elemToSave;
            }
        }


    }
}
