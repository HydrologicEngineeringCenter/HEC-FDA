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

namespace ViewModel.Alternatives
{
    public class CreateNewAlternativeVM : BaseEditorVM
    {

        public List<AlternativeRowItem> Rows { get; set; }

        public CreateNewAlternativeVM(List<InflowOutflowElement> conditions, EditorActionManager actionManager) : base(actionManager)
        {
            List<IASElementSet> elems = StudyCache.GetChildElementsOfType<IASElementSet>();

            List<AlternativeRowItem> rows = new List<AlternativeRowItem>();
            foreach(IASElementSet elem in elems)
            {
                AlternativeRowItem condWrapper = new AlternativeRowItem(elem);
                rows.Add(condWrapper);
            }
            Rows = rows;
        }
        //public override void AddValidationRules()
        //{
        //    AddRule(nameof(Name), () => Name != null, "Name cannot be empty.");
        //    AddRule(nameof(Name), () => Name != "", "Name cannot be empty.");
        //}
        public override void Save()
        {
            List<AlternativeRowItem> selectedRows = GetSelectedRows();
            FdaValidationResult vr = IsValid(selectedRows);
            if(vr.IsValid)
            {
                AlternativeElement elemToSave = new AlternativeElement(GetSelectedIASSets());
                Saving.PersistenceFactory.GetAlternativeManager().SaveNew(elemToSave);
            }

        }

        private FdaValidationResult IsValid(List<AlternativeRowItem> selectedRows)
        {

            FdaValidationResult vr = new FdaValidationResult();

            return vr;
        }

        private List<IASElementSet> GetSelectedIASSets()
        {
            List<IASElementSet> selectedSets = new List<IASElementSet>();
            foreach (AlternativeRowItem row in Rows)
            {
                if (row.IsSelected)
                {
                    selectedSets.Add(row.Element);
                }
            }
            return selectedSets;
        }
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

        //public class ConditionWrapper
        //{
        //    public bool IsSelected { get; set; }
        //    public string Name { get; set; }
        //    public InflowOutflowElement Condition { get; set; }
        //    public ConditionWrapper(InflowOutflowElement condition)
        //    {
        //        Condition = condition;
        //        Name = Condition.Name;
        //        IsSelected = false;
        //    }
        //}


    }
}
