using HEC.CS.Collections;

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
