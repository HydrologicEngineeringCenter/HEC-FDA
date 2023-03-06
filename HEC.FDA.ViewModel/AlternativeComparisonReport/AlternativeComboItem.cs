using HEC.FDA.ViewModel.Alternatives;

namespace HEC.FDA.ViewModel.AlternativeComparisonReport
{
    public class AlternativeComboItem:BaseViewModel
    {
        private AlternativeElement _Alternative;
        private string _Name;
        public AlternativeElement Alternative { get { return _Alternative; } }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }

        public AlternativeComboItem(AlternativeElement elem)
        {
            _Alternative = elem;
            Name = elem.Name;
        }

        public void UpdateAlternative(AlternativeElement elem)
        {
            _Alternative = elem;
            Name = elem.Name;
        }
    }
}
