using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario.Results
{
    public class AdditionalThresholdsVM : BaseViewModel
    {

        //private List<AdditionalThresholdRowItem> _rows;
        private AdditionalThresholdRowItem _selectedRow;

        public ObservableCollection<AdditionalThresholdRowItem> Rows
        {
            get; set;
        }
        public AdditionalThresholdRowItem SelectedRow
        {
            get { return _selectedRow; }
            set { _selectedRow = value; NotifyPropertyChanged(); }
        }

        public AdditionalThresholdsVM()
        {
            Rows = new ObservableCollection<AdditionalThresholdRowItem>();


        }

        public void AddRow()
        {
            Rows.Add(new AdditionalThresholdRowItem(getNextIdInteger(), IMetricEnum.ExteriorStage, 0));
            SelectedRow = Rows[Rows.Count - 1];
        }

        public void Copy()
        {
            int selectedIndex = Rows.IndexOf(SelectedRow);
            if (selectedIndex > -1)
            {
                AdditionalThresholdRowItem newRI = new AdditionalThresholdRowItem(getNextIdInteger(), SelectedRow.ThresholdType, SelectedRow.ThresholdValue);
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
                return 0;
            }
        }

        public void Remove()
        {
            int selectedIndex = Rows.IndexOf(SelectedRow);
            if (selectedIndex > -1)
            {
                Rows.Remove(SelectedRow);
                if(selectedIndex>0 && selectedIndex< Rows.Count)
                {
                    SelectedRow = Rows[selectedIndex];
                }
                else if(selectedIndex == Rows.Count)
                {
                    SelectedRow = Rows[Rows.Count - 1];
                }
                else if(selectedIndex == 0 && Rows.Count>0)
                {
                    SelectedRow = Rows[0];
                }
            }


        }

        public void OkClicked()
        {
            int i = 0;
        }


    }
}
