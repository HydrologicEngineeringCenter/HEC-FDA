using ViewModel.ImpactAreaScenario;
using ViewModel.Editors;
using ViewModel.FlowTransforms;
using Functions;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Utilities;
using System.Windows;
using System.Collections.ObjectModel;

namespace ViewModel.Alternatives
{
    public class CreateNewAlternativeVM : BaseEditorVM
    {

        public ObservableCollection<AlternativeRowItem> Rows { get; set; }

        #region Constructors
        /// <summary>
        /// Create new ctor
        /// </summary>
        /// <param name="actionManager"></param>
        public CreateNewAlternativeVM( EditorActionManager actionManager) : base(actionManager)
        {
            Rows = CreateRowsForIASElementsInStudy();
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
            List<int> iASElementSets = elem.IASElementSets;
            SelectSavedRows(iASElementSets);
            ListenToIASEvents();
        }

        #endregion

        private void ListenToIASEvents()
        {
            StudyCache.IASElementAdded += IASAdded;
            StudyCache.IASElementRemoved += IASRemoved;
            StudyCache.IASElementUpdated += IASUpdated;
        }

        private void IASAdded(object sender, Saving.ElementAddedEventArgs e)
        {
            Rows.Add(new AlternativeRowItem((IASElementSet)e.Element));
        }

        private void IASRemoved(object sender, Saving.ElementAddedEventArgs e)
        {
            Rows.Remove(Rows.Where(row => row.ID == e.ID).Single());
        }

        private void IASUpdated(object sender, Saving.ElementUpdatedEventArgs e)
        {
            IASElementSet oldElement = (IASElementSet)e.OldElement;
            IASElementSet newElement = (IASElementSet)e.NewElement;
            int idToUpdate = newElement.GetElementID();

            //find the row with this id and update the row's values;
            AlternativeRowItem foundRow = Rows.Where(row => row.ID == idToUpdate).SingleOrDefault();
            if(foundRow != null)
            {
                foundRow.Name = newElement.Name;
                foundRow.HasComputed = newElement.HasComputed;
                foundRow.Year = newElement.AnalysisYear;
            }
           
        }

        private void SelectSavedRows(List<int> savedIASIDs)
        {
            //if an id that was saved no longer exists then i will ignore that id when populating the rows. The alternative will
            //still point to the missing id until the user saves. If a new id exists that wasn't saved then i will make a row for it.
            ObservableCollection<AlternativeRowItem> currentIASRowsInStudy = CreateRowsForIASElementsInStudy();
            foreach(int id in savedIASIDs)
            {
                foreach(AlternativeRowItem row in currentIASRowsInStudy)
                {
                    if(row.ID == id)
                    {
                        row.IsSelected = true;
                        //i don't think it is possible for a saved IAS to not be computed so i mark it computed.
                        row.HasComputed = true;
                    }
                }
            }
            Rows = currentIASRowsInStudy;
        }
        

        private ObservableCollection<AlternativeRowItem> CreateRowsForIASElementsInStudy()
        {
            List<IASElementSet> elems = StudyCache.GetChildElementsOfType<IASElementSet>();

            ObservableCollection<AlternativeRowItem> rows = new ObservableCollection<AlternativeRowItem>();
            foreach (IASElementSet elem in elems)
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
            if(vr.IsValid)
            {

                if (selectedRows[0].Year == selectedRows[1].Year)
                {
                    var Result = MessageBox.Show("Two impact area scenarios with different analysis years must be selected to calculate average annual equivalent damage." + Environment.NewLine +
                        Environment.NewLine + "Do you want to continue saving?", "Same Analysis Years", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (Result == MessageBoxResult.No)
                    {
                        //don't save.
                        return;
                    }
                }

                AlternativeElement elemToSave = new AlternativeElement(Name, Description, GetSelectedIASSets());
                Saving.PersistenceFactory.GetAlternativeManager().SaveNew(elemToSave);
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
                vr.AddValidationResult( new FdaValidationResult(false, "Two impact area scenarios are required to create an alternative."));
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
