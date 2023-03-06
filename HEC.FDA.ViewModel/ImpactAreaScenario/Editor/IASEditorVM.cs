using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using HEC.CS.Collections;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class IASEditorVM : BaseEditorVM
    {
        #region Fields
        private List<ImpactAreaRowItem> _ImpactAreaNames = new List<ImpactAreaRowItem>(); 
        private ImpactAreaRowItem _SelectedImpactArea;
        private Dictionary<ImpactAreaRowItem, SpecificIASEditorVM> _ImpactAreaEditorDictionary = new Dictionary<ImpactAreaRowItem, SpecificIASEditorVM>();
        private SpecificIASEditorVM _SelectedEditorVM;
        private bool _HasImpactArea = true;
        private ChildElementComboItem _SelectedStageDamageElement;
        #endregion

        #region Properties
        public ChildElementComboItem SelectedStageDamageElement
        {
            get { return _SelectedStageDamageElement; }
            set { _SelectedStageDamageElement = value; StageDamageSelectionChanged(); }
        }

        public CustomObservableCollection<ChildElementComboItem> StageDamageElements { get; } = new CustomObservableCollection<ChildElementComboItem>();

        public bool HasImpactArea
        {
            get { return _HasImpactArea; }
            set { _HasImpactArea = value; NotifyPropertyChanged(); }
        }
        public int? Year { get; set; } = DateTime.Now.Year;
        
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
            Initialize();
            CreateEmptySpecificIASEditors();
        }

        /// <summary>
        /// This is the ctor for opening an existing element.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="manager"></param>
        public IASEditorVM(IASElement elem, EditorActionManager manager) : base(elem, manager)
        {
            Initialize();
            FillForm(elem);

            SelectedStageDamageElement = StageDamageElements.FirstOrDefault(stage => stage.ChildElement != null && stage.ChildElement.ID == elem.StageDamageID);
            if (SelectedStageDamageElement == null)
            {
                SelectedStageDamageElement = StageDamageElements[0];
            }

        }

        private void Initialize()
        {
            List<AggregatedStageDamageElement> aggregatedStageDamageElements = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            List<ChildElement> childElems = new List<ChildElement>();
            childElems.AddRange( aggregatedStageDamageElements);
            StageDamageElements.AddRange(CreateComboItems(childElems));
            SelectedStageDamageElement = StageDamageElements.First();

            StudyCache.StageDamageAdded += AddStageDamageElement;
            StudyCache.StageDamageRemoved += RemoveStageDamageElement;
            StudyCache.StageDamageUpdated += UpdateStageDamageElement;
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

        /// <summary>
        /// Loads the dictionary that links the specific impact area with the specific ias.
        /// </summary>
        private void CreateEmptySpecificIASEditors()
        {
            List<ImpactAreaRowItem> impactAreaRows = GetImpactAreaRowItems();
            //we don't allow this editor to open unless there are impact areas so this should always be true.
            if (impactAreaRows.Count > 0)
            {
                foreach (ImpactAreaRowItem row in impactAreaRows)
                {
                    ImpactAreas.Add(row);
                    SpecificIASEditorVM specificIASEditorVM = new SpecificIASEditorVM(row, GetSelectedStageDamage);
                    specificIASEditorVM.RequestNavigation += Navigate;
                    _ImpactAreaEditorDictionary.Add(row, specificIASEditorVM);
                    RegisterChildViewModel(specificIASEditorVM);
                }
                SelectedImpactArea = ImpactAreas[0];
            }
        }

        /// <summary>
        /// Grabs the list of impact area rows from the impact area element. There should only ever be one
        /// impact area element in the study. If there is none, then this editor should not be able to open.
        /// </summary>
        /// <returns></returns>
        private List<ImpactAreaRowItem> GetImpactAreaRowItems()
        {
            List<ImpactAreaRowItem> impactAreaRows = new List<ImpactAreaRowItem>();
            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
            if (impactAreaElements.Count > 0)
            {
                impactAreaRows = impactAreaElements[0].ImpactAreaRows;
            }
            return impactAreaRows;
        }
        
        /// <summary>
        /// The user has changed the selected impact area in the combobox. We swap out the selected view model
        /// and the view will adjust based on binding.
        /// </summary>
        private void SelectedImpactAreaChanged()
        {
            if(_ImpactAreaEditorDictionary.ContainsKey(SelectedImpactArea))
            {
                SelectedEditorVM = _ImpactAreaEditorDictionary[SelectedImpactArea];
            }
        }

        /// <summary>
        /// This method compares the specific ias's that were saved to the current state of the impact areas.
        /// It seems possible that there are new impact areas or deleted impact areas. If there are new impact areas
        /// I create an empty specific IAS for it. If the impact area doesn't exist (was deleted), then the specific ias 
        /// that was linked to it will not get added to this editor. If the user saves, it will be gone for good.
        /// </summary>
        /// <param name="elem"></param>
        private void FillForm(IASElement elem)
        {
            Year = elem.AnalysisYear;

            //this is the list of current impact area rows in the study. They might not match the items
            //that were saved in the db for this IAS. The user might have deleted the old impact area set and brought in 
            //a new one. I think we should only display saved items that still match up.
            List<ImpactAreaRowItem> impactAreaRows = GetImpactAreaRowItems();

            //this is the list that was saved
            List<SpecificIAS> specificIASElements = elem.SpecificIASElements;
            foreach (ImpactAreaRowItem row in impactAreaRows)
            {
                ImpactAreas.Add(row);

                //try to find the saved ias with this row's id.
                SpecificIAS foundElement = specificIASElements.FirstOrDefault(ias => ias.ImpactAreaID == row.ID);
                if (foundElement != null)
                {
                    SpecificIASEditorVM specificIASEditorVM = new SpecificIASEditorVM(foundElement, row, GetSelectedStageDamage);
                    specificIASEditorVM.RequestNavigation += Navigate;
                    _ImpactAreaEditorDictionary.Add(row, specificIASEditorVM);
                    RegisterChildViewModel(specificIASEditorVM);
                }
                else
                {
                    SpecificIASEditorVM specificIASEditorVM = new SpecificIASEditorVM(row, GetSelectedStageDamage);
                    specificIASEditorVM.RequestNavigation += Navigate;
                    _ImpactAreaEditorDictionary.Add(row, specificIASEditorVM);
                    RegisterChildViewModel(specificIASEditorVM);
                }
            }
            //There should always be an impact area.
            SelectedImpactArea = ImpactAreas[0];
        }

        #region validation
        private FdaValidationResult IsYearValid()
        {
            FdaValidationResult vr = new FdaValidationResult();
            if (Year == null || Year < 1900 || Year > 3000)
            {
                vr.AddErrorMessage( "A year is required and must be greater than 1900 and less than 3000");
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

            vr.AddErrorMessage( IsYearValid().ErrorMessage);

            foreach (KeyValuePair<ImpactAreaRowItem, SpecificIASEditorVM>  entry in _ImpactAreaEditorDictionary)
            {
                SpecificIASEditorVM vm = entry.Value;
                FdaValidationResult validationResult = vm.GetEditorValidationResult();
                if (!validationResult.IsValid)
                {
                    vr.AddErrorMessage(validationResult.ErrorMessage + Environment.NewLine);
                }
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
                    elementsToSave.Add( vm.CreateSpecificIAS());
                }

                if(Description == null)
                {
                    Description = "";
                }
                //todo: shouldn't this pass the save to base?
                IASPersistenceManager iASPersistenceManager = PersistenceFactory.GetIASManager();
                int id = GetElementID<IASElement>();

                IASElement elemToSave = new IASElement(Name, Description, DateTime.Now.ToString("G"), Year.Value, SelectedStageDamageElement.ChildElement.ID, elementsToSave, id);

                if (IsCreatingNewElement)
                {
                    iASPersistenceManager.SaveNew(elemToSave);
                    IsCreatingNewElement = false;
                }
                else
                {
                    iASPersistenceManager.SaveExisting(elemToSave);
                }

                SavingText = "Last Saved: " + elemToSave.LastEditDate;
                HasChanges = false;
                HasSaved = true;
                OriginalElement = elemToSave;
            }
        }

        public ChildElementComboItem GetSelectedStageDamage()
        {
            return SelectedStageDamageElement;
        }

        private void RemoveStageDamageElement(object sender, ElementAddedEventArgs e)
        {
            SpecificIASEditorVM.RemoveElement(e.Element.ID, StageDamageElements);
            SelectedStageDamageElement = StageDamageElements[0];
        }

        private void UpdateStageDamageElement(object sender, ElementUpdatedEventArgs e)
        {
            SpecificIASEditorVM.UpdateElement(StageDamageElements, SelectedStageDamageElement, e.NewElement);
            if(e.NewElement.ID == SelectedStageDamageElement.ID)
            {
                _SelectedEditorVM.StageDamageSelectionChanged(_SelectedStageDamageElement);
            }
        }

        private void AddStageDamageElement(object sender, ElementAddedEventArgs e)
        {
            StageDamageElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }

        private void StageDamageSelectionChanged()
        {
            foreach(KeyValuePair< ImpactAreaRowItem, SpecificIASEditorVM > specificIAS in _ImpactAreaEditorDictionary)
            {
                specificIAS.Value.StageDamageSelectionChanged(_SelectedStageDamageElement);

            }
        }

    }
}
