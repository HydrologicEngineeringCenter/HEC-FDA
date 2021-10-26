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

        #region Fields
        private IASElementSet _currentElement;
        private bool _isInEditMode;
        private List<ImpactAreaRowItem> _ImpactAreaNames; 
        private ImpactAreaRowItem _SelectedImpactArea;
        private Dictionary<ImpactAreaRowItem, SpecificIASEditorVM> _ImpactAreaEditorDictionary;

        private SpecificIASEditorVM _SelectedEditorVM;

        #endregion


        #region Properties
        public int Year { get; set; } = DateTime.Now.Year;

        
        public ObservableCollection<ChildElementComboItem> ImpactAreaElements { get; set; }
        public List<ImpactAreaRowItem> ImpactAreas
        {
            get { return _ImpactAreaNames; }
            set { _ImpactAreaNames = value; NotifyPropertyChanged(); }
        }
        public ImpactAreaRowItem SelectedImpactArea
        {
            get { return _SelectedImpactArea; }
            set { _SelectedImpactArea = value; SelectedImpactAreaChanged(); NotifyPropertyChanged(); }
        }
        public SpecificIASEditorVM SelectedEditorVM
        {
            get { return _SelectedEditorVM; }
            set { _SelectedEditorVM = value; NotifyPropertyChanged(); }
        }

        #endregion
        /// <summary>
        /// This is the create new ctor
        /// </summary>
        public IASEditorVM(EditorActionManager manager):base(manager)
        {
            CreateEmptySpecificIASEditors();
        }



        /// <summary>
        /// This is the ctor for opening an existing element.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="manager"></param>
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

        
        /// <summary>
        /// The user has changed the selected impact area in the combobox.
        /// </summary>
        private void SelectedImpactAreaChanged()
        {
            if(_ImpactAreaEditorDictionary.ContainsKey(SelectedImpactArea))
            {
                SelectedEditorVM = _ImpactAreaEditorDictionary[SelectedImpactArea];
            }
        }


        private void FillForm(IASElementSet elem)
        {
            Name = elem.Name;
            Description = elem.Description;
            Year = elem.AnalysisYear;

            ImpactAreas = new List<ImpactAreaRowItem>();
            _ImpactAreaEditorDictionary = new Dictionary<ImpactAreaRowItem, SpecificIASEditorVM>();

            //this is the list of current impact area rows in the study. They might not match the items
            //that were saved in the db for this IAS. The user might have deleted the old impact area set and brought in 
            //a new one. I think we should only display saved items that still match up.
            ObservableCollection<ImpactAreaRowItem> impactAreaRows = GetImpactAreaRowItems();

            //this is the list that was saved
            List<SpecificIAS> specificIASElements = elem.SpecificIASElements;
            foreach (ImpactAreaRowItem row in impactAreaRows)
            {
                ImpactAreas.Add(row);
                //try to find the saved ias with this row's id.
                SpecificIAS foundElement = specificIASElements.FirstOrDefault(ias => ias.ImpactAreaID == row.ID);
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

        

        

        #region validation

       

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

      
       


        /// <summary>
        /// Validates that the data entered is sufficient to save the form
        /// </summary>
        /// <returns></returns>
        private Boolean ValidateIAS()
        {

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
                //if no msg's then we can save
                return true;
            }

        }

        #endregion

        public override void Save()
        {
            bool isValid = ValidateIAS();

            if (isValid)
            {

                //get the list of specific IAS elements.
                List<SpecificIAS> elementsToSave = new List<SpecificIAS>();
                foreach (KeyValuePair<ImpactAreaRowItem, SpecificIASEditorVM> entry in _ImpactAreaEditorDictionary)
                {
                    SpecificIASEditorVM vm = entry.Value;

                    elementsToSave.Add( vm.GetElement());
                }

                if(Description == null)
                {
                    Description = "";
                }
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
