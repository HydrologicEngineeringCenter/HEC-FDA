using HEC.CS.Collections;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HEC.FDA.ViewModel.Alternatives
{
    public class CreateNewAlternativeVM : BaseEditorVM
    {
        private static readonly ScenarioRowItem _NoneItem = new();
        public CustomObservableCollection<ScenarioRowItem> Scenarios { get; } = new CustomObservableCollection<ScenarioRowItem>();

        private ScenarioRowItem _SelectedBaseScenario;
        public ScenarioRowItem SelectedBaseScenario
        {
            get { return _SelectedBaseScenario; }
            set { _SelectedBaseScenario = value; NotifyPropertyChanged(); }
        }

        private ScenarioRowItem _SelectedFutureScenario;
        public ScenarioRowItem SelectedFutureScenario
        {
            get { return _SelectedFutureScenario; }
            set { _SelectedFutureScenario = value; NotifyPropertyChanged(); }
        }

        public int BaseYear { get; set; } = DateTime.Now.Year;
        public int FutureYear { get; set; } = DateTime.Now.Year;


        #region Constructors
        /// <summary>
        /// Create new ctor
        /// </summary>
        /// <param name="actionManager"></param>
        public CreateNewAlternativeVM( EditorActionManager actionManager) : base(actionManager)
        {
            LoadScenarios();
            ListenToIASEvents();
        }

        /// <summary>
        /// Open for edit ctor.
        /// </summary>
        /// <param name="elem"></param>
        public CreateNewAlternativeVM(AlternativeElement elem, EditorActionManager actionManager) :base(elem, actionManager)
        {
            LoadScenarios();
            Name = elem.Name;
            Description = elem.Description;
            SelectedBaseScenario = FindRowItem(elem.BaseScenario.GetElement());
            BaseYear = elem.BaseScenario.Year;
            if (elem.FutureScenario != null)
            {
                SelectedFutureScenario = FindRowItem(elem.FutureScenario.GetElement());
                FutureYear = elem.FutureScenario.Year;
            }
            ListenToIASEvents();
        }

        #endregion

        private void LoadScenarios()
        {
            Scenarios.Add(_NoneItem);
            Scenarios.AddRange(StudyCache.GetChildElementsOfType<IASElement>().Select(e => new ScenarioRowItem(e)));
        }

        private ScenarioRowItem FindRowItem(IASElement element)
        {
            if (element == null) return _NoneItem;
            return Scenarios.FirstOrDefault(row => row.Element != null && row.Element.ID == element.ID) ?? _NoneItem;
        }

        private void ListenToIASEvents()
        {
            StudyCache.IASElementAdded += IASAdded;
            StudyCache.IASElementRemoved += IASRemoved;
            StudyCache.IASElementUpdated += IASUpdated;
        }

        private void IASAdded(object sender, ElementAddedEventArgs e)
        {
            Scenarios.Add(new ScenarioRowItem((IASElement)e.Element));
        }

        private void IASRemoved(object sender, ElementAddedEventArgs e)
        {
            ScenarioRowItem toRemove = Scenarios.Where(row => row.Element != null && row.Element.ID == e.Element.ID).Single();
            if (SelectedBaseScenario == toRemove) SelectedBaseScenario = _NoneItem;
            if (SelectedFutureScenario == toRemove) SelectedFutureScenario = _NoneItem;
            Scenarios.Remove(toRemove);
        }

        private void IASUpdated(object sender, ElementUpdatedEventArgs e)
        {
            IASElement newElement = (IASElement)e.NewElement;
            int idToUpdate = newElement.ID;

            //find the row with this id and update the row's values;
            ScenarioRowItem foundRow = Scenarios.Where(row => row.Element != null && row.Element.ID == idToUpdate).SingleOrDefault();
            if(foundRow != null)
            {
                foundRow.Element.Name = newElement.Name;
            }
        }

        private FdaValidationResult ValidateYears()
        {
            FdaValidationResult result = new();
            if(BaseYear<1900 || BaseYear > 3000)
            {
                result.AddErrorMessage("A base year is required and must be greater than 1900 and less than 3000.");
            }
            if (SelectedFutureScenario?.Element != null)
            {
                if (FutureYear < 1900 || FutureYear > 3000)
                {
                    result.AddErrorMessage("A future year is required and must be greater than 1900 and less than 3000.");
                }
                if (BaseYear >= FutureYear)
                {
                    result.AddErrorMessage("The base year must be before the future year.");
                }
            }
            return result;
        }

        private FdaValidationResult ValidateScenarioSelections()
        {
            FdaValidationResult vr = new();
            if(SelectedBaseScenario?.Element == null)
            {
                vr.AddErrorMessage("Base scenario is required.");
            }
            return vr;
        }

        private FdaValidationResult ValidateAlternative()
        {
            FdaValidationResult vr = new();
            vr.AddErrorMessage(ValidateYears().ErrorMessage);
            vr.AddErrorMessage(ValidateScenarioSelections().ErrorMessage);
            return vr;
        }

        public override void Save()
        {
            FdaValidationResult vr = ValidateAlternative();
            if (vr.IsValid)
            {
                int id = PersistenceFactory.GetElementManager<AlternativeElement>().GetNextAvailableId();
                if (!IsCreatingNewElement)
                {
                    id = OriginalElement.ID;
                }
                AlternativeScenario baseScenario = new(SelectedBaseScenario.Element.ID, BaseYear);
                AlternativeScenario futureScenario = SelectedFutureScenario?.Element != null ? new(SelectedFutureScenario.Element.ID, FutureYear) : null;
                AlternativeElement elemToSave = new(Name, Description, DateTime.Now.ToString("G"), baseScenario, futureScenario, id);
                Save(elemToSave);
            }
            else
            {
                MessageBox.Show(vr.ErrorMessage.ToString(), "Cannot Save", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
