using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Editors;

namespace ViewModel.ImpactAreaScenario.Editor
{
    public class AdditionalThresholdsVM : BaseEditorVM
    {

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

        public AdditionalThresholdsVM() : base(null)
        {
            Rows = new ObservableCollection<AdditionalThresholdRowItem>();
        }

        public AdditionalThresholdsVM(List<AdditionalThresholdRowItem> thresholds):base(null)
        {
            Rows = new ObservableCollection<AdditionalThresholdRowItem>(thresholds);

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

        public override void Save()
        {
            Saving.PersistenceFactory.GetIASManager().UpdateThresholds();

        }

        public override void AddValidationRules()
        {
            //intentially left blank
        }

        public List<AdditionalThresholdRowItem> GetThresholds()
        {
            return Rows.ToList();
        }
    }
}
