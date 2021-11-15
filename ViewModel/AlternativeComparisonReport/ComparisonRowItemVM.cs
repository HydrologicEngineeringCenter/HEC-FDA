using HEC.CS.Collections;
using System.Collections.Generic;

namespace ViewModel.AlternativeComparisonReport
{
    public class ComparisonRowItemVM : BaseViewModel
    {
        private AlternativeComboItem _SelectedAlternative;

        public AlternativeComboItem SelectedAlternative
        {
            get { return _SelectedAlternative; }
            set { _SelectedAlternative = value;NotifyPropertyChanged(); }
        }
        //public List<AlternativeComboItem> Alternatives { get;  } = new List<AlternativeComboItem>();
        public CustomObservableCollection<AlternativeComboItem> Alternatives { get; }
        public ComparisonRowItemVM( CustomObservableCollection<AlternativeComboItem> elems)
        {
            Alternatives = elems;
            if(elems.Count>0)
            {
                SelectedAlternative = elems[0];
            }
        }

    }
}
