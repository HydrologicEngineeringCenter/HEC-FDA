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
        public CustomObservableCollection<IASElement> Scenarios { get; } = new CustomObservableCollection<IASElement>();
        public IASElement SelectedBaseScenario { get; set; }
        public IASElement SelectedFutureScenario { get; set; }
        public int BaseYear { get; set; } = DateTime.Now.Year;
        public int FutureYear { get; set; } = DateTime.Now.Year;


        #region Constructors
        /// <summary>
        /// Create new ctor
        /// </summary>
        /// <param name="actionManager"></param>
        public CreateNewAlternativeVM( EditorActionManager actionManager) : base(actionManager)
        {
            Scenarios.AddRange(StudyCache.GetChildElementsOfType<IASElement>());
            ListenToIASEvents();
        }

        /// <summary>
        /// Open for edit ctor.
        /// </summary>
        /// <param name="elem"></param>
        public CreateNewAlternativeVM(AlternativeElement elem, EditorActionManager actionManager) :base(elem, actionManager)
        {
            Scenarios.AddRange(StudyCache.GetChildElementsOfType<IASElement>());
            Name = elem.Name;
            Description = elem.Description;
            SelectedBaseScenario = elem.BaseScenario.GetElement();
            BaseYear = elem.BaseScenario.Year;
            if (elem.FutureScenario != null)
            {
                SelectedFutureScenario = elem.FutureScenario.GetElement();
                FutureYear = elem.FutureScenario.Year;
            }
            ListenToIASEvents();
        }

        #endregion

        private void ListenToIASEvents()
        {
            StudyCache.IASElementAdded += IASAdded;
            StudyCache.IASElementRemoved += IASRemoved;
            StudyCache.IASElementUpdated += IASUpdated;
        }

        private void IASAdded(object sender, ElementAddedEventArgs e)
        {
            Scenarios.Add((IASElement)e.Element);
        }

        private void IASRemoved(object sender, ElementAddedEventArgs e)
        {
            Scenarios.Remove(Scenarios.Where(row => row.ID == e.Element.ID).Single());
        }

        private void IASUpdated(object sender, ElementUpdatedEventArgs e)
        {
            IASElement newElement = (IASElement)e.NewElement;
            int idToUpdate = newElement.ID;

            //find the row with this id and update the row's values;
            IASElement foundRow = Scenarios.Where(row => row.ID == idToUpdate).SingleOrDefault();
            if(foundRow != null)
            {
                foundRow.Name = newElement.Name;
            }   
        }

        private FdaValidationResult ValidateYears()
        {
            FdaValidationResult result = new();
            if(BaseYear<1900 || BaseYear > 3000)
            {
                result.AddErrorMessage("A base year is required and must be greater than 1900 and less than 3000.");
            }
            if (SelectedFutureScenario != null)
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
            if(SelectedBaseScenario == null)
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
                AlternativeScenario baseScenario = new(SelectedBaseScenario.ID, BaseYear);
                AlternativeScenario futureScenario = SelectedFutureScenario != null ? new(SelectedFutureScenario.ID, FutureYear) : null;
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
