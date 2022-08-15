using HEC.CS.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;

namespace HEC.FDA.ViewModel.Alternatives
{
    public class CreateNewAlternativeVM : BaseEditorVM
    {
        public CustomObservableCollection<AlternativeRowItem> Rows { get; } = new CustomObservableCollection<AlternativeRowItem>();
        
        #region Constructors
        /// <summary>
        /// Create new ctor
        /// </summary>
        /// <param name="actionManager"></param>
        public CreateNewAlternativeVM( EditorActionManager actionManager) : base(actionManager)
        {
            Rows.AddRange(CreateRowsForIASElementsInStudy());
            ListenToIASEvents();
        }

        /// <summary>
        /// Open for edit ctor.
        /// </summary>
        /// <param name="elem"></param>
        public CreateNewAlternativeVM(AlternativeElement elem, EditorActionManager actionManager) :base(elem, actionManager)
        {
            Name = elem.Name;
            Description = elem.Description;
            SelectSavedRows(elem.IASElementSets);
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
            Rows.Add(new AlternativeRowItem((IASElement)e.Element));
        }

        private void IASRemoved(object sender, ElementAddedEventArgs e)
        {
            Rows.Remove(Rows.Where(row => row.ID == e.Element.ID).Single());
        }

        private void IASUpdated(object sender, ElementUpdatedEventArgs e)
        {
            IASElement newElement = (IASElement)e.NewElement;
            int idToUpdate = newElement.ID;

            //find the row with this id and update the row's values;
            AlternativeRowItem foundRow = Rows.Where(row => row.ID == idToUpdate).SingleOrDefault();
            if(foundRow != null)
            {
                foundRow.Year = newElement.AnalysisYear;
                //name has to be set after the year for the display name to get updated properly.
                foundRow.Name = newElement.Name;
            }   
        }

        private void SelectSavedRows(List<int> savedIASIDs)
        {
            //if an id that was saved no longer exists then i will ignore that id when populating the rows. The alternative will
            //still point to the missing id until the user saves. If a new id exists that wasn't saved then i will make a row for it.
            List<AlternativeRowItem> currentIASRowsInStudy = CreateRowsForIASElementsInStudy();
            foreach(int id in savedIASIDs)
            {
                foreach(AlternativeRowItem row in currentIASRowsInStudy)
                {
                    if(row.ID == id)
                    {
                        row.IsSelected = true;
                    }
                }
            }
            Rows.AddRange( currentIASRowsInStudy);
        }

        private List<AlternativeRowItem> CreateRowsForIASElementsInStudy()
        {
            List<IASElement> elems = StudyCache.GetChildElementsOfType<IASElement>();

            List<AlternativeRowItem> rows = new List<AlternativeRowItem>();
            foreach (IASElement elem in elems)
            {
                AlternativeRowItem condWrapper = new AlternativeRowItem(elem);
                rows.Add(condWrapper);
            }
            return rows;
        }

        public override void Save()
        {
            List<AlternativeRowItem> selectedRows = GetSelectedRows();
            FdaValidationResult vr = IsValid(selectedRows);
            if (vr.IsValid)
            {
                if (selectedRows[0].Year == selectedRows[1].Year)
                {
                    var Result = MessageBox.Show("Two scenarios with different analysis years must be selected to calculate average annual equivalent damage." + Environment.NewLine +
                        Environment.NewLine + "Do you want to continue saving?", "Same Analysis Years", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (Result == MessageBoxResult.No)
                    {
                        //don't save.
                        return;
                    }
                }

                int id = PersistenceFactory.GetElementManager<AlternativeElement>().GetNextAvailableId();
                if (!IsCreatingNewElement)
                {
                    id = OriginalElement.ID;
                }
                AlternativeElement elemToSave = new AlternativeElement(Name, Description, DateTime.Now.ToString("G"), GetSelectedIASSets(), id);
                Save(elemToSave);
            }
            else
            {
                MessageBox.Show(vr.ErrorMessage.ToString(), "Cannot Save", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        private FdaValidationResult IsValid(List<AlternativeRowItem> selectedRows)
        {
            FdaValidationResult vr = new FdaValidationResult();
            //There has to be two and only two selected impact areas.
            if(selectedRows.Count != 2)
            {
                vr.AddErrorMessage("Two impact area scenarios are required to create an alternative.");
            }

            return vr;
        }

        /// <summary>
        /// Gets the IAS element id's for the selected rows.
        /// </summary>
        /// <returns></returns>
        private List<int> GetSelectedIASSets()
        {
            List<int> selectedSets = new List<int>();
            foreach (AlternativeRowItem row in Rows)
            {
                if (row.IsSelected)
                {
                    selectedSets.Add(row.ID);
                }
            }
            return selectedSets;
        }

        /// <summary>
        /// Gets the rows that the user has selected.
        /// </summary>
        /// <returns></returns>
        private List<AlternativeRowItem> GetSelectedRows()
        {
            List<AlternativeRowItem> selectedRows = new List<AlternativeRowItem>();
            foreach(AlternativeRowItem row in Rows)
            {
                if(row.IsSelected)
                {
                    selectedRows.Add(row);
                }
            }
            return selectedRows;
        }

    }
}
