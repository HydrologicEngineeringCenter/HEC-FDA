using HEC.CS.Collections;
using HEC.FDA.Model.metrics;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Saving;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HEC.FDA.ViewModel.Results
{
    public class ScenarioDamageSummaryVM : BaseViewModel
    {
        private DataTable _DamCatTable;

        public CustomObservableCollection<SelectableChildElement> SelectableElements { get; } = [];
        public CustomObservableCollection<ScenarioDamageRowItem> Rows { get; } = [];
        public CustomObservableCollection<ScenarioPerformanceRowItem> PerformanceRows { get; } = [];
        public CustomObservableCollection<AssuranceOfAEPRowItem> AssuranceOfAEPRows { get; } = [];

        public DataTable DamCatTable
        {
            get { return _DamCatTable; }
            set { _DamCatTable = value; NotifyPropertyChanged(); }
        }

        public ScenarioDamageSummaryVM(List<IASElement> selectedScenarioElems)
        {
            List<IASElement> allElements = StudyCache.GetChildElementsOfType<IASElement>();

            foreach (IASElement element in allElements)
            {
                SelectableChildElement selectElem = new SelectableChildElement(element);
                SelectableElements.Add(selectElem);
                //the selectable elements are selected by default. We want to toggle all the elements that 
                //aren't in the passed in list off.
                if(!selectedScenarioElems.Contains(element))
                {
                    selectElem.IsSelected = false;
                }
                selectElem.SelectionChanged += SelectElem_SelectionChanged;
            }

            LoadTables();
            ListenToChildElementUpdateEvents();
        }

        public ScenarioDamageSummaryVM()
        {
            List<IASElement> allElements = StudyCache.GetChildElementsOfType<IASElement>();

            foreach (IASElement element in allElements)
            {
                SelectableChildElement selectElem = new SelectableChildElement(element);
                selectElem.SelectionChanged += SelectElem_SelectionChanged;
                SelectableElements.Add(selectElem);
            }

            LoadTables();
            ListenToChildElementUpdateEvents();
        }

        public void ListenToChildElementUpdateEvents()
        {
            StudyCache.IASElementAdded += IASAdded;
            StudyCache.IASElementRemoved += IASRemoved;
            StudyCache.IASElementUpdated += IASUpdated;
        }

        private void IASAdded(object sender, ElementAddedEventArgs e)
        {
            SelectableChildElement newRow = new SelectableChildElement((IASElement)e.Element);
            SelectableElements.Add(newRow);
        }

        private void IASRemoved(object sender, ElementAddedEventArgs e)
        {
            SelectableElements.Remove(SelectableElements.Where(row => row.Element.ID == e.Element.ID).Single());
        }

        private void IASUpdated(object sender, ElementUpdatedEventArgs e)
        {
            IASElement newElement = (IASElement)e.NewElement;
            int idToUpdate = newElement.ID;

            //find the row with this id and update the row's values;
            SelectableChildElement foundRow = SelectableElements.Where(row => row.Element.ID == idToUpdate).SingleOrDefault();
            if (foundRow != null)
            {
                foundRow.Update(newElement);
            }
        }

        private void LoadTables()
        {
            List<IASElement> elems = GetSelectedElements();
            List<ScenarioDamCatRowItem> damCatRows = new List<ScenarioDamCatRowItem>();
            Rows.Clear();
            PerformanceRows.Clear();
            AssuranceOfAEPRows.Clear();
            foreach (IASElement element in elems)
            {
                Rows.AddRange(ScenarioDamageRowItem.CreateScenarioDamageRowItems(element));
                damCatRows.AddRange(ScenarioDamCatRowItem.CreateScenarioDamCatRowItems(element));
                List<ImpactAreaScenarioResults> resultsList = element.Results.ResultsList;
                foreach (ImpactAreaScenarioResults impactAreaScenarioResults in resultsList)
                { 
                    int iasID = impactAreaScenarioResults.ImpactAreaID;
                    SpecificIAS ias = element.SpecificIASElements.Where(ias => ias.ImpactAreaID == iasID).First();

                    foreach (Threshold threshold in impactAreaScenarioResults.PerformanceByThresholds.ListOfThresholds)
                    {
                        PerformanceRows.Add(new ScenarioPerformanceRowItem(element, ias, threshold));
                        AssuranceOfAEPRows.Add(new AssuranceOfAEPRowItem(element, ias, threshold));
                    }
                }
            }
            LoadDamCatDataTable(damCatRows);
        }

        private void SelectElem_SelectionChanged(object sender, System.EventArgs e)
        {
            LoadTables();
        }

        private List<IASElement> GetSelectedElements()
        {
            List<IASElement> selectedElements = new List<IASElement>();
            foreach(SelectableChildElement selectElem in SelectableElements)
            {
                if(selectElem.IsSelected)
                {
                    selectedElements.Add(selectElem.Element as IASElement);
                }
            }
            return selectedElements;
        }

        private void LoadDamCatDataTable(List<ScenarioDamCatRowItem> rows)
        {
            
            _DamCatTable = new DataTable();
            DataColumn nameCol = new("Name", typeof(string));
            _DamCatTable.Columns.Add(nameCol);
            DataColumn yearCol = new("Analysis Year", typeof(int));
            _DamCatTable.Columns.Add(yearCol);
            DataColumn impactAreaCol = new("Impact Area", typeof(string));
            _DamCatTable.Columns.Add(impactAreaCol);
            List<string> allUniqueDamCats = GetAllDamCats(rows);
            foreach (string damCat in allUniqueDamCats)
            {
                DataColumn dataColumn = new(damCat, typeof(double));
                _DamCatTable.Columns.Add(dataColumn);
            }

            foreach(ScenarioDamCatRowItem row in rows)
            {
                AddDamCatRowToTable(row, allUniqueDamCats);
            }
            NotifyPropertyChanged(nameof(DamCatTable));
        }

        private void AddDamCatRowToTable(ScenarioDamCatRowItem row, List<string> allDamCats)
        {
            DataRow myRow = _DamCatTable.NewRow();
            myRow["Name"] = row.Name;
            myRow["Analysis Year"] = row.AnalysisYear;
            myRow["Impact Area"] = row.ImpactAreaName;
            foreach(string damCat in allDamCats)
            {
                if(row.DamCatMap.ContainsKey(damCat))
                {
                    myRow[damCat] = row.DamCatMap[damCat];
                }
                else
                {
                    //this scenario doesn't have a value for that dam cat. Assign 0.
                    myRow[damCat] = 0;
                }
            }
            _DamCatTable.Rows.Add(myRow);
        }

        private List<string> GetAllDamCats(List<ScenarioDamCatRowItem> rows)
        {
            HashSet<string> uniqueDamCats = new HashSet<string>();
            foreach(ScenarioDamCatRowItem row in rows)
            {
                foreach(string damCat in row.DamCatMap.Keys)
                {
                    uniqueDamCats.Add(damCat);
                }
            }
            return uniqueDamCats.ToList();
        }
    }
}
