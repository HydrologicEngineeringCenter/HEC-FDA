using HEC.CS.Collections;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.LifeLoss;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class IASEditorVM : BaseEditorVM
    {
        #region Fields
        private List<ImpactAreaRowItem> _ImpactAreaNames = new List<ImpactAreaRowItem>();
        private bool _HasImpactArea = true;
        private ChildElementComboItem _SelectedStageDamageElement;
        private ChildElementComboItem _NonFailureSelectedStageDamageElement;
        private bool _HasFailureStageDamage;
        private bool _HasNonFailureStageDamage;
        private bool _HasFailureStageLifeLoss;
        private bool _HasNonFailureStageLifeLoss;
        private ScenarioResults _Results;
        #endregion

        #region Properties

        public bool HasFailureStageDamage
        {
            get { return _HasFailureStageDamage; }
            set
            {
                _HasFailureStageDamage = value;
                OnHasFailureStageDamageChanged(value);
                NotifyPropertyChanged();
            }
        }

        public bool HasNonFailureStageDamage
        {
            get { return _HasNonFailureStageDamage; }
            set
            {
                _HasNonFailureStageDamage = value;
                OnHasNonFailureStageDamageChanged(value);
                NotifyPropertyChanged();
            }
        }

        public bool HasFailureStageLifeLoss
        {
            get { return _HasFailureStageLifeLoss; }
            set
            {
                _HasFailureStageLifeLoss = value;
                OnHasFailureStageLifeLossChanged(value);
                NotifyPropertyChanged();
            }
        }

        public bool HasNonFailureStageLifeLoss
        {
            get { return _HasNonFailureStageLifeLoss; }
            set
            {
                _HasNonFailureStageLifeLoss = value;
                OnHasNonFailureStageLifeLossChanged(value);
                NotifyPropertyChanged();
            }
        }

        public List<SpecificIASEditorVM> ImpactAreaTabs { get; } = new List<SpecificIASEditorVM>();
        public ChildElementComboItem SelectedFailureStageDamageElement
        {
            get { return _SelectedStageDamageElement; }
            set
            {
                _SelectedStageDamageElement = value;
                StageDamageSelectionChanged();
                NotifyPropertyChanged();
            }
        }
        public ChildElementComboItem SelectedNonFailureStageDamageElement
        {
            get { return _NonFailureSelectedStageDamageElement; }
            set
            {
                _NonFailureSelectedStageDamageElement = value;
                NonFailureStageDamageSelectionChanged();
                NotifyPropertyChanged();
            }
        }
        public ChildElementComboItem SelectedNonFailureStageLifeLossElement
        {
            get { return _NonFailureSelectedStageDamageElement; }
            set
            {
                _NonFailureSelectedStageDamageElement = value;
                NotifyPropertyChanged();
            }
        }
        public ChildElementComboItem SelectedFailureStageLifeLossElement
        {
            get { return _NonFailureSelectedStageDamageElement; }
            set
            {
                _NonFailureSelectedStageDamageElement = value;
                NotifyPropertyChanged();
            }
        }

        public CustomObservableCollection<ChildElementComboItem> StageDamageElements { get; } = new CustomObservableCollection<ChildElementComboItem>();
        public CustomObservableCollection<ChildElementComboItem> StageLifeLossElements { get; } = new CustomObservableCollection<ChildElementComboItem>();

        public bool HasImpactArea
        {
            get { return _HasImpactArea; }
            set { _HasImpactArea = value; NotifyPropertyChanged(); }
        }
        public string Year { get; set; } = DateTime.Now.Year.ToString();

        public List<ImpactAreaRowItem> ImpactAreas
        {
            get { return _ImpactAreaNames; }
            set { _ImpactAreaNames = value; NotifyPropertyChanged(); }
        }

        #endregion
        /// <summary>
        /// This is the create new ctor
        /// </summary>
        public IASEditorVM(EditorActionManager manager) : base(manager)
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

            //store the results so that we can attach them on the element that we save.
            _Results = elem.Results;
        }

        private void Initialize()
        {
            List<AggregatedStageDamageElement> aggregatedStageDamageElements = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            List<ChildElement> childElems = new List<ChildElement>();
            childElems.AddRange(aggregatedStageDamageElements);
            StageDamageElements.AddRange(CreateComboItems(childElems));
            SelectedFailureStageDamageElement = StageDamageElements.First();

            List<StageLifeLossElement> stageLifeLossElements = StudyCache.GetChildElementsOfType<StageLifeLossElement>();
            List<ChildElement> lifeLossElems = [];
            lifeLossElems.AddRange(stageLifeLossElements);
            StageLifeLossElements.AddRange(CreateComboItems(lifeLossElems));


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
                    ImpactAreaTabs.Add(specificIASEditorVM);
                    RegisterChildViewModel(specificIASEditorVM);
                }
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
                    ImpactAreaTabs.Add(specificIASEditorVM);
                    RegisterChildViewModel(specificIASEditorVM);
                }
                else
                {
                    SpecificIASEditorVM specificIASEditorVM = new SpecificIASEditorVM(row, GetSelectedStageDamage);
                    specificIASEditorVM.RequestNavigation += Navigate;
                    ImpactAreaTabs.Add(specificIASEditorVM);
                    RegisterChildViewModel(specificIASEditorVM);
                }
            }

            //select the correct stage damage curve
            SelectedFailureStageDamageElement = StageDamageElements.FirstOrDefault(stage => stage.ChildElement != null && stage.ChildElement.ID == elem.StageDamageID);
            if (SelectedFailureStageDamageElement == null && StageDamageElements.Count > 0)
                SelectedFailureStageDamageElement = StageDamageElements[0];
            else
                HasFailureStageDamage = true; // the selected element is not null

            //select the correct non-failure stage damage curve. 
            SelectedNonFailureStageDamageElement = StageDamageElements.FirstOrDefault(stage => stage.ChildElement != null && stage.ChildElement.ID == elem.NonFailureStageDamageID);
            if (SelectedNonFailureStageDamageElement == null && StageDamageElements.Count > 0)
                SelectedNonFailureStageDamageElement = StageDamageElements[0];
            else
                HasNonFailureStageDamage = true;

            //setting this one after the specific ias editors have been created is important
            //because it updates those editors as well. 
            HasNonFailureStageDamage = elem.HasNonFailureStageDamage;
        }

        private void NotifyUserOfImpactAreasInError()
        {
            FdaValidationResult vr = new FdaValidationResult();

            foreach (SpecificIASEditorVM vm in ImpactAreaTabs)
            {
                FdaValidationResult result = vm.GetValidationResults();
                if (!result.IsValid)
                {
                    vr.AddErrorMessage(result.ErrorMessage);
                }
            }

            if (!vr.IsValid)
            {
                MessageBox.Show("Scenario has saved successfully.\n" +
                    "This scenario will not be able to compute for the following reasons:\n" +
                    vr.ErrorMessage.ToString(), "Saved with Errors", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public override void Save()
        {
            //we now have no validation. The only thing that we require is a name and the save button will not be enabled if there is no name
            //so we don't have to check for that here.

            //get the list of specific IAS elements.
            List<SpecificIAS> elementsToSave = new List<SpecificIAS>();
            foreach (SpecificIASEditorVM vm in ImpactAreaTabs)
            {
                elementsToSave.Add(vm.CreateSpecificIAS());
            }

            if (Description == null)
            {
                Description = "";
            }
            //todo: shouldn't this pass the save to base?
            IASPersistenceManager iASPersistenceManager = PersistenceFactory.GetIASManager();
            int id = GetElementID<IASElement>();
            //todo: is this what I want?
            int stageDamageId = -1;
            if (SelectedFailureStageDamageElement != null && SelectedFailureStageDamageElement.ChildElement != null)
            {
                stageDamageId = SelectedFailureStageDamageElement.ChildElement.ID;
            }

            int nonBreachStageDamageId = -1;
            if (SelectedNonFailureStageDamageElement != null && SelectedNonFailureStageDamageElement.ChildElement != null)
            {
                nonBreachStageDamageId = SelectedNonFailureStageDamageElement.ChildElement.ID;
            }
            IASElement elemToSave = new IASElement(Name, Description, DateTime.Now.ToString("G"), Year, stageDamageId, nonBreachStageDamageId, HasNonFailureStageDamage, elementsToSave, id);

            if (IsCreatingNewElement)
            {
                iASPersistenceManager.SaveNew(elemToSave);
                IsCreatingNewElement = false;
            }
            else
            {
                elemToSave.Results = _Results;
                iASPersistenceManager.SaveExisting(elemToSave);
                IASTooltipHelper.UpdateTooltip(elemToSave);
            }

            SavingText = "Last Saved: " + elemToSave.LastEditDate;
            HasChanges = false;
            HasSaved = true;
            OriginalElement = elemToSave;
            NotifyUserOfImpactAreasInError();
        }

        public ChildElementComboItem GetSelectedStageDamage()
        {
            return SelectedFailureStageDamageElement;
        }

        private void RemoveStageDamageElement(object sender, ElementAddedEventArgs e)
        {
            SpecificIASEditorVM.RemoveElement(e.Element.ID, StageDamageElements);
            SelectedFailureStageDamageElement = StageDamageElements[0];
        }

        private void UpdateStageDamageElement(object sender, ElementUpdatedEventArgs e)
        {
            SpecificIASEditorVM.UpdateElement(StageDamageElements, SelectedFailureStageDamageElement, e.NewElement);
            if (e.NewElement.ID == SelectedFailureStageDamageElement.ID)
            {
                foreach (SpecificIASEditorVM specificIAS in ImpactAreaTabs)
                {
                    specificIAS.StageDamageSelectionChanged(_SelectedStageDamageElement);
                }
            }
        }

        private void AddStageDamageElement(object sender, ElementAddedEventArgs e)
        {
            StageDamageElements.Add(new ChildElementComboItem((ChildElement)e.Element));
        }

        private void NonFailureStageDamageSelectionChanged()
        {
            foreach (SpecificIASEditorVM specificIAS in ImpactAreaTabs)
            {
                specificIAS.NonFailureSelectedStageDamage = _NonFailureSelectedStageDamageElement;
                specificIAS.UpdateSufficientToCompute();
            }
        }
        private void StageDamageSelectionChanged()
        {
            foreach (SpecificIASEditorVM specificIAS in ImpactAreaTabs)
            {
                specificIAS.StageDamageSelectionChanged(_SelectedStageDamageElement);
                specificIAS.UpdateSufficientToCompute();
            }
        }

        private void OnHasFailureStageDamageChanged(bool newVal)
        {
            if (!newVal)
                SelectedFailureStageDamageElement = StageDamageElements[0];
        }

        private void OnHasNonFailureStageDamageChanged(bool newValue)
        {
            if (!newValue)
                SelectedNonFailureStageDamageElement = StageDamageElements[0];

            foreach (SpecificIASEditorVM specificIAS in ImpactAreaTabs)
            {
                //pass the boolean value to the tab vm's so that they can flip
                //their boolean and notify property changed so that the exterior interior
                //option updates. 
                specificIAS.HasNonFailureStageDamage = HasNonFailureStageDamage;
                specificIAS.UpdateSufficientToCompute();

            }
        }

        private void OnHasFailureStageLifeLossChanged(bool newVal)
        {
            if (!newVal)
                SelectedFailureStageLifeLossElement = StageLifeLossElements[0];
        }

        private void OnHasNonFailureStageLifeLossChanged(bool newVal)
        {
            if (!newVal)
                SelectedNonFailureStageLifeLossElement = StageLifeLossElements[0];
        }

    }
}
