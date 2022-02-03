using metrics;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Editors;
using ViewModel.Utilities;

namespace ViewModel.ImpactAreaScenario.Editor
{
    public class ThresholdsVM : BaseViewModel
    {
        private ThresholdRowItem _selectedRow;

        public ObservableCollection<ThresholdRowItem> Rows { get; } = new ObservableCollection<ThresholdRowItem>();

        public ThresholdRowItem SelectedRow
        {
            get { return _selectedRow; }
            set { _selectedRow = value; NotifyPropertyChanged(); }
        }

        public ThresholdsVM()
        {
        }
 
        public void AddRows(List<ThresholdRowItem> rows)
        {
            foreach (ThresholdRowItem row in rows)
            {
                Rows.Add(row);
            }
        }
        public void AddRow()
        {
            Rows.Add(new ThresholdRowItem(getNextIdInteger(), ThresholdEnum.ExteriorStage, 0));
            SelectedRow = Rows[Rows.Count - 1];
        }

        public void Copy()
        {
            int selectedIndex = Rows.IndexOf(SelectedRow);
            if (selectedIndex > -1)
            {
                ThresholdRowItem newRI = new ThresholdRowItem(getNextIdInteger(), SelectedRow.ThresholdType.Metric, SelectedRow.ThresholdValue);
                Rows.Add(newRI);
                SelectedRow = newRI;
            }
        }

        private int getNextIdInteger()
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

        public List<ThresholdRowItem> GetThresholds()
        {
            return Rows.ToList();
        }

        public FdaValidationResult IsValid()
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
            List<ThresholdRowItem> selections = Rows.Where(row => row.ThresholdType.Metric == rowItem.ThresholdType.Metric && row.ThresholdValue == rowItem.ThresholdValue).ToList();
            return selections.Count() == 1;    
        }
    }
}
