using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using HEC.CS.Collections;
using HEC.FDA.Model.metrics;
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
        private SpecificIASEditorVM _SelectedEditorVM;
        private bool _HasImpactArea = true;
        private ChildElementComboItem _SelectedStageDamageElement;
        private ChildElementComboItem _NonFailureSelectedStageDamageElement;
        private bool _HasNonFailureStageDamage;
        private ScenarioResults _Results;
        #endregion

        #region Properties
 
        public string StageDamageText
        {
            get
            {
                if(HasNonFailureStageDamage)
                {
                    return "Failure\nStage-Damage";
                }
                else
                {
                    return "Stage-Damage";
                }
            }
        }

        public bool HasNonFailureStageDamage
        {
            get { return _HasNonFailureStageDamage; }
            set 
            { 
                _HasNonFailureStageDamage = value; 
                HasNonFailureStageDamageChanged(); 
                NotifyPropertyChanged("StageDamageText");
                NotifyPropertyChanged();
            }
        }
        public List<SpecificIASEditorVM> ImpactAreaTabs { get; } = new List<SpecificIASEditorVM>();
        public ChildElementComboItem SelectedStageDamageElement
        {
            get { return _SelectedStageDamageElement; }
            set { _SelectedStageDamageElement = value; StageDamageSelectionChanged(); }
        }
        public ChildElementComboItem NonFailureSelectedStageDamageElement
        {
            get { return _NonFailureSelectedStageDamageElement; }
            set { _NonFailureSelectedStageDamageElement = value; StageDamageSelectionChanged(); }
        }

        public CustomObservableCollection<ChildElementComboItem> StageDamageElements { get; } = new CustomObservableCollection<ChildElementComboItem>();

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

            //store the results so that we can attach them on the element that we save.
            _Results = elem.Results;
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
            SelectedStageDamageElement = StageDamageElements.FirstOrDefault(stage => stage.ChildElement != null && stage.ChildElement.ID == elem.StageDamageID);
            if (SelectedStageDamageElement == null && StageDamageElements.Count > 0)
            {
                SelectedStageDamageElement = StageDamageElements[0];
            }

            //select the correct non-failure stage damage curve. 
            NonFailureSelectedStageDamageElement = StageDamageElements.FirstOrDefault(stage => stage.ChildElement != null && stage.ChildElement.ID == elem.NonFailureStageDamageID);
            if (NonFailureSelectedStageDamageElement == null && StageDamageElements.Count > 0)
            {
                NonFailureSelectedStageDamageElement = StageDamageElements[0];
            }

            //setting this one after the specific ias editors have been created is important
            //because it updates those editors as well. 
            HasNonFailureStageDamage = elem.HasNonFailureStageDamage;
        }

        private void NotifyUserOfImpactAreasInError()
        {
            FdaValidationResult vr = new FdaValidationResult();

            foreach (SpecificIASEditorVM vm in ImpactAreaTabs)
            {
                FdaValidationResult result = vm.GetEditorValidationResult();
                if(!result.IsValid)
                {
                    vr.AddErrorMessage(result.ErrorMessage);
                }
            }

            if(!vr.IsValid)
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
            if (SelectedStageDamageElement != null && SelectedStageDamageElement.ChildElement != null)
            {
                stageDamageId = SelectedStageDamageElement.ChildElement.ID;
            }

            int nonBreachStageDamageId = -1;
            if (NonFailureSelectedStageDamageElement != null && NonFailureSelectedStageDamageElement.ChildElement != null)
            {
                nonBreachStageDamageId = NonFailureSelectedStageDamageElement.ChildElement.ID;
            }
            IASElement elemToSave = new IASElement(Name, Description, DateTime.Now.ToString("G"), Year, stageDamageId,nonBreachStageDamageId,HasNonFailureStageDamage, elementsToSave, id);

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
            foreach(SpecificIASEditorVM specificIAS in ImpactAreaTabs)
            {
                specificIAS.StageDamageSelectionChanged(_SelectedStageDamageElement);
                specificIAS.UpdateSufficientToCompute();
            }
        }

        private void HasNonFailureStageDamageChanged()
        {
            foreach(SpecificIASEditorVM specificIAS in ImpactAreaTabs)
            {
                //pass the boolean value to the tab vm's so that they can flip
                //their boolean and notify property changed so that the exterior interior
                //option updates. 
                specificIAS.HasNonFailureStageDamage = HasNonFailureStageDamage;
            }
        }

    }
}
