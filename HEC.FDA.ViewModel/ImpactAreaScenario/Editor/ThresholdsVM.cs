using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.Model.metrics;
using System.Windows;

namespace HEC.FDA.ViewModel.ImpactAreaScenario.Editor
{
    public class ThresholdsVM : BaseViewModel
    {
        private ThresholdRowItem _selectedRow;
        public ObservableCollection<ThresholdRowItem> Rows { get; } = [];
        public bool IsThresholdsValid { get; set; } = false;
        public ThresholdRowItem SelectedRow
        {
            get { return _selectedRow; }
            set { _selectedRow = value; NotifyPropertyChanged(); }
        }

        public ThresholdsVM(List<ThresholdRowItem> rows)
        {
            WasCanceled = true;
            foreach (ThresholdRowItem row in rows)
            {
                Rows.Add(row);
            }

        }
 
        public void AddRow()
        {
            Rows.Add(new ThresholdRowItem(GetNextIdInteger(), ThresholdEnum.AdditionalExteriorStage, 0));
            SelectedRow = Rows[^1];
        }

        private int GetNextIdInteger()
        {
            if(Rows.Count>0)
            {
                int lastId = Rows[Rows.Count - 1].ID;
                return lastId + 1;
            }
            else
            {
                return 1;
            }
        }

        public void Remove()
        {
            int selectedIndex = Rows.IndexOf(SelectedRow);
            if (selectedIndex > -1)
            {
                Rows.Remove(SelectedRow);
                //row at beginning or in middle removed: select next row
                if( selectedIndex< Rows.Count)
                {
                    SelectedRow = Rows[selectedIndex];
                }
                //only row remove: select nothing
                else if(selectedIndex == 0 && Rows.Count == 0)
                {
                    SelectedRow = null;
                }
                //last row removed: select previous row
                else if(selectedIndex == Rows.Count)
                {
                    SelectedRow = Rows[Rows.Count - 1];
                }           
            }
        }

        public FdaValidationResult ValidateThresholds()
        {
            FdaValidationResult result = new FdaValidationResult();
            foreach(ThresholdRowItem ri in Rows)
            {
                if(!IsRowUnique(ri))
                {
                    result.AddErrorMessage("Threshold rows must be unique.");
                    break;
                }
            }
            return result;
        }

        private bool IsRowUnique(ThresholdRowItem rowItem)
        {
            List<ThresholdRowItem> selections = Rows.Where(row => row.ThresholdType == rowItem.ThresholdType && row.ThresholdValue == rowItem.ThresholdValue).ToList();
            return selections.Count() == 1;    
        }

        public void OkClicked()
        {
            FdaValidationResult result = ValidateThresholds();
            if (result.IsValid)
            {
                IsThresholdsValid = true;
            }
            else
            {
                IsThresholdsValid = false;
                MessageBox.Show(result.ErrorMessage.ToString(), "Invalid Thresholds", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
