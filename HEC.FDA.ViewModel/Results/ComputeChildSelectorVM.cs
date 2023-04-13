using HEC.CS.Collections;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Results
{
    public class ComputeChildSelectorVM : BaseViewModel
    {
        public enum ChildType
        {
            SCENARIOS,
            ALTERNATIVES
        }

        private ChildType _ChildType;

        public string SelectionLabel { get; set; }

        public CustomObservableCollection<ComputeChildRowItem> Rows { get; } = new CustomObservableCollection<ComputeChildRowItem>();

        public ComputeChildSelectorVM(ChildType childType)
        {
            _ChildType = childType;
            if (childType == ChildType.SCENARIOS)
            {
                SelectionLabel = "Select Scenarios to Compute:";
                LoadScenarios();
            }
            else if(childType == ChildType.ALTERNATIVES)
            {
                SelectionLabel = "Select Alternatives to View:";
                LoadAlternatives();
            }
        }

        private void LoadScenarios()
        {
            List<IASElement> elems = StudyCache.GetChildElementsOfType<IASElement>();

            foreach (IASElement elem in elems)
            {
                Rows.Add(new ComputeChildRowItem(elem));
            }
        }

        private void LoadAlternatives()
        {
            List<AlternativeElement> elems = StudyCache.GetChildElementsOfType<AlternativeElement>();

            foreach (AlternativeElement elem in elems)
            {
                Rows.Add(new ComputeChildRowItem(elem));
            }
        }

        private List<ComputeChildRowItem> GetSelectedRows()
        {
            List<ComputeChildRowItem> selectedRows = new List<ComputeChildRowItem>();
            foreach (ComputeChildRowItem row in Rows)
            {
                if (row.IsSelected)
                {
                    selectedRows.Add(row);
                }
            }
            return selectedRows;
        }

        public void ComputeClicked()
        {
            List<ComputeChildRowItem> computeChildRowItems = GetSelectedRows();
            if(_ChildType == ChildType.SCENARIOS)
            {
                ComputeScenarios(computeChildRowItems);
                //ScenarioDamageSummaryVM vm = new ScenarioDamageSummaryVM();
                ////todo: add to string constants
                //DynamicTabVM tab = new DynamicTabVM("Summary Table", vm, "SummaryTable");
                //Navigate(tab, false, false);
            }
            
        }

        private void ComputeScenarios(List<ComputeChildRowItem> scenarioRows)
        {
            List<Task> taskList = new List<Task>();
            foreach (ComputeChildRowItem row in scenarioRows)
            {
                IASElement elem = (IASElement)row.ChildElement;
                FdaValidationResult canComputeVR = elem.CanCompute();
                if (canComputeVR.IsValid)
                {
                    ComputeScenario(elem);
                }
                else
                {
                    row.MarkInError(canComputeVR.ErrorMessage);
                }
            }
        }

        private async Task ComputeScenario(IASElement elem)
        {
            //elem.c
        }
    }
}
